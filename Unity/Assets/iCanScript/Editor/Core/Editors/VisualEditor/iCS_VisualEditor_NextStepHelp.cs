using UnityEngine;
using UnityEditor;
using System.Collections;


public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIStyle    myNextStepLabelStyle = null;
    GUIStyle    myNextStepButtonStyle= null;
    Texture2D   myLargeLogo          = null;
    Texture2D   myMediumLogo         = null;
    
    // ----------------------------------------------------------------------
    void ShowNextStepHelp() {
        // -- Build the styles used --
        BuildNextStepStyles();
        // -- Ask user the select a game object if none selected --
        var activeGameObject= Selection.activeGameObject;
        if(activeGameObject == null) {
            ShowNextStepHelpWithBlink("Select a Game Object");
            ShowLargeLogo();
        }
        else {
            // -- Ask user to create a visual script --
            var visualScript= activeGameObject.GetComponent("iCS_MonoBehaviourImp") as iCS_MonoBehaviourImp;
            if(visualScript == null) {
                var content= new GUIContent("Create Visual Script", myMediumLogo);
                var contentSize= myNextStepButtonStyle.CalcSize(content);
                var r= Math3D.BuildRectCenteredAt(Math3D.Middle(ViewportRectForGraph), 1.2f*contentSize.x, 1.2f*contentSize.y);
                EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
                var savedColor= GUI.color;
                GUI.color= iCS_BlinkController.NormalBlinkHighColor;
                if(GUI.Button(r, content, myNextStepButtonStyle)) {
                    EditorApplication.ExecuteMenuItem("iCanScript/Create Visual Script"); 
                }
                GUI.color= savedColor;
                Repaint();
            }
        }
    }
    // ----------------------------------------------------------------------
    void ShowNextStepHelpWithBlink(string message) {
        var r= new Rect(0,iCS_ToolbarUtility.GetHeight(),position.width,position.height);
        var savedColor= GUI.color;
        GUI.color= iCS_BlinkController.NormalBlinkHighColor;
        GUI.Label(r, "-> "+message, myNextStepLabelStyle);
        GUI.color= savedColor;
        Repaint();
    }
    // ----------------------------------------------------------------------
    void ShowLargeLogo() {
        ShowLargeLogoCenteredAt(Math3D.Middle(ViewportRectForGraph));
    }
    // ----------------------------------------------------------------------
    void ShowLargeLogoCenteredAt(Vector2 center) {
        Texture2D largeLogo= null;
        iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out largeLogo);
        var r= Math3D.BuildRectCenteredAt(center, largeLogo.width, largeLogo.height);
        GUI.DrawTexture(r, largeLogo);
    }
    // ----------------------------------------------------------------------
    void BuildNextStepStyles() {
        if(myNextStepLabelStyle == null) {
            myNextStepLabelStyle= new GUIStyle(EditorStyles.whiteLabel);
            myNextStepLabelStyle.fontSize= 30;
            myNextStepLabelStyle.fontStyle= FontStyle.Bold;
            myNextStepLabelStyle.normal.textColor= Color.grey;
        }        
        if(myNextStepButtonStyle == null) {
            myNextStepButtonStyle= new GUIStyle(GUI.skin.button);
            myNextStepButtonStyle.fontSize= 30;
            myNextStepButtonStyle.fontStyle= FontStyle.Bold;
        }        
        if(myLargeLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out myLargeLogo);
        }
        if(myMediumLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LogoIcon, out myMediumLogo);            
        }
    }
}
