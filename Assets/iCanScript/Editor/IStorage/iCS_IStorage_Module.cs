using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {    
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string EnablePortStr= "enable";
    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateEnablePort(int parentId) {
        iCS_EditorObject enablePort= CreatePort(EnablePortStr, parentId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
        enablePort.IsNameEditable= false;
        return enablePort;
    }
    // ----------------------------------------------------------------------
    public void AddPortToModule(iCS_EditorObject port) {
        iCS_EditorObject module= GetParent(port);
		port.PortIndex= GetSortedChildDataPorts(module).Length;
		SetInitialPortValue(port, iCS_Types.DefaultValue(port.RuntimeType));
    }
    // ----------------------------------------------------------------------
    public void RemovePortFromModule(iCS_EditorObject port) {
        // Reorganize runtime parameter information.
        iCS_EditorObject module= GetParent(port);
        // Rearrange port indexes
        GetSortedChildDataPorts(module);
    }
    
    // ======================================================================
    // Module helpers
    // ----------------------------------------------------------------------
    public bool HasEnablePort(iCS_EditorObject module) {
        return ForEachChildPort(module, p=> p.IsEnablePort);
    }
    public iCS_EditorObject GetEnablePort(iCS_EditorObject module) {
        iCS_EditorObject enablePort= null;
        ForEachChildPort(module, p=> { if(p.IsEnablePort) { enablePort= p; return true; } return false; });
        return enablePort;
    }
}
