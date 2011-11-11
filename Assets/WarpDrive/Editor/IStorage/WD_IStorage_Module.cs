using UnityEngine;
using System;
using System.Collections;

public partial class WD_IStorage {    
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string EnablePortStr= "enable";
    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateEnablePort(int parentId) {
        WD_EditorObject enablePort= CreatePort(EnablePortStr, parentId, typeof(bool), WD_ObjectTypeEnum.EnablePort);
        enablePort.IsNameEditable= false;
        return enablePort;
    }
    // ----------------------------------------------------------------------
    public void AddPortToModule(WD_EditorObject port) {
        WD_EditorObject module= GetParent(port);
        WD_RuntimeDesc rtDesc= new WD_RuntimeDesc(module.RuntimeArchive);
        int len= rtDesc.ParamTypes.Length;
        port.PortIndex= len;
        Array.Resize(ref rtDesc.ParamNames, len+1);
        rtDesc.ParamNames[len]= port.Name;
        Array.Resize(ref rtDesc.ParamTypes, len+1);
        rtDesc.ParamTypes[len]= port.RuntimeType;
        Array.Resize(ref rtDesc.ParamIsOuts, len+1);
        rtDesc.ParamIsOuts[len]= port.IsOutputPort;
        Array.Resize(ref rtDesc.ParamDefaultValues, len+1);
        rtDesc.ParamDefaultValues[len]= rtDesc.ParamIsOuts[len] ? null : WD_Types.DefaultValue(port.RuntimeType);
        module.RuntimeArchive= rtDesc.Encode(module.InstanceId);
    }
    // ----------------------------------------------------------------------
    public void RemovePortFromModule(WD_EditorObject port) {
        // Reorganize runtime parameter information.
        WD_EditorObject module= GetParent(port);
        WD_RuntimeDesc rtDesc= new WD_RuntimeDesc(module.RuntimeArchive);
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
    public bool HasEnablePort(WD_EditorObject module) {
        return ForEachChildPort(module, p=> p.IsEnablePort);
    }
    public WD_EditorObject GetEnablePort(WD_EditorObject module) {
        WD_EditorObject enablePort= null;
        ForEachChildPort(module, p=> { if(p.IsEnablePort) { enablePort= p; return true; } return false; });
        return enablePort;
    }
}
