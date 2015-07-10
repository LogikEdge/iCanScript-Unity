using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public class ProjectSelectionWindow : EditorWindow {

        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        const  int         kWidth            = 1000;
        const  int         kHeaderHeight     = 100;
        const  int         kListAreaHeight   = 450;
        static Rect        ourHeaderRect     = new Rect(0, 0, kWidth, kHeaderHeight);
        static Rect        ourListAreaRect   = new Rect(0, kHeaderHeight, kWidth, kListAreaHeight);
        static Texture2D   ourHeaderTexture  = null;
        static Texture2D   ourListAreaTexture= null;
        
        // =================================================================================
        /// Creates a project selection window.
        public static ProjectSelectionWindow Init() {
            // -- Create window. --
            var editor= EditorWindow.CreateInstance<ProjectSelectionWindow>();

            // -- Set window title --
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            editor.titleContent= new GUIContent("iCanScript Project Selection", iCanScriptLogo);
    
            // -- Fix window size. --
            editor.minSize= new Vector2(kWidth, kHeaderHeight+kListAreaHeight);
            editor.maxSize= new Vector2(kWidth, kHeaderHeight+kListAreaHeight);

            // -- Build the background textures. --
            ourHeaderTexture  = new Texture2D(kWidth, kHeaderHeight);
            ourListAreaTexture= new Texture2D(kWidth, kListAreaHeight);
            iCS_TextureUtil.Fill(ref ourHeaderTexture, Color.white);
    		ourHeaderTexture.Apply();
    		ourHeaderTexture.hideFlags= HideFlags.DontSave;
            iCS_TextureUtil.Fill(ref ourListAreaTexture, new Color(0.9f, 0.9f, 0.9f));
    		ourListAreaTexture.Apply();
    		ourListAreaTexture.hideFlags= HideFlags.DontSave;
                            
            // -- Show the window. --
            editor.ShowUtility();
            return editor;
        }

        // =================================================================================
        /// Draw window
        public void OnGUI() {
            var headerRect  = new Rect(0,0,position.width, 100);
            var listAreaRect= new Rect(0,100, position.width, position.height-100);
            GUI.Box(ourHeaderRect, ourHeaderTexture);
            GUI.Box(ourListAreaRect, ourListAreaTexture);
        }
    }

}
