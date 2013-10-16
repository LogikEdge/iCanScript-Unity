using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {    
    // =========================================================================
    // Trigger Port
    // -------------------------------------------------------------------------
    public iCS_EditorObject CreateTriggerPort(int parentId) {
        var existingTriggerPort= GetTriggerPort(EditorObjects[parentId]);
        if(existingTriggerPort != null) return existingTriggerPort;
        iCS_EditorObject port= CreatePort(iCS_Strings.TriggerPort, parentId, typeof(bool), iCS_ObjectTypeEnum.TriggerPort);
        port.IsNameEditable= false;
        port.InitialPortValue= true;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasTriggerPort(iCS_EditorObject node)  {
        return GetTriggerPort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetTriggerPort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsTriggerPort);
    }

    // ======================================================================
    // Enable Ports
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateEnablePort(int parentId) {
        iCS_EditorObject port= CreatePort(iCS_Strings.EnablePort, parentId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
        port.IsNameEditable= false;
        port.InitialPortValue= true;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasEnablePort(iCS_EditorObject package) {
        return GetEnablePorts(package).Length != 0;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject[] GetEnablePorts(iCS_EditorObject package) {
        return BuildFilteredListOfChildren(c=> c.IsEnablePort, package);
    }

    // =========================================================================
    // Input This Port
    // -------------------------------------------------------------------------
    public bool HasInInstancePort(iCS_EditorObject node)  {
        return GetInInstancePort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetInInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInInstancePort);
    }

    // =========================================================================
    // Input This Port
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateOutInstancePort(int parentId, Type runtimeType) {
        iCS_EditorObject port= CreatePort(iCS_Strings.DefaultInstanceName, parentId, runtimeType,
                                          iCS_ObjectTypeEnum.OutProposedDataPort, (int)iCS_PortIndex.OutThis);
        port.IsNameEditable= false;
        return port;
    }
    // -------------------------------------------------------------------------
    public bool HasOutInstancePort(iCS_EditorObject node)  {
        return GetOutInstancePort(node) != null;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject GetOutInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsOutInstancePort);
    }
    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public void MoveDynamicPortToLastIndex(iCS_EditorObject port) {
		// Relocate the port at the end.
        iCS_EditorObject parent= port.Parent;
		port.PortIndex= RecalculatePortIndexes(parent).Length;
        // Rearrange port indexes
        RecalculatePortIndexes(parent);
    }

	// ======================================================================
	// High-order functions
    // ----------------------------------------------------------------------
	public iCS_EditorObject[] RecalculatePortIndexes(iCS_EditorObject node) {
		List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
		// Get all child data ports.
		ForEachChildDataPort(node, child=> ports.Add(child));
		// Sort child ports according to index.
		iCS_EditorObject[] result= SortPortsOnIndex(ports.ToArray());
		// Find first dynamic or proposed port
		int firstDynamicIdx= 0;
		for(int i= 0; i < result.Length; ++i) {
		    if(result[i].IsParameterDataPort) {
		        firstDynamicIdx= result[i].PortIndex+1;
		    }
		}
		// Re-index dynamic and proposed ports
        for(int i= 0; i < result.Length; ++i) {
            if(result[i].IsDynamicDataPort || result[i].IsProposedDataPort) {
                result[i].PortIndex= firstDynamicIdx++;                
            }
        }
		return result;
	}
    // -------------------------------------------------------------------------
    public int GetNextDynamicOrProposedPortIndex(iCS_EditorObject node) {
        var ports= RecalculatePortIndexes(node);
        int lastIdx= 0;
        for(int i= 0; i < ports.Length; ++i) {
            var p= ports[i];
            if(p.IsDynamicDataPort || p.IsProposedDataPort) {
                lastIdx= p.PortIndex+1;
            }
        }
        return lastIdx;
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject FindInputInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInInstancePort);
    }
    // -------------------------------------------------------------------------
    public iCS_EditorObject FindOutputInstancePort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsOutInstancePort);
    }
    // ----------------------------------------------------------------------
    public static iCS_EditorObject[] SortPortsOnIndex(iCS_EditorObject[] lst) {
        // Find largest dynamic or proposed port.
        int lastIndex= -1;
        foreach(var p in lst) {
			if(p.IsDynamicDataPort || p.IsProposedDataPort) {
	            if(p.PortIndex > lastIndex) {
	                lastIndex= p.PortIndex;
	            }				
			}
        }
        // Assign all unassigned port indexes (we assume that it is a dynmic port).
        if(lastIndex != -1) {
            foreach(var p in lst) {
                if(p.PortIndex < 0) {
                    p.PortIndex= ++lastIndex;
                }
            }
        }
		Array.Sort(lst, (x,y)=> x.PortIndex - y.PortIndex);
        return lst;
    }

}
