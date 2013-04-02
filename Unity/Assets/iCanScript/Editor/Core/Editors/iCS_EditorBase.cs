using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorBase : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myIStorage= null;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
	public iCS_EditorObject SelectedObject {
	    get { return myIStorage != null ? IStorage.SelectedObject : null; }
	    set { if(IStorage != null) IStorage.SelectedObject= value; }
	}
        	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnEnable() {
        iCS_EditorMgr.Add(this);
    }
    public void OnDisable() {
        iCS_EditorMgr.Remove(this);
    }

    // =================================================================================
    // Update the editor manager.
    // ---------------------------------------------------------------------------------
    protected void UpdateMgr() {
        iCS_EditorMgr.Update();
        myIStorage= iCS_StorageMgr.IStorage;
    }
}
