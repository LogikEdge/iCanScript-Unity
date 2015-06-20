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
            PublicOverride    = NodeSpecification.PublicOverrideFunction,
            PublicNew         = NodeSpecification.PublicNewFunction,
            PublicNewStatic   = NodeSpecification.PublicNewStaticFunction,
            Private           = NodeSpecification.PrivateFunction,
            PrivateStatic     = NodeSpecification.PrivateStaticFunction,
            Protected         = NodeSpecification.ProtectedFunction,
            ProtectedVirtual  = NodeSpecification.ProtectedVirtualFunction,
            ProtectedOverride = NodeSpecification.ProtectedOverrideFunction,
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
            var variableType= ConvertEnum(vsObject.NodeSpec, FunctionType.Public) as System.Enum;
            variableType= EditorGUILayout.EnumPopup("Function Type", variableType);
            SetNodeSpec(ConvertEnum(variableType, NodeSpecification.PublicFunction));                        
    	}
        
		// ===================================================================
        /// Sets the node specififcation.
		///
		/// @param nodeSpec The new node specification.
		///
        protected void SetNodeSpec(NodeSpecification nodeSpec) {
			iCS_UserCommands.ChangeNodeSpec(vsObject, nodeSpec);
        }

    }
    
}
