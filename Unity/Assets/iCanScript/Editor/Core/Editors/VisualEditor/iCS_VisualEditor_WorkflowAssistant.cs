using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIStyle    myNextStepLabelStyle  = null;
    GUIStyle    myNextStepButtonStyle = null;
    Texture2D   myiCanScriptLargeLogo = null;
    Texture2D   myiCanScriptMediumLogo= null;
    Texture2D   myHelpLogo            = null;
    
    // ----------------------------------------------------------------------
    void ShowNextStepHelp() {
        // -- Build the styles used --
        BuildNextStepStyles();
        // -- Ask user the select a game object if none selected --
        var activeGameObject= Selection.activeGameObject;
        if(activeGameObject == null) {
            ShowNextStepHelpWithBlink("Select a Game Object");
            ShowLargeLogo();
            return;
        }
        // -- Ask user to create a visual script --
        var visualScript= activeGameObject.GetComponent("iCS_MonoBehaviourImp") as iCS_MonoBehaviourImp;
        if(visualScript == null) {
            var content= new GUIContent("Create Visual Script", myiCanScriptMediumLogo);
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
            return;
        }
        var pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
        if(pickInfo == null || pickInfo.PickedObject.IsBehaviour) {
            ShowNextStepHelpWithBlink("Right-Click to Add Message Handler or Public Function\nDrag Builder from Library to Create Public Variable");
            return;
        }
        Debug.Log("Object is=> "+pickInfo.PickedObject.Name);
    }
    // ----------------------------------------------------------------------
    void ShowNextStepHelpWithBlink(string message) {
        var r= new Rect(0,iCS_ToolbarUtility.GetHeight(),position.width,position.height);
        var content= new GUIContent(" "+message, myHelpLogo);
        var savedColor= GUI.color;
        GUI.color= iCS_BlinkController.NormalBlinkHighColor;
        GUI.Label(r, content, myNextStepLabelStyle);
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
        if(myiCanScriptLargeLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out myiCanScriptLargeLogo);
        }
        if(myiCanScriptMediumLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LogoIcon, out myiCanScriptMediumLogo);            
        }
        if(myHelpLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.HelpMediumIcon, out myHelpLogo);                        
        }
    }
}
