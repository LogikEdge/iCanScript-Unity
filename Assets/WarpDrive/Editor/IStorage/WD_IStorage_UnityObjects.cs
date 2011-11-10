using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class WD_IStorage {
    // ======================================================================
    // Unity Object containement functions
    // ----------------------------------------------------------------------
    public bool IsValidUnityObject(int id) { return id < Storage.UnityObjects.Count; }
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return IsValidUnityObject(id) ? Storage.UnityObjects[id] : null;        
    }
    // ----------------------------------------------------------------------
    public void SetUnityObject(int id, Object value) {
        if(IsValidUnityObject(id)) Storage.UnityObjects[id]= value;
    }
    // ----------------------------------------------------------------------
    public T GetUnityObject<T>(int id) where T : Object {
        return GetUnityObject(id) as T;
    }    
    // ----------------------------------------------------------------------
    public int AddUnityObject(Object obj) {
        int id= 0;
        for(; id < Storage.UnityObjects.Count; ++id) {
            if(Storage.UnityObjects[id] == null) {
                Storage.UnityObjects[id]= obj;
                return id;
            }
        }
        Storage.UnityObjects.Add(obj);
        return id;
    }
    // ----------------------------------------------------------------------
    public void RemoveUnityObject(int id) {
        SetUnityObject(id, null);
    }
}
