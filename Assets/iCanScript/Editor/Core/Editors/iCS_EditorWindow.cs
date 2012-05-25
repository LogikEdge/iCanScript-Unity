using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class iCS_EditorWindow : EditorWindow {

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myIStorage      = null;

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_EditorObject SelectedObject { get { return myIStorage != null ? IStorage.SelectedObject : null; } set { if(IStorage != null) IStorage.SelectedObject= value; }}
	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
	
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    protected void OnEnable() {
        iCS_EditorMgr.Add(this);
    }
    protected void OnDisable() {
        iCS_EditorMgr.Remove(this);
    }
    
    // =================================================================================
    // Functions that all editor window must respond to.
    // ---------------------------------------------------------------------------------
	public virtual void OnStorageChange()        {}
	public virtual void OnSelectedObjectChange() {}		
}
