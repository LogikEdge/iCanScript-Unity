using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_DemoDialogs {
    [MenuItem("Help/iCanScript/Purchase...", false, 71)]
    public static void PurchaseDialog() {
        string title= "iCanScript Activation Needed ("+iCS_LicenseController.RemainingTrialDays+" days remaining)";
        var option= EditorUtility.DisplayDialogComplex(title, "Activation is needed to use the Unity Asset Store edition of iCanScript.  Please choose one of the following options.",
                                                              "Use Demo",
                                                              "Purchase",
                                                              "Activate");
        switch(option) {
            // Use Demo
            case 0: {}
            break;
            // Purchase
            case 1: {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");
            }
            break;                
            // Activate
            case 2: {
                ActivationDialog();
            } 
            break;
        }
    }
    [MenuItem("Help/iCanScript/Activation...", false, 70)]
    public static void ActivationDialog() {
        string title= "Activate Your User License ("+iCS_LicenseController.RemainingTrialDays+" days remaining)";
        var option= EditorUtility.DisplayDialogComplex(title, "Activation of the Unity Asset Store edition of iCanScript requires a user license.  Please request a license if you haven't already do so.",
                                                              "Waiting for License",
                                                              "Request License",
                                                              "Activate License");
        switch(option) {
            // Waiting for License
            case 0: break;
            // Request License
            case 1: {
                RequestLicense();
            }
            break;
            // Activate License
            case 2: {
                ActivateLicense();
            }
            break;
        }        
    }
    [MenuItem("Help/iCanScript/Activation...", true, 70)]
    public static bool IsNotActivated() {
        return !iCS_LicenseController.IsActivated;
    }
    [MenuItem("Help/iCanScript/Purchase...", true, 71)]        
    public static bool IsNotPurchased() {
        return !iCS_LicenseController.IsActivated;
    }    
    
    public static void RequestLicense() {
        FunctionalityNotAvailableInDemo();
        iCS_LicenseController.Initialize();
    }
    public static void ActivateLicense() {
        FunctionalityNotAvailableInDemo();
        iCS_LicenseController.Initialize();
    }
    public static void FunctionalityNotAvailableInDemo() {
        if(!EditorUtility.DisplayDialog("Functionality not available in the Demo Edition",
                                       "The requested functionality is only available in the full version of iCanScript.  Consider purchasing iCanScript to access this feature.",
                                       "Cancel","Purchase")) {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");                                        
        }
    }
}
