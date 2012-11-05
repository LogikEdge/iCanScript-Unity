using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ======================================================================
    // Port Archiving / Initial Value
    // ----------------------------------------------------------------------
	public void LoadInitialPortValueFromArchive(iCS_EditorObject port) {
		if(!port.IsInDataPort) return;
		if(port.SourceId != -1) return;
		if(iCS_Strings.IsEmpty(port.InitialValueArchive)) {
			port.InitialValue= null;
			return;
		}
		iCS_Coder coder= new iCS_Coder(port.InitialValueArchive);
		port.InitialValue= coder.DecodeObjectForKey("InitialValue", Storage);
	}
    // ----------------------------------------------------------------------
    public void StoreInitialPortValueInArchive(iCS_EditorObject port) {
        if(port.InitialValue == null) {
            port.InitialValueArchive= null;
            return;
        }
		iCS_Coder coder= new iCS_Coder();
		coder.EncodeObject("InitialValue", port.InitialValue, Storage);
		port.InitialValueArchive= coder.Archive;         
    }
    // ======================================================================
    // Graph port value services.
    // ----------------------------------------------------------------------
    // Fetches the runtime value if it exists, otherwise returns the initial value
	public object GetPortValue(iCS_EditorObject port) {
		if(!port.IsDataPort) return null;
		while(GetSource(port) != null) port= GetSource(port);
		iCS_IParams funcBase= GetRuntimeObject(port) as iCS_IParams;
		if(funcBase != null) {
		    return funcBase.GetParameter(0);
		}
		funcBase= GetRuntimeObject(GetParent(port)) as iCS_IParams;
		return funcBase == null ? port.InitialPortValue : funcBase.GetParameter(port.PortIndex);
	}
    // ----------------------------------------------------------------------
	public void SetPortValue(iCS_EditorObject port, object newValue) {
		port.InitialPortValue= newValue;
		SetRuntimePortValue(port, newValue);
        SetDirty(GetParent(port));
    }
    

    // ======================================================================
    // Runtime port update.
    // ----------------------------------------------------------------------
    // Sets the runtime port value if it exists.
    public void SetRuntimePortValue(iCS_EditorObject port, object newValue) {
        if(!port.IsInDataPort) return;
        // Just set the port if it has its own runtime.
		iCS_IParams funcBase= GetRuntimeObject(port) as iCS_IParams;
        if(funcBase != null) {
            funcBase.SetParameter(0, newValue);
            return;
        }
        // Propagate value for module port.
        if(port.IsModulePort) {
            iCS_EditorObject[] connectedPorts= FindConnectedPorts(port);
            foreach(var cp in connectedPorts) {
                SetRuntimePortValue(cp, newValue);
            }
            return;
        }
        if(port.PortIndex < 0) return;
        iCS_EditorObject parent= GetParent(port);
        if(parent == null) return;
        // Get runtime object if it exists.
        iCS_IParams runtimeObject= GetRuntimeObject(parent) as iCS_IParams;
        if(runtimeObject == null) return;
        runtimeObject.SetParameter(port.PortIndex, newValue);
    }
    // ----------------------------------------------------------------------
	public object GetRuntimePortValue(iCS_EditorObject port) {
		if(!port.IsDataPort) return null;
		while(GetSource(port) != null) port= GetSource(port);
		iCS_IParams funcBase= GetRuntimeObject(port) as iCS_IParams;
		if(funcBase != null) {
		    return funcBase.GetParameter(0);
		}
		funcBase= GetRuntimeObject(GetParent(port)) as iCS_IParams;
		return funcBase == null ? port.InitialPortValue : null;
	}
	
}
