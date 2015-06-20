using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

    public class InlineCodeEditor : NodeEditor {
        // ===================================================================
        // TYPES
        // -------------------------------------------------------------------
        public enum FunctionType {
            Public, Private, PublicAndStatic, PrivateAndStatic
        };
        
        // ===================================================================
        // BUILDER
        // -------------------------------------------------------------------
        /// Creates an inline code node editor at the given screen position.
        ///
        /// @param screenPosition The screen position where the editor
        ///                       should be displayed.
        ///
        public static new EditorWindow Create(iCS_EditorObject node, Vector2 screenPosition) {
            if(node == null) return null;
            var self= InlineCodeEditor.CreateInstance<InlineCodeEditor>();
            self.vsObject= node;
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Inline Code Editor", iCanScriptLogo);
            self.minSize= new Vector2(480, 400);
            self.ShowUtility();
            return self;
        }
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
        /// Edit node specific information.
    	protected override void OnNodeSpecificGUI() {
            // -- Edit inline code --
            string code= vsObject.Value as string;
            if(string.IsNullOrEmpty(code)) code= EmptyStr;
            GUI.changed= false;
            EditorGUILayout.LabelField("Inline Code");
            var newCode= EditorGUILayout.TextArea(code, GUILayout.Height(CodeEditorHeight(code)));
            if(GUI.changed) {
                newCode.TrimStart();
                iCS_UserCommands.ChangeValue(vsObject, newCode);
            }            
    	}
        
        // ===================================================================
        /// Determines needed editor height.
        ///
        /// @param code The code string to fit.
        /// @return The needed height.
        ///
        float CodeEditorHeight(string code) {
            var lineHeight= GUI.skin.textArea.CalcHeight(new GUIContent("A"), position.width);
            int nbOfLines= 1;   // Always give an extra line.
            var codeLen= code.Length;
            for(int i= 0; i < codeLen; ++i) {
                if(code[i] == '\n') ++nbOfLines;
            }
            return nbOfLines * lineHeight;
        }
        
    }
    
}
