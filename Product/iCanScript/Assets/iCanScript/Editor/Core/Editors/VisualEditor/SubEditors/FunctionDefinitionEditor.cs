using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;

namespace iCanScript.Editor {

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
            GUI.changed= false;
            var newAccess= EditorGUILayout.EnumPopup("Access Specifier", vsObject.accessSpecifier);
            vsObject.accessSpecifier= (AccessSpecifier)newAccess;
            var newScope= EditorGUILayout.EnumPopup("Scope Specifier", vsObject.scopeSpecifier);
            vsObject.scopeSpecifier= (ScopeSpecifier)newScope;
            var newAttributes= EditorGUILayout.TextField(".Net Attributes", vsObject.dotNetAttributes);
            vsObject.dotNetAttributes= newAttributes;
            if(GUI.changed) {
                vsObject.IStorage.SaveStorage();
            }
    	}
        
    }
    
}
