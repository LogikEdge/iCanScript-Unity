using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class iCS_EditorWindow : EditorWindow {
    // =================================================================================
    // Editor registration
    // ---------------------------------------------------------------------------------
    protected void Register(System.Object editor) {
        iCS_EditorMgr.Add(editor.GetType().Name, this, editor);
    }
    protected void Unregister(Type type) {
        iCS_EditorMgr.Remove(type.Name);
    }
    // ---------------------------------------------------------------------------------
    // Periodically repaint the library panel.
    void OnInspectorUpdate() {
        Repaint();
    }
}
