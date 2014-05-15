using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PurchaseForm : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    string  userLicenseStr;
    
    // =================================================================================
    // Menu
    // ---------------------------------------------------------------------------------
    [MenuItem("Help/iCanScript/Purchase...", false, 71)]
    static void Initialize() {
        EditorWindow.GetWindow(typeof(iCS_PurchaseForm), true, "iCanScript Purchase Form");
    }
    [MenuItem("Help/iCanScript/Purchase...", true, 70)]
    public static bool IsNotPurchased() {
        return !iCS_LicenseController.IsActivated;
    }
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnEnable() {
        ReadLicense();
    }
    public void OnDisable() {
    }


    // =================================================================================
    // OnGUI
    // ---------------------------------------------------------------------------------
    public void OnGUI() {
        userLicenseStr= EditorGUILayout.TextField("License", userLicenseStr);
        if(GUI.Button(new Rect(50,50,50,50), "Activate")) {
            iCS_LicenseType licenseType;
            uint licenseVersion;
            string errorMessage;
            if(iCS_LicenseController.ValidateLicense(userLicenseStr, out licenseType, out licenseVersion, out errorMessage)) {
                iCS_PreferencesController.UserLicense= userLicenseStr;
                Debug.Log("Thank you for purchasing iCanScript.  Now go and make you games better...");
            }
            else {
                Debug.Log("iCanScript: "+errorMessage);
            }
        }
    }

    // =================================================================================
    // Utility
    // ---------------------------------------------------------------------------------
    void ReadLicense() {
        userLicenseStr= iCS_PreferencesController.UserLicense;
    }
}
