using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
	public void LoadInitialPortValueFromArchive(iCS_EditorObject port) {
		if(!port.IsInDataPort) return;
		if(port.Source != -1) return;
		if(port.InitialValueArchive == null || port.InitialValueArchive == "") {
			TreeCache[port.InstanceId].InitialValue= null;
			return;
		}
		iCS_Coder coder= new iCS_Coder(port.InitialValueArchive);
		TreeCache[port.InstanceId].InitialValue= coder.DecodeObjectForKey("InitialValue", Storage);
	}
    // ----------------------------------------------------------------------
	public object GetInitialPortValue(iCS_EditorObject port) {
		if(!port.IsInDataPort) return null;
		if(port.Source != -1) return null;
		return TreeCache[port.InstanceId].InitialValue;
	}
    // ----------------------------------------------------------------------
	public void SetInitialPortValue(iCS_EditorObject port, object value) {
		if(!port.IsInDataPort) return;
		if(port.Source != -1) return;
		TreeCache[port.InstanceId].InitialValue= value;
        ArchiveInitialPortValue(port);
	}
    // ----------------------------------------------------------------------
    public void ArchiveInitialPortValue(iCS_EditorObject port) {
        var cache= TreeCache[port.InstanceId];
        if(cache.InitialValue == null) {
            port.InitialValueArchive= null;
            return;
        }
		iCS_Coder coder= new iCS_Coder();
		coder.EncodeObject("InitialValue", cache.InitialValue, Storage);
		port.InitialValueArchive= coder.Archive;         
    }
    // ----------------------------------------------------------------------
	public object GetPortValue(iCS_EditorObject port) {
		if(!port.IsDataPort) return null;
		while(GetSource(port) != null) port= GetSource(port);
		iCS_IParams funcBase= GetRuntimeObject(port) as iCS_IParams;
		if(funcBase != null) {
		    return funcBase.GetParameter(0);
		}
		funcBase= GetRuntimeObject(GetParent(port)) as iCS_IParams;
		return funcBase == null ? GetInitialPortValue(port) : funcBase.GetParameter(port.PortIndex);
	}
    // ----------------------------------------------------------------------
	public void SetPortValue(iCS_EditorObject port, object value) {
		if(!port.IsDataPort) return;
		iCS_IParams funcBase= GetRuntimeObject(port) as iCS_IParams;
        if(funcBase != null) {
            funcBase.SetParameter(0, value);
            return;
        }
		funcBase= GetRuntimeObject(GetParent(port)) as iCS_IParams;
		if(funcBase == null) return;
		funcBase.SetParameter(port.PortIndex, value);
	}
}
