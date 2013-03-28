using UnityEngine;
using UnityEditor;

public class iCS_EditorWindow : EditorWindow {
    // =================================================================================
    // Dispatch execution
    // ---------------------------------------------------------------------------------
    void OnEnable()             { iCS_EditorMgr.OnEnable(this); }
    void OnDisable()            { iCS_EditorMgr.OnDisable(this); }
    void OnGUI()                { iCS_EditorMgr.OnGUI(this); }
    void Update()               { iCS_EditorMgr.Update(this); }
    void OnSelectionChange()    { iCS_EditorMgr.OnSelectionChange(this); }
}
