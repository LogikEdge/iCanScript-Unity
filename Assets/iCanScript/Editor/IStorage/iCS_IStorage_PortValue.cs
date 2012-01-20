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
		TreeCache[port.InstanceId].InitialValue= coder.DecodeObjectForKey("InitialValue");
	}
    // ----------------------------------------------------------------------
	public object GetInitialPortValue(iCS_EditorObject port) {
		if(!port.IsInDataPort) return null;
		if(port.Source != -1) return null;
		object portValue= TreeCache[port.InstanceId].InitialValue;
		// Special case for UnityObjects.
        if(iCS_Types.IsA<UnityEngine.Object>(port.RuntimeType)) {
            if(portValue == null) return null;
            return GetUnityObject((int)portValue);
        }
		return portValue;
	}
    // ----------------------------------------------------------------------
	public void SetInitialPortValue(iCS_EditorObject port, object value) {
		if(!port.IsInDataPort) return;
		if(port.Source != -1) return;
		// Special case for UnityObjects.
        if(iCS_Types.IsA<UnityEngine.Object>(port.RuntimeType)) {
            object idObj= TreeCache[port.InstanceId].InitialValue;
            if(idObj == null) {
                value= AddUnityObject(value as UnityEngine.Object);
            } else {
	            int id= (int)idObj;
	            if(IsValidUnityObject(id)) {
	                value= SetUnityObject(id, value as UnityEngine.Object);
	            } else {
	                value= AddUnityObject(value as UnityEngine.Object);
	            }
	            return;	
			}
        }
		TreeCache[port.InstanceId].InitialValue= value;
		iCS_Coder coder= new iCS_Coder();
		coder.EncodeObject("InitialValue", value);
		port.InitialValueArchive= coder.Archive; 
	}
    // ----------------------------------------------------------------------
	public object GetPortValue(iCS_EditorObject port) {
		if(!port.IsDataPort) return null;
		iCS_FunctionBase funcBase= GetRuntimeObject(GetParent(port)) as iCS_FunctionBase;
		if(funcBase == null) return GetInitialPortValue(port);
		return funcBase[port.PortIndex];
	}
    // ----------------------------------------------------------------------
	public void SetPortValue(iCS_EditorObject port, object value) {
		if(!port.IsDataPort) return;
		iCS_FunctionBase funcBase= GetRuntimeObject(GetParent(port)) as iCS_FunctionBase;
		if(funcBase == null) return;
		funcBase[port.PortIndex]= value;
	}
}
