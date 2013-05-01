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
        var download = WebRequest( iCS_WebConfig.LatestReleaseIdScript, form );
        if(!String.IsNullOrEmpty(download.error)) {
            return null;
        }
        var xmlDoc= new XmlDocument();
        xmlDoc.Load(download.text);
        var parentNode= xmlDoc.GetElementsByTagName("version");
        string version= null;
        foreach (XmlNode childrenNode in parentNode)
        {
            version= childrenNode.SelectSingleNode("//version").Value;
        }
//        return version;
        return download.text;
    }

    // ----------------------------------------------------------------------
    // Returns true if the current version is the latest version.
    public static bool IsLatestVersion() {
        var latestVersion= GetLatestReleaseId();
        if(String.IsNullOrEmpty(latestVersion)) {
            return true;
        }
        var currentVersion= "v"+iCS_Config.MajorVersion+"."+iCS_Config.MinorVersion+"."+iCS_Config.BugFixVersion;
        return currentVersion == latestVersion;
    }
}
