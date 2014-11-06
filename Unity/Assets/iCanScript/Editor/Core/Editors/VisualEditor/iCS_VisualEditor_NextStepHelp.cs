using UnityEngine;
using UnityEditor;
using System.Collections;


public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIStyle    myNextStepStyle= null;
    
    // ----------------------------------------------------------------------
    void ShowNextStepHelp() {
        if(Selection.activeGameObject == null) {
            ShowNextStepHelpWithBlink("Select a Game Object");
            ShowLargeLogo();
        }
    }
    // ----------------------------------------------------------------------
    void ShowNextStepHelpWithBlink(string message) {
        if(myNextStepStyle == null) {
            myNextStepStyle= new GUIStyle(EditorStyles.whiteLabel);
            myNextStepStyle.fontSize= 30;
            myNextStepStyle.fontStyle= FontStyle.Bold;
            myNextStepStyle.normal.textColor= Color.grey;
        }
        var r= new Rect(0,iCS_ToolbarUtility.GetHeight(),position.width,position.height);
        var savedColor= GUI.color;
        GUI.color= iCS_BlinkController.NormalBlinkColor;
        GUI.Label(r, "-> "+message, myNextStepStyle);
        GUI.color= savedColor;
        Repaint();
    }
    // ----------------------------------------------------------------------
    void ShowLargeLogo() {
        var toolbarHeight= iCS_ToolbarUtility.GetHeight();
        var r= new Rect(0, toolbarHeight, position.width, position.height-toolbarHeight);
        Texture2D largeLogo= null;
        iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out largeLogo);
        r= Math3D.BuildRectCenteredAt(Math3D.Middle(r), largeLogo.width, largeLogo.height);
        GUI.DrawTexture(r, largeLogo);
    }
}
