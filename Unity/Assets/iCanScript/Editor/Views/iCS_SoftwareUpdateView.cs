// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// iCS_PreferencesController.cs
//
// Revised: 2014-01-29
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_SoftwareUpdateView {
    // ---------------------------------------------------------------------------------
	public static int ShowNewVersionDialog(iCS_Version currentVersion, iCS_Version latestVersion) {
        return EditorUtility.DisplayDialogComplex("A new version of iCanScript is available!",
                                                  "Version "+latestVersion+" is available for download.\n"+
                                                  "Would you like to download it?",
                                                  "Download", "Skip This Version","Cancel");
	}
	
    // ---------------------------------------------------------------------------------
	public static void ShowServerUnavailableDialog() {
		EditorUtility.DisplayDialog("Unable to determine latest version !!!",
									"Problem accessing iCanScript version information.\nPlease try again later.",
									"Ok");			
	}

    // ---------------------------------------------------------------------------------
	public static void ShowAlreadyCurrentDialog() {
		EditorUtility.DisplayDialog("You have the latest version of iCanScript!",
									 "The version installed is: v"+iCS_EditorConfig.VersionId+".\nNo updates are available.",
									 "Ok");
	}
}
