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
//            var self= EditorWindow.GetWindow(typeof(PackagePortEditor)) as PackagePortEditor;
            var self= new PackagePortEditor();
            self.vsObject= port;
            self.title= "Package Port Editor";
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            var port= vsObject;
            if(port.ProducerPort != null && port.ConsumerPorts.Length != 0) {
                EditorGUILayout.EnumPopup("Port Type", PackagePassThroughPortType.PassThrough);                
            }
            else if(port.IsInDataPort) {
                if(iCS_Types.IsA<UnityEngine.Object>(port.RuntimeType)) {
                    EditorGUILayout.EnumPopup("Port Type", PackageUnityObjectPortType.PublicVariable);                    
                }
                else {
                    EditorGUILayout.EnumPopup("Port Type", PackageInputPortType.PublicVariable);                    
                }
            }
            else if(port.IsOutDataPort) {
                EditorGUILayout.EnumPopup("Port Type", PackageOutputPortType.PublicVariable);                                
            }
        }
    }
    
}
