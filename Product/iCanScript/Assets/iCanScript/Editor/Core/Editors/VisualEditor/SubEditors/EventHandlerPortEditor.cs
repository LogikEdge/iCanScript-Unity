using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

	public class EventHandlerPortEditor : PortEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum InVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable,
            Constant=              PortSpecification.Constant
        };
        public enum InUnityObjectVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum OutVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable            
        };
        
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
            var self= EventHandlerPortEditor.CreateInstance<EventHandlerPortEditor>();
            self.vsObject= port;
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Event Handler Port Editor", iCanScriptLogo);
            self.ShowUtility();
            return self;
        }

        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        protected override void OnPortSpecificGUI() {
            // -- Port type selection. --
            if(vsObject.IsFixDataPort) {
                // -- Edit port variable type. --
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup("Variable Type", vsObject.PortSpec);
                EditorGUI.EndDisabledGroup();
            }
            else {
                // -- Edit port variable type. --
                if(vsObject.IsInDataPort) {
                    if(iCS_Types.IsA<UnityEngine.Object>(vsObject.RuntimeType)) {
                        InUnityObjectVariableType variableType= ConvertEnum(vsObject.PortSpec, InUnityObjectVariableType.PublicVariable);
                        variableType= (InUnityObjectVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                        SetPortSpec(ConvertEnum(variableType, PortSpecification.Default));
                    }
                    else {
                        InVariableType variableType= ConvertEnum(vsObject.PortSpec, InVariableType.PublicVariable);
                        variableType= (InVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                        SetPortSpec(ConvertEnum(variableType, PortSpecification.Default));
                    }
                }
                else if(vsObject.IsOutDataPort) {
                    OutVariableType variableType= ConvertEnum(vsObject.PortSpec, OutVariableType.PublicVariable);
                    variableType= (OutVariableType)EditorGUILayout.EnumPopup("Variable Type", variableType);
                    SetPortSpec(ConvertEnum(variableType, PortSpecification.Default));                
                }
            
                // -- Edit the value of the port. --
                EditPortValue();                
            }
            // -- Show port value type. --
            EditPortValueType();
        }
                
	}

}

