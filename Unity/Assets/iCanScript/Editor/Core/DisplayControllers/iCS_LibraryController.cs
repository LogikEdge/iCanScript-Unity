using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_LibraryController : DSTreeViewDataSource {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    public enum NodeTypeEnum { Root, Company, Package, Class, Constructor, Field, Property, Method, Message, InParameter, OutParameter };
    public class Node {
        public NodeTypeEnum        Type;
        public string              Name;
        public iCS_MemberInfo  Desc;
        public Node(NodeTypeEnum type, string name, iCS_MemberInfo desc) {
            Type= type;
            Name= name;
            Desc= desc;
        }
        public override bool Equals(System.Object theOther) {
            Node other= theOther as Node;
            if(other == null) return false;
            if(Type != other.Type) return false;
            if(Name != other.Name) return false;
            if(Desc != other.Desc) return false;
            return true;
        }
        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    };
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    Node                        mySelected     = null;
    Rect                        mySelectedArea = new Rect(0,0,0,0);
	DSTreeView		    		myTreeView     = null;
	float               		myFoldOffset   = 0;
	string              		mySearchString = null;
	Prelude.Tree<Node>	        myTree		   = null;
    // Used to move selection up/down
    Node                        myLastDisplayed  = null;
    int                         myChangeSelection= 0;
    // Used to iterate through content
	Stack<Prelude.Tree<Node>>   myIterStackNode    = null;
	Stack<int>					myIterStackChildIdx= null;

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 		View 	     { get { return myTreeView; }}
	public Node 		Selected     { get { return mySelected; } set { mySelected= value; }}
	public Rect         SelectedArea { get { return mySelectedArea; }}
	public bool         IsSelected   { get { return IterNode != null ? IterNode.Value.Equals(Selected) : false; }}
	public string		SearchString {
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
	            if(!iCS_Strings.IsEmpty(mySearchString) && mySearchString.Length != 1) {
	                ShowAllFiltered();
	            }
	        }
	    }
	}

	Prelude.Tree<Node>  IterNode	 { get { return myIterStackNode.Count != 0 ? myIterStackNode.Peek() : null; }}
	int					IterChildIdx { get { return myIterStackChildIdx.Count  != 0 ? myIterStackChildIdx.Peek()  : 0; }}
	Node				IterValue	 { get { return IterNode != null ? IterNode.Value : null; }}
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kIconWidth  = 16;
    const int   kIconHeight = 16;
    const float kLabelSpacer= 4f;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_LibraryController() {
		BuildTree();
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16, 2);
		myIterStackNode= new Stack<Prelude.Tree<Node>>();
		myIterStackChildIdx = new Stack<int>();
	}
	
	// =================================================================================
    // Filter & reorder.
    // ---------------------------------------------------------------------------------
    void BuildTree() {
        // Build filter list of object...
        var allFunctions= iCS_LibraryDatabase.AllFunctions();
		// Build tree and sort it elements.
		myTree= BuildTreeNode(allFunctions);
    }
	Prelude.Tree<Node> BuildTreeNode(List<iCS_MethodBaseInfo> functions) {
        if(functions.Count == 0) return null;
		Prelude.Tree<Node> tree= new Prelude.Tree<Node>(new Node(NodeTypeEnum.Root, "Root", null));
        for(int i= 0; i < functions.Count; ++i) {
            if(FilterIn(functions[i])) {
                var desc= functions[i];
                var parentTree= GetParentTree(desc, tree);
                Node toAdd= null;
                if(desc.IsField) {
                    toAdd= new Node(NodeTypeEnum.Field, desc.ToFieldInfo.FieldName, desc);
                } else if(desc.IsProperty) {
                    toAdd= new Node(NodeTypeEnum.Property, desc.ToPropertyInfo.PropertyName, desc);
                } else if(desc.IsConstructor) {
                    toAdd= new Node(NodeTypeEnum.Constructor, desc.ToConstructorInfo.FunctionSignature, desc);
                } else if(desc.IsMethod) {
                    toAdd= new Node(NodeTypeEnum.Method, desc.ToMethodInfo.FunctionSignature, desc);                
                } else if(desc.IsMessage) {
                    toAdd= new Node(NodeTypeEnum.Message, desc.ToMessageInfo.FunctionSignature, desc);
                }
                if(toAdd != null) {
                    parentTree.AddChild(toAdd);
//                    var newTree= parentTree.AddChild(toAdd);
//                    if(desc.IsConstructor || desc.IsMethod) {
//                        var inputParamNames= desc.InputParameterNames;
//                        foreach(var ip in inputParamNames) {
//                            newTree.AddChild(new Node(NodeTypeEnum.InParameter, ip, desc));
//                        }
//                        var outputParamNames= desc.OutputParameterNames;
//                        foreach(var op in outputParamNames) {
//                            newTree.AddChild(new Node(NodeTypeEnum.OutParameter, op, desc));
//                        }
//                    }
                }                            
            }
        }
        Sort(tree);
		return tree;
	}
    int FindInTreeChildren(string name, Prelude.Tree<Node> tree) {
        var children= tree.Children;
        if(children == null) return -1;
        for(int i= 0; i < children.Count; ++i) {
            if(children[i].Value.Name == name) return i;
        }
        return -1;
    }
    Prelude.Tree<Node> GetParentTree(iCS_MemberInfo desc, Prelude.Tree<Node> tree) {
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Company)) {
            var idx= FindInTreeChildren(desc.ParentTypeInfo.Company, tree);
            if(idx < 0) {
                tree.AddChild(new Node(NodeTypeEnum.Company, desc.ParentTypeInfo.Company, desc));
                idx= FindInTreeChildren(desc.ParentTypeInfo.Company, tree);
            }
            tree= tree.Children[idx];
        }
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Package)) {
            var idx= FindInTreeChildren(desc.ParentTypeInfo.Package, tree);
            if(idx < 0) {
                tree.AddChild(new Node(NodeTypeEnum.Package, desc.ParentTypeInfo.Package, desc));
                idx= FindInTreeChildren(desc.ParentTypeInfo.Package, tree);
            }
            tree= tree.Children[idx];            
        }
        string className= iCS_Types.TypeName(desc.ParentTypeInfo.CompilerType);
        if(!iCS_Strings.IsEmpty(className)) {
            var idx= FindInTreeChildren(className, tree);
            if(idx < 0) {
                tree.AddChild(new Node(NodeTypeEnum.Class, className, desc));
                idx= FindInTreeChildren(className, tree);
            }
            tree= tree.Children[idx];            
        }
        return tree;
    }
    void Sort(Prelude.Tree<Node> tree) {
        if(tree == null) return;
        var children= tree.Children;
        if(children != null) {
            tree.Sort(SortComparaison);
            foreach(var child in children) {
                Sort(child);
            }
        }
    }
	int SortComparaison(Node x, Node y) {
        if(x.Type == NodeTypeEnum.InParameter && y.Type != NodeTypeEnum.InParameter) return -1;
        if(x.Type != NodeTypeEnum.InParameter && y.Type == NodeTypeEnum.InParameter) return 1;
        if(x.Type == NodeTypeEnum.OutParameter && y.Type != NodeTypeEnum.OutParameter) return -1;
        if(x.Type != NodeTypeEnum.OutParameter && y.Type == NodeTypeEnum.OutParameter) return 1;
        if(x.Type == NodeTypeEnum.Field && y.Type != NodeTypeEnum.Field) return -1;
        if(x.Type != NodeTypeEnum.Field && y.Type == NodeTypeEnum.Field) return 1;
        if(x.Type == NodeTypeEnum.Property && y.Type != NodeTypeEnum.Property) return -1;
        if(x.Type != NodeTypeEnum.Property && y.Type == NodeTypeEnum.Property) return 1;
        if(x.Type == NodeTypeEnum.Constructor && y.Type != NodeTypeEnum.Constructor) return -1;
        if(x.Type != NodeTypeEnum.Constructor && y.Type == NodeTypeEnum.Constructor) return 1;
        if(x.Type == NodeTypeEnum.Method && y.Type != NodeTypeEnum.Method) return -1;
        if(x.Type != NodeTypeEnum.Method && y.Type == NodeTypeEnum.Method) return 1;
        if(x.Type == NodeTypeEnum.Class && y.Type != NodeTypeEnum.Class) return -1;
        if(x.Type != NodeTypeEnum.Class && y.Type == NodeTypeEnum.Class) return 1;
        if(x.Type == NodeTypeEnum.Package && y.Type != NodeTypeEnum.Package) return -1;
        if(x.Type != NodeTypeEnum.Package && y.Type == NodeTypeEnum.Package) return 1;
        if(x.Type == NodeTypeEnum.Company && y.Type != NodeTypeEnum.Company) return -1;
        if(x.Type != NodeTypeEnum.Company && y.Type == NodeTypeEnum.Company) return 1;
        // The types are the same so lets sort according to input/output direction.
        if(x.Type == NodeTypeEnum.Field) {
            bool isXGet= x.Desc.IsGetField;
            bool isYGet= y.Desc.IsGetField;
            if(isXGet != isYGet) return isXGet ? 1 : -1;
        }
        if(x.Type == NodeTypeEnum.Property) {
            bool isXGet= x.Desc.IsGetProperty;
            bool isYGet= y.Desc.IsGetProperty;
            if(isXGet != isYGet) return isXGet ? 1 : -1;
        }
        // Everything is the same so lets sort according to the name.
		return String.Compare(x.Name, y.Name);
	}
    // ---------------------------------------------------------------------------------
    bool FilterInCompany(iCS_MemberInfo desc) {
        if(desc == null) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        string upperSearchStr= mySearchString.ToUpper();
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Company) && desc.ParentTypeInfo.Company.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    bool FilterInPackage(iCS_MemberInfo desc) {
        if(desc == null) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        string upperSearchStr= mySearchString.ToUpper();
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Package) && desc.ParentTypeInfo.Package.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    bool FilterInName(iCS_MemberInfo desc) {
        if(desc == null) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        string upperSearchStr= mySearchString.ToUpper();
        if(desc.DisplayName.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    bool FilterIn(iCS_MemberInfo desc) {
        if(desc == null) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        string upperSearchStr= mySearchString.ToUpper();
        if(desc.DisplayName.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Package) && desc.Package.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Company) && desc.Company.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    bool FilterIn(Node node) {
        if(node == null) return false;
        if(iCS_Strings.IsEmpty(mySearchString)) return true;
        string upperSearchStr= mySearchString.ToUpper();
        if(node.Name.ToUpper().IndexOf(upperSearchStr) != -1) return true;
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
		if(myTree == null || myIterStackNode.Count == 0) return false;
		if(MoveToFirstChild()) return true;
		if(MoveToNextSibling()) return true;
		do {
			if(!MoveToParent()) return false;			
		} while(!MoveToNextChild());
		return true;
	}
    // ---------------------------------------------------------------------------------
	public bool	MoveToNextSibling() {
		if(myTree == null || myIterStackNode.Count == 0) return false;
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
		if(myTree == null || myIterStackNode.Count == 0) return false;
		myIterStackNode.Pop();
		myIterStackChildIdx.Pop();
		return myIterStackNode.Count != 0;
	}
	// ---------------------------------------------------------------------------------
	public bool	MoveToFirstChild() {
		if(myTree == null || myIterStackNode.Count == 0) return false;
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
		if(myTree == null || myIterStackNode.Count == 0) return false;
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
	public Vector2	CurrentObjectLayoutSize() {
		if(myFoldOffset == 0) {
            var emptySize= EditorStyles.foldout.CalcSize(new GUIContent(""));
    		myFoldOffset= emptySize.x;
		}
        var nameSize= EditorStyles.label.CalcSize(new GUIContent(IterValue.Name));
        return new Vector2(myFoldOffset+kIconWidth+kLabelSpacer+nameSize.x, kIconHeight);
	}
    // ---------------------------------------------------------------------------------
	public bool	DisplayCurrentObject(Rect displayArea, bool foldout, Rect frameArea) {
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
    	GUI.Label(pos, content.text, labelStyle);            
        ProcessChangeSelection();
		return result;
	}
    // ---------------------------------------------------------------------------------
	public object	CurrentObjectKey() {
		return IterValue;
	}
    // ---------------------------------------------------------------------------------
    GUIContent GetContent() {
        /*
            FIXME: Not accessible on Windows.
        */
//        EditorGUIUtility.SetIconSize(new Vector2(16.0f,12.0f));
        Texture2D icon= null;
		var current= IterValue;
		var nodeType= current.Type;
		string name= current.Name;
        if(nodeType == NodeTypeEnum.Company) {
            if(name == "iCanScript") {
                icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.iCanScript);
            } else if(name == "Unity") {
                icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Unity);
            } else if(name == "NET") {
                icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.DotNet);
            } else {
                icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Company);
            }
        } else if(nodeType == NodeTypeEnum.Package) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Package);                            
        } else if(nodeType == NodeTypeEnum.Class) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.ObjectInstance);            
        } else if(nodeType == NodeTypeEnum.Field) {
            if(current.Desc.IsGetField) {
                icon= iCS_BuiltinTextures.OutEndPortIcon;
            } else {
                icon= iCS_BuiltinTextures.InEndPortIcon;
            }
        } else if(nodeType == NodeTypeEnum.Property) {
            if(current.Desc.IsGetProperty) {
                icon= iCS_BuiltinTextures.OutEndPortIcon;
            } else {
                icon= iCS_BuiltinTextures.InEndPortIcon;
            }
        } else if(nodeType == NodeTypeEnum.Constructor) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Builder);            
        } else if(nodeType == NodeTypeEnum.Method) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Function);            
        } else if(nodeType == NodeTypeEnum.Message) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Message);            
        } else if(nodeType == NodeTypeEnum.InParameter) {
            icon= iCS_BuiltinTextures.InEndPortIcon;
        } else if(nodeType == NodeTypeEnum.OutParameter) {
            icon= iCS_BuiltinTextures.OutEndPortIcon;
        }
        return new GUIContent(name, icon); 
    }
    // ---------------------------------------------------------------------------------
    bool ShouldUseFoldout() {
        return IterValue.Type == NodeTypeEnum.Company || IterValue.Type == NodeTypeEnum.Package || IterValue.Type == NodeTypeEnum.Class;
    }
    // ---------------------------------------------------------------------------------
    public void MouseDownOn(object key, Vector2 mouseInScreenPoint, Rect screenArea) {
        if(key == null) {
            return;
        }
        Node node= key as Node;
        Selected= node;
    }
    // ---------------------------------------------------------------------------------
    public void SelectPrevious() {
        myChangeSelection= -1;
    }
    // ---------------------------------------------------------------------------------
    public void SelectNext() {
        myChangeSelection= 1;
    }
    // ---------------------------------------------------------------------------------
    void ProcessChangeSelection() {
        if(myChangeSelection == -1) {   // Move up
            if(Selected == IterValue) {
                Selected= myLastDisplayed;
                myChangeSelection= 0;
            }
        }
        if(myChangeSelection == 1) {    // Move down
            if(Selected == myLastDisplayed) {
                Selected= IterValue;
                myChangeSelection= 0;
            }
        }
        myLastDisplayed= IterValue;
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
    bool ShowAllFilteredFrom(Prelude.Tree<Node> tree) {
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
