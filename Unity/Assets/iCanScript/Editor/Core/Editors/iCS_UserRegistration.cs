using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_UserRegistration : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    string  userLicenseStr;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    [MenuItem("DevTools/User Registration", false, 1050)]
    static void Initialize() {
        EditorWindow.GetWindow(typeof(iCS_UserRegistration), true, "iCanScript User Registration");
    }
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
