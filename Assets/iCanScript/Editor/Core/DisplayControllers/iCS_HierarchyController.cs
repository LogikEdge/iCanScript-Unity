using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_HierarchyController : DSTreeViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject    			        myTarget           = null;
	iCS_IStorage	    			        myStorage          = null;
	DSTreeView		    			        myTreeView         = null;
	Rect                                    mySelectedArea     = new Rect(0,0,0,0);
	float               			        myFoldOffset       = 0;
	bool                			        myNameEdition      = false;
	string                                  myNameBeforeEdition= null;
	string              			        mySearchString     = null;
    int                                     myTreeSize         = 0;
	Prelude.Tree<iCS_EditorObject>	        myTree		       = null;
    // Used to move selection up/down
    iCS_EditorObject                        myLastDisplayed  = null;
    int                                     myChangeSelection= 0;
    // Used to iterate through content
	Stack<Prelude.Tree<iCS_EditorObject>>	myIterStackNode    = null;
	Stack<int>								myIterStackChildIdx= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 					View 	     { get { return myTreeView; }}
	public iCS_EditorObject 		Target	     { get { return myTarget; }}
	public iCS_IStorage             IStorage     { get { return myStorage; }}
	public iCS_EditorObject 		Selected     { get { return myStorage.SelectedObject; } set { myStorage.SelectedObject= value; }}
	public Rect                     SelectedArea { get { return mySelectedArea; }}
	public bool             		IsSelected   { get { return IterNode != null ? IterNode.Value == Selected : false; }}
    public int                      TreeSize     { get { return myTreeSize; }}
	public bool             		NameEdition  { get { return myNameEdition; } set { myNameEdition= value; }}
	public string SearchString {
	    get { return mySearchString; }
	    set {
	        if(mySearchString != value) {
                if(iCS_Strings.IsEmpty(mySearchString)) {
                    myTreeView.CopyActiveFoldDictionaryTo(1);
                    myTreeView.SwitchFoldDictionaryTo(1);
                } else if(iCS_Strings.IsEmpty(value)) {
                    myTreeView.SwitchFoldDictionaryTo(0);
                }
	            mySearchString= value;
	            BuildTree();
	            if(!iCS_Strings.IsEmpty(mySearchString)) {
	                ShowAllFiltered();
	            }
	        }
	    }
	}

	Prelude.Tree<iCS_EditorObject>	IterNode	 { get { return myIterStackNode.Count != 0 ? myIterStackNode.Peek() : null; }}
	int								IterChildIdx { get { return myIterStackChildIdx.Count  != 0 ? myIterStackChildIdx.Peek()  : 0; }}
	iCS_EditorObject				IterValue	 { get { return IterNode != null ? IterNode.Value : null; }}
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float kIconWidth= 16.0f;
    const float kLabelSpacer= 4.0f;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_HierarchyController(iCS_EditorObject target, iCS_IStorage storage) {
        Init(target, storage);
	}
    // ---------------------------------------------------------------------------------
	public void Init(iCS_EditorObject target, iCS_IStorage storage) {
		myTarget= target;
		myStorage= storage;
		BuildTree();
		if(myTreeView == null) {
		    myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16, 2);
	    }
		if(myIterStackNode == null) {
		    myIterStackNode= new Stack<Prelude.Tree<iCS_EditorObject>>();
	    }
		if(myIterStackChildIdx == null) {
		    myIterStackChildIdx = new Stack<int>();
	    }
	}
	
	// =================================================================================
    // Filter & reorder.
    // ---------------------------------------------------------------------------------
    void BuildTree() {
        // Build filter list of object...
        var filterFlags= Prelude.map(o=> FilterIn(o), myStorage.EditorObjects);
        // ... make certain the parents are also filtered in !!!
        myTreeSize= 0;
        for(int i= 0; i < filterFlags.Count; ++i) {
            if(filterFlags[i]) {
                ++myTreeSize;
                Prelude.until(
                    myStorage.IsValid,
                    id=> { filterFlags[id]= true; return myStorage.EditorObjects[id].ParentId; },
                    myStorage.EditorObjects[i].ParentId
                );
            }
        }
		// Build tree and sort it elements.
		if(!filterFlags[0]) {
			myTree= null;
			return;
		}
        var rootNode= myStorage.EditorObjects[0];
		myTree= BuildTreeNode(rootNode, filterFlags);
    }
	Prelude.Tree<iCS_EditorObject> BuildTreeNode(iCS_EditorObject nodeRoot, List<bool> filterFlags) {
		Prelude.Tree<iCS_EditorObject> tree= new Prelude.Tree<iCS_EditorObject>(nodeRoot);
		myStorage.ForEachChild(nodeRoot,
			c=> {
				Prelude.Tree<iCS_EditorObject> newNode= BuildTreeNode(c, filterFlags);
				if(filterFlags[c.InstanceId]) tree.AddChild(newNode);
			}
		);
		tree.Sort(SortComparaison);
		return tree;
	}
	int SortComparaison(iCS_EditorObject x, iCS_EditorObject y) {
		if(x.IsInputPort && !y.IsInputPort) return -1;
		if(y.IsInputPort && !x.IsInputPort) return 1;
		if(x.IsOutputPort && !y.IsOutputPort) return -1;
		if(y.IsOutputPort && !x.IsOutputPort) return 1;
		if(x.IsClassModule && !y.IsClassModule) return -1;
		if(y.IsClassModule && !x.IsClassModule) return 1;
		return String.Compare(x.Name, y.Name);
	}
    // ---------------------------------------------------------------------------------
    bool FilterIn(iCS_EditorObject eObj) {
        if(eObj == null || !myStorage.IsValid(eObj)) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        if(eObj.Name.ToUpper().IndexOf(mySearchString.ToUpper()) != -1) return true;
        return false;
    }
    

	// =================================================================================
    // TreeViewDataSource
    // ---------------------------------------------------------------------------------
	public void	Reset() {
		myIterStackNode.Clear();
		myIterStackChildIdx.Clear();
		if(myTree != null) {
			myIterStackNode.Push(myTree);
			myIterStackChildIdx.Push(0);
		}
	}
	public void BeginDisplay() { EditorGUIUtility.LookLikeControls(); }
	public void EndDisplay() {}
	public bool	MoveToNext() {
		if(myStorage == null || myTree == null || myIterStackNode.Count == 0) return false;
		if(MoveToFirstChild()) return true;
		if(MoveToNextSibling()) return true;
		do {
			if(!MoveToParent()) return false;			
		} while(!MoveToNextChild());
		return true;
	}
    // ---------------------------------------------------------------------------------
	public bool	MoveToNextSibling() {
		if(myStorage == null || myTree == null || myIterStackNode.Count == 0) return false;
		if(myIterStackNode.Count == 1) return false;
		var savedNode= myIterStackNode.Pop();
		var savedIdx= myIterStackChildIdx.Pop();
		if(!MoveToNextChild()) {
			myIterStackNode.Push(savedNode);
			myIterStackChildIdx.Push(savedIdx);
			return false;
		}
		return true;
	}
    // ---------------------------------------------------------------------------------
	public bool MoveToParent() {
		if(myStorage == null || myTree == null || myIterStackNode.Count == 0) return false;
		myIterStackNode.Pop();
		myIterStackChildIdx.Pop();
		return myIterStackNode.Count != 0;
	}
	// ---------------------------------------------------------------------------------
	public bool	MoveToFirstChild() {
		if(myStorage == null || myTree == null || myIterStackNode.Count == 0) return false;
		var node= IterNode;
		if(node == null || node.Children == null) return false;
		myIterStackChildIdx.Pop();
		myIterStackChildIdx.Push(1);
		if(node.Children.Count < 1) return false;
		myIterStackNode.Push(node.Children[0]);
		myIterStackChildIdx.Push(0);
		return true;
	}
	// ---------------------------------------------------------------------------------
	public bool	MoveToNextChild() {
		if(myStorage == null || myTree == null || myIterStackNode.Count == 0) return false;
		var node= IterNode;
		if(node == null || node.Children == null) return false;
		int idx= myIterStackChildIdx.Pop();
		myIterStackChildIdx.Push(idx+1);
		if(idx >= node.Children.Count) return false;
		myIterStackNode.Push(node.Children[idx]);
		myIterStackChildIdx.Push(0);
		return true;
	}

    // ---------------------------------------------------------------------------------
	public Vector2	CurrentObjectDisplaySize() {
		if(myStorage == null) return Vector2.zero;
		if(myFoldOffset == 0) {
            var emptySize= EditorStyles.foldout.CalcSize(new GUIContent(""));
    		myFoldOffset= emptySize.x;
		}
        var nameSize= EditorStyles.label.CalcSize(new GUIContent(IterValue.Name));
        return new Vector2(myFoldOffset+kIconWidth+kLabelSpacer+nameSize.x, nameSize.y);
//        return EditorStyles.foldout.CalcSize(new GUIContent(GetContent()));
	}
    // ---------------------------------------------------------------------------------
	public bool	DisplayCurrentObject(Rect displayArea, bool foldout, Rect frameArea) {
		if(myStorage == null) return true;
        // Show selected outline.
        GUIStyle labelStyle= EditorStyles.label;
		if(IsSelected) {
		    mySelectedArea= frameArea;
            Color selectionColor= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).settings.selectionColor;
            iCS_Graphics.DrawBox(frameArea, selectionColor, selectionColor, new Color(1.0f, 1.0f, 1.0f, 0.65f));
            labelStyle= EditorStyles.whiteLabel;
		}
		bool result= ShouldUseFoldout() ? EditorGUI.Foldout(new Rect(displayArea.x, displayArea.y, myFoldOffset, displayArea.height), foldout, "") : false;
        var content= GetContent();
        var pos= new Rect(myFoldOffset+displayArea.x, displayArea.y, displayArea.width-myFoldOffset, displayArea.height);
	    GUI.Label(pos, content.image);
        pos= new Rect(pos.x+kIconWidth+kLabelSpacer, pos.y-1f, pos.width-(kIconWidth+kLabelSpacer), pos.height);  // Move label up a bit.
        if(NameEdition && IsSelected) {
            ProcessNameChange(pos, frameArea);
        } else {            
            var savedColor= GUI.color;
            if(!IterValue.IsNameEditable) {
                labelStyle= EditorStyles.boldLabel;
                pos.width= pos.width*1.2f;
                GUI.color= new Color(1f, 0.8f, 0.4f);
            }
    	    GUI.Label(pos, content.text, labelStyle);            
            GUI.color= savedColor;
        }
        ProcessChangeSelection();
		return result;
	}
    // ---------------------------------------------------------------------------------
    void ProcessNameChange(Rect pos, Rect frameArea) {
        GUI.SetNextControlName("iCS_Hierarchy_NameChange");
	    string newName= GUI.TextField(new Rect(pos.x, pos.y, frameArea.xMax-pos.x, pos.height+2.0f), IterValue.RawName);            
        if(iCS_InputMgr.IsGUI("iCS_Hierarchy_NameChange")) {
            bool isEscape= iCS_InputMgr.IsEscape;
            if(isEscape) {
                newName= myNameBeforeEdition;
            }
            if(isEscape || iCS_InputMgr.IsReturn) {
                NameEdition= false;
                Event.current.Use();
            }
        }
        if(newName != IterValue.RawName) {
            IterValue.Name= newName;
            IStorage.SetDirty(IterValue);
        }        
    }
    // ---------------------------------------------------------------------------------
	public object	CurrentObjectKey() {
		return IterValue;
	}
    // ---------------------------------------------------------------------------------
    GUIContent GetContent() {
        /*
            FIXME: Must assure that the display width & height suffises for the icon.
        */
//        EditorGUIUtility.SetIconSize(new Vector2(16.0f,12.0f));
        Texture2D icon= null;
		var current= IterValue;
        if(current.IsFunction) {
            icon= iCS_TextureCache.GetIcon(iCS_EditorStrings.FunctionHierarchyIcon);            
        } else if(current.IsState || current.IsStateChart) {
            icon= iCS_TextureCache.GetIcon(iCS_EditorStrings.StateHierarchyIcon);                        
        } else if(current.IsClassModule) {
            icon= iCS_TextureCache.GetIcon(iCS_EditorStrings.ClassHierarchyIcon);                            
        } else if(current.IsNode) {
            icon= iCS_TextureCache.GetIcon(iCS_EditorStrings.ModuleHierarchyIcon);            
        } else if(current.IsDataPort) {
            if(current.IsInputPort) {
                if(current.SourceId != -1) {
                    icon= iCS_BuiltinTextures.InDataPortIcon;                    
                } else {
                    icon= iCS_BuiltinTextures.InValuePortIcon;
                }
            } else {
                icon= iCS_BuiltinTextures.OutDataPortIcon;                                    
            }
        } else if(current.IsStatePort || current.IsTransitionPort) {
            if(current.IsInTransitionPort || current.IsInStatePort) {
                icon= iCS_TextureCache.GetIcon(iCS_EditorStrings.RightArrowHeadIcon);
            } else {
                icon= iCS_BuiltinTextures.OutTransitionPortIcon;                
            }
        }
        return new GUIContent(current.Name, icon); 
    }
    // ---------------------------------------------------------------------------------
    bool ShouldUseFoldout() {
        if(myStorage == null) return false;
        return IterValue.IsNode;
    }
    // ---------------------------------------------------------------------------------
    public void MouseDownOn(object key, Vector2 mouseInScreenPoint, Rect screenArea) {
        if(key == null) {
            return;
        }
        iCS_EditorObject eObj= key as iCS_EditorObject;
        if(!myNameEdition && eObj == Selected) {
            if(eObj.IsNameEditable) {
                myNameEdition= true;
                myNameBeforeEdition= eObj.RawName;
            } else {       
                myNameEdition= false;    
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("The selected name cannot be changed !!!"));            
            }
        } else {
            myNameEdition= false;
        }
        Selected= eObj;
        FocusGraphOnSelected();
    }
    // ---------------------------------------------------------------------------------
    public void SelectPrevious() {
        myChangeSelection= -1;
        NameEdition= false;
    }
    // ---------------------------------------------------------------------------------
    public void SelectNext() {
        myChangeSelection= 1;
        NameEdition= false;
    }
    // ---------------------------------------------------------------------------------
    void ProcessChangeSelection() {
        if(myChangeSelection == -1) {   // Move up
            if(Selected == IterValue) {
                Selected= myLastDisplayed;
                myChangeSelection= 0;
                FocusGraphOnSelected();                
            }
        }
        if(myChangeSelection == 1) {    // Move down
            if(Selected == myLastDisplayed) {
                Selected= IterValue;
                myChangeSelection= 0;
                FocusGraphOnSelected();
            }
        }
        myLastDisplayed= IterValue;
    }
    // ---------------------------------------------------------------------------------
    void FocusGraphOnSelected() {
        var myEditor= EditorWindow.focusedWindow;
        var graphEditor= iCS_EditorMgr.FindVisualEditor();
        if(graphEditor != null) graphEditor.CenterAndScaleOn(Selected);
        myEditor.Focus();
    }
    // ---------------------------------------------------------------------------------
    public void FoldSelected() {
        if(Selected == null) return;
        myTreeView.Fold(Selected);
    }
    // ---------------------------------------------------------------------------------
    public void UnfoldSelected() {
        if(Selected == null) return;
        myTreeView.Unfold(Selected);
    }
    // ---------------------------------------------------------------------------------
    public void ToggleFoldUnfoldSelected() {
        if(Selected == null) return;
        myTreeView.ToggleFoldUnfold(Selected);
    }
    // ---------------------------------------------------------------------------------
    public void ShowElement(iCS_EditorObject eObj) {
        if(eObj == null) return;
        var parent= eObj.Parent;
        while(parent != null) {
            myTreeView.Unfold(parent);
            parent= parent.Parent;
        }
    }
    // ---------------------------------------------------------------------------------
    public void ShowAllFiltered() {
        if(myTree == null) return;
        var children= myTree.Children;
        if(children == null) return;
        bool result= false;
        foreach(var child in children) {
            result |= ShowAllFilteredFrom(child);
        }
        if(result) myTreeView.Unfold(myTree.Value);
    }
    bool ShowAllFilteredFrom(Prelude.Tree<iCS_EditorObject> tree) {
        if(tree == null) return false;
        bool result= false;
        var children= tree.Children;
        if(children != null) {
            foreach(var child in children) {
                result |= ShowAllFilteredFrom(child);
            }            
            if(result) myTreeView.Unfold(tree.Value);
        }
        return result | FilterIn(tree.Value);
    }
}
