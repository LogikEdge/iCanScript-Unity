using UnityEngine;
using System.Collections;

public class iCS_Object {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_VisualScriptImp  VisualScript    { get; set; }

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public int InstanceId {
        get {
            if(VisualScript != null) {
                var runtimeNodes= VisualScript.RuntimeNodes;
                for(int i= 0; i < runtimeNodes.Length; ++i) {
                    if(runtimeNodes[i] == this) {
                        return i;
                    }
                }                
            }
            return -1;
        }
    }
    public iCS_EngineObject EngineObject {
        get {
            int id= InstanceId;
            return id != -1 ? VisualScript.EngineObjects[InstanceId] : null;
        }
    }
    public string Name {
        get {
            var eObj= EngineObject;
            return eObj != null ? eObj.Name : "unnamed";
        }
    }
    public int ParentId {
        get {
            var eObj= EngineObject;
            return eObj != null ? EngineObject.ParentId : -1;
        }
    }
    public iCS_EngineObject ParentObject {
        get {
            var parentId= ParentId;
            return parentId != -1 ? VisualScript.EngineObjects[parentId] : null;
        }
    }
    public string ParentName {
        get {
            return ParentObject != null ? ParentObject.Name : "unnamed";
        }
    }
    public string FullName {
        get {
			return VisualScript.GetFullName(EngineObject);
        }
    }
    public string           TypeName        { get { return GetType().Name; }}
    public int              Priority        { get; set; }

#if UNITY_EDITOR
    public iCS_EngineObject GetPortWithIndex(int idx) {
        var ourId= InstanceId;
        for(int i= 0; i < VisualScript.EngineObjects.Count; ++i) {
            var eObj= VisualScript.EngineObjects[i];
            if(eObj != null && eObj.InstanceId != -1) {
                if(eObj.IsPort && eObj.ParentId == ourId && eObj.PortIndex == idx) {
                    return eObj;
                }
            }
        }
        return null;
    }
#endif
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Object(iCS_VisualScriptImp visualScript, int priority) {
        Priority= priority;
        VisualScript= visualScript;
    }

    public override string ToString() {
#if UNITY_EDITOR
        return Name;
#else
        return TypeName;
#endif
    }
}
