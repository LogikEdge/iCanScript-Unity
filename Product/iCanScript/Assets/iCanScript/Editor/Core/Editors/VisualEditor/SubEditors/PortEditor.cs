using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

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
			if(parent.IsEventHandler) {
				return EventHandlerPortEditor.Create(port, screenPosition);
			}
			if(parent.IsFunctionDefinition) {
				return FunctionDefinitionPortEditor.Create(port, screenPosition);
			}
            if(parent.IsPackage) {
                return PackagePortEditor.Create(port, screenPosition);
            }
			if(parent.IsKindOfFunction) {
				return FunctionCallPortEditor.Create(port, screenPosition);
			}
            // Create a generic port editor.
            var self= PortEditor.CreateInstance<PortEditor>();
            self.vsObject= port;
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Port Editor", iCanScriptLogo);
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Port specific information.
    	public void OnGUI() {
            // -- Display port name. --
            EditName("Port Name");
            
            // -- Edit the value of the port. --
            EditPortValue();

            // -- Edit port specification. --
            var variableType= ConvertEnum(vsObject.PortSpec, GraphInfo.GetAllowedPortSpecification(vsObject));
            variableType= EditorGUILayout.EnumPopup("Variable Specification", variableType);
            SetPortSpec(ConvertEnum(variableType, PortSpecification.Default));                        
			
            // -- Show port type. --
            var portValue= vsObject.Value;
            if(portValue != null) {
                EditorGUILayout.LabelField("Variable Type", iCS_Types.TypeName(portValue.GetType()));                
            }
            
            // -- Show port index. --
            EditorGUILayout.IntField("Parameter Index", vsObject.PortIndex);
            
            // -- Edit port description. --
            EditDescription();        
    	}
        
        // -------------------------------------------------------------------
        /// Display port value type information
        protected void EditPortValueType() {
            var typeName= NameUtility.ToTypeName(iCS_Types.TypeName(vsObject.RuntimeType));
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

		// ===================================================================
        /// Sets the port specififcation.
		///
		/// @param portSpec The new port specification.
		///
        protected void SetPortSpec(PortSpecification portSpec) {
			GraphEditor.SetPortSpec(vsObject, portSpec);
        }
    }
    
}
