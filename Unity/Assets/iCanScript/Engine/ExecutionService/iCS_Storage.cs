using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO : Should storage be changed to a scriptable object ?
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of iCanScript.  All object are derived
// from this storage class.
[AddComponentMenu("")]
public class iCS_Storage : MonoBehaviour {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
                      public iCS_EngineObject   EngineObject  = null;
    [HideInInspector] public int			    MajorVersion  = iCS_Config.MajorVersion;
    [HideInInspector] public int    		    MinorVersion  = iCS_Config.MinorVersion;
    [HideInInspector] public int    		    BugFixVersion = iCS_Config.BugFixVersion;
    [HideInInspector] public int                UndoRedoId    = 0;
	[HideInInspector] public Vector2		    ScrollPosition= Vector2.zero;
	[HideInInspector] public float  		    GuiScale      = 1f;	
	[HideInInspector] public int    		    SelectedObject= -1;	
    [HideInInspector] public List<Object>       UnityObjects  = new List<Object>();
    [HideInInspector] public string             FileName      = null;
    [HideInInspector] public List<iCS_EngineObject>   EngineObjects = new List<iCS_EngineObject>();

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool IsValidEngineObject(int id) {
		return id >= 0 && id < EngineObjects.Count && EngineObjects[id].InstanceId != -1;
	}
    public bool IsValidUnityObject(int id)  {
		return id >= 0 && id < UnityObjects.Count && UnityObjects[id] != null;
	}

    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public void ClearUnityObjects() {
        UnityObjects.Clear();
    }
    // ----------------------------------------------------------------------
    public int AddUnityObject(Object obj) {
        if(obj == null) return -1;
		// Search for an existing entry.
        int id= 0;
		int availableSlot= -1;
		for(id= 0; id < UnityObjects.Count; ++id) {
			if(UnityObjects[id] == obj) {
				return id;
			}
			if(UnityObjects[id] == null) {
				availableSlot= id;
			}
		}
		if(availableSlot != -1) {
			UnityObjects[availableSlot]= obj;
			return availableSlot;
		}
        UnityObjects.Add(obj);
        return id;
    }
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return (id >= 0 && id < UnityObjects.Count) ? UnityObjects[id] : null;
    }

    // ======================================================================
    // EnginObject Utilities
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetParent(iCS_EngineObject child) {
        if(child == null || child.ParentId == -1) return null;
        return EngineObjects[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
	public iCS_EngineObject GetParentNode(iCS_EngineObject child) {
		var parentNode= GetParent(child);
		while(parentNode != null && !parentNode.IsNode) {
			parentNode= GetParent(parentNode);
		}
		return parentNode;
	}
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetSource(iCS_EngineObject port) {
        if(port == null || port.SourceId == -1) return null;
        return EngineObjects[port.SourceId];
    }
    // ----------------------------------------------------------------------
	public bool IsInModulePort(iCS_EngineObject obj) {
		if(!obj.IsInDataPort) return false;
		var parent= GetParentNode(obj);
		return parent != null && parent.IsKindOfModule;
	}
    // ----------------------------------------------------------------------
	public bool IsOutModulePort(iCS_EngineObject obj) {
		if(!obj.IsOutDataPort) return false;
		var parent= GetParentNode(obj);
		return parent != null && parent.IsKindOfModule;
	}
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public iCS_EngineObject GetDataConnectionSource(iCS_EngineObject port) {
        if(port == null || !port.IsDataPort) return null;
        for(iCS_EngineObject sourcePort= GetSource(port); sourcePort != null && sourcePort.IsDataPort; sourcePort= GetSource(port)) {
            port= sourcePort;
        }
        return port;
    }
    
}
