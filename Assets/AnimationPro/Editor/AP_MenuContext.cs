using UnityEngine;
using UnityEditor;

public class AP_MenuContext : ScriptableObject {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_Object            SelectedObject;
    public Vector2              ScreenPosition;
    public Vector2              GraphPosition;

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_MenuContext CreateInstance(AP_Object _selectedObject,
                                                Vector2 _screenPos,
                                                Vector2 _graphPos) {
        return CreateInstance<AP_MenuContext>().Init(_selectedObject, _screenPos, _graphPos);
    }
    // ----------------------------------------------------------------------
    AP_MenuContext Init(AP_Object _selectedObject, Vector2 _screenPos, Vector2 _graphPos) {
        SelectedObject= _selectedObject;
        ScreenPosition= _screenPos;
        GraphPosition= _graphPos;
        return this;
    }

//    void OnEnable() {
//        Debug.Log("MenuContext enabled");
//    }
//    void OnDisable() {
//        Debug.Log("MenuContext disable");
//    }
//    void OnDestroy() {
//        Debug.Log("MenuContext destroyed");
//    }
}
