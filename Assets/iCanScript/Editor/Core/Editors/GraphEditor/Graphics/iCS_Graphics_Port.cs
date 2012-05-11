using UnityEngine;
using System;
using System.Collections;

// ==========================================================================
// Port utilities.
// ==========================================================================
public partial class iCS_Graphics {
    // ======================================================================
    // Common port utilities.
    // ----------------------------------------------------------------------
    Vector2 GetPortCenter(iCS_EditorObject port, iCS_IStorage iStorage) {
        return Math3D.ToVector2(GetDisplayPosition(port, iStorage));
    }
	// ----------------------------------------------------------------------
    string GetValueAsString(object value) {
        if(value is bool) return ((bool)value) ? "true" : "false";
        if(value is float) return ((float)value).ToString();
        if(value is int) return ((int)value).ToString();
        if(value is Vector2) return ((Vector2)value).ToString();
        if(value is Vector3) return ((Vector3)value).ToString();
        if(value is Vector4) return ((Vector4)value).ToString();
        if(value is Color) return ((Color)value).ToString();
        if(value is string) return (string)value;
        if(value is UnityEngine.Object) return (value as UnityEngine.Object).name;
        return null;
    }

    // ======================================================================
    // Port name utilities.
    // ----------------------------------------------------------------------
    string GetPortName(iCS_EditorObject port) {
        Type portValueType= GetPortValueType(port);
        return portValueType.IsArray ? "["+port.Name+"]" : port.Name;
    }
    // ----------------------------------------------------------------------
    Vector2 GetPortNameSize(iCS_EditorObject port) {
        return LabelStyle.CalcSize(new GUIContent(GetPortName(port)));
    }

    // ======================================================================
    // Port type utilities.
    // ----------------------------------------------------------------------
    Type GetPortValueType(iCS_EditorObject port) {
        return iCS_Types.GetElementType(port.RuntimeType);
    }
    object GetPortValue(iCS_EditorObject port, iCS_IStorage iStorage) {
        return iStorage.GetPortValue(port);
    }
    string GetPortValueAsString(iCS_EditorObject port, iCS_IStorage iStorage) {
        object portValue= GetPortValue(port, iStorage);
        return (portValue != null) ? GetValueAsString(portValue) : null;
    }
    Vector2 GetPortValueSize(iCS_EditorObject port, iCS_IStorage iStorage) {
		string valueAsStr= GetPortValueAsString(port, iStorage);
		return (valueAsStr != null && valueAsStr != "") ? ValueStyle.CalcSize(new GUIContent(valueAsStr)) : Vector2.zero;        
    }
}
