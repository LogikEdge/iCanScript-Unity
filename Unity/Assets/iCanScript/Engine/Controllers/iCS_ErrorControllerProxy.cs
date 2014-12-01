using UnityEngine;
using System;
using System.Collections;

public static class iCS_ErrorControllerProxy {
	// ==========================================================================
	// Interface Callbacks
	public static Action<string>									_Clear     = null;
	public static Action<string, string, iCS_VisualScriptImp, int>	_AddError  = null; 
	public static Action<string, string, iCS_VisualScriptImp, int>	_AddWarning= null; 

	// ==========================================================================
	public static void Clear(string serviceId) {
		if(_Clear != null) {
			_Clear(serviceId);
		}
	}
	public static void AddError(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		if(_AddError != null) {
			_AddError(serviceId, message, vs, objectId);
		}
		else {
            Debug.LogWarning("iCanScript: ERROR: "+message);
		}
	}
	public static void AddWarning(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		if(_AddWarning != null) {
			_AddWarning(serviceId, message, vs, objectId);
		}
		else {
            Debug.LogWarning("iCanScript: WARNING: "+message);
		}
	}
}
