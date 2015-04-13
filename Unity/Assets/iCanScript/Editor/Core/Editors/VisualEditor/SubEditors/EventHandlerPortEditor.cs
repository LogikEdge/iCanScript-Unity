using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Engine;

namespace iCanScript.Editor {

	public class EventHandlerPortEditor : PortEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum InputPortType {
            PublicVariable=        PortType.PublicVariable,
            PrivateVariable=       PortType.PrivateVariable,
            StaticPublicVariable=  PortType.StaticPublicVariable,
            StaticPrivateVariable= PortType.StaticPrivateVariable,
            Constant=              PortType.Constant
        };
        public enum OutputPortType {
            PublicVariable=        PortType.PublicVariable,
            PrivateVariable=       PortType.PrivateVariable,
            StaticPublicVariable=  PortType.StaticPublicVariable,
            StaticPrivateVariable= PortType.StaticPrivateVariable            
        };
        public enum ThisPortType {
            Owner= PortType.Owner
        };
        public enum ParameterPortType {
            Parameter= PortType.Parameter
        }
        
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates a port editor window at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static new EditorWindow Create(iCS_EditorObject port, Vector2 screenPosition) {
            if(port == null) return null;
            var self= EventHandlerPortEditor.CreateInstance<EventHandlerPortEditor>();
            self.vsObject= port;
            self.title= "Event Handler Port Editor";
            self.ShowUtility();
            return self;
        }

        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            // Port type selection.
            EditGeneratedVariableType();
            
            // Edit the value of the port.
            if(!(IsEventParameter() || IsHelperPort())) {
                EditPortValue();                
            }
            
            // Show port value type.
            EditPortValueType();
        }
                
        // -------------------------------------------------------------------
        /// Edits the generated variable specification.
        void EditGeneratedVariableType() {
            var port= vsObject;
            var generatedVariableLabel= "Generated Variable";
            if(port.IsInInstancePort) {
                var newPortType= EditorGUILayout.EnumPopup(generatedVariableLabel, ThisPortType.Owner);
                vsObject.portType= (PortType)newPortType;
                return;                
            }
            if(IsHelperPort()) {
                var newPortType= EditorGUILayout.EnumPopup(generatedVariableLabel, ThisPortType.Owner);
                vsObject.portType= (PortType)newPortType;
                return;                                
            }
            if(IsEventParameter()) {
                var newPortType= EditorGUILayout.EnumPopup(generatedVariableLabel, ParameterPortType.Parameter);                
                vsObject.portType= (PortType)newPortType;
                return;                
            }
            if(port.IsInDataPort) {
                var newPortType= EditorGUILayout.EnumPopup(generatedVariableLabel, ToInputPortType());                
                vsObject.portType= (PortType)newPortType;
                return;
            }
            if(port.IsOutDataPort) {
                var newPortType= EditorGUILayout.EnumPopup(generatedVariableLabel, ToOutputPortType());                                
                vsObject.portType= (PortType)newPortType;
                return;
            }
        }

        // -------------------------------------------------------------------
        /// Determines if the port is a fix parameter for the event handler.
        ///
        /// @return _true_ if the port is a fix parameter. _false_ otherwise.
        ///
        bool IsEventParameter() {
            return vsObject.IsParameterPort || vsObject.IsInDataPort;
        }

        // -------------------------------------------------------------------
        /// Determines if the port is an helper port for this event handler.
        ///
        /// @return _true_ if the port is a fix parameter. _false_ otherwise.
        ///
        bool IsHelperPort() {
            return vsObject.IsProposedDataPort;
        }

        // -------------------------------------------------------------------
        /// Converts from a standard PortType.
        InputPortType ToInputPortType() {
            switch(vsObject.portType) {
                case PortType.PublicVariable: {
                    return InputPortType.PublicVariable;
                }
                case PortType.PrivateVariable: {
                    return InputPortType.PrivateVariable;
                }
                case PortType.StaticPublicVariable: {
                    return InputPortType.StaticPublicVariable;
                }
                case PortType.StaticPrivateVariable: {
                    return InputPortType.StaticPrivateVariable;
                }
                case PortType.Constant: {
                    return InputPortType.Constant;
                }
                default: {
                    return InputPortType.PrivateVariable;
                }
            }
        }
        // -------------------------------------------------------------------
        /// Converts from a standard PortType.
        OutputPortType ToOutputPortType() {
            switch(vsObject.portType) {
                case PortType.PublicVariable: {
                    return OutputPortType.PublicVariable;
                }
                case PortType.PrivateVariable: {
                    return OutputPortType.PrivateVariable;
                }
                case PortType.StaticPublicVariable: {
                    return OutputPortType.StaticPublicVariable;
                }
                case PortType.StaticPrivateVariable: {
                    return OutputPortType.StaticPrivateVariable;
                }
                default: {
                    return OutputPortType.PrivateVariable;
                }
            }
        }
        // -------------------------------------------------------------------
        R ConvertEnum<R,T>(T value, R defaultValue) where R,T: Enum {
            var allowedValues= Enum.GetValues(R);
            foreach(var v in allowedValues) {
                if((int)v == (int)value) {
                    return (R)value;
                }
            }
            return defaultValue;
        }
	}

}

//switch(portType) {
//    case PortType.Parameter:
//    case PortType.Return:
//    case PortType.PublicVariable:
//    case PortType.PrivateVariable:
//    case PortType.StaticPublicVariable:
//    case PortType.StaticPrivateVariable:
//    case PortType.Enable:
//    case PortType.Trigger:
//    case PortType.Constant:
//    case PortType.Owner:
//    case PortType.Other:
//    case PortType.Default:
//}
