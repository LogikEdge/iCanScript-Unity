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
		// Initialize last watch date if not in database.
		// (The date returned will be the "now" date if it is not found in the database.)
		DateTime now= DateTime.Now;
		DateTime nextWatchDate= Prefs.SoftwareUpdateLastWatchDate;
		if(now.CompareTo(nextWatchDate) <= 0 && nextWatchDate.CompareTo(DateTime.Now) <= 0) {
#if DEBUG
			Debug.Log("iCanScript: Software update last watch date not initialized. Initializing...");
#endif
			Prefs.SoftwareUpdateLastWatchDate= now;
		}
		// Return if we already verified within the prescribed interval;
		nextWatchDate= AddInterval(nextWatchDate);
		if(nextWatchDate.CompareTo(now) >= 0) {
#if DEBUG
			Debug.Log("iCanScript: Software Update does not need to be verified before: "+nextWatchDate);
#endif
//			return;
		}
		// Get the last revision from the server.
		string serverVersion= GetLatestReleaseId();
		if(serverVersion == null) {
			Debug.Log("iCanScript: Unable to contact version server. Software update verification postponed.");
			return;
		}
		// Update last watch date now that we can contact the version server.
		Prefs.SoftwareUpdateLastWatchDate= AddInterval(now);
		// Return if the user wants to skip this version.
		if(Prefs.SoftwareUpdateSkippedVersion == serverVersion) {
#if DEBUG
			Debug.Log("iCanScript: User requested to skipped software update for: "+serverVersion);
#endif
			return;
		}
		// Determine if we are up-to-date.
		Maybe<bool> isUpToDate= IsUpToDate(serverVersion);
		if(isUpToDate.isNothing) {
#if DEBUG
			Debug.Log("iCanScript: Unable to contact version server.");
#endif
			return;
		}
		Debug.Log("iCanScript: Latest version is: "+serverVersion+" up to date: "+isUpToDate.Value);
		if(!isUpToDate.Value) {
			EditorApplication.ExecuteMenuItem("Help/iCanScript/Check for Updates...");
		}
	}
    // ----------------------------------------------------------------------
    // Returns the version string of the latest available release.
    public static string GetLatestReleaseId(float waitTime= 2f) {
		var url= iCS_WebConfig.WebService_Versions;
		url= "www.icanscript.com/versions.txt";
        var download = iCS_WebUtils.WebRequest(url, waitTime);
        if(!String.IsNullOrEmpty(download.error)) {
            return null;
        }
#if DEBUG
        Debug.Log(download.text);
#endif
        JString jVersion= null;
        try {
			JObject rootObject= JSON.GetRootObject(download.text);
            jVersion= rootObject.GetValueFor("versions.iCanScript") as JString;
        }
#if DEBUG
        catch(System.Exception e) {
			Debug.LogWarning("iCanScript: JSON exception: "+e.Message);
        }
#else
        catch(System.Exception) {}
#endif        	
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
    // ----------------------------------------------------------------------
    // Returns true if the current version is equal or younger then the
	// version returned by the server.
    public static Maybe<bool> IsUpToDate(string serverVersionStr) {
        if(String.IsNullOrEmpty(serverVersionStr)) {
            return new Nothing<bool>();
        }
        var currentVersion= new iCS_Version(iCS_Config.MajorVersion,
											iCS_Config.MinorVersion,
											iCS_Config.BugFixVersion);

		serverVersionStr= serverVersionStr.Substring(1);
		var serverVersion= iCS_Version.FromString(serverVersionStr);
        return new Just<bool>(currentVersion.IsNewerOrSameAs(serverVersion));
    }
    // ----------------------------------------------------------------------
	// Returns the given plus the software update interval.
	public static DateTime AddInterval(DateTime date) {
		switch(Prefs.SoftwareUpdateInterval) {
			case iCS_UpdateInterval.Daily:
				return date.AddDays(1);
			case iCS_UpdateInterval.Weekly:
				return date.AddDays(7);
			case iCS_UpdateInterval.Monthly:
				return date.AddMonths(1);
		}
		return date.AddDays(1);
	}
}
