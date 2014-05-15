using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PurchaseForm {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    
    // =================================================================================
    // Menu
    // ---------------------------------------------------------------------------------
    [MenuItem("Help/iCanScript/Purchase...", false, 71)]
    public static void Initialize() {
        string title= "iCanScript Demo ("+iCS_LicenseController.RemainingTrialDays+" days remaining)";
        if(EditorUtility.DisplayDialog(title, "You may purchase the full version of iCanScript on the Unity Asset Store.", "Purchase", "Use Demo")) {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");
        }
        
//        EditorWindow.GetWindow(typeof(iCS_PurchaseForm), true, "iCanScript Purchase Form");
    }
    [MenuItem("Help/iCanScript/Purchase...", true, 71)]
    public static bool IsNotPurchased() {
        return !iCS_LicenseController.IsActivated;
    }
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnEnable() {
    }
    public void OnDisable() {
    }


    // =================================================================================
    // OnGUI
    // ---------------------------------------------------------------------------------
    public void OnGUI() {
//        // Data to be displayed.
//        string logo= iCS_EditorStrings.LogoIcon;
//        string title= "iCanScript Demo";
//        string[] messages= new string[]{"Thanks you for using the demo edition of iCanScript.",
//                                        "Your trial will expire in "+iCS_LicenseController.RemainingTrialDays+" days.",
//                                        "You can purchase the full version of iCanScript on the Unity Asset Store."};
//        string[] buttons= new string[]{"Use Demo", "Purchase"};
//
//        // Convert to GUI Content
//        var titleContent= new GUIContent(title);
//        GUIContent messagesContent= new GUIContent[messages.Length];
//        for(int i= 0; i < messages.Length; ++i) {
//            messagesContent[i]= new GUIContent(messages[i]);
//        }
//        GUIContent buttonsContent= new GUIContent[buttons.Length];
//        for(int i= 0; i < buttons.Length; ++i) {
//            buttonsContent[i]= new GUIContent(buttons[i]);
//        }
//        
//        // Compute logo layout.
//        const float kSpacer= 5f;
//        var logoWidth= 64f;
//        var logoHeight= 64f;
//        Rect logoRect= new Rect(kSpacer, kSpacer, logoWidth+kSpacer, logoHeight+kSpacer);
//
//        // Compute section #1 layout
//        GUIStyle titleStyle= new GUIStyle(GUI.skin.label);
//        titleStyle.fontStyle= FontStyle.Bold;
//        titleStyle.fontSize= 18;
//        var titleSize= titleStyle.CalcSize(titleContent);
//        Rect r1= new Rect(logoRect.xMax, kSpacer, titleSize.x+kSpacer, titleSize.y);
//        
//        
//            
//        // Show product icon
//        Texture2D iCanScriptLogo= null;
//        if(iCS_TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out iCanScriptLogo)) {
//            GUI.DrawTexture(logoRect, iCanScriptLogo);
//        }        		
//
//        // Show Title
//        GUI.Label(titleRect, title, titleStyle);
        
    }

    // =================================================================================
    // Utility
    // ---------------------------------------------------------------------------------
}
