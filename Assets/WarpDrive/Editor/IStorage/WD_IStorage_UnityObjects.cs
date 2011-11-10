using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class WD_IStorage {
    // ======================================================================
    // Unity Object containement functions
    // ----------------------------------------------------------------------
    public bool IsValidUnityObject(int id) { return id < UnityObjects.Count; }
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return IsValidUnityObject(id) ? UnityObjects[id] : null;        
    }
    // ----------------------------------------------------------------------
    public void SetUnityObject(int id, Object value) {
        if(IsValidUnityObject(id)) UnityObjects[id]= value;
    }
    // ----------------------------------------------------------------------
    public T GetUnityObject<T>(int id) where T : Object {
        return GetUnityObject(id) as T;
    }    
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
    public void RemoveUnityObject(int id) {
        SetUnityObject(id, null);
    }
    // ----------------------------------------------------------------------
    public object GetDefaultValue(WD_RuntimeDesc desc, int idx) {
        if(WD_Types.IsA<UnityEngine.Object>(desc.ParamTypes[idx])) {
            object id= desc.ParamDefaultValues[idx];
            if(id == null) return null;
            return GetUnityObject((int)id);
        }
        return desc.ParamDefaultValues[idx];    
    }
    // ----------------------------------------------------------------------
    public void SetDefaultValue(WD_RuntimeDesc desc, int idx, object obj) {
        if(WD_Types.IsA<UnityEngine.Object>(desc.ParamTypes[idx])) {
            object idObj= desc.ParamDefaultValues[idx];
            if(idObj == null) {
                desc.ParamDefaultValues[idx]= AddUnityObject(obj as Object);
                return;
            }
            int id= (int)idObj;
            if(IsValidUnityObject(id)) {
                SetUnityObject(id, obj as Object);
            } else {
                desc.ParamDefaultValues[idx]= AddUnityObject(obj as Object);
            }
            return;
        }
        desc.ParamDefaultValues[idx]= obj;
    }
}
