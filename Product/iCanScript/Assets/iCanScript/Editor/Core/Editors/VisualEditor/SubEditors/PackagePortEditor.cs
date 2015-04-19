using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

    public class PackagePortEditor : PortEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum PackageInputPortType {
            PublicVariable, PrivateVariable, ConstantVariable
        }
        public enum PackageOutputPortType {
            PublicVariable, PrivateVariable
        }
        public enum PackageUnityObjectPortType {
            PublicVariable
        }
        public enum PackagePassThroughPortType {
            PassThrough
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
            var self= PackagePortEditor.CreateInstance<PackagePortEditor>();
            self.vsObject= port;
            self.title= "Package Port Editor";
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            // Show code generation choices.
            EditGeneratedVariableType();
            
            // Edit the value of the port.
            EditPortValue();
            
            // Show port value type.
            EditPortValueType();
        }
        
        // -------------------------------------------------------------------
        /// Edits the generated variable specification.
        void EditGeneratedVariableType() {
            var port= vsObject;
            var generatedVariableLabel= "Generated Variable";
            if(port.ProducerPort != null && port.ConsumerPorts.Length != 0) {
                EditorGUILayout.EnumPopup(generatedVariableLabel, PackagePassThroughPortType.PassThrough);                
            }
            else if(port.IsInDataPort) {
                if(iCS_Types.IsA<UnityEngine.Object>(port.RuntimeType)) {
                    EditorGUILayout.EnumPopup(generatedVariableLabel, PackageUnityObjectPortType.PublicVariable);                    
                }
                else {
                    EditorGUILayout.EnumPopup(generatedVariableLabel, PackageInputPortType.PublicVariable);                    
                }
            }
            else if(port.IsOutDataPort) {
                EditorGUILayout.EnumPopup(generatedVariableLabel, PackageOutputPortType.PublicVariable);                                
            }            
        }
    }
    
}
