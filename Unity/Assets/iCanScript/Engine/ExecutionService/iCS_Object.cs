using UnityEngine;
using System.Collections;

public class iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
#if UNITY_EDITOR
    public iCS_Storage      Storage         { get; set; }
    public int              InstanceId      { get; set; }
    public iCS_EngineObject EngineObject    { get { return Storage.EngineObjects[InstanceId]; }}
    public string           Name            { get { return EngineObject.Name; } }
    public iCS_EngineObject ParentObject {
        get {
            var parentId= EngineObject.ParentId;
            return parentId != -1 ? Storage.EngineObjects[parentId] : null;
        }
    }
    public string           ParentName      { get { return ParentObject != null ? ParentObject.Name : null; }}
    public string           FullName    {
        get {
            string fullName= Name;
            for(var parentObject= ParentObject; parentObject != null; parentObject= Storage.GetParent(parentObject)) {
                fullName= parentObject.Name+"."+fullName;
            }
            return Storage.gameObject.name+"."+fullName;
        }
    }
#endif                                  
    public string           TypeName        { get { return GetType().Name; }}
    public int              Priority        { get; set; }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Object(iCS_Storage storage, int instanceId, int priority) {
        Priority= priority;
#if UNITY_EDITOR
        Storage   = storage;
        InstanceId= instanceId;
#endif
    }

    public override string ToString() {
#if UNITY_EDITOR
        return Name;
#else
        return TypeName;
#endif
    }
}
