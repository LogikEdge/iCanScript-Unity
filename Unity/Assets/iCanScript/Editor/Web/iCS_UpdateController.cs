using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using iCanScript;
using P=Prelude;

public static class iCS_UpdateController {
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
        var latestVersion= GetLatestReleaseId();
        if(String.IsNullOrEmpty(latestVersion)) {
            return new Nothing<bool>();
        }
        var currentVersion= "v"+iCS_EditorConfig.VersionId;
        return new Just<bool>(currentVersion == latestVersion);
    }

}
