using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ObjectHierarchyController : DSTreeViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject	    myTarget     = null;
	iCS_IStorage		    myStorage    = null;
	iCS_EditorObject		myCursor     = null;
	DSTreeView				myTreeView   = null;
	float                   myFoldOffset = 0;
	iCS_EditorObject        mySelected   = null;
	bool                    myNameEdition= false;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView 			View 	    { get { return myTreeView; }}
	public iCS_EditorObject Target	    { get { return myTarget; }}
	public iCS_EditorObject Selected    { get { return mySelected; } set { mySelected= value; }}
	public bool             IsSelected  { get { return myCursor == mySelected; }}
	public bool             NameEdition { get { return myNameEdition; } set { myNameEdition= value; }}
	
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
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16);
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
				if(takeNext) {
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
            mySelected= null;
            return;
        }
        iCS_EditorObject eObj= key as iCS_EditorObject;
        myNameEdition= eObj.IsNameEditable && eObj == mySelected;
        mySelected= eObj;
        FocusGraphOnSelected();
    }
    // ---------------------------------------------------------------------------------
    void FocusGraphOnSelected() {
        myStorage.SelectedObject= mySelected;
        var myEditor= EditorWindow.focusedWindow;
        iCS_EditorMgr.GetGraphEditor().CenterAndScaleOn(mySelected);
        myEditor.Focus();
        
    }
}
