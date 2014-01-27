using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using iCanScript;
using P=Prelude;

public static class iCS_UpdateController {
    // ----------------------------------------------------------------------
	// Performs the periodic verification for an update.
	public static void PeriodicUpdateVerification() {
		if(!iCS_PreferencesEditor.SoftwareUpdateWatchEnabled) return;
		string latestVersion= iCS_UpdateController.GetLatestReleaseId();
		if(latestVersion == null) {
			Debug.Log("iCanScript: Version server not accessible...");
			return;
		}
		Maybe<bool> isLatest= IsLatestVersion(latestVersion);
		if(isLatest.isNothing) return;
		Debug.Log("iCanScript: Latest version is: "+latestVersion+" up to date: "+isLatest.Value);
		
	}
    // ----------------------------------------------------------------------
    // Returns the version string of the latest available release.
    public static string GetLatestReleaseId(float waitTime= 500f) {
		var url= iCS_WebConfig.WebService_Versions;
        var download = iCS_WebUtils.WebRequest(url, waitTime);
        if(!String.IsNullOrEmpty(download.error)) {
            return null;
        }
//        Debug.Log(download.text);
        JString jVersion= null;
        try {
            jVersion= JSON.GetValueFor(download.text, "versions.iCanScript") as JString;            
        }
        catch(System.Exception) {}
        return jVersion == null ? null : jVersion.value;
    }

    // ----------------------------------------------------------------------
    // Returns true if the current version is the latest version.
    public static Maybe<bool> IsLatestVersion() {
		return IsLatestVersion(GetLatestReleaseId());
    }
    // ----------------------------------------------------------------------
    // Returns true if the current version is the latest version.
    public static Maybe<bool> IsLatestVersion(string latestVersion) {
        if(String.IsNullOrEmpty(latestVersion)) {
            return new Nothing<bool>();
        }
        var currentVersion= "v"+iCS_EditorConfig.VersionId;
        return new Just<bool>(currentVersion == latestVersion);
    }

}
