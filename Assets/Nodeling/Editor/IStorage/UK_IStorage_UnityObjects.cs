using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
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
    public object GetDefaultValue(UK_RuntimeDesc desc, int portId) {
        if(UK_Types.IsA<UnityEngine.Object>(desc.PortTypes[portId])) {
            object id= desc.PortDefaultValues[portId];
            if(id == null) return null;
            return GetUnityObject((int)id);
        }
        return desc.PortDefaultValues[portId];    
    }
    // ----------------------------------------------------------------------
    public void SetDefaultValue(UK_RuntimeDesc desc, int portId, object obj) {
        if(UK_Types.IsA<UnityEngine.Object>(desc.PortTypes[portId])) {
            object idObj= desc.PortDefaultValues[portId];
            if(idObj == null) {
                desc.PortDefaultValues[portId]= AddUnityObject(obj as Object);
                return;
            }
            int id= (int)idObj;
            if(IsValidUnityObject(id)) {
                SetUnityObject(id, obj as Object);
            } else {
                desc.PortDefaultValues[portId]= AddUnityObject(obj as Object);
            }
            return;
        }
        desc.PortDefaultValues[portId]= obj;
    }
}
