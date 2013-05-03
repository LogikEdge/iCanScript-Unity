using UnityEditor;
using UnityEngine;
using System;
using System.Xml;
using System.Collections;

public static class iCS_WebUtils {
    // ----------------------------------------------------------------------
    // Performs a Web request (POST/GET) on the given url.
    public static WWW WebRequest(string url, WWWForm form, float waitTime= 500f) {
        var transaction= new WWW(url, form);
        var startTime= Time.realtimeSinceStartup;
        while(!transaction.isDone) {
            if((Time.realtimeSinceStartup-startTime) > waitTime) {
                Debug.LogWarning("iCanScript: Timeout waiting for URL: "+url);
                break;
            }
        }
        return transaction;
    }
    
    // ----------------------------------------------------------------------
    // Returns the version string of the latest available release.
    public static string GetLatestReleaseId() {
        var form= new WWWForm();
        form.AddField("major", iCS_Config.MajorVersion);
        form.AddField("minor", iCS_Config.MinorVersion);
        form.AddField("bugFix", iCS_Config.BugFixVersion);
        form.AddField("build", 0);
//		var url= iCS_WebConfig.WebService_Versions;
		var url= "www.infaunier.com/scripts/version.pl";
        var download = WebRequest(url, form);
        if(!String.IsNullOrEmpty(download.error)) {
			Debug.Log("URL: "+url);
			Debug.Log("Error trying to access Version: "+download.error);
            return null;
        }
        string version= download.text;
        return version;
    }

    // ----------------------------------------------------------------------
    // Returns true if the current version is the latest version.
    public static bool IsLatestVersion() {
        var latestVersion= GetLatestReleaseId();
        if(String.IsNullOrEmpty(latestVersion)) {
            return true;
        }
        var currentVersion= "v"+iCS_EditorConfig.VersionId;
        return currentVersion == latestVersion;
    }
}
