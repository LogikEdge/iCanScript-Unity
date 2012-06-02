using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ObjectHierarchyController : DSTreeViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject    			myTarget       = null;
	iCS_IStorage	    			myStorage      = null;
	DSTreeView		    			myTreeView     = null;
	float               			myFoldOffset   = 0;
	bool                			myNameEdition  = false;
	string              			mySearchString = null;
	Prelude.Tree<iCS_EditorObject>	myTree		   = null;

	Stack<Prelude.Tree<iCS_EditorObject>>	myIterStackNode    = null;
	Stack<int>								myIterStackChildIdx= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 					View 	     { get { return myTreeView; }}
	public iCS_EditorObject 		Target	     { get { return myTarget; }}
	public iCS_EditorObject 		Selected     { get { return myStorage.SelectedObject; } set { myStorage.SelectedObject= value; }}
	public bool             		IsSelected   { get { return IterNode != null ? IterNode.Value == Selected : false; }}
	public bool             		NameEdition  { get { return myNameEdition; } set { myNameEdition= value; }}
	public string					SearchString { get { return mySearchString; } set { if(mySearchString != value) { mySearchString= value; BuildTree(); }}}

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
	public iCS_ObjectHierarchyController(iCS_EditorObject target, iCS_IStorage storage) {
		myTarget= target;
		myStorage= storage;
		BuildTree();
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16);
		myIterStackNode= new Stack<Prelude.Tree<iCS_EditorObject>>();
		myIterStackChildIdx = new Stack<int>();
	}
	
	// =================================================================================
    // Filter & reorder.
    // ---------------------------------------------------------------------------------
    void BuildTree() {
        // Build filter list of object...
        var filterFlags= Prelude.map(o=> FilterIn(o), myStorage.EditorObjects);
        // ... make certain the parents are also filtered in !!!
        for(int i= 0; i < filterFlags.Count; ++i) {
            if(filterFlags[i]) {
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
		myTree= BuildTreeNode(myStorage.EditorObjects[0], filterFlags);
    }
	Prelude.Tree<iCS_EditorObject> BuildTreeNode(iCS_EditorObject nodeRoot, List<bool> filterFlags) {
		Prelude.Tree<iCS_EditorObject> tree= new Prelude.Tree<iCS_EditorObject>(nodeRoot);
		myStorage.ForEachChild(nodeRoot,
			c=> {
				if(filterFlags[c.InstanceId]) tree.AddChild(BuildTreeNode(c, filterFlags));
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
        return FilterIn(myStorage.GetParent(eObj));
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
		myIterStackNode.Pop();
		myIterStackChildIdx.Pop();
		if(myIterStackNode.Count == 0) return false;
		return MoveToNextChild();
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

//	// =================================================================================
//    // TreeViewDataSource
//    // ---------------------------------------------------------------------------------
//	public void	Reset() { myCursor= myTarget; }
//	public void BeginDisplay() { EditorGUIUtility.LookLikeControls(); }
//	public void EndDisplay() {}
//	public bool	MoveToNext() {
//		if(myStorage == null) return false;
//		if(MoveToFirstChild()) return true;
//		if(MoveToNextSibling()) return true;
//		do {
//			myCursor= myStorage.GetParent(myCursor);
//			if(myCursor == null) return false;
//			if(myTarget != null && !myStorage.IsChildOf(myCursor, myTarget)) {
//				return false;
//			}
//		} while(!MoveToNextSibling());
//		return true;
//	}
//    // ---------------------------------------------------------------------------------
//	public bool	MoveToNextSibling() {
//		if(myCursor == null || myCursor == myTarget) return false;
//		bool takeNext= false;
//		iCS_EditorObject parent= myStorage.GetParent(myCursor);
//        if(parent == null) return false;
//		return myStorage.ForEachChild(parent,
//			c=> {
//				if(takeNext && myFilterFlags[c.InstanceId]) {
//					myCursor= c;
//					return true;
//				}
//				if(c == myCursor) {
//					takeNext= true;
//				}
//				return false;
//			}
//		);
//	}
//    // ---------------------------------------------------------------------------------
//	public bool MoveToParent() {
//		if(myStorage == null || myCursor == null) return false;
//		if(myStorage.EditorObjects.Count == 0) return false;
//		myCursor= myStorage.GetParent(myCursor);
//		return myCursor != myTarget;
//	}
//	// ---------------------------------------------------------------------------------
//	public bool	MoveToFirstChild() {
//		if(myStorage == null) return false;
//        if(myStorage.EditorObjects.Count == 0) return false;
//        if(myCursor == null) {
//            myCursor= myStorage.EditorObjects[0];
//            return true;
//        }
//		if(myStorage.NbOfChildren(myCursor) == 0) return false;
//		myStorage.ForEachChild(myCursor, c=> { myCursor= c; return true; });
//		return true;
//	}
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
    	    IterValue.Name= GUI.TextField(new Rect(pos.x, pos.y, frameArea.xMax-pos.x, pos.height+2.0f), IterValue.RawName);            
        } else {
    	    GUI.Label(pos, content.text, labelStyle);            
        }
		return result;
//        bool result= false;
//        if(ShouldUseFoldout()) {
//            result= EditorGUI.Foldout(displayArea, foldout, GetContent());
//        } else {
//            GUI.Label(new Rect(displayArea.x+myFoldOffset, displayArea.y, displayArea.width, displayArea.height), GetContent());
//        }
//        return result;
	}
    // ---------------------------------------------------------------------------------
	public object	CurrentObjectKey() {
		return IterValue;
	}
    // ---------------------------------------------------------------------------------
    GUIContent GetContent() {
        EditorGUIUtility.SetIconSize(new Vector2(16.0f,12.0f));
        Texture2D icon= null;
		var current= IterValue;
        if(current.IsFunction) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.FunctionHierarchyIcon, myStorage);            
        } else if(current.IsState || current.IsStateChart) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.StateHierarchyIcon, myStorage);                        
        } else if(current.IsClassModule) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.ClassHierarchyIcon, myStorage);                            
        } else if(current.IsNode) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.ModuleHierarchyIcon, myStorage);            
        } else if(current.IsDataPort) {
            if(current.IsInputPort) {
                icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.InPortHierarchyIcon, myStorage);                
            } else {
                icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.OutPortHierarchyIcon, myStorage);                                    
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
        myNameEdition= eObj.IsNameEditable && eObj == Selected;
        Selected= eObj;
        FocusGraphOnSelected();
    }
    // ---------------------------------------------------------------------------------
    void FocusGraphOnSelected() {
        var myEditor= EditorWindow.focusedWindow;
        iCS_EditorMgr.GetGraphEditor().CenterAndScaleOn(Selected);
        myEditor.Focus();
        
    }
}
