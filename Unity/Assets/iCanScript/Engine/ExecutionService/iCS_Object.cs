using UnityEngine;
using System.Collections;

public class iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
#if UNITY_EDITOR
    public iCS_Storage  Storage     { get; set; }
    public int          InstanceId  { get; set; }
    public string       Name        { get { return Storage.EngineObjects[InstanceId].Name; } }
#endif                              
    public string       TypeName    { get { return GetType().Name; }}
    public int          Priority    { get; set; }

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
