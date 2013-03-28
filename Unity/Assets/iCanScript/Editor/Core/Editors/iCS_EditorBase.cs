using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage		myIStorage= null;
    EditorWindow        myEditorWindow= null;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
	public iCS_IStorage 	IStorage 	   { get { return myIStorage; } set { myIStorage= value; }}
	public Rect             position       { get { return myEditorWindow.position; }}
	public EditorWindow     MyWindow       { get { return myEditorWindow; } set { myEditorWindow= value; }}
	public iCS_EditorObject SelectedObject {
	    get { return myIStorage != null ? IStorage.SelectedObject : null; }
	    set { if(IStorage != null) IStorage.SelectedObject= value; }
	}
        	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnEnable(EditorWindow window) {
        MyWindow= window;
        OnEnable();
    }
    public void OnDisable(EditorWindow window) {
        OnDisable();
        MyWindow= null;
    }

    // =================================================================================
    // Update the editor manager.
    // ---------------------------------------------------------------------------------
    protected void UpdateMgr() {
        iCS_EditorMgr.Update();
        myIStorage= iCS_StorageMgr.IStorage;
    }
    
    // =================================================================================
    // Functions that all editor window must respond to.
    // ---------------------------------------------------------------------------------
    public virtual void OnEnable()          {}
    public virtual void OnDisable()         {}
    public virtual void OnGUI()             {}
    public virtual void Update()            {}
    public virtual void OnSelectionChange() {}
}
