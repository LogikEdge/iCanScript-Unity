using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class iCS_VisualScriptData {
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    public static bool IsValidEngineObject(iCS_IVisualScriptEngineData vsd, int id) {
        var engineObjects= vsd.GetEngineObjects();
		return id >= 0 && id < engineObjects.Count && engineObjects[id].InstanceId != -1;
	}
    public static bool IsValidUnityObject(iCS_IVisualScriptEngineData vsd, int id)  {
        var unityObjects= vsd.GetUnityObjects();
		return id >= 0 && id < unityObjects.Count && unityObjects[id] != null;
	}

    // ======================================================================
    // Duplication Utilities
    // ----------------------------------------------------------------------
    public static void CopyFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
        CopyDataFromTo(from, to);
        CopyEditorDataFromTo(from, to);
    }

    // ----------------------------------------------------------------------
    public static void CopyDataFromTo(iCS_IVisualScriptEngineData from, iCS_IVisualScriptEngineData to) {
        to.SetHostName(from.GetHostName());
        to.SetMajorVersion(from.GetMajorVersion());
        to.SetMinorVersion(from.GetMinorVersion());
        to.SetBugFixVersion(from.GetBugFixVersion());
        to.SetUndoRedoId(from.GetUndoRedoId());
        
        // Resize destination engine object array.
        var fromEngineObjects= from.GetEngineObjects();
        var toEngineObjects= to.GetEngineObjects();
        int fromLen= fromEngineObjects.Count;
        int toLen= toEngineObjects.Count;
        if(toLen > fromLen) {
            toEngineObjects.RemoveRange(fromLen, toLen-fromLen);
        }
        toEngineObjects.Capacity= fromLen;
        // Copy engine objects.
        for(int i= 0; i < fromLen; ++i) {
            var fromObj= fromEngineObjects[i];
            if(fromObj == null) fromObj= iCS_EngineObject.CreateInvalidInstance();
            if(toEngineObjects.Count <= i) {
                toEngineObjects.Add(fromObj.Clone());
            }
            else if(toEngineObjects[i] == null) {
                toEngineObjects[i]= fromObj.Clone();                
            }
            else {
                toEngineObjects[i]= fromObj.CopyTo(toEngineObjects[i]);                                
            }
        }            
        // Resize Unity object reference array
        var fromUnityObjects= from.GetUnityObjects();
        var toUnityObjects= to.GetUnityObjects();
        fromLen= fromUnityObjects.Count;
        toLen= toUnityObjects.Count;
        if(toLen > fromLen) {
            toUnityObjects.RemoveRange(fromLen, toLen-fromLen);
        }
        toUnityObjects.Capacity= fromLen;
        // Copy Unity Object references.
        for(int i= 0; i < fromLen; ++i) {
            var fromObj= fromUnityObjects[i];
            if(toUnityObjects.Count <= i) {
                toUnityObjects.Add(fromObj);
            }
            else {
                toUnityObjects[i]= fromObj;                
            }
        }
    }
    // ----------------------------------------------------------------------
    public static void CopyEditorDataFromTo(iCS_IVisualScriptEditorData from, iCS_IVisualScriptEditorData to) {
        to.SetShowDisplayRootNode(from.GetShowDisplayRootNode());
        to.SetScrollPosition(from.GetScrollPosition());
        to.SetGuiScale(from.GetGuiScale());
        to.SetSelectedObject(from.GetSelectedObject());
        to.SetDisplayRoot(from.GetDisplayRoot());
        // Copy navigation history
        to.GetNavigationHistory().CopyFrom(from.GetNavigationHistory());                    
    }
    
    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public static void ClearUnityObjects(iCS_IVisualScriptEngineData vsd) {
        vsd.GetUnityObjects().Clear();
    }
    // ----------------------------------------------------------------------
    public static int AddUnityObject(iCS_IVisualScriptEngineData vsd, Object obj) {
        if(obj == null) return -1;
		// Search for an existing entry.
        int id= 0;
		int availableSlot= -1;
        var unityObjects= vsd.GetUnityObjects();
		for(id= 0; id < unityObjects.Count; ++id) {
			if(unityObjects[id] == obj) {
				return id;
			}
			if(unityObjects[id] == null) {
				availableSlot= id;
			}
		}
		if(availableSlot != -1) {
			unityObjects[availableSlot]= obj;
			return availableSlot;
		}
        unityObjects.Add(obj);
        return id;
    }
    // ----------------------------------------------------------------------
    public static Object GetUnityObject(iCS_IVisualScriptEngineData vsd, int id) {
        var unityObjects= vsd.GetUnityObjects();
        return (id >= 0 && id < unityObjects.Count) ? unityObjects[id] : null;
    }

    // ======================================================================
    // Tree Navigation Queries
    // ----------------------------------------------------------------------
    public static iCS_EngineObject GetParent(iCS_IVisualScriptEngineData vsd, iCS_EngineObject child) {
        if(child == null || child.ParentId == -1) return null;
        return vsd.GetEngineObjects()[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
	public static iCS_EngineObject GetParentNode(iCS_IVisualScriptEngineData vsd, iCS_EngineObject child) {
		var parentNode= GetParent(vsd, child);
		while(parentNode != null && !parentNode.IsNode) {
			parentNode= GetParent(vsd, parentNode);
		}
		return parentNode;
	}
    // ----------------------------------------------------------------------
	public static string GetFullName(iCS_IVisualScriptEngineData vsd, iCS_EngineObject obj) {
		if(obj == null) return "";
		string fullName= "";
		for(; obj != null; obj= GetParentNode(vsd, obj)) {
            if( !obj.IsBehaviour ) {
    			fullName= obj.Name+(string.IsNullOrEmpty(fullName) ? "" : "."+fullName);                
            }
		}
		return vsd.GetHostName()+"."+fullName;
	}
	
    // ======================================================================
    // Connection Queries
    // ----------------------------------------------------------------------
    // Returns the immediate source of the port.
    public static iCS_EngineObject GetSourcePort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        if(port == null || port.SourceId == -1) return null;
        return vsd.GetEngineObjects()[port.SourceId];
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public static iCS_EngineObject GetFirstProviderPort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        if(port == null) return null;
        int linkLength= 0;
        for(iCS_EngineObject sourcePort= GetSourcePort(vsd, port); sourcePort != null; sourcePort= GetSourcePort(vsd, port)) {
            port= sourcePort;
            if(++linkLength > 1000) {
                Debug.LogWarning("iCanScript: Circular port connection detected on: "+GetParentNode(vsd, port).Name+"."+port.Name);
                return null;                
            }
        }
        return port;
    }
    // ----------------------------------------------------------------------
    // Returns the list of consumer ports.
    public static iCS_EngineObject[] GetConsumerPorts(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        if(port == null) return new iCS_EngineObject[0];
        var consumerPorts= new List<iCS_EngineObject>();
        var engineObjects= vsd.GetEngineObjects();
        foreach(var obj in engineObjects) {
            if(obj.IsPort && GetSourcePort(vsd, obj) == port) {
                consumerPorts.Add(obj);
            }
        }
        return consumerPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    public static bool IsEndPort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        if(port == null) return false;
        if(!HasASource(vsd, port)) return true;
        return !HasADestination(vsd, port);
    }
    // ----------------------------------------------------------------------
    public static bool IsRelayPort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        if(port == null) return false;
        return HasASource(vsd, port) && HasADestination(vsd, port);
    }
    // ----------------------------------------------------------------------
    public static bool HasASource(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        var source= GetSourcePort(vsd, port);
        return source != null && source != port; 
    }
    // ----------------------------------------------------------------------
    public static bool HasADestination(iCS_IVisualScriptEngineData vsd, iCS_EngineObject port) {
        return GetConsumerPorts(vsd, port).Length != 0;
    }
    
    // ======================================================================
    // EnginObject Utilities
    // ----------------------------------------------------------------------
	public static bool IsInPackagePort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject obj) {
		if(!obj.IsInDataOrControlPort) return false;
		var parent= GetParentNode(vsd, obj);
		return parent != null && parent.IsKindOfPackage;
	}
    // ----------------------------------------------------------------------
	public static bool IsOutPackagePort(iCS_IVisualScriptEngineData vsd, iCS_EngineObject obj) {
		if(!obj.IsOutDataOrControlPort) return false;
		var parent= GetParentNode(vsd, obj);
		return parent != null && parent.IsKindOfPackage;
	}
    
}
