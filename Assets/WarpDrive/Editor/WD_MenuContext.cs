using UnityEngine;
using UnityEditor;

public class WD_MenuContext : ScriptableObject {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_Behaviour Graph;
    public WD_Object    SelectedObject;
    public Vector2      ScreenPosition;
    public Vector2      GraphPosition;

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static WD_MenuContext CreateInstance(WD_Object _selectedObject,
                                                Vector2 _screenPos,
                                                Vector2 _graphPos, WD_Behaviour graph) {
        return CreateInstance<WD_MenuContext>().Init(_selectedObject, _screenPos, _graphPos, graph);
    }
    // ----------------------------------------------------------------------
    WD_MenuContext Init(WD_Object _selectedObject, Vector2 _screenPos, Vector2 _graphPos, WD_Behaviour graph) {
        Graph= graph;
        SelectedObject= _selectedObject;
        ScreenPosition= _screenPos;
        GraphPosition= _graphPos;
        return this;
    }
}
