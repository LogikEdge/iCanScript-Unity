using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WD_EditorObjectMgr {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool                     IsDirty= true;
    public List<WD_EditorObject>    EditorObjects= new List<WD_EditorObject>();

    // ======================================================================
    // Editor Object Container Management
    // ----------------------------------------------------------------------
    public WD_EditorObject this[int i] {
        get { return EditorObjects[i]; }
    }
    // ----------------------------------------------------------------------
    public void AddObject(WD_Object obj) {
        // Attempt to use an empty slot.
        for(int i= 0; i < EditorObjects.Count; ++i) {
            if(EditorObjects[i].InstanceId == -1) {
                EditorObjects[i].Serialize(obj, i);
                IsDirty= true;
                return;
            }
        }
        // Serialize the given object.
        WD_EditorObject so= new WD_EditorObject();
        so.Serialize(obj, EditorObjects.Count);
        EditorObjects.Add(so);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------
    public void ReplaceObject(WD_Object obj) {
        EditorObjects[obj.InstanceId].Serialize(obj, obj.InstanceId);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------
    public void RemoveObject(int id) {
        if(id < 0 || id >= EditorObjects.Count) return;
        EditorObjects[id].InstanceId= -1;
        IsDirty= true;        
    }
    public void RemoveObject(WD_EditorObject obj) {
        RemoveObject(obj.InstanceId);
    }
    public void RemoveObject(WD_Object obj) {
        RemoveObject(obj.InstanceId);
    }

    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    // Executes the given action if the given object matches the T type.
    public void executeIf<T>(int id, Action<WD_EditorObject> fnc) where T : class {
        if(id < 0 || id >= EditorObjects.Count) return;
        WD_EditorObject obj= EditorObjects[id];
        Type t= Type.GetType(obj.QualifiedType);
        if(t == typeof(T)) fnc(obj);
    }
    public void executeIf<T>(WD_EditorObject obj, Action<WD_EditorObject> fnc) where T : class {
        executeIf<T>(obj.InstanceId, fnc);
    }
}
