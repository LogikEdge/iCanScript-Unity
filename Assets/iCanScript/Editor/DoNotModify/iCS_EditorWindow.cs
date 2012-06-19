using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorWindow : EditorWindow {
    // =================================================================================
    // Editor registration
    // ---------------------------------------------------------------------------------
    protected void Register(string key, System.Object editor) {
        iCS_EditorMgr.Add(key, this, editor);
    }
    protected void Unregister(string key) {
        iCS_EditorMgr.Remove(key);
    }
    // ---------------------------------------------------------------------------------
    // Periodically repaint the library panel.
    void OnInspectorUpdate() {
        Repaint();
    }
}
