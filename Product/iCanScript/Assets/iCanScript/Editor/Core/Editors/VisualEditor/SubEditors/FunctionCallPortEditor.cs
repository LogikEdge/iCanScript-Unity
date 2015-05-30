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
        public enum VariableType {
            LocalVariable        = PortSpecification.LocalVariable,
            PublicVariable       = PortSpecification.PublicVariable,
            PrivateVariable      = PortSpecification.PrivateVariable,
            Constant             = PortSpecification.Constant,
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
            VariableType variableType= ConvertEnum(vsObject.PortSpec, VariableType.LocalVariable);
            variableType= (VariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
            vsObject.PortSpec= ConvertEnum(variableType, PortSpecification.Default);
            
            // -- Show port value type. --
            EditPortValueType();
        }
                
	}

}