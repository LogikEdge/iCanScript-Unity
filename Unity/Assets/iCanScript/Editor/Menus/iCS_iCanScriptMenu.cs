using UnityEngine;
using UnityEditor;
using P=Prelude;

public static class iCS_iCanScriptMenu {
    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("iCanScript/Create Behaviour #&b", false, 1)]
	public static void CreateBehaviour() {
        iCS_Menu.CreateBehaviour();
	}
	[MenuItem("iCanScript/Create Behaviour #&b", true, 1)]
	public static bool ValidateCreateBehaviour() {
        return iCS_Menu.ValidateCreateBehaviour();
	}

    // ======================================================================
    // Navigation
    [MenuItem("iCanScript/",false,20)]
    [MenuItem("iCanScript/Center Visual Script _#&f",false,21)]
    public static void CenterGraph() {
        iCS_Menu.CenterAndScaleOnRoot();
    }
    [MenuItem("iCanScript/Center On Selected _&f",false,22)]
    public static void CenterOnSelected() {
        iCS_Menu.CenterOnSelected();
    }
    // ======================================================================
    // Documentation Access
    [MenuItem("iCanScript/",false,30)]
    [MenuItem("iCanScript/Documentation/Home Page",false,31)]
    public static void HomePage() {
        Application.OpenURL("http://www.icanscript.com");
    }
    [MenuItem("iCanScript/Documentation/User's Manual",false,32)]
    public static void UserManual() {
        Application.OpenURL("http://www.icanscript.com/documentation/user_guide");
    }
    [MenuItem("iCanScript/Documentation/Release Notes",false,34)]
    public static void ReleaseNotes() {
        Application.OpenURL("http://www.icanscript.com/support/release_notes");
    }
    // ======================================================================
    // Support Access
    [MenuItem("iCanScript/Customer Request",false,60)]
    public static void ReportBug() {
        Application.OpenURL("http://www.disruptive-sw.com/support/customer_request");
    }
    [MenuItem("iCanScript/Check for Updates...",false,61)]
    public static void CheckForUpdate() {
		var isLatest= iCS_InstallerMgr.CheckForUpdates();
		if(isLatest.isNothing) {
			EditorUtility.DisplayDialog("Unable to determine latest version !!!",
										"Problem accessing iCanScript version information.\nPlease try again later.",
										"Ok");			
			return;
		}
        if(isLatest.Value) {
			EditorUtility.DisplayDialog("You have the latest version of iCanScript!",
										 "The version installed is: v"+iCS_EditorConfig.VersionId+".\nNo updates are available.",
										 "Ok");
		}
    }
}
