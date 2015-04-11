using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

    public class NodeEditor : VSObjectEditor {
        // ===================================================================
        // FUNCTIONS TO OVERRIDE FOR EACH SPECIFIC EDITOR
        // -------------------------------------------------------------------
        protected virtual void OnNodeSpecificGUI() {}
        
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates a port editor window at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static EditorWindow Create(iCS_EditorObject node, Vector2 screenPosition) {
            if(node == null) return null;
            if(node.IsMessageHandler) {
                return EventHandlerEditor.Create(node, screenPosition);
            }
            if(node.IsPublicFunction) {
                return FunctionDefinitionEditor.Create(node, screenPosition);
            }
            var self= new NodeEditor();
            self.vsObject= node;
            self.title= "Node Editor";
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Edit node specific information.
    	protected override void OnObjectSpecificGUI() {
            var node= vsObject;

            // Show function name (if it exists).
            if(vsObject.IsKindOfFunction) {
                var functionName= vsObject.MethodName;
                if(!string.IsNullOrEmpty(functionName)) {
                    EditorGUILayout.LabelField("Function", functionName);                    
                }
            }
            
            // Node specific editor
            OnNodeSpecificGUI();

            // Show Iconic image configuration.
            Texture2D iconicTexture= iCS_TextureCache.GetIconFromGUID(node.IconGUID);
            Texture2D newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
            if(newTexture != iconicTexture) {
                iCS_UserCommands.ChangeIcon(node, newTexture);
            }
    	}
        
    }
    
}
