using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class iCS_EditorWindow : EditorWindow {

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
    public abstract void OnActivate(iCS_EditorObject target, iCS_IStorage storage);
    public abstract void OnDeactivate();
}
