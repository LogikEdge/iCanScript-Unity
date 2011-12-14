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
    public bool IsValidUnityObject(int id) { return id < UnityObjects.Count; }
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
    public void SetUnityObject(int id, Object value) {
        if(IsValidUnityObject(id)) UnityObjects[id]= value;
    }
}
