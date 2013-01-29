using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myIStorage= null;
    Rect                myPosition;
    EditorWindow        myEditorWindow= null;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_EditorObject SelectedObject { get { return myIStorage != null ? IStorage.SelectedObject : null; } set { if(IStorage != null) IStorage.SelectedObject= value; }}
	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
	public Rect             position       { get { return myPosition; }}
	public EditorWindow     MyWindow       { get { return myEditorWindow; } set { myEditorWindow= value; }}
	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnEnable()          { MyWindow= iCS_EditorMgr.FindWindow(this.GetType()); }
    public void OnDisable()         { MyWindow= null; }

    // =================================================================================
    // Update the editor manager.
    // ---------------------------------------------------------------------------------
    protected void UpdateMgr() {
        iCS_EditorMgr.Update();
        myIStorage= iCS_StorageMgr.IStorage;
    }
	public void OnGUI(Rect _position) {
        myPosition= _position;
        OnGUI();
    }
    
    // =================================================================================
    // Functions that all editor window must respond to.
    // ---------------------------------------------------------------------------------
    public virtual void OnGUI()                  {}
}
