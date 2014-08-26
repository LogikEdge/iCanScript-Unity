using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ObjectEditor : EditorWindow {
    iCS_EditorObject    myObject= null;
    
    // ======================================================================
    // Initialization/Teardown
    // ----------------------------------------------------------------------
    public static iCS_ObjectEditor CreateInstance(Vector2 pos, iCS_EditorObject theObject) {
        iCS_ObjectEditor objectEditor= ScriptableObject.CreateInstance(typeof(iCS_ObjectEditor)) as iCS_ObjectEditor;
        objectEditor.Init(pos, theObject);
        return objectEditor;
    }
    public void Init(Vector2 pos, iCS_EditorObject theObject) {
        myObject= theObject;
        title= theObject.Name;
        position= new Rect(pos.x, pos.y, 300, 200);
        ShowAuxWindow();
    }
    
    public void OnEnable() {}
    public void OnDisable() {}

    // ======================================================================
    // GUI Update
    // ----------------------------------------------------------------------
    public void OnGUI() {
        if(myObject == null) return;

        // Update the title
        title= myObject.Name;
    }
}

