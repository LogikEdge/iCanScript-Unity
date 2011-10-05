using UnityEngine;
using UnityEditor;

public class WD_MenuContext : ScriptableObject {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_EditorObjectMgr EditorObjects;
    public WD_EditorObject    SelectedObject;
    public Vector2            ScreenPosition;
    public Vector2            GraphPosition;

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static WD_MenuContext CreateInstance(WD_EditorObject selectedObject,
                                                Vector2 screenPos,
                                                Vector2 graphPos, WD_EditorObjectMgr editorObjects) {
        return CreateInstance<WD_MenuContext>().Init(selectedObject, screenPos, graphPos, editorObjects);
    }
    // ----------------------------------------------------------------------
    WD_MenuContext Init(WD_EditorObject selectedObject, Vector2 screenPos, Vector2 graphPos, WD_EditorObjectMgr editorObjects) {
        EditorObjects= editorObjects;
        SelectedObject= selectedObject;
        ScreenPosition= screenPos;
        GraphPosition= graphPos;
        return this;
    }
}
