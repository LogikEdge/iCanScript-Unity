using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_TrialDialogs {
    public static void PurchaseDialog() {
        string title= "iCanScript Activation Needed ("+iCS_LicenseController.RemainingTrialDays+" days remaining)";
        if(EditorUtility.DisplayDialog(title, "Activation is needed to use the Unity Asset Store edition of iCanScript.  Please choose one of the following options.",
                                                              "Purchase",
                                                              "Use Trial")) {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/16872");
        }
    }
}
