using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class iCS_VisualScriptData {
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    public static bool IsValidEngineObject(iCS_IVisualScriptData vsd, int id) {
        var engineObjects= vsd.GetEngineObjects();
		return id >= 0 && id < engineObjects.Count && engineObjects[id].InstanceId != -1;
	}
    public static bool IsValidUnityObject(iCS_IVisualScriptData vsd, int id)  {
        var unityObjects= vsd.GetUnityObjects();
		return id >= 0 && id < unityObjects.Count && unityObjects[id] != null;
	}

    // ======================================================================
    // Duplication Utilities
    // ----------------------------------------------------------------------
    public static void CopyFromTo(iCS_StorageImp from, iCS_StorageImp to) {
        to.name               = from.name;
        to.ShowDisplayRootNode= from.ShowDisplayRootNode;
        to.EngineObject       = from.EngineObject;
        to.MajorVersion       = from.MajorVersion;
        to.MinorVersion       = from.MinorVersion;
        to.BugFixVersion      = from.BugFixVersion;
        to.UndoRedoId         = from.UndoRedoId;
        to.ScrollPosition     = from.ScrollPosition;
        to.GuiScale           = from.GuiScale;
        to.SelectedObject     = from.SelectedObject;
        to.DisplayRoot        = from.DisplayRoot;
        // Resize destination engine object array.
        int fromLen= from.EngineObjects.Count;
        int toLen= to.EngineObjects.Count;
        if(toLen > fromLen) {
            to.EngineObjects.RemoveRange(fromLen, toLen-fromLen);
        }
        to.EngineObjects.Capacity= fromLen;
        // Copy engine objects.
        for(int i= 0; i < fromLen; ++i) {
            var fromObj= from.EngineObjects[i];
            if(fromObj == null) fromObj= iCS_EngineObject.CreateInvalidInstance();
            if(to.EngineObjects.Count <= i) {
                to.EngineObjects.Add(fromObj.Clone());
            }
            else if(to.EngineObjects[i] == null) {
                to.EngineObjects[i]= fromObj.Clone();                
            }
            else {
                to.EngineObjects[i]= fromObj.CopyTo(to.EngineObjects[i]);                                
            }
        }            
        // Resize Unity object reference array
        fromLen= from.UnityObjects.Count;
        toLen= to.UnityObjects.Count;
        if(toLen > fromLen) {
            to.UnityObjects.RemoveRange(fromLen, toLen-fromLen);
        }
        to.UnityObjects.Capacity= fromLen;
        // Copy Unity Object references.
        for(int i= 0; i < fromLen; ++i) {
            var fromObj= from.UnityObjects[i];
            if(to.UnityObjects.Count <= i) {
                to.UnityObjects.Add(fromObj);
            }
            else {
                to.UnityObjects[i]= fromObj;                
            }
        }
        // Copy navigation history
        to.NavigationHistory.CopyFrom(from.NavigationHistory);                    
    }

    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public static void ClearUnityObjects(iCS_IVisualScriptData vsd) {
        vsd.GetUnityObjects().Clear();
    }
    // ----------------------------------------------------------------------
    public static int AddUnityObject(iCS_IVisualScriptData vsd, Object obj) {
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
    public static Object GetUnityObject(iCS_IVisualScriptData vsd, int id) {
        var unityObjects= vsd.GetUnityObjects();
        return (id >= 0 && id < unityObjects.Count) ? unityObjects[id] : null;
    }

    // ======================================================================
    // Tree Navigation Queries
    // ----------------------------------------------------------------------
    public static iCS_EngineObject GetParent(iCS_IVisualScriptData vsd, iCS_EngineObject child) {
        if(child == null || child.ParentId == -1) return null;
        return vsd.GetEngineObjects()[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
	public static iCS_EngineObject GetParentNode(iCS_IVisualScriptData vsd, iCS_EngineObject child) {
		var parentNode= GetParent(vsd, child);
		while(parentNode != null && !parentNode.IsNode) {
			parentNode= GetParent(vsd, parentNode);
		}
		return parentNode;
	}
    // ----------------------------------------------------------------------
	public static string GetFullName(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
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
    public static iCS_EngineObject GetSourcePort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null || port.SourceId == -1) return null;
        return vsd.GetEngineObjects()[port.SourceId];
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public static iCS_EngineObject GetFirstProviderPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
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
    public static iCS_EngineObject[] GetConsumerPorts(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
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
    public static bool IsEndPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null) return false;
        if(!HasASource(vsd, port)) return true;
        return !HasADestination(vsd, port);
    }
    // ----------------------------------------------------------------------
    public static bool IsRelayPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null) return false;
        return HasASource(vsd, port) && HasADestination(vsd, port);
    }
    // ----------------------------------------------------------------------
    public static bool HasASource(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        var source= GetSourcePort(vsd, port);
        return source != null && source != port; 
    }
    // ----------------------------------------------------------------------
    public static bool HasADestination(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        return GetConsumerPorts(vsd, port).Length != 0;
    }
    
    // ======================================================================
    // EnginObject Utilities
    // ----------------------------------------------------------------------
	public static bool IsInPackagePort(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
		if(!obj.IsInDataOrControlPort) return false;
		var parent= GetParentNode(vsd, obj);
		return parent != null && parent.IsKindOfPackage;
	}
    // ----------------------------------------------------------------------
	public static bool IsOutPackagePort(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
		if(!obj.IsOutDataOrControlPort) return false;
		var parent= GetParentNode(vsd, obj);
		return parent != null && parent.IsKindOfPackage;
	}
    
}
