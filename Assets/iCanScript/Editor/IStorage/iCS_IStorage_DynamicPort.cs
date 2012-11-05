using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public void AddDynamicPort(iCS_EditorObject port) {
        iCS_EditorObject parent= GetParent(port);
		port.PortIndex= GetSortedChildDataPorts(parent).Length;
		port.InitialPortValue= iCS_Types.DefaultValue(port.RuntimeType);
    }
    // ----------------------------------------------------------------------
    public void RemoveDynamicPort(iCS_EditorObject port) {
		// Relocate the port at the end.
        iCS_EditorObject parent= GetParent(port);
		port.PortIndex= GetSortedChildDataPorts(parent).Length;
        // Rearrange port indexes
        GetSortedChildDataPorts(parent);
    }
}
