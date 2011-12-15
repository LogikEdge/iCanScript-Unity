using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of uCode.  All object are derived
// from this storage class.
public class UK_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
                      public UK_UserPreferences       Preferences  = new UK_UserPreferences();
    [HideInInspector] public List<UK_EditorObject>    EditorObjects= new List<UK_EditorObject>();
    [HideInInspector] public List<Object>             UnityObjects = new List<Object>();
    [HideInInspector] public int                      UndoRedoId   = 0;

    // ----------------------------------------------------------------------
    public bool IsValidEditorObject(int id) { return id < EditorObjects.Count && EditorObjects[id] != null; }
    public bool IsValidUnityObject(int id)  { return id < UnityObjects.Count && UnityObjects[id] != null; }

    // ======================================================================
    // UnityObject Utilities
    // ----------------------------------------------------------------------
    public int AddUnityObject(Object obj) {
        int id= 0;
        for(; id < UnityObjects.Count; ++id) {
            if(UnityObjects[id] == null) {
                UnityObjects[id]= obj;
                return id;
            }
        }
        UnityObjects.Add(obj);
        return id;
    }
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return (id < UnityObjects.Count) ? UnityObjects[id] : null;
    }
    // ----------------------------------------------------------------------
    public void SetUnityObject(int id, Object value) {
        if(IsValidUnityObject(id)) UnityObjects[id]= value;
    }

    // ======================================================================
    // EditorObject Utilities
    // ----------------------------------------------------------------------
    public UK_EditorObject GetParent(UK_EditorObject child) {
        if(child == null || child.ParentId == -1) return null;
        return EditorObjects[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
    // Returns the absolute position of the node.
    public Rect GetPosition(UK_EditorObject node) {
        if(!IsValidEditorObject(node.ParentId)) return node.LocalPosition;
        Rect position= GetPosition(EditorObjects[node.ParentId]);
        return new Rect(position.x+node.LocalPosition.x,
                        position.y+node.LocalPosition.y,
                        node.LocalPosition.width,
                        node.LocalPosition.height);
    }
}
