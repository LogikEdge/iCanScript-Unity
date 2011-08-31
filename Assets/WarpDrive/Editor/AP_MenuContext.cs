using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AP_MenuContext : ScriptableObject {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_Object            SelectedObject;
    public Vector2              ScreenPosition;
    public Vector2              GraphPosition;
    public List<EditorWindow>   WindowList= new List<EditorWindow>();

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_MenuContext CreateInstance(AP_Object _selectedObject,
                                                Vector2 _screenPos,
                                                Vector2 _graphPos,
                                                List<EditorWindow> _windowList) {
        return CreateInstance<AP_MenuContext>().Init(_selectedObject, _screenPos, _graphPos, _windowList);
    }
    // ----------------------------------------------------------------------
    AP_MenuContext Init(AP_Object _selectedObject, Vector2 _screenPos, Vector2 _graphPos, List<EditorWindow> _windowList) {
        SelectedObject= _selectedObject;
        ScreenPosition= _screenPos;
        GraphPosition= _graphPos;
        WindowList= _windowList;
        return this;
    }

}
