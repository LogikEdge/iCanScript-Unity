using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ObjectEditor : EditorWindow {
    // ======================================================================
    // Initialization/Teardown
    // ----------------------------------------------------------------------
    public void Init(Vector2 pos, string _title) {
        title= _title;
        position= new Rect(pos.x, pos.y, 300, 200);
        ShowAuxWindow();
    }
    
    public void OnEnable() {}
    public void OnDisable() {}

    // ======================================================================
    // GUI Update
    // ----------------------------------------------------------------------
    public void OnGUI() {
    }
}

