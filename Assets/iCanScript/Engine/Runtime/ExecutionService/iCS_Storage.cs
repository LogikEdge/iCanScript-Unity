using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of uCode.  All object are derived
// from this storage class.
public class iCS_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
                      public iCS_UserPreferences      Preferences  = new iCS_UserPreferences();
    [HideInInspector] public List<iCS_EditorObject>   EditorObjects= new List<iCS_EditorObject>();
    [HideInInspector] public List<Object>             UnityObjects = new List<Object>();
    [HideInInspector] public int                      UndoRedoId   = 0;

    // ----------------------------------------------------------------------
    public bool IsValidEditorObject(int id) { return id >= 0 && id < EditorObjects.Count && EditorObjects[id] != null; }
    public bool IsValidUnityObject(int id)  { return id >= 0 && id < UnityObjects.Count && UnityObjects[id] != null; }

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
    public iCS_EditorObject GetParent(iCS_EditorObject child) {
        if(child == null || child.ParentId == -1) return null;
        return EditorObjects[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetSource(iCS_EditorObject port) {
        if(port == null || port.Source == -1) return null;
        return EditorObjects[port.Source];
    }
    // ----------------------------------------------------------------------
    // Returns the absolute position of the node.
    public Rect GetPosition(iCS_EditorObject node) {
        if(!IsValidEditorObject(node.ParentId)) return node.LocalPosition;
        Rect position= GetPosition(EditorObjects[node.ParentId]);
        return new Rect(position.x+node.LocalPosition.x,
                        position.y+node.LocalPosition.y,
                        node.LocalPosition.width,
                        node.LocalPosition.height);
    }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public iCS_EditorObject GetDataConnectionSource(iCS_EditorObject port) {
        if(port == null || !port.IsDataPort) return null;
        for(iCS_EditorObject sourcePort= GetSource(port); sourcePort != null && sourcePort.IsDataPort; sourcePort= GetSource(port)) {
            port= sourcePort;
        }
        return port;
    }
    
}
