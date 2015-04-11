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
    	protected override void OnObjectSpecificGUI() {
            // Port specific information.
            OnPortSpecificGUI();
            var port= vsObject;
            iCS_EditorObject parent= port.Parent;
            EditorGUILayout.LabelField("Parent", parent.DisplayName);
            EditorGUILayout.LabelField("Port Index", port.PortIndex.ToString());
            iCS_GuiUtilities.OnInspectorDataPortGUI(port, port.IStorage, 0, foldoutDB);        
    	}
        
    }
    
}
