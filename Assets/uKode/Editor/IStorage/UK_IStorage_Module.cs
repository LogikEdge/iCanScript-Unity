using UnityEngine;
using System;
using System.Collections;

public partial class UK_IStorage {    
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string EnablePortStr= "enable";
    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateEnablePort(int parentId) {
        UK_EditorObject enablePort= CreatePort(EnablePortStr, parentId, typeof(bool), UK_ObjectTypeEnum.EnablePort);
        enablePort.IsNameEditable= false;
        return enablePort;
    }
    // ----------------------------------------------------------------------
    public void AddPortToModule(UK_EditorObject port) {
        UK_EditorObject module= GetParent(port);
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(module.RuntimeArchive);
        int len= rtDesc.ParamTypes.Length;
        port.PortIndex= len;
        Array.Resize(ref rtDesc.ParamNames, len+1);
        rtDesc.ParamNames[len]= port.Name;
        Array.Resize(ref rtDesc.ParamTypes, len+1);
        rtDesc.ParamTypes[len]= port.RuntimeType;
        Array.Resize(ref rtDesc.ParamIsOuts, len+1);
        rtDesc.ParamIsOuts[len]= port.IsOutputPort;
        Array.Resize(ref rtDesc.ParamDefaultValues, len+1);
        rtDesc.ParamDefaultValues[len]= rtDesc.ParamIsOuts[len] || port.RuntimeType == typeof(void) ? null : UK_Types.DefaultValue(port.RuntimeType);
        module.RuntimeArchive= rtDesc.Encode(module.InstanceId);
    }
    // ----------------------------------------------------------------------
    public void RemovePortFromModule(UK_EditorObject port) {
        // Reorganize runtime parameter information.
        UK_EditorObject module= GetParent(port);
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(module.RuntimeArchive);
        int idx= port.PortIndex;
        int len= rtDesc.ParamTypes.Length;
        for(int i= idx; i < len-1; ++i) {
            rtDesc.ParamNames[i]= rtDesc.ParamNames[i+1];
            rtDesc.ParamTypes[i]= rtDesc.ParamTypes[i+1];
            rtDesc.ParamIsOuts[i]= rtDesc.ParamIsOuts[i+1];
            rtDesc.ParamDefaultValues[i]= rtDesc.ParamDefaultValues[i+1];
        }
        Array.Resize(ref rtDesc.ParamNames, len-1);
        Array.Resize(ref rtDesc.ParamTypes, len-1);
        Array.Resize(ref rtDesc.ParamIsOuts, len-1);
        Array.Resize(ref rtDesc.ParamDefaultValues, len-1);
        module.RuntimeArchive= rtDesc.Encode(module.InstanceId);
        // Rearrange port indexes
        ForEachChildPort(module, p=> { if(p.PortIndex > idx) --p.PortIndex; });
    }
    
    // ======================================================================
    // Module helpers
    // ----------------------------------------------------------------------
    public bool HasEnablePort(UK_EditorObject module) {
        return ForEachChildPort(module, p=> p.IsEnablePort);
    }
    public UK_EditorObject GetEnablePort(UK_EditorObject module) {
        UK_EditorObject enablePort= null;
        ForEachChildPort(module, p=> { if(p.IsEnablePort) { enablePort= p; return true; } return false; });
        return enablePort;
    }
}
