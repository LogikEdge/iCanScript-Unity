using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {    
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
        for(int i= 0, idx= 0; i < result.Length; ++i, ++idx) {
            if(result[i].IsFixDataPort) {
                idx= result[i].PortIndex;
            } else {
                result[i].PortIndex= idx;                
            }
        }
		return result;
	}
    public iCS_EditorObject FindThisInputPort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInDataOrControlPort && c.Name == iCS_Strings.InstanceObjectName);
    }
    // ----------------------------------------------------------------------
    public static iCS_EditorObject[] SortPortsOnIndex(iCS_EditorObject[] lst) {
        // Find largest positiove index.
        int lastIndex= -1;
        foreach(var p in lst) {
            if(p.PortIndex > lastIndex) {
                lastIndex= p.PortIndex;
            }
        }
        // Assign all unassigned port indexes.
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
