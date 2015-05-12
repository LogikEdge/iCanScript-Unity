using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

    public class FunctionDefinitionEditor : NodeEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum FunctionType {
            Public, Private, PublicAndStatic, PrivateAndStatic
        };
        
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates a port editor window at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static new EditorWindow Create(iCS_EditorObject node, Vector2 screenPosition) {
            if(node == null) return null;
            var self= FunctionDefinitionEditor.CreateInstance<FunctionDefinitionEditor>();
            self.vsObject= node;
            self.title= "Function Definition Editor";
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Edit node specific information.
    	protected override void OnNodeSpecificGUI() {
            EditorGUILayout.EnumPopup("Function Type", FunctionType.Public);
    	}
        
    }
    
}
