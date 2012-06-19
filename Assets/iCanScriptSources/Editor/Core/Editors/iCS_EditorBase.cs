using UnityEngine;
using System.Collections;

public class iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myIStorage= null;
    Rect                myPosition;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_EditorObject SelectedObject { get { return myIStorage != null ? IStorage.SelectedObject : null; } set { if(IStorage != null) IStorage.SelectedObject= value; }}
	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
	public Rect             position       { get { return myPosition; }}
	
    // =================================================================================
    // Update the editor manager.
    // ---------------------------------------------------------------------------------
    protected void UpdateMgr() {
        iCS_EditorMgr.Update();
    }
    public void OnStorageChange(iCS_IStorage iStorage) {
        myIStorage= iStorage;
        OnStorageChange();
	}
	public void OnGUI(Rect _position, iCS_IStorage iStorage) {
        myPosition= _position;
        myIStorage= iStorage;
        OnGUI();
    }
    
    // =================================================================================
    // Functions that all editor window must respond to.
    // ---------------------------------------------------------------------------------
    public virtual void OnGUI()                  {}
	public virtual void OnStorageChange()        {}
	public virtual void OnSelectedObjectChange() {}
}
