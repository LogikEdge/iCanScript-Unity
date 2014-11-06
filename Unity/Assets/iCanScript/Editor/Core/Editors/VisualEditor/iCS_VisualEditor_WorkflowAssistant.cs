using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIStyle    myAssistantLabelStyle = null;
    GUIStyle    myAssistaneButtonStyle= null;
    Texture2D   myiCanScriptLargeLogo = null;
    Texture2D   myiCanScriptMediumLogo= null;
    Texture2D   myAssistantLogo       = null;
    
    // ----------------------------------------------------------------------
    void ShowWorkflowAssistant() {
        // -- Build the styles used --
        BuildWorkflowAssistantStyles();
        // -- Ask user the select a game object if none selected --
        var activeGameObject= Selection.activeGameObject;
        if(activeGameObject == null) {
            ShowNextStepHelpWithBlink("Select a Game Object");
	        ShowTextureCenteredAt(Math3D.Middle(ViewportRectForGraph), myiCanScriptLargeLogo);
            return;
        }
        // -- Ask user to create a visual script --
        var visualScript= activeGameObject.GetComponent("iCS_MonoBehaviourImp") as iCS_MonoBehaviourImp;
        if(visualScript == null) {
            var content= new GUIContent("Create Visual Script", myiCanScriptMediumLogo);
            var contentSize= myAssistaneButtonStyle.CalcSize(content);
            var r= Math3D.BuildRectCenteredAt(Math3D.Middle(ViewportRectForGraph), 1.2f*contentSize.x, 1.2f*contentSize.y);
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            var savedColor= GUI.color;
            GUI.color= iCS_BlinkController.NormalBlinkHighColor;
            if(GUI.Button(r, content, myAssistaneButtonStyle)) {
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
    }
    // ----------------------------------------------------------------------
    void ShowNextStepHelpWithBlink(string message) {
        var r= new Rect(0,iCS_ToolbarUtility.GetHeight(),position.width,position.height);
        var content= new GUIContent(" "+message, myAssistantLogo);
        var savedColor= GUI.color;
        GUI.color= iCS_BlinkController.NormalBlinkHighColor;
        GUI.Label(r, content, myAssistantLabelStyle);
        GUI.color= savedColor;
        Repaint();
    }
    // ----------------------------------------------------------------------
    void ShowTextureCenteredAt(Vector2 center, Texture2D texture) {
        var r= Math3D.BuildRectCenteredAt(center, texture.width, texture.height);
        GUI.DrawTexture(r, texture);
    }
    // ----------------------------------------------------------------------
    void BuildWorkflowAssistantStyles() {
        if(myAssistantLabelStyle == null) {
            myAssistantLabelStyle= new GUIStyle(EditorStyles.whiteLabel);
            myAssistantLabelStyle.fontSize= 30;
            myAssistantLabelStyle.fontStyle= FontStyle.Bold;
            myAssistantLabelStyle.normal.textColor= Color.grey;
        }        
        if(myAssistaneButtonStyle == null) {
            myAssistaneButtonStyle= new GUIStyle(GUI.skin.button);
            myAssistaneButtonStyle.fontSize= 30;
            myAssistaneButtonStyle.fontStyle= FontStyle.Bold;
        }        
        if(myiCanScriptLargeLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out myiCanScriptLargeLogo);
        }
        if(myiCanScriptMediumLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LogoIcon, out myiCanScriptMediumLogo);            
        }
        if(myAssistantLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.HelpMediumIcon, out myAssistantLogo);                   
        }
    }
}
