using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

	public class FunctionDefinitionPortEditor : PortEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum VariableType {
            Parameter            = PortSpecification.Parameter,
            ReturnValue          = PortSpecification.ReturnValue,
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
            var self= FunctionDefinitionPortEditor.CreateInstance<FunctionDefinitionPortEditor>();
            self.vsObject= port;
            self.title= "Function Definition Port Editor";
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
            VariableType variableType= ConvertEnum(vsObject.PortSpec, VariableType.Parameter);
            variableType= (VariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
            vsObject.PortSpec= ConvertEnum(variableType, PortSpecification.Default);
            
            // -- Show port value type. --
            EditPortValueType();
        }
                
	}

}