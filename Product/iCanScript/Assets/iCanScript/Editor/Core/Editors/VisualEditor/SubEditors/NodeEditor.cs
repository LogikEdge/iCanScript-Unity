using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

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
            if(node.IsEventHandler) {
                return EventHandlerEditor.Create(node, screenPosition);
            }
            if(node.IsFunctionDefinition) {
                return FunctionDefinitionEditor.Create(node, screenPosition);
            }
            if(node.IsInstanceNode) {
                return PropertyEditor.Create(node, screenPosition);
            }
            if(node.IsInlineCode) {
                return InlineCodeEditor.Create(node, screenPosition);                
            }
            var self= NodeEditor.CreateInstance<NodeEditor>();
            self.vsObject= node;
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Node Editor", iCanScriptLogo);
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Edit node specific information.
    	public void OnGUI() {
            var node= vsObject;

            // Display node name.
            EditName("Node Name");

            // Show parent type.
            var parentTypeName= iCS_Types.TypeName(vsObject.RuntimeType);
            EditorGUILayout.LabelField("Member of Type", NameUtility.ToTypeName(parentTypeName));
                
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
            Texture2D iconicTexture= TextureCache.GetIconFromGUID(node.IconGUID);
            Texture2D newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
            if(newTexture != iconicTexture) {
                iCS_UserCommands.ChangeIcon(node, newTexture);
            }
            
            EditDescription();
    	}
        
    }
    
}
