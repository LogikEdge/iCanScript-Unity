using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorWindow : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    public iCS_IStorage      myIStorage      = null;
    public iCS_EditorObject  mySelectedObject= null;
    
    // =================================================================================
    // Editor registration
    // ---------------------------------------------------------------------------------
    protected void Register(string key, System.Object editor) {
        iCS_EditorMgr.Add(key, this, editor, OnStorageChangeImp, OnSelectedObjectChangeImp);
    }
    protected void Unregister(string key) {
        iCS_EditorMgr.Remove(key);
    }
    protected void OnStorageChangeImp() {
        myIStorage= iCS_StorageMgr.IStorage;
        OnStorageChange();
    }
    protected virtual void OnSelectedObjectChangeImp() {
        mySelectedObject= myIStorage.SelectedObject;
        OnStorageChange();
    }
    
    // =================================================================================
    // Service to be overriden
    // ---------------------------------------------------------------------------------
    protected virtual void OnStorageChange() {}
    protected virtual void OnSelectedObjectChange() {}

    // ---------------------------------------------------------------------------------
    // Periodically repaint the library panel.
    void OnInspectorUpdate() {
        Repaint();
    }
}
