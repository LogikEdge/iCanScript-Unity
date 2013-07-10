using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public void AddDynamicPort(iCS_EditorObject port) {
        iCS_EditorObject parent= port.Parent;
		port.PortIndex= RecalculatePortIndexes(parent).Length;
		port.InitialPortValue= iCS_Types.DefaultValue(port.RuntimeType);
    }
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
		iCS_EditorObject[] result= ports.ToArray();
		Array.Sort(result, (x,y)=> x.PortIndex - y.PortIndex);
        for(int i= 0; i < result.Length; ++i) result[i].PortIndex= i;
		return result;
	}
    public iCS_EditorObject FindThisInputPort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInDataPort && c.Name == "this");
    }
}
