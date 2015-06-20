using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

    public class FunctionDefinitionEditor : NodeEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum FunctionType {
            Public            = NodeSpecification.PublicFunction,
            PublicStatic      = NodeSpecification.PublicStaticFunction,
            PublicVirtual     = NodeSpecification.PublicVirtualFunction,
            PublicOverride    = NodeSpecification.PublicVirtualFunction,
            PublicNew         = NodeSpecification.PublicNewFunction,
            PublicNewStatic   = NodeSpecification.PublicNewStaticFunction,
            Private           = NodeSpecification.PrivateFunction,
            PrivateStatic     = NodeSpecification.PrivateStaticFunction,
            Protected         = NodeSpecification.ProtectedFunction,
            ProtectedVirtual  = NodeSpecification.ProtectedVirtualFunction,
            ProtectedOverride = NodeSpecification.ProtectedVirtualFunction,
            ProtectedNew      = NodeSpecification.ProtectedNewFunction,
            ProtectedNewStatic= NodeSpecification.ProtectedNewStaticFunction
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
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Function Definition Editor", iCanScriptLogo);
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
