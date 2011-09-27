using UnityEngine;
using UnityEditor;

public class WD_MenuContext : ScriptableObject {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_Behaviour Graph;
    public WD_EditorObject    SelectedObject;
    public Vector2      ScreenPosition;
    public Vector2      GraphPosition;

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static WD_MenuContext CreateInstance(WD_EditorObject selectedObject,
                                                Vector2 screenPos,
                                                Vector2 graphPos, WD_Behaviour graph) {
        return CreateInstance<WD_MenuContext>().Init(selectedObject, screenPos, graphPos, graph);
    }
    // ----------------------------------------------------------------------
    WD_MenuContext Init(WD_EditorObject selectedObject, Vector2 screenPos, Vector2 graphPos, WD_Behaviour graph) {
        Graph= graph;
        SelectedObject= selectedObject;
        ScreenPosition= screenPos;
        GraphPosition= graphPos;
        return this;
    }
}
