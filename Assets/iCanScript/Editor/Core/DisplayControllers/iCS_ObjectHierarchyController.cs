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
		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16);
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
		var content= new GUIContent(myCursor.Name);
		return EditorStyles.foldout.CalcSize(content);
	}
    // ---------------------------------------------------------------------------------
	public bool	DisplayCurrentObject(Rect displayArea, bool foldout) {
		if(myStorage == null) return true;
		return EditorGUI.Foldout(displayArea, foldout, myCursor.Name);
	}
    // ---------------------------------------------------------------------------------
	public object	CurrentObjectKey() {
		return myCursor;
	}
}
