using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    // ==========================================================================
    // Port utilities.
    // ==========================================================================
    public partial class iCS_Graphics {
        // ======================================================================
        // Common port utilities.
        // ----------------------------------------------------------------------
        // Returns the port center in graph coordinates.
        Vector2 GetPortCenter(iCS_EditorObject port) {
            return port.AnimatedPosition;
        }
    	// ----------------------------------------------------------------------
        string GetValueAsString(object value) {
            if(value == null) return null;
            if(value is bool) return ((bool)value) ? "true" : "false";
            if(value is float) return ((float)value).ToString();
            if(value is int) return ((int)value).ToString();
            if(value is Vector2) return ((Vector2)value).ToString();
            if(value is Vector3) return ((Vector3)value).ToString();
            if(value is Vector4) return ((Vector4)value).ToString();
            if(value is string) return "\""+(string)value+"\"";
            if(value is char) return "\'"+(char)value+"\'";
            if(value is Color) {
                Color c= (Color)value;
                if(c == Color.black)   return "Color.black";
                if(c == Color.blue)    return "Color.blue";
                if(c == Color.clear)   return "Color.clear";
                if(c == Color.cyan)    return "Color.cyan";
                if(c == Color.gray)    return "Color.grey";
                if(c == Color.green)   return "Color.green";
                if(c == Color.gray)    return "Color.grey";
                if(c == Color.magenta) return "Color.magenta";
                if(c == Color.red)     return "Color.red";
                if(c == Color.white)   return "Color.white";
                if(c == Color.yellow)  return "Color.yellow";
                var r= c.r.ToString(); if(r.Length > 5) r= r.Substring(0,5);
                var g= c.g.ToString(); if(g.Length > 5) g= g.Substring(0,5);
                var b= c.b.ToString(); if(b.Length > 5) b= b.Substring(0,5);
                var a= c.a.ToString(); if(a.Length > 5) a= a.Substring(0,5);
                return "Color("+r+", "+g+", "+b+", "+a+")";
            }
            if(value is UnityEngine.Object) {
                var obj= value as UnityEngine.Object;
                if(obj != null) {
                    return obj.name;
                }
            }
            var valueType= iCS_Types.GetElementType(value.GetType());
            if(valueType.IsEnum) {
                System.Enum valueAsEnum= value as System.Enum;
                return valueAsEnum.ToString();
            }
            return null;
        }
    
        // ======================================================================
        // Port name utilities.
        // ----------------------------------------------------------------------
        static string GetPortName(iCS_EditorObject port) {
            return port.DisplayName;
        }
        // ----------------------------------------------------------------------
    	static string GetPortPath(iCS_EditorObject port, iCS_IStorage iStorage) {
    		iCS_EditorObject parent= port.Parent;
    		string path= parent.DisplayName;
    		for(parent= parent.Parent; parent != null && parent != iStorage[0]; parent= parent.Parent) {
    			path+= "."+parent.DisplayName;			
    		}
    		return path;
    	}
        // ----------------------------------------------------------------------
    	// Returns the full path name of the port.
    	static string GetPortFullPathName(iCS_EditorObject port, iCS_IStorage iStorage) {
    		return GetPortName(port)+"."+GetPortPath(port,iStorage);
    	}
        // ----------------------------------------------------------------------
        bool ShouldDisplayPortName(iCS_EditorObject port) {
            if(!ShouldShowLabel()) return false;
            if(port.IsChildMuxPort) return false;
            if(port.IsStatePort || port.IsTransitionPort) return false;
            if(!port.IsVisibleOnDisplay) return false;
            var parent= port.ParentNode;
            if(parent.IsIconizedOnDisplay) return false;
            if(port.IsEndPort) return true;
            var provider= port.ProducerPort;
            if(provider == null) return true;
            if(!provider.IsVisibleOnDisplay) return true;
            if(provider.ParentNode.IsIconizedOnDisplay) return true;
            var consumerPorts= port.ConsumerPorts;
            foreach(var cp in consumerPorts) {
                if(cp.IsVisibleOnDisplay && !cp.ParentNode.IsIconizedOnDisplay) {
                    return false;
                }
            }
            return true;
        }
        // ----------------------------------------------------------------------
        // Returns the port name size in GUI scale.
        Vector2 GetPortNameSize(iCS_EditorObject port) {
    		if(Layout.DynamicLabelStyle == null) return Vector2.zero;
            return Layout.DynamicLabelStyle.CalcSize(new GUIContent(GetPortName(port)));
        }
        // ----------------------------------------------------------------------
        // Returns the port name position in graph coordinate and GUI scale size.
        Rect GetPortNamePosition(iCS_EditorObject port, iCS_IStorage iStorage) {
            Vector2 labelSize= GetPortNameSize(port);
    		Vector2 labelPos= GetPortCenter(port);
            switch(port.Edge) {
                case iCS_EdgeEnum.Left:
                    labelPos.x+= iCS_EditorConfig.PortDiameter;
                    labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                    break;
                case iCS_EdgeEnum.Right:
                    labelPos.x-= labelSize.x/Scale + iCS_EditorConfig.PortDiameter;
                    labelPos.y-= 1 + 0.5f * labelSize.y/Scale;
                    break;
                case iCS_EdgeEnum.Top:            
                    labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                    labelPos.y-= iCS_EditorConfig.PortDiameter+0.8f*(labelSize.y/Scale);
                    break;
                case iCS_EdgeEnum.Bottom:
                    labelPos.x-= 1 + 0.5f*labelSize.x/Scale;
                    labelPos.y+= iCS_EditorConfig.PortDiameter;
                    break;
            }
            return new Rect(labelPos.x, labelPos.y, labelSize.x, labelSize.y);	    
        }
        // ----------------------------------------------------------------------
        // Returns port name in GUI coordinates and scale.
        public Rect GetPortNameGUIPosition(iCS_EditorObject port) {
    		var iStorage= port.IStorage;
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
            // -- Special case for "Owner". --
            if(port.IsOwner && port.IsInputPort) return "Owner";
            // -- Convert the initial value to its string representation. --
            object portValue= port.Value;
            return (portValue != null) ? GetValueAsString(portValue) : null;
        }
        // ----------------------------------------------------------------------
        bool ShouldDisplayPortValue(iCS_EditorObject port) {
            if(!ShouldShowLabel()) return false;
            if(ShouldDisplayPortName(port) == false) return false;
            bool isPlaying= Application.isPlaying;
            if(isPlaying == false && port.IsSourceValid) return false;
            if(!port.IsDataOrControlPort || port.IsChildMuxPort) return false;
            // Declutter graph by not displaying port name if it's an input and very close to the output.
            if((port.IsInputPort || port.IsKindOfPackagePort) && port.ProducerPortId != -1) {
                var sourcePort= port.ProducerPort;
                if(sourcePort == null) return true;
                var sourceCenter= sourcePort.AnimatedPosition;
                var portCenter= port.AnimatedPosition;
                var distance= Vector2.Distance(portCenter, sourceCenter);
                if(distance < 200.0f) return false;
            }
            if(port.IsOwner) return true;
            object portValue= port.Value;
            if(portValue == null) return false;
            if(!Application.isPlaying) return true;
            return false;
        }
        // ----------------------------------------------------------------------
        // Returns the port value display size in GUI scale.
        Vector2 GetPortValueSize(iCS_EditorObject port) {
            if(Layout.DynamicValueStyle == null) return Vector2.zero;
    		string valueAsStr= GetPortValueAsString(port);
    		return iCS_Strings.IsNotEmpty(valueAsStr) ?
                Layout.DynamicValueStyle.CalcSize(new GUIContent(valueAsStr)):
                Vector2.zero;        
        }
        // ----------------------------------------------------------------------
        // Returns the port value position in graph coordinate and GUI scale size.
        Rect GetPortValuePosition(iCS_EditorObject port) {
    		Vector2 valueSize= GetPortValueSize(port);
    		Vector2 valuePos= GetPortCenter(port);
            switch(port.Edge) {
                case iCS_EdgeEnum.Left:
    				valuePos.x-= valueSize.x/Scale + iCS_EditorConfig.PortDiameter;
    				valuePos.y-= 1 + 0.5f * valueSize.y/Scale;
                    break;
                case iCS_EdgeEnum.Right:
    				valuePos.x+= iCS_EditorConfig.PortDiameter;
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
        public Rect GetPortValueGUIPosition(iCS_EditorObject port) {
            Rect graphRect= GetPortValuePosition(port);
            var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
            return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    	}
    }

}