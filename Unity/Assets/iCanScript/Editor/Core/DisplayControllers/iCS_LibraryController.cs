using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;

public class iCS_LibraryController : DSTreeViewDataSource {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    public enum NodeTypeEnum { Root, Company, Library, Package, Class, Constructor, Field, Property, Method, Message, InParameter, OutParameter };
    public class Node {
        public NodeTypeEnum    Type;
        public string          Name;
        public Vector2         NameSize;
        public iCS_MemberInfo  MemberInfo;
		private GUIContent	   myGuiContent	 = null;
		
        public Node(NodeTypeEnum type, string name, iCS_MemberInfo memberInfo) {
            Init(type, name, memberInfo);
        }
        public void Init(NodeTypeEnum type, string name, iCS_MemberInfo memberInfo) {
            Type= type;
            Name= name;
            MemberInfo= memberInfo;
            NameSize= EditorStyles.label.CalcSize(new GUIContent(Name));
        }
        public override bool Equals(System.Object theOther) {
            Node other= theOther as Node;
            if(other == null) return false;
            if(Type != other.Type) return false;
            if(Name != other.Name) return false;
            if(MemberInfo != other.MemberInfo) return false;
            return true;
        }
        public override int GetHashCode() {
            return Name.GetHashCode();
        }
	    // ----------------------------------------------------------------------	
		// Create the GUIContent as they are needed, and cache them.
	    public GUIContent getGUIContent() {
	            if(myGuiContent == null) {
					myGuiContent= new GUIContent(Name, MemberInfo.Summary);
	            }
	            return myGuiContent;            
	    }
		

    };
    public class SearchCriterias {
        iCS_LibraryController   myController   = null;
        bool                    myShowClasses  = true;
        bool                    myShowFunctions= true;
        bool                    myShowVariables= true;
        string                  mySearchString = "";

        public SearchCriterias(iCS_LibraryController controller) {
            myController= controller;
        }
        public bool ShowClasses { 
            get { return myShowClasses; }
            set {
                if(value != myShowClasses) {
                    myShowClasses= value;
                    myController.RebuildActiveTree();
                }
            }
        }
        public bool ShowFunctions { 
            get { return myShowFunctions; }
            set {
                if(value != myShowFunctions) {
                    myShowFunctions= value;
                    myController.RebuildActiveTree();
                }
            }
        }
        public bool ShowVariables { 
            get { return myShowVariables; }
            set {
                if(value != myShowVariables) {
                    myShowVariables= value;
                    myController.RebuildActiveTree();
                }
            }
        }
        public string SearchString {
            get { return mySearchString; }
            set {
                if(value != mySearchString) {
                    mySearchString= value;
                    myController.RebuildActiveTree();
                }
            }
        }
    };
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    float   kGUIEmptyFoldOffset= 0;
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    bool                        myShowInherited    = false;
    Node                        mySelected         = null;
    Rect                        mySelectedArea     = new Rect(0,0,0,0);
	DSTreeView		    		myTreeView         = null;
	float               		myFoldOffset       = 0;
    SearchCriterias             mySearchCriterias_1= null;
	string              		mySearchString     = null;
    P.Tree<Node>                myUnfilteredTree   = null;
    P.Tree<Node>                myActiveTree       = null;
    int                         myActiveTreeSize   = 0;
    // Used to move selection up/down
    Node                        myLastDisplayed  = null;
    int                         myChangeSelection= 0;
    // Used to iterate through content
	Stack<Prelude.Tree<Node>>   myIterStackNode    = null;
	Stack<int>					myIterStackChildIdx= null;

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 		View 	        { get { return myTreeView; }}
	public Node 		Selected        { get { return mySelected; } set { mySelected= value; }}
	public Rect         SelectedArea    { get { return mySelectedArea; }}
	public bool         IsSelected      { get { return IterNode != null ? IterNode.Value.Equals(Selected) : false; }}
    public P.Tree<Node> UnfilteredTree  { get { return myUnfilteredTree; } set { myUnfilteredTree= value; }}
    public bool         ShowInherited   {
        get { return myShowInherited; }
        set {
            if(value != myShowInherited) {
                myShowInherited= value;
                RebuildActiveTree();                
            }
        }
    }
    public SearchCriterias SearchCriteria_1 {
        get { return mySearchCriterias_1; }
    }
    public int          TreeIndex {
        get { return SearchString == null ? 0 : SearchString.Length; }
    }
    public int          NumberOfItems {
        get { return myActiveTreeSize; }
        set { myActiveTreeSize= value; }
    }
    public P.Tree<Node> ActiveTree {
        get { return myActiveTree; }
        set { myActiveTree= value; }
    }
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
                RebuildActiveTree();
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
        // Build the unfiltered tree.
        var allFunctions= iCS_LibraryDatabase.AllFunctions();
	    UnfilteredTree= BuildTreeNode(allFunctions);
        ActiveTree= UnfilteredTree;
        // Initialize search criterias.
        mySearchCriterias_1= new SearchCriterias(this);
        // Initialize panel.
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16, 2);
		myIterStackNode= new Stack<Prelude.Tree<Node>>();
		myIterStackChildIdx = new Stack<int>();
        kGUIEmptyFoldOffset= EditorStyles.foldout.CalcSize(new GUIContent("")).x;
	}
	
	// =================================================================================
    // Filter & reorder.
    // ---------------------------------------------------------------------------------
    void BuildTree() {
        // Build filter list of object...
        var allFunctions= iCS_LibraryDatabase.AllFunctions();
		// Build tree and sort it elements.
	    ActiveTree= BuildTreeNode(allFunctions);
    }
	Prelude.Tree<Node> BuildTreeNode(List<iCS_MethodBaseInfo> functions) {
        if(functions.Count == 0) return null;
        var myTreeSize= 0;
		Prelude.Tree<Node> tree= new Prelude.Tree<Node>(new Node(NodeTypeEnum.Root, "Root", null));
        string upperSearchStr= string.IsNullOrEmpty(mySearchString) ? null : mySearchString.ToUpper();
        for(int i= 0; i < functions.Count; ++i) {
            if(FilterIn(functions[i], upperSearchStr)) {
                var memberInfo= functions[i];
                var parentTree= GetParentTree(memberInfo, tree, ref myTreeSize);
                Node toAdd= null;
                if(memberInfo.IsField) {
                    toAdd= new Node(NodeTypeEnum.Field, memberInfo.ToFieldInfo.FieldName, memberInfo);
                } else if(memberInfo.IsProperty) {
                    toAdd= new Node(NodeTypeEnum.Property, memberInfo.ToPropertyInfo.PropertyName, memberInfo);
                } else if(memberInfo.IsConstructor) {
                    toAdd= new Node(NodeTypeEnum.Constructor, memberInfo.ToConstructorInfo.FunctionSignature, memberInfo);
                    } else if(memberInfo.IsTypeCast) {
                        // Don't add the typecasts in the library.
                    } else if(memberInfo.IsMethod) {
                    toAdd= new Node(NodeTypeEnum.Method, memberInfo.ToMethodInfo.FunctionSignature, memberInfo);                
                } else if(memberInfo.IsMessage) {
                    toAdd= new Node(NodeTypeEnum.Message, memberInfo.ToMessageInfo.FunctionSignature, memberInfo);
                }
                if(toAdd != null) {
                    ++myTreeSize;
                    parentTree.AddChild(toAdd);
                }                            
            }
        }
        NumberOfItems= myTreeSize;
        Sort(tree);
		return tree;
	}
    // ---------------------------------------------------------------------------------
    /// Rebuilds the active tree using the existing search criterias.
    public void RebuildActiveTree() {
        BuildTreeFrom(UnfilteredTree);
        if(!iCS_Strings.IsEmpty(mySearchString) && mySearchString.Length != 1) {
            string upperSearchStr= mySearchString.ToUpper();
            ShowAllFiltered(upperSearchStr);
        }        
    }
    // ---------------------------------------------------------------------------------
    /// Builds a filtered tree starting for a prefiltered tree
    void BuildTreeFrom(P.Tree<Node> baseTree) {
        var treeSize= 0;
        ActiveTree= BuildTreeNodeFrom(baseTree, ref treeSize);
        NumberOfItems= treeSize;
    }
	Prelude.Tree<Node> BuildTreeNodeFrom(P.Tree<Node> baseTree, ref int treeSize) {
        // Protect against empty tree
        if(baseTree == null || baseTree.Value == null) return null;
        string upperSearchStr= string.IsNullOrEmpty(mySearchString) ? "" : mySearchString.ToUpper();
        
        // Add if we match the search criteria
        P.Tree<Node> tree= null;
        var node= baseTree.Value;
        switch(node.Type) {
            case NodeTypeEnum.Root: {
        		tree= new Prelude.Tree<Node>(baseTree.Value);
                break;
            }
            case NodeTypeEnum.Company:
            case NodeTypeEnum.Library:
            case NodeTypeEnum.Package: {
                break;
            }
            case NodeTypeEnum.Class: {
                if(SearchCriteria_1.ShowClasses) {
                    if(NameMatches(node.Name, upperSearchStr)) {
                		tree= new Prelude.Tree<Node>(baseTree.Value);
                    }
                }
                break;
            }
            case NodeTypeEnum.Field:
            case NodeTypeEnum.Property:
            case NodeTypeEnum.InParameter:
            case NodeTypeEnum.OutParameter: {
                if(!ShowInherited && iCS_LibraryDatabase.IsInherited(node.MemberInfo)) {
                    return null;
                }
                if(SearchCriteria_1.ShowVariables == false) {
                    if(SearchCriteria_1.ShowClasses == false) {
                        return null;
                    }
                    var parentName= iCS_Types.TypeName(node.MemberInfo.ParentTypeInfo.ClassType);
                    if(NameMatches(parentName, upperSearchStr)) {
                        return new P.Tree<Node>(node);
                    }
                    return null;
                }
                if(FilterIn(node.MemberInfo, upperSearchStr)) {
                    return new P.Tree<Node>(node);                    
                }
                return null;
            }
            case NodeTypeEnum.Message:
            case NodeTypeEnum.Constructor:
            case NodeTypeEnum.Method: {
                if(!ShowInherited && iCS_LibraryDatabase.IsInherited(node.MemberInfo)) {
                    return null;
                }
                if(SearchCriteria_1.ShowFunctions == false) {
                    if(SearchCriteria_1.ShowClasses == false) {
                        return null;
                    }
                    var parentName= iCS_Types.TypeName(node.MemberInfo.ParentTypeInfo.ClassType);
                    if(NameMatches(parentName, upperSearchStr)) {
                        return new P.Tree<Node>(node);
                    }
                    return null;
                }
                if(FilterIn(node.MemberInfo, upperSearchStr)) {
                    return new P.Tree<Node>(node);                    
                }
                return null;
            }
        }
                              
        // Process each child
        var nbOfChild= baseTree.Children == null ? 0 : baseTree.Children.Count;
        for(int i= 0; i < nbOfChild; ++i) {
            var child= baseTree.Children[i];
            if(child == null || child.Value == null) continue;
            var childTree= BuildTreeNodeFrom(child, ref treeSize);
            if(childTree != null) {
                // Add parent node.
                if(tree == null) {
                    tree= new P.Tree<Node>(node);
                }
                tree.AddChild(childTree);
                ++treeSize;                
            }
        }
		return tree;
	}
    // ---------------------------------------------------------------------------------
    bool NameMatches(string name, string matchString) {
        var result= name.ToUpper().IndexOf(matchString);
        return result >= 0;
    }
    // ---------------------------------------------------------------------------------
    int FindInTreeChildren(string name, Prelude.Tree<Node> tree) {
        var children= tree.Children;
        if(children == null) return -1;
        for(int i= 0; i < children.Count; ++i) {
            if(children[i].Value.Name == name) return i;
        }
        return -1;
    }
    Prelude.Tree<Node> GetParentTree(iCS_MemberInfo desc, Prelude.Tree<Node> tree, ref int treeSize) {
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Company)) {
            var idx= FindInTreeChildren(desc.ParentTypeInfo.Company, tree);
            if(idx < 0) {
                tree.AddChild(new Node(NodeTypeEnum.Company, desc.ParentTypeInfo.Company, desc));
                ++treeSize;
                idx= FindInTreeChildren(desc.ParentTypeInfo.Company, tree);
            }
            tree= tree.Children[idx];
        }
        if(!iCS_Strings.IsEmpty(desc.ParentTypeInfo.Library)) {
            var idx= FindInTreeChildren(desc.ParentTypeInfo.Library, tree);
            if(idx < 0) {
                tree.AddChild(new Node(NodeTypeEnum.Library, desc.ParentTypeInfo.Library, desc));
                ++treeSize;
                idx= FindInTreeChildren(desc.ParentTypeInfo.Library, tree);
            }
            tree= tree.Children[idx];            
        }
		if(desc.ParentTypeInfo.HideFromLibrary == false) {
			var compilerType= desc.ParentTypeInfo.CompilerType;
	        string className= iCS_Types.TypeName(compilerType);
	        if(!iCS_Strings.IsEmpty(className)) {
	            var idx= FindInTreeChildren(className, tree);
	            if(idx < 0) {
					var nodeType= iCS_Types.IsStaticClass(compilerType) ? NodeTypeEnum.Package : NodeTypeEnum.Class;
	                tree.AddChild(new Node(nodeType, className, desc.ParentTypeInfo));
                    ++treeSize;
	                idx= FindInTreeChildren(className, tree);
	            }
	            tree= tree.Children[idx];            
	        }			
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
        if(x.Type == NodeTypeEnum.Library && y.Type != NodeTypeEnum.Library) return -1;
        if(x.Type != NodeTypeEnum.Library && y.Type == NodeTypeEnum.Library) return 1;
        if(x.Type == NodeTypeEnum.Package && y.Type != NodeTypeEnum.Package) return -1;
        if(x.Type != NodeTypeEnum.Package && y.Type == NodeTypeEnum.Package) return 1;
        if(x.Type == NodeTypeEnum.Class && y.Type != NodeTypeEnum.Class) return -1;
        if(x.Type != NodeTypeEnum.Class && y.Type == NodeTypeEnum.Class) return 1;
        if(x.Type == NodeTypeEnum.Company && y.Type != NodeTypeEnum.Company) return -1;
        if(x.Type != NodeTypeEnum.Company && y.Type == NodeTypeEnum.Company) return 1;
        // The types are the same so lets sort according to input/output direction.
        if(x.Type == NodeTypeEnum.Field) {
            bool isXGet= x.MemberInfo.IsGetField;
            bool isYGet= y.MemberInfo.IsGetField;
            if(isXGet != isYGet) return isXGet ? 1 : -1;
        }
        if(x.Type == NodeTypeEnum.Property) {
            bool isXGet= x.MemberInfo.IsGetProperty;
            bool isYGet= y.MemberInfo.IsGetProperty;
            if(isXGet != isYGet) return isXGet ? 1 : -1;
        }
        // Everything is the same so lets sort according to the name.
		return String.Compare(x.Name, y.Name);
	}
    // ---------------------------------------------------------------------------------
    bool FilterIn(iCS_MemberInfo desc, string upperSearchStr) {
        if(desc == null) return false;
        if(iCS_Strings.IsEmpty(upperSearchStr)) return true;
        if(desc.DisplayName.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        if(desc.ParentTypeInfo.DisplayName.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    bool FilterIn(Node node, string upperSearchStr) {
        if(node == null) return false;
        if(iCS_Strings.IsEmpty(upperSearchStr)) return true;
        if(node.Name.ToUpper().IndexOf(upperSearchStr) != -1) return true;
        return false;
    }
    
	// =================================================================================
    // TreeViewDataSource
    // ---------------------------------------------------------------------------------
	public void	Reset() {
		myIterStackNode.Clear();
		myIterStackChildIdx.Clear();
		if(ActiveTree != null) {
			myIterStackNode.Push(ActiveTree);
			myIterStackChildIdx.Push(0);
		}
	}
	public void BeginDisplay() { EditorGUIUtility.LookLikeControls(); }
	public void EndDisplay() {}
	public bool	MoveToNext() {
		if(ActiveTree == null || myIterStackNode.Count == 0) return false;
		if(MoveToFirstChild()) return true;
		if(MoveToNextSibling()) return true;
		do {
			if(!MoveToParent()) return false;			
		} while(!MoveToNextChild());
		return true;
	}
    // ---------------------------------------------------------------------------------
	public bool	MoveToNextSibling() {
		if(ActiveTree == null || myIterStackNode.Count == 0) return false;
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
		if(ActiveTree == null || myIterStackNode.Count == 0) return false;
		myIterStackNode.Pop();
		myIterStackChildIdx.Pop();
		return myIterStackNode.Count != 0;
	}
	// ---------------------------------------------------------------------------------
	public bool	MoveToFirstChild() {
		if(ActiveTree == null || myIterStackNode.Count == 0) return false;
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
		if(ActiveTree == null || myIterStackNode.Count == 0) return false;
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
    		myFoldOffset= kGUIEmptyFoldOffset;
		}
        var nameSize= IterValue.NameSize;
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
		
		var current= IterValue;
		GUI.Label(pos, current.getGUIContent(),labelStyle);    

		ProcessChangeSelection();
		return result;
	}
    // ---------------------------------------------------------------------------------
	public object	CurrentObjectKey() {
		return IterValue;
	}
    // ---------------------------------------------------------------------------------
    GUIContent GetContent() {
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
        } else if(nodeType == NodeTypeEnum.Library) {
            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Library);                            
        } else if(nodeType == NodeTypeEnum.Package) {
			icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Package);            
        } else if(nodeType == NodeTypeEnum.Class) {
			icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.ObjectInstance);            
        } else if(nodeType == NodeTypeEnum.Field) {
            if(current.MemberInfo.IsGetField) {
                icon= iCS_BuiltinTextures.OutEndPortIcon;
            } else {
                icon= iCS_BuiltinTextures.InEndPortIcon;
            }
        } else if(nodeType == NodeTypeEnum.Property) {
            if(current.MemberInfo.IsGetProperty) {
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
        return IterValue.Type == NodeTypeEnum.Company ||
			   IterValue.Type == NodeTypeEnum.Library ||
			   IterValue.Type == NodeTypeEnum.Class ||
			   IterValue.Type == NodeTypeEnum.Package;
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
    public void Help() {
		iCS_HelpController.openDetailedHelp( mySelected.MemberInfo );
    }
    // ---------------------------------------------------------------------------------
    public void ToggleFoldUnfoldSelected() {
        if(Selected == null) return;
        myTreeView.ToggleFoldUnfold(Selected);
    }
    // ---------------------------------------------------------------------------------
    public void ShowAllFiltered(string searchString) {
        if(ActiveTree == null) return;
        var children= ActiveTree.Children;
        if(children == null) return;
        bool result= false;
        foreach(var child in children) {
            result |= ShowAllFilteredFrom(child, searchString);
        }
        if(result) myTreeView.Unfold(ActiveTree.Value);
    }
    bool ShowAllFilteredFrom(Prelude.Tree<Node> tree, string searchString) {
        if(tree == null) return false;
        bool result= false;
        var children= tree.Children;
        var myTreeSize= NumberOfItems;
        if(children != null) {
            foreach(var child in children) {
                result |= ShowAllFilteredFrom(child, searchString);
            }            
            // Don't unfold everthing if we have too many search results.
            if(result) {
                switch(tree.Value.Type) {
                    case NodeTypeEnum.Root:
                    case NodeTypeEnum.Company:
                    case NodeTypeEnum.Library:
                    case NodeTypeEnum.Package: {
                        myTreeView.Unfold(tree.Value);                    
                        break;                        
                    }
                    case NodeTypeEnum.Class: {
                        if(SearchCriteria_1.ShowClasses == true &&
                           SearchCriteria_1.ShowFunctions == false && SearchCriteria_1.ShowVariables == false) {
                            myTreeView.Fold(tree.Value);                                                
                            break;
                        }
                        if(myTreeSize < 3000) {
                            myTreeView.Unfold(tree.Value);                                                
                        }
                        else {
                            myTreeView.Fold(tree.Value);
                        }
                        break;
                    }
                    default: {
                        myTreeView.Unfold(tree.Value);                                                
                        break;
                    }
                }
            }
        }
        return result | FilterIn(tree.Value, searchString);
    }
}
