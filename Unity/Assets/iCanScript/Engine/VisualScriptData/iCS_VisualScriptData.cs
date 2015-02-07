using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

// ==========================================================================
// The iCS_VisualScriptData class is divided into an instance section and
// a utility section.
//
// The instance section can be used to create mementos of the visual
// script data for caching and duplication purposes.
//
// The Utility section consists of class function used to manipulate the
// visual script data.
//
public partial class iCS_VisualScriptData : iCS_IVisualScriptData {
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //               VISUAL SCRIPT DATA INSTANCE SECTION
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string                   SourceFileGUID        = null;
    public int			            MajorVersion          = iCS_Config.MajorVersion;
    public int    		            MinorVersion          = iCS_Config.MinorVersion;
    public int    		            BugFixVersion         = iCS_Config.BugFixVersion;
    public int                      DisplayRoot           = -1;	
	public int    		            SelectedObject        = -1;
    public Vector2                  SelectedObjectPosition= Vector2.zero;
	public bool                     ShowDisplayRootNode   = true;
	public float  		            GuiScale              = 1f;	
	public Vector2		            ScrollPosition        = Vector2.zero;
    public int                      UndoRedoId            = 0;
    public List<iCS_EngineObject>   EngineObjects         = new List<iCS_EngineObject>();
    public List<UnityEngine.Object> UnityObjects          = new List<UnityEngine.Object>();
    public iCS_NavigationHistory    NavigationHistory     = new iCS_NavigationHistory();
    

    // ======================================================================
    // Visual Script Data Interface Implementation
    // ----------------------------------------------------------------------
    string iCS_IVisualScriptData.SourceFileGUID {
        get { return SourceFileGUID; }
        set { SourceFileGUID= value; }
    }
    int iCS_IVisualScriptData.MajorVersion {
        get { return MajorVersion; }
        set { MajorVersion= value; }
    }
    int iCS_IVisualScriptData.MinorVersion {
        get { return MinorVersion; }
        set { MinorVersion= value; }
    }
    int iCS_IVisualScriptData.BugFixVersion {
        get { return BugFixVersion; }
        set { BugFixVersion= value; }
    }
    List<iCS_EngineObject>  iCS_IVisualScriptData.EngineObjects {
        get { return EngineObjects; }
    }
    List<UnityEngine.Object> iCS_IVisualScriptData.UnityObjects {
        get { return UnityObjects; }
    }
    int iCS_IVisualScriptData.UndoRedoId {
        get { return UndoRedoId; }
        set { UndoRedoId= value; }
    }
    
    // ======================================================================
    // Visual Script Editor Data Interface Implementation
    // ----------------------------------------------------------------------
    int iCS_IVisualScriptData.DisplayRoot {
        get { return DisplayRoot; }
        set { DisplayRoot= value; }
    }
    int iCS_IVisualScriptData.SelectedObject {
        get { return SelectedObject; }
        set { SelectedObject= value; }
    }
    Vector2 iCS_IVisualScriptData.SelectedObjectPosition {
        get { return SelectedObjectPosition; }
        set { SelectedObjectPosition= value; }
    }
    bool iCS_IVisualScriptData.ShowDisplayRootNode {
        get { return ShowDisplayRootNode; }
        set { ShowDisplayRootNode= value; }
    }
    float iCS_IVisualScriptData.GuiScale {
        get { return GuiScale; }
        set { GuiScale= value; }
    }
    Vector2 iCS_IVisualScriptData.ScrollPosition {
        get { return ScrollPosition; }
        set { ScrollPosition= value; }
    }
    iCS_NavigationHistory iCS_IVisualScriptData.NavigationHistory {
        get { return NavigationHistory; }
    }

    // ======================================================================
    // Builders
    // ----------------------------------------------------------------------
    public iCS_VisualScriptData(/*UnityEngine.Object host*/) {
//        HostObject= host;
    }
    public iCS_VisualScriptData(/*UnityEngine.Object host, */iCS_IVisualScriptData vsd) {
//        HostObject= host;
        iCS_VisualScriptData.CopyFromTo(vsd, this);
    }

    // ======================================================================
    // Instance Utility functions.
    // ----------------------------------------------------------------------
    public bool IsValidEngineObject(int id) {
        return IsValidEngineObject(this, id);
	}
    public bool IsValidUnityObject(int id)  {
        return IsValidUnityObject(this, id);
	}

    // ======================================================================
    // Duplication Utilities
    // ----------------------------------------------------------------------
    public void CopyTo(iCS_IVisualScriptData to) {
        CopyFromTo(this, to);
    }

    // ----------------------------------------------------------------------
    public void CopyDataTo(iCS_IVisualScriptData to) {
        CopyDataFromTo(this, to);
    }
    // ----------------------------------------------------------------------
    public void CopyEditorDataTo(iCS_IVisualScriptData to) {
        CopyEditorDataFromTo(this, to);
    }
    
    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public void ClearUnityObjects() {
        ClearUnityObjects(this);
    }
    // ----------------------------------------------------------------------
    public int AddUnityObject(UnityEngine.Object obj) {
        return AddUnityObject(this, obj);
    }
    // ----------------------------------------------------------------------
    public UnityEngine.Object GetUnityObject(int id) {
        return GetUnityObject(this, id);
    }

    // ======================================================================
    // Tree Navigation Queries
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetParent(iCS_EngineObject child) {
        return GetParent(this, child);
    }
    // ----------------------------------------------------------------------
	public iCS_EngineObject GetParentNode(iCS_EngineObject child) {
        return GetParentNode(this, child);
	}
    // ----------------------------------------------------------------------
	public string GetFullName(UnityEngine.Object host, iCS_EngineObject obj) {
        return GetFullName(this, host, obj);
	}
	
    // ======================================================================
    // Connection Queries
    // ----------------------------------------------------------------------
    // Returns the immediate source of the port.
    public iCS_EngineObject GetSourcePort(iCS_EngineObject port) {
        return GetSourcePort(this, port);
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public iCS_EngineObject GetFirstProducerPort(iCS_EngineObject port) {
        return GetFirstProducerPort(this, port);
    }
    // ----------------------------------------------------------------------
    // Returns the list of consumer ports.
    public iCS_EngineObject[] GetConsumerPorts(iCS_EngineObject port) {
        return GetConsumerPorts(this, port);
    }
    // ----------------------------------------------------------------------
    public bool IsEndPort(iCS_EngineObject port) {
        return IsEndPort(this, port);
    }
    // ----------------------------------------------------------------------
    public bool IsRelayPort(iCS_EngineObject port) {
        return IsRelayPort(this, port);
    }
    // ----------------------------------------------------------------------
    public bool HasASource(iCS_EngineObject port) {
        return HasASource(this, port);
    }
    // ----------------------------------------------------------------------
    public bool HasADestination(iCS_EngineObject port) {
        return HasADestination(this, port);
    }
    
    // ======================================================================
    // EngineObject Utilities
    // ----------------------------------------------------------------------
	public bool IsInPackagePort(iCS_EngineObject obj) {
        return IsInPackagePort(this, obj);
	}
    // ----------------------------------------------------------------------
	public bool IsOutPackagePort(iCS_EngineObject obj) {
        return IsOutPackagePort(this, obj);
    }


    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //               VISUAL SCRIPT DATA UTILITY SECTION
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    
    // ======================================================================
    // Initialize / Destroy
    // ----------------------------------------------------------------------
    public static void DestroyEngineObject(iCS_IVisualScriptData vsd, iCS_EngineObject toDelete) {
        // -- Disconnect all connected ports --
        if(toDelete.IsPort) {
            int id= toDelete.InstanceId;
            if(id != -1) {
                FilterWith(o=> o.IsPort && o.SourceId == id, p=> p.SourceId= -1, vsd);
            }
        }
        // -- Destroy the instance --
        toDelete.DestroyInstance();
    }
    // ----------------------------------------------------------------------
    public static void AddEngineObject(iCS_IVisualScriptData vsd, iCS_EngineObject toAdd) {
        // Try to find an available empty slot.
        int emptySlot= P.findFirst(o=> o.InstanceId == -1, (i,o)=> i, -1, vsd.EngineObjects);
        
        // Grow engine object array if no free slot exists.
        if(emptySlot != -1) {
			toAdd.InstanceId= emptySlot;
            vsd.EngineObjects[emptySlot]= toAdd;
            return;
        }
		toAdd.InstanceId= P.length(vsd.EngineObjects);
        vsd.EngineObjects.Add(toAdd);
    }
    
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    public static bool IsValidEngineObject(iCS_IVisualScriptData vsd, int id) {
        var engineObjects= vsd.EngineObjects;
		return id >= 0 && id < engineObjects.Count && engineObjects[id].InstanceId != -1;
	}
    public static bool IsValidUnityObject(iCS_IVisualScriptData vsd, int id)  {
        var unityObjects= vsd.UnityObjects;
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
    public static void CopyDataFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
        to.SourceFileGUID= from.SourceFileGUID;
        to.MajorVersion  = from.MajorVersion;
        to.MinorVersion  = from.MinorVersion;
        to.BugFixVersion = from.BugFixVersion;
        to.UndoRedoId    = from.UndoRedoId;
        
        // Resize destination engine object array.
        var fromEngineObjects= from.EngineObjects;
        var toEngineObjects= to.EngineObjects;
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
        var fromUnityObjects= from.UnityObjects;
        var toUnityObjects= to.UnityObjects;
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
    public static void CopyEditorDataFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
        to.ShowDisplayRootNode   = from.ShowDisplayRootNode;
        to.ScrollPosition        = from.ScrollPosition;
        to.GuiScale              = from.GuiScale;
        to.SelectedObject        = from.SelectedObject;
        to.SelectedObjectPosition= from.SelectedObjectPosition;
        to.DisplayRoot           = from.DisplayRoot;
        // Copy navigation history
        to.NavigationHistory.CopyFrom(from.NavigationHistory);                    
    }
    
    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public static void ClearUnityObjects(iCS_IVisualScriptData vsd) {
        vsd.UnityObjects.Clear();
    }
    // ----------------------------------------------------------------------
    public static int AddUnityObject(iCS_IVisualScriptData vsd, UnityEngine.Object obj) {
        if(obj == null) return -1;
		// Search for an existing entry.
        var unityObjects= vsd.UnityObjects;
		for(int id= 0; id < unityObjects.Count; ++id) {
			if(unityObjects[id] == obj) {
				return id;
			}
			if(unityObjects[id] == null) {
    			unityObjects[id]= obj;
                return id;
			}
		}
        unityObjects.Add(obj);
        return unityObjects.Count-1;
    }
    // ----------------------------------------------------------------------
    public static UnityEngine.Object GetUnityObject(iCS_IVisualScriptData vsd, int id) {
        var unityObjects= vsd.UnityObjects;
        return (id >= 0 && id < unityObjects.Count) ? unityObjects[id] : null;
    }

    // ======================================================================
    // Tree Navigation Queries
    // ----------------------------------------------------------------------
    public static iCS_EngineObject GetParent(iCS_IVisualScriptData vsd, iCS_EngineObject child) {
        if(child == null || child.ParentId == -1) return null;
        return vsd.EngineObjects[child.ParentId]; 
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
    /// Returns the full name of the object including the name of the game object.
	public static string GetFullName(iCS_IVisualScriptData vsd, UnityEngine.Object host, iCS_EngineObject obj) {
		return host.name+"."+GetRelativeName(vsd, obj);
	}
    // ----------------------------------------------------------------------
    /// Returns the relative name of the object reference by the visual script.
	public static string GetRelativeName(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
		if(obj == null) return "";
		string fullName= "";
		for(; obj != null; obj= GetParentNode(vsd, obj)) {
            if( !obj.IsBehaviour ) {
    			fullName= obj.Name+(string.IsNullOrEmpty(fullName) ? "" : "."+fullName);                
            }
		}
		return fullName;
	}
    
    // ======================================================================
    // Connection Queries
    // ----------------------------------------------------------------------
    // Returns the immediate source of the port.
    public static iCS_EngineObject GetSourcePort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null || port.SourceId == -1) return null;
        return vsd.EngineObjects[port.SourceId];
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public static iCS_EngineObject GetFirstProducerPort(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null || port.InstanceId == -1) return null;
        int linkLength= 0;
        for(iCS_EngineObject sourcePort= GetSourcePort(vsd, port); sourcePort != null; sourcePort= GetSourcePort(vsd, port)) {
            port= sourcePort;
            if(++linkLength > 1000) {
                Debug.LogWarning("iCanScript: Circular port connection detected on: "+GetParentNode(vsd, port).Name+"."+port.Name);
                return null;                
            }
        }
        return IsValid(port, vsd) ? port : null;
    }
    // ----------------------------------------------------------------------
    // Returns the list of consumer ports.
    public static iCS_EngineObject[] GetConsumerPorts(iCS_IVisualScriptData vsd, iCS_EngineObject port) {
        if(port == null) return new iCS_EngineObject[0];
        var consumerPorts= new List<iCS_EngineObject>();
        var engineObjects= vsd.EngineObjects;
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
    // EngineObject Utilities
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
    
    // ======================================================================
    // General Queries
    // ----------------------------------------------------------------------
    public static iCS_EngineObject[] GetChildPorts(iCS_IVisualScriptData vsd, iCS_EngineObject node) {
        List<iCS_EngineObject> childPorts= new List<iCS_EngineObject>();
        FilterWith(p=> p.IsPort && p.ParentId == node.InstanceId, childPorts.Add, vsd);
        return childPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    public static iCS_EngineObject GetChildPortWithIndex(iCS_IVisualScriptData vsd, iCS_EngineObject node, int index) {
        iCS_EngineObject port= null;
        FilterWith(p=> p.IsPort && p.ParentId == node.InstanceId && p.PortIndex == index, fp=> port= fp, vsd);
        return port;
    }
    // ----------------------------------------------------------------------
    public static iCS_EngineObject GetInInstancePort(iCS_IVisualScriptData vsd, iCS_EngineObject node) {
        return GetChildPortWithIndex(vsd, node, (int)iCS_PortIndex.InInstance);
    }
}
