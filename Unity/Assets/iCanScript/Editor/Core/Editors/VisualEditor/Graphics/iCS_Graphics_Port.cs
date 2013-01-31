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
    Vector2 GetPortCenter(iCS_EditorObject port) {
        return port.GlobalDisplayPosition;
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
    static string GetPortName(iCS_EditorObject port) {
        Type portValueType= GetPortValueType(port);
        return portValueType.IsArray ? "["+port.Name+"]" : port.Name;
    }
    // ----------------------------------------------------------------------
	static string GetPortPath(iCS_EditorObject port, iCS_IStorage iStorage) {
		iCS_EditorObject parent= port.Parent;
		string path= parent.Name;
		for(parent= parent.Parent; parent != null && parent != iStorage[0]; parent= parent.Parent) {
			path+= "."+parent.Name;			
		}
		return path;
	}
    // ----------------------------------------------------------------------
	// Returns the full path name of the port.
	static string GetPortFullPathName(iCS_EditorObject port, iCS_IStorage iStorage) {
		return GetPortName(port)+"."+GetPortPath(port,iStorage);
	}
    // ----------------------------------------------------------------------
    bool ShouldDisplayPortName(iCS_EditorObject port, iCS_IStorage iStorage) {
        if(port.IsInMuxPort) return false;
        if(port.IsStatePort || port.IsTransitionPort) return false;
        if(!ShouldShowLabel()) return false;
        if(!IsVisible(port, iStorage)) return false;
        // Declutter graph by not displaying port name if it's an input and very close to the output.
        if((port.IsInputPort || port.IsModulePort) && port.SourceId != -1) {
            var sourcePort= port.Source;
            if(sourcePort.Name != port.Name) return true;
            if(!sourcePort.IsVisibleInLayout) return true;
            var sourceCenter= sourcePort.GlobalDisplayPosition;
            var portCenter= port.GlobalDisplayPosition;
            var distance= Vector2.Distance(portCenter, sourceCenter);
            if(distance < 200.0f) return false;
        }
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
		Vector2 labelPos= GetPortCenter(port);
        switch(port.Edge) {
            case iCS_EdgeEnum.Left:
                labelPos.x+= 1 + iCS_EditorConfig.PortDiameter;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                break;
            case iCS_EdgeEnum.Right:
                labelPos.x-= 1 + labelSize.x/Scale + iCS_EditorConfig.PortDiameter;
                labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                break;
            case iCS_EdgeEnum.Top:            
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y-= iCS_EditorConfig.PortDiameter+0.8f*(labelSize.y/Scale)*(1+TopBottomLabelOffset(port, iStorage));
                break;
            case iCS_EdgeEnum.Bottom:
                labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                labelPos.y+= iCS_EditorConfig.PortDiameter+0.8f*(labelSize.y/Scale)*TopBottomLabelOffset(port, iStorage)-0.2f*labelSize.y/Scale;
                break;
        }
        return new Rect(labelPos.x, labelPos.y, labelSize.x, labelSize.y);	    
    }
    // ----------------------------------------------------------------------
    // Returns port name in GUI coordinates and scale.
    public Rect GetPortNameGUIPosition(iCS_EditorObject port, iCS_IStorage iStorage) {
        Rect graphRect= GetPortNamePosition(port, iStorage);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    }

    // ======================================================================
    // Port type utilities.
    // ----------------------------------------------------------------------
    static Type GetPortValueType(iCS_EditorObject port) {
        return iCS_Types.GetElementType(port.RuntimeType);
    }
    // ----------------------------------------------------------------------
    string GetPortValueAsString(iCS_EditorObject port) {
        object portValue= port.PortValue;
        return (portValue != null) ? GetValueAsString(portValue) : null;
    }
    // ----------------------------------------------------------------------
    bool ShouldDisplayPortValue(iCS_EditorObject port, iCS_IStorage iStorage) {
        if(!port.IsDataPort || port.IsInMuxPort) return false;
        if(!ShouldShowLabel()) return false;
        // Declutter graph by not displaying port name if it's an input and very close to the output.
        if((port.IsInputPort || port.IsModulePort) && port.SourceId != -1) {
            var sourcePort= port.Source;
            var sourceCenter= sourcePort.GlobalDisplayPosition;
            var portCenter= port.GlobalDisplayPosition;
            var distance= Vector2.Distance(portCenter, sourceCenter);
            if(distance < 200.0f) return false;
        }
        object portValue= port.PortValue;
        if(portValue == null) return false;
        if(Application.isPlaying && iCS_PreferencesEditor.ShowRuntimePortValue) return true;
        if(!Application.isPlaying) return true;
        return false;
    }
    // ----------------------------------------------------------------------
    // Returns the port value display size in GUI scale.
    Vector2 GetPortValueSize(iCS_EditorObject port, iCS_IStorage iStorage) {
		string valueAsStr= GetPortValueAsString(port);
		return iCS_Strings.IsNotEmpty(valueAsStr) ? ValueStyle.CalcSize(new GUIContent(valueAsStr)) : Vector2.zero;        
    }
    // ----------------------------------------------------------------------
    // Returns the port value position in graph coordinate and GUI scale size.
    Rect GetPortValuePosition(iCS_EditorObject port, iCS_IStorage iStorage) {
		Vector2 valueSize= GetPortValueSize(port, iStorage);
		Vector2 valuePos= GetPortCenter(port);
        switch(port.Edge) {
            case iCS_EdgeEnum.Left:
				valuePos.x-= 1 + valueSize.x/Scale + iCS_EditorConfig.PortDiameter;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EdgeEnum.Right:
				valuePos.x+= 1 + iCS_EditorConfig.PortDiameter;
				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                break;
            case iCS_EdgeEnum.Top:            
                break;
            case iCS_EdgeEnum.Bottom:
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
	// ----------------------------------------------------------------------
    // Returns the tooltip for the given port.
	public static string GetPortTooltip(iCS_EditorObject port, iCS_IStorage iStorage) {
		string tooltip= "Name: "+(port.RawName ?? "")+"\n";
		// Type information
		Type runtimeType= port.RuntimeType;
		if(runtimeType != null) tooltip+= "Type: "+iCS_Types.TypeName(runtimeType)+"\n";
		// Source information.
		if(port.IsDataPort) {
			iCS_EditorObject sourcePort= iStorage.GetDataConnectionSource(port);
			if(sourcePort != null && sourcePort != port) {
				tooltip+= "Source: "+GetPortFullPathName(sourcePort, iStorage)+"\n";
			}
		}
		// User defined tooltip
		if(iCS_Strings.IsNotEmpty(port.Tooltip)) tooltip+= port.Tooltip;
		return tooltip;
	}
	
}
