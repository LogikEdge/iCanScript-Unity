using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_SoftwareUpdateView {
        // ---------------------------------------------------------------------------------
    	public static int ShowNewVersionDialog(Version currentVersion, Version latestVersion) {
            return EditorUtility.DisplayDialogComplex("A new version of iCanScript is available!",
                                                      "iCanScript "+latestVersion+" is available for download.\n"+
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
    									 "iCanScript "+iCS_EditorConfig.VersionId+" is the newest version available.",
    									 "Ok");
    	}
    }
}

