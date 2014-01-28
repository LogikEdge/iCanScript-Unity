#define DEBUG

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using iCanScript;
using P=Prelude;
using Prefs=iCS_PreferencesController;

public static class iCS_SoftwareUpdateController {
    // ----------------------------------------------------------------------
	// Performs the periodic verification for an update.
	public static void PeriodicUpdateVerification() {
		// Return if software update watch is disabled.
		if(!Prefs.SoftwareUpdateWatchEnabled) {
#if DEBUG
			Debug.Log("iCanScript: Software Update disabled.");
#endif
			return;
		}
		// Return if we already verified within the prescribed interval;
		DateTime nextWatchDate= Prefs.SoftwareUpdateLastWatchDate;
		switch(Prefs.SoftwareUpdateInterval) {
			case iCS_UpdateInterval.Daily:
				nextWatchDate= nextWatchDate.AddDays(1);
				break;
			case iCS_UpdateInterval.Weekly:
				nextWatchDate= nextWatchDate.AddDays(7);
				break;
			case iCS_UpdateInterval.Monthly:
				nextWatchDate= nextWatchDate.AddMonths(1);
				break;
		}
		DateTime now= DateTime.Now;
		if(nextWatchDate.CompareTo(now) >= 0) {
#if DEBUG
			Debug.Log("iCanScript: Software Update does not need to be verified before: "+nextWatchDate);
#endif
			return;
		}
		
		// Get the last revision from the server.
		string latestVersion= GetLatestReleaseId();
		if(latestVersion == null) {
			Debug.Log("iCanScript: Unable to contact version server. Software update verification aborted.");
			return;
		}
		// Return if the user wants to skip this version.
		if(Prefs.SoftwareUpdateSkippedVersion == latestVersion) {
#if DEBUG
			Debug.Log("iCanScript: User requested to skipped software update for: "+latestVersion);
#endif
			return;
		}
		// Determine if we are up-to-date.
		Maybe<bool> isLatest= IsLatestVersion(latestVersion);
		if(isLatest.isNothing) {
#if DEBUG
			Debug.Log("iCanScript: Unable to contact version server.");
#endif
			return;
		}
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
