#define DEBUG

using UnityEngine;
using UnityEditor;
using P=Prelude;

public static class iCS_iCanScriptMenu {
    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("Edit/", false, 100)]
	[MenuItem("Edit/iCanScript/Create Visual Script", false, 101)]
	public static void CreateVisualScript() {
		iCS_Storage storage = Selection.activeGameObject.GetComponent<iCS_Storage>();
		if(storage == null) {
		    iCS_MenuUtility.InstallVisualScriptOn(Selection.activeGameObject);
		}
	}
	[MenuItem("Edit/iCanScript/Create Visual Script", true, 101)]
	public static bool ValidateCreateVisualScript() {
		if(Selection.activeTransform == null) return false;
		iCS_Storage storage = Selection.activeGameObject.GetComponent<iCS_Storage>();
		return storage == null;
	}

    // ======================================================================
    // Navigation
    [MenuItem("Edit/iCanScript/Center Visual Script #f",false,121)]
    public static void FocusOnVisualScript() {
        iCS_VisualEditor visualEditor= iCS_EditorMgr.FindVisualEditor();
        if(visualEditor != null) visualEditor.CenterAndScaleOnRoot();
    }
    [MenuItem("Edit/iCanScript/Focus On Selected _f",false,122)]
    public static void FocusOnSelected() {
        iCS_VisualEditor graphEditor= iCS_EditorMgr.FindVisualEditor();
        if(graphEditor != null) graphEditor.CenterAndScaleOnSelected();
    }
    // ======================================================================
    // Documentation Access
    [MenuItem("Help/iCanScript/Home Page",false,31)]
    public static void HomePage() {
        Application.OpenURL("http://www.icanscript.com");
    }
    [MenuItem("Help/iCanScript/User Guide",false,32)]
    public static void UserManual() {
        Application.OpenURL("http://www.user-guide.icanscript.com/all-in-one");
    }
    [MenuItem("Help/iCanScript/Release Notes",false,34)]
    public static void ReleaseNotes() {
        Application.OpenURL("http://www.icanscript.com/support/release-notes");
    }
    // ======================================================================
    // Support Access
    [MenuItem("Help/iCanScript/Customer Request",false,60)]
    public static void ReportBug() {
        Application.OpenURL("http://www.disruptive-sw.com/support/customer-request");
    }
    [MenuItem("Help/iCanScript/Check for Updates...",false,61)]
    public static void CheckForUpdate() {
		iCS_SoftwareUpdateController.ManualUpdateVerification();
    }
    
#if DEBUG
    // ======================================================================
    // Sanity Check
	[MenuItem("DEBUG/Sanity Check Selection",false,1000)]
	public static void MenuSanityCheck() {
		iCS_IStorage storage= iCS_StorageMgr.IStorage;
		if(storage == null) return;
		Debug.Log("iCanScript: Start Sanity Check on: "+storage.Storage.name);
		storage.SanityCheck();
		Debug.Log("iCanScript: Completed Sanity Check on: "+storage.Storage.name);
	}
    // ======================================================================
    // Trigger Periodic Software Update Verification
	[MenuItem("DEBUG/Invoke Periodic Software Update Verification",false,1001)]
	public static void MenuPeriodicSoftwareUpdateVerification() {
		iCS_SoftwareUpdateController.PeriodicUpdateVerification();
	}	
#endif
	    
//    // ======================================================================
//	// iCanScript License.
//    [MenuItem("iCanScript/Get FingerPrint")]
//    public static void GetFingerPrint() {
//        Debug.Log(iCS_LicenseUtil.ToString(iCS_FingerPrint.FingerPrint));
//    }
//    [MenuItem("iCanScript/Get Standard License")]
//    public static void GetStandardLicense() {
//        Debug.Log(iCS_LicenseUtil.ToString(iCS_UnlockKeyGenerator.Standard));
//    }
//    [MenuItem("iCanScript/Get Pro License")]
//    public static void GetProLicense() {
//        Debug.Log(iCS_LicenseUtil.ToString(iCS_UnlockKeyGenerator.Pro));
//    }
//    [MenuItem("iCanScript/License Manager")]
//    public static void EnterLicense() {
//    }
    
}
