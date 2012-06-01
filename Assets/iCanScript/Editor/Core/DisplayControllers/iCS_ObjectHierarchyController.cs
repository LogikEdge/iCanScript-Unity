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
	iCS_EditorObject    			myCursor       = null;
	DSTreeView		    			myTreeView     = null;
	float               			myFoldOffset   = 0;
	bool                			myNameEdition  = false;
	string              			mySearchString = null;
	List<bool>          			myFilterFlags  = null;
	Prelude.Tree<iCS_EditorObject>	myTree		   = null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 			View 	     { get { return myTreeView; }}
	public iCS_EditorObject Target	     { get { return myTarget; }}
	public iCS_EditorObject Selected     { get { return myStorage.SelectedObject; } set { myStorage.SelectedObject= value; }}
	public bool             IsSelected   { get { return myCursor == Selected; }}
	public bool             NameEdition  { get { return myNameEdition; } set { myNameEdition= value; }}
	public string			SearchString { get { return mySearchString; } set { if(mySearchString != value) { mySearchString= value; BuildTree(); }}}
	
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
		myCursor= target;
		BuildTree();
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16);
	}
	
	// =================================================================================
    // Filter & reorder.
    // ---------------------------------------------------------------------------------
    void BuildTree() {
        // Build filter list of object...
        myFilterFlags= Prelude.map(o=> FilterIn(o), myStorage.EditorObjects);
        // ... make certain the parents are also filtered in !!!
        for(int i= 0; i < myFilterFlags.Count; ++i) {
            if(myFilterFlags[i]) {
                Prelude.until(
                    myStorage.IsValid,
                    id=> { myFilterFlags[id]= true; return myStorage.EditorObjects[id].ParentId; },
                    myStorage.EditorObjects[i].ParentId
                );
            }
        }
		// Build tree and sort it elements.
		if(!myFilterFlags[0]) {
			myTree= null;
			return;
		}
		myTree= BuildTreeNode(myStorage.EditorObjects[0]);
    }
	Prelude.Tree<iCS_EditorObject> BuildTreeNode(iCS_EditorObject nodeRoot) {
		Prelude.Tree<iCS_EditorObject> tree= new Prelude.Tree<iCS_EditorObject>(nodeRoot);
		myStorage.ForEachChild(nodeRoot,
			c=> {
				if(myFilterFlags[c.InstanceId]) tree.AddChild(BuildTreeNode(c));
			}
		);
		return tree;
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
	public void	Reset() { myCursor= myTarget; }
	public void BeginDisplay() { EditorGUIUtility.LookLikeControls(); }
	public void EndDisplay() {}
	public bool	MoveToNext() {
		if(myStorage == null) return false;
		if(MoveToFirstChild()) return true;
		if(MoveToNextSibling()) return true;
		do {
			myCursor= myStorage.GetParent(myCursor);
			if(myCursor == null) return false;
			if(myTarget != null && !myStorage.IsChildOf(myCursor, myTarget)) {
				return false;
			}
		} while(!MoveToNextSibling());
		return true;
	}
    // ---------------------------------------------------------------------------------
	public bool	MoveToNextSibling() {
		if(myCursor == null || myCursor == myTarget) return false;
		bool takeNext= false;
		iCS_EditorObject parent= myStorage.GetParent(myCursor);
        if(parent == null) return false;
		return myStorage.ForEachChild(parent,
			c=> {
				if(takeNext && myFilterFlags[c.InstanceId]) {
					myCursor= c;
					return true;
				}
				if(c == myCursor) {
					takeNext= true;
				}
				return false;
			}
		);
	}
    // ---------------------------------------------------------------------------------
	public bool MoveToParent() {
		if(myStorage == null || myCursor == null) return false;
		if(myStorage.EditorObjects.Count == 0) return false;
		myCursor= myStorage.GetParent(myCursor);
		return myCursor != myTarget;
	}
	// ---------------------------------------------------------------------------------
	public bool	MoveToFirstChild() {
		if(myStorage == null) return false;
        if(myStorage.EditorObjects.Count == 0) return false;
        if(myCursor == null) {
            myCursor= myStorage.EditorObjects[0];
            return true;
        }
		if(myStorage.NbOfChildren(myCursor) == 0) return false;
		myStorage.ForEachChild(myCursor, c=> { myCursor= c; return true; });
		return true;
	}
    // ---------------------------------------------------------------------------------
	public Vector2	CurrentObjectDisplaySize() {
		if(myStorage == null) return Vector2.zero;
		if(myFoldOffset == 0) {
            var emptySize= EditorStyles.foldout.CalcSize(new GUIContent(""));
    		myFoldOffset= emptySize.x;
		}
        var nameSize= EditorStyles.label.CalcSize(new GUIContent(myCursor.Name));
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
    	    myCursor.Name= GUI.TextField(new Rect(pos.x, pos.y, frameArea.xMax-pos.x, pos.height+2.0f), myCursor.RawName);            
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
		return myCursor;
	}
    // ---------------------------------------------------------------------------------
    GUIContent GetContent() {
        EditorGUIUtility.SetIconSize(new Vector2(16.0f,12.0f));
        Texture2D icon= null;
        if(myCursor.IsFunction) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.FunctionHierarchyIcon, myStorage);            
        } else if(myCursor.IsState || myCursor.IsStateChart) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.StateHierarchyIcon, myStorage);                        
        } else if(myCursor.IsClassModule) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.ClassHierarchyIcon, myStorage);                            
        } else if(myCursor.IsNode) {
            icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.ModuleHierarchyIcon, myStorage);            
        } else if(myCursor.IsDataPort) {
            if(myCursor.IsInputPort) {
                icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.InPortHierarchyIcon, myStorage);                
            } else {
                icon= iCS_TextureCache.GetIcon(iCS_Config.GuiAssetPath+"/"+iCS_EditorStrings.OutPortHierarchyIcon, myStorage);                                    
            }
        }
        return new GUIContent(myCursor.Name, icon); 
    }
    // ---------------------------------------------------------------------------------
    bool ShouldUseFoldout() {
        if(myStorage == null) return false;
        return myCursor.IsNode;
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
