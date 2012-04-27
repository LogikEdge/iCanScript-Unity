using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ObjectHierarchyController : DSTreeViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject	    myTarget  = null;
	iCS_IStorage		    myStorage = null;
	iCS_EditorObject		myCursor  = null;
	DSTreeView				myTreeView= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public DSView View { get { return myTreeView; }}
	
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_ObjectHierarchyController(iCS_EditorObject target, iCS_IStorage storage) {
		myTarget= target;
		myStorage= storage;
		myCursor= target;
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this);
	}
	
	// =================================================================================
    // TreeViewDataSource
    // ---------------------------------------------------------------------------------
	public void	Reset() { myCursor= myTarget; }
	public bool	MoveToNext() {
		if(myStorage == null) return false;
		if(MoveToFirstChild()) return true;
		if(MoveToNextSibling()) return true;
		do {
			myCursor= myStorage.GetParent(myCursor);
			if(!myStorage.IsChildOf(myCursor, myTarget)) {
				return false;
			}
		} while(!MoveToNextSibling());
		return true;
	}
	public bool	MoveToNextSibling() {
		if(myCursor == null || myCursor == myTarget) return false;
		bool takeNext= false;
		return myStorage.ForEachChild(myStorage.GetParent(myCursor),
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
	public bool	MoveToFirstChild() {
		if(myStorage == null) return false;
		if(myStorage.NbOfChildren(myCursor) == 0) return false;
		myStorage.ForEachChild(myCursor, c=> { myCursor= c; return true; });
		return true;
	}
	public Vector2	CurrentObjectDisplaySize() {
		if(myStorage == null) return Vector2.zero;
		var content= new GUIContent(myCursor.Name);
		return EditorStyles.foldout.CalcSize(content);
	}
	public bool	DisplayCurrentObject(Rect displayArea, bool foldout) {
		if(myStorage == null) return true;
		return EditorGUI.Foldout(displayArea, foldout, myCursor.Name);
	}
	public object	CurrentObjectKey() {
		return myCursor;
	}
}
