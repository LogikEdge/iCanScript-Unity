using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

    public class PortEditor : VSObjectEditor {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
    	Dictionary<string,object>   foldoutDB= new Dictionary<string,object>();
        
            
        // ===================================================================
        // FUNCTIONS TO OVERRIDE FOR EACH SPECIFIC EDITOR
        // -------------------------------------------------------------------
        protected virtual void OnPortSpecificGUI() {}
        
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates a port editor window at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static EditorWindow Create(iCS_EditorObject port, Vector2 screenPosition) {
            if(port == null) return null;
            // Create the specific port editors.
            var parent= port.ParentNode;
            if(parent.IsPackage) {
                return PackagePortEditor.Create(port, screenPosition);
            }
            // Create a generic port editor.
            var self= new PortEditor();
            self.vsObject= port;
            self.title= "Port Editor";
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Port specific information.
    	public void OnGUI() {
            // Display port name.
            EditName("Port Name");

            OnPortSpecificGUI();
            
            EditDescription();        
    	}
        
        // -------------------------------------------------------------------
        /// Display port value type information
        protected void EditPortValueType() {
            var typeName= iCS_ObjectNames.ToTypeName(iCS_Types.TypeName(vsObject.RuntimeType));
            if(!string.IsNullOrEmpty(typeName)) {
                var label= "Port is a";
                if(iCS_TextUtility.StartsWithAVowel(typeName)) {
                    label+= "n";
                }
                EditorGUILayout.LabelField(label, typeName);
            }            
        }

        // -------------------------------------------------------------------
        /// Edit the port value.
        protected void EditPortValue() {
            iCS_GuiUtilities.OnInspectorDataPortGUI("Initial Value", vsObject, 0, foldoutDB);
        }
    }
    
}
