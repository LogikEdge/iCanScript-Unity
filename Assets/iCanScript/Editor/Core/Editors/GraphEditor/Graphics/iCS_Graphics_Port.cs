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
    // Returns the port center in graph coordinates.
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
    bool ShouldDisplayPortName(iCS_EditorObject port, iCS_IStorage iStorage) {
        if(!ShouldShowLabel()) return false;
        if(!IsVisible(port, iStorage)) return false;
        return true;        
    }
    // ----------------------------------------------------------------------
    // Returns the port name size in GUI scale.
    Vector2 GetPortNameSize(iCS_EditorObject port) {
        return LabelStyle.CalcSize(new GUIContent(GetPortName(port)));
    }
    // ----------------------------------------------------------------------
    // Returns the port name position in graph coordinate and GUI scale size.
    Rect GetPortNamePosition(iCS_EditorObject port, iCS_IStorage iStorage) {
        Vector2 labelSize= GetPortNameSize(port);
		Vector2 labelPos= GetPortCenter(port, iStorage);
        switch(port.Edge) {
            case iCS_EditorObject.EdgeEnum.Left:
                labelPos.x+= 1 + iCS_Config.PortSize;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Right:
                labelPos.x-= 1 + labelSize.x/Scale + iCS_Config.PortSize;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Top:            
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y-= iCS_Config.PortSize+0.8f*(labelSize.y/Scale)*(1+TopBottomLabelOffset(port, iStorage));
                break;
            case iCS_EditorObject.EdgeEnum.Bottom:
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y+= iCS_Config.PortSize+0.8f*(labelSize.y/Scale)*TopBottomLabelOffset(port, iStorage)-0.2f*labelSize.y/Scale;
                break;
        }
        return new Rect(labelPos.x, labelPos.y, labelSize.x, labelSize.y);	    
    }
    // ----------------------------------------------------------------------
    // Returns port name in GUI coordinates and scale.
    Rect GetPortNameGUIPosition(iCS_EditorObject port, iCS_IStorage iStorage) {
        Rect graphRect= GetPortNamePosition(port, iStorage);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    }

    // ======================================================================
    // Port type utilities.
    // ----------------------------------------------------------------------
    Type GetPortValueType(iCS_EditorObject port) {
        return iCS_Types.GetElementType(port.RuntimeType);
    }
    // ----------------------------------------------------------------------
    object GetPortValue(iCS_EditorObject port, iCS_IStorage iStorage) {
        return iStorage.GetPortValue(port);
    }
    // ----------------------------------------------------------------------
    string GetPortValueAsString(iCS_EditorObject port, iCS_IStorage iStorage) {
        object portValue= GetPortValue(port, iStorage);
        return (portValue != null) ? GetValueAsString(portValue) : null;
    }
    // ----------------------------------------------------------------------
    bool ShouldDisplayPortValue(iCS_EditorObject port, iCS_IStorage iStorage) {
        if(!port.IsDataPort) return false;
        if(!ShouldShowLabel()) return false;
        object portValue= iStorage.GetPortValue(port);
        if(portValue == null) return false;
        if(Application.isPlaying && iStorage.Preferences.DisplayOptions.PlayingPortValues) return true;
        if(!Application.isPlaying && iStorage.Preferences.DisplayOptions.EditorPortValues) return true;
        return false;
    }
    // ----------------------------------------------------------------------
    // Returns the port value display size in GUI scale.
    Vector2 GetPortValueSize(iCS_EditorObject port, iCS_IStorage iStorage) {
		string valueAsStr= GetPortValueAsString(port, iStorage);
		return (valueAsStr != null && valueAsStr != "") ? ValueStyle.CalcSize(new GUIContent(valueAsStr)) : Vector2.zero;        
    }
    // ----------------------------------------------------------------------
    // Returns the port value position in graph coordinate and GUI scale size.
    Rect GetPortValuePosition(iCS_EditorObject port, iCS_IStorage iStorage) {
		Vector2 valueSize= GetPortValueSize(port, iStorage);
		Vector2 valuePos= GetPortCenter(port, iStorage);
        switch(port.Edge) {
            case iCS_EditorObject.EdgeEnum.Left:
				valuePos.x-= 1 + valueSize.x/Scale + iCS_Config.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Right:
				valuePos.x+= 1 + iCS_Config.PortSize;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EditorObject.EdgeEnum.Top:            
                break;
            case iCS_EditorObject.EdgeEnum.Bottom:
                break;
        }
        return new Rect(valuePos.x, valuePos.y, valueSize.x, valueSize.y);	    
	}
    // ----------------------------------------------------------------------
    // Returns the port value position in GUI coordinates and size.
    Rect GetPortValueGUIPosition(iCS_EditorObject port, iCS_IStorage iStorage) {
        Rect graphRect= GetPortValuePosition(port, iStorage);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
	}
	
}
