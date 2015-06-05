using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

	public class FunctionCallPortEditor : PortEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum InVariableType {
            LocalVariable        = PortSpecification.LocalVariable,
            PublicVariable       = PortSpecification.PublicVariable,
            PrivateVariable      = PortSpecification.PrivateVariable,
            StaticPublicVariable = PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable,
            Constant             = PortSpecification.Constant
        };
        public enum InUnityObjectVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum OutVariableType {
            LocalVariable        = PortSpecification.LocalVariable,
            PublicVariable       = PortSpecification.PublicVariable,
            PrivateVariable      = PortSpecification.PrivateVariable,
            StaticPublicVariable = PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        
        
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
            var self= FunctionCallPortEditor.CreateInstance<FunctionCallPortEditor>();
            self.vsObject= port;
            self.title= "Function Call Port Editor";
            self.ShowUtility();
            return self;
        }

        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            // -- Edit the value of the port. --
            EditPortValue();

            // -- Edit port variable type. --
            if(vsObject.IsInDataPort) {
                if(iCS_Types.IsA<UnityEngine.Object>(vsObject.RuntimeType)) {
                    InUnityObjectVariableType variableType= ConvertEnum(vsObject.PortSpec, InUnityObjectVariableType.PublicVariable);
                    variableType= (InUnityObjectVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                    SetPortSpec(ConvertEnum(variableType, PortSpecification.Default)); 
                }
                else {
                    InVariableType variableType= ConvertEnum(vsObject.PortSpec, InVariableType.PublicVariable);
                    variableType= (InVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                    SetPortSpec(ConvertEnum(variableType, PortSpecification.Default));
                }
            }
            else if(vsObject.IsOutDataPort) {
                OutVariableType variableType= ConvertEnum(vsObject.PortSpec, OutVariableType.LocalVariable);
                variableType= (OutVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                SetPortSpec(ConvertEnum(variableType, PortSpecification.Default)); 
            }
            
            // -- Show port value type. --
            EditPortValueType();
        }
                
	}

}