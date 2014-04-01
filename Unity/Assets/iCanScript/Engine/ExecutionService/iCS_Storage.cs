using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO : Should storage be changed to a scriptable object ?
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of iCanScript.  All object are derived
// from this storage class.
[AddComponentMenu("")]
public class iCS_Storage : ScriptableObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    [HideInInspector] public uint			          MajorVersion       = iCS_Config.MajorVersion;
    [HideInInspector] public uint    		          MinorVersion       = iCS_Config.MinorVersion;
    [HideInInspector] public uint    		          BugFixVersion      = iCS_Config.BugFixVersion;
    [HideInInspector] public int                      DisplayRoot        = -1;	
	[HideInInspector] public int    		          SelectedObject     = -1;
	[HideInInspector] public bool                     ShowDisplayRootNode= true;
	[HideInInspector] public float  		          GuiScale           = 1f;	
	[HideInInspector] public Vector2		          ScrollPosition     = Vector2.zero;
    [HideInInspector] public int                      UndoRedoId         = 0;
//    public int myDisplayRoot= -2;
    [HideInInspector] public List<iCS_EngineObject>   EngineObjects      = new List<iCS_EngineObject>();
    [HideInInspector] public List<Object>             UnityObjects       = new List<Object>();
    [HideInInspector] public iCS_NavigationHistory    NavigationHistory  = new iCS_NavigationHistory();
                      public iCS_EngineObject         EngineObject       = null;
    

//    public int DisplayRoot {
//        get { return myDisplayRoot; }
//        set { Debug.Log("iCanScript: Setting display root from => "+myDisplayRoot+ " to => "+value); myDisplayRoot= value; }
//    }
    
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
    // Duplication Utilities
    // ----------------------------------------------------------------------
    public void CopyTo(iCS_Storage to) {
        Copy(this, to);
    }
    // ----------------------------------------------------------------------
    public static void Copy(iCS_Storage from, iCS_Storage to) {
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
//        to.UnityObjects       = from.UnityObjects;
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
    // Tree Navigation Queries
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
	public string GetFullName(iCS_EngineObject obj) {
		if(obj == null) return "";
		string fullName= "";
		for(; obj != null; obj= GetParentNode(obj)) {
            if( !obj.IsBehaviour ) {
    			fullName= obj.Name+(string.IsNullOrEmpty(fullName) ? "" : "."+fullName);                
            }
		}
		return name+"."+fullName;
	}
	
    // ======================================================================
    // Connection Queries
    // ----------------------------------------------------------------------
    // Returns the immediate source of the port.
    public iCS_EngineObject GetSourcePort(iCS_EngineObject port) {
        if(port == null || port.SourceId == -1) return null;
        return EngineObjects[port.SourceId];
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public iCS_EngineObject GetFirstProviderPort(iCS_EngineObject port) {
        if(port == null) return null;
        int linkLength= 0;
        for(iCS_EngineObject sourcePort= GetSourcePort(port); sourcePort != null; sourcePort= GetSourcePort(port)) {
            port= sourcePort;
            if(++linkLength > 1000) {
                Debug.LogWarning("iCanScript: Circular port connection detected on: "+GetParentNode(port).Name+"."+port.Name);
                return null;                
            }
        }
        return port;
    }
    // ----------------------------------------------------------------------
    // Returns the list of consumer ports.
    public iCS_EngineObject[] GetConsumerPorts(iCS_EngineObject port) {
        if(port == null) return new iCS_EngineObject[0];
        var consumerPorts= new List<iCS_EngineObject>();
        foreach(var obj in EngineObjects) {
            if(obj.IsPort && GetSourcePort(obj) == port) {
                consumerPorts.Add(obj);
            }
        }
        return consumerPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    public bool IsEndPort(iCS_EngineObject port) {
        if(port == null) return false;
        if(!HasASource(port)) return true;
        return !HasADestination(port);
    }
    // ----------------------------------------------------------------------
    public bool IsRelayPort(iCS_EngineObject port) {
        if(port == null) return false;
        return HasASource(port) && HasADestination(port);
    }
    // ----------------------------------------------------------------------
    public bool HasASource(iCS_EngineObject port) {
        var source= GetSourcePort(port);
        return source != null && source != port; 
    }
    // ----------------------------------------------------------------------
    public bool HasADestination(iCS_EngineObject port) {
        return GetConsumerPorts(port).Length != 0;
    }
    
    // ======================================================================
    // EnginObject Utilities
    // ----------------------------------------------------------------------
    // ----------------------------------------------------------------------
	public bool IsInPackagePort(iCS_EngineObject obj) {
		if(!obj.IsInDataOrControlPort) return false;
		var parent= GetParentNode(obj);
		return parent != null && parent.IsKindOfPackage;
	}
    // ----------------------------------------------------------------------
	public bool IsOutPackagePort(iCS_EngineObject obj) {
		if(!obj.IsOutDataOrControlPort) return false;
		var parent= GetParentNode(obj);
		return parent != null && parent.IsKindOfPackage;
	}
    
}
