using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class iCS_EditorWindow : EditorWindow {

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_IStorage IStorage 		   { get { return iCS_StorageMgr.IStorage; }}
	public iCS_Storage  Storage  		   { get { return iCS_StorageMgr.Storage; }}
	public iCS_EditorObject SelectedObject { get { return null; }}
	
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
	public virtual  void OnStorageChange()        {}
	public virtual  void OnSelectedObjectChange() {}		
    public abstract void OnActivate(iCS_EditorObject target, iCS_IStorage storage);
    public abstract void OnDeactivate();
}
