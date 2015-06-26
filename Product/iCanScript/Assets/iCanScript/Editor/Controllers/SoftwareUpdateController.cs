//#define DEBUG_INFO

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.JSON;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    using Prefs=PreferencesController;
    
    public static class SoftwareUpdateController {
    	// =================================================================================
        // Server URL Information
        // ---------------------------------------------------------------------------------
    	const string URL_VersionFile=  iCS_WebConfig.LatestVersionInfoFile;
    	const string URL_DownloadPage= "http://"+iCS_WebConfig.DownloadsPage;
    	
    		
        static SoftwareUpdateController() {
            PeriodicUpdateVerification();
        }
        public static void Start() {}
        public static void Shutdown() {}
        
    	// =================================================================================
        // Manual & Periodic Software Update Verification functions.
        // ---------------------------------------------------------------------------------
    	//
    	// Periodic Verification
    	//
    	public static void PeriodicUpdateVerification() {
    		// Return if software update watch is disabled.
    		if(!Prefs.SoftwareUpdateWatchEnabled) {
#if DEBUG_INFO
    			Debug.Log("iCanScript: Software Update disabled.");
#endif
    			return;
    		}
    		// Initialize last watch date if not in database.
    		// (The date returned will be the "now" date if it is not found in the database.)
    		DateTime now= DateTime.Now;
    		DateTime nextWatchDate= Prefs.SoftwareUpdateLastWatchDate;
    		if(now.CompareTo(nextWatchDate) <= 0 && nextWatchDate.CompareTo(DateTime.Now) <= 0) {
#if DEBUG_INFO
    			Debug.Log("iCanScript: Software update last watch date not initialized. Initializing...");
#endif
    			Prefs.SoftwareUpdateLastWatchDate= now;
    		}
    		// Return if we already verified within the prescribed interval;
    		nextWatchDate= AddInterval(nextWatchDate);
    		if(nextWatchDate.CompareTo(now) >= 0) {
#if DEBUG_INFO
    			Debug.Log("iCanScript: Software Update does not need to be verified before: "+nextWatchDate);
#else
    //			return;
#endif
    		}
    		// Get the last revision from the server.
    		string downloadUrl= null;
    		Version serverVersion= GetLatestReleaseId(out downloadUrl);
    		if(serverVersion == null) {
    			Debug.Log("iCanScript: Unable to contact version server. Software update verification postponed.");
    			return;
    		}
    		// Update last watch date now that we can contact the version server.
    		Prefs.SoftwareUpdateLastWatchDate= AddInterval(now);
    		// Return if the user wants to skip this version.
    		if(Prefs.SoftwareUpdateSkippedVersion == serverVersion.ToString()) {
#if DEBUG_INFO
    			Debug.Log("iCanScript: User requested to skipped software update for: "+serverVersion);
#endif
    			return;
    		}
    		// Determine if we are up-to-date.
    		Maybe<bool> isUpToDate= IsUpToDate(serverVersion);
    		if(isUpToDate.isNothing) {
#if DEBUG_INFO
    			Debug.Log("iCanScript: Unable to contact version server.");
#endif
    			return;
    		}
#if DEBUG_INFO
    		Debug.Log("iCanScript: Latest version is: "+serverVersion+" up to date: "+isUpToDate.Value);
#endif
    		if(!isUpToDate.Value) {
    			ManualUpdateVerification();
    		}
    	}
    
        // ---------------------------------------------------------------------------------
    	//
    	// Manual Verification
    	//
    	public static void ManualUpdateVerification() {
    		// Check if we have the most up-to-date software
    		string downloadUrl= null;
    		Version latestVersion= GetLatestReleaseId(out downloadUrl);
    		// Tell the user we can't contact the server and abort.
    		if(latestVersion == null || downloadUrl == null) {
    			iCS_SoftwareUpdateView.ShowServerUnavailableDialog();
    			return;
    		}
    		// Tell the user we have the latest version.
    		var isLatest= IsUpToDate(latestVersion);
            if(isLatest.Value) {
    			iCS_SoftwareUpdateView.ShowAlreadyCurrentDialog();
    			return;
    		}
    		// Tell the user a new version exists and ask him/her to download it.
    		var selection= iCS_SoftwareUpdateView.ShowNewVersionDialog(Version.Current, latestVersion);
    		switch(selection) {
    			case 0:	// Download
    				Application.OpenURL(downloadUrl);
    //				Application.OpenURL(URL_DownloadPage);            
    				break;
    			case 1:	// Skip this version
    				PreferencesController.SoftwareUpdateSkippedVersion= latestVersion.ToString();
    				break;
    			default: // Cancel
    				break;
    		}
    	}
    	
    	
    	// =================================================================================
    	// Software Update Verification Support Functions.
        // ---------------------------------------------------------------------------------
        // Returns the version string of the latest available release.
        static Version GetLatestReleaseId(out string downloadUrl, float waitTime= 2f) {
    		downloadUrl= null;
    		var url= URL_VersionFile;
            var download = iCS_WebUtils.WebRequest(url, waitTime);
            if(!String.IsNullOrEmpty(download.error)) {
                return null;
            }
#if DEBUG_INFO
            Debug.Log(download.text);
#endif
            JNumber jMajor = null;
    		JNumber jMinor = null;
    		JNumber jBugFix= null;
            try {
    			JObject rootObject= JSON.JSON.GetRootObject(download.text);
                JObject latestVersion=  rootObject.GetValueFor("iCanScript") as JObject;
    			if(!latestVersion.isNull) {
    				jMajor = latestVersion.GetValueFor("major") as JNumber;
    				jMinor = latestVersion.GetValueFor("minor") as JNumber;
    				jBugFix= latestVersion.GetValueFor("bugFix") as JNumber;
    				JString jDownloadUrl= rootObject.GetValueFor("downloadUrl") as JString;
    				if(jDownloadUrl != null) {
    					downloadUrl= jDownloadUrl.value;
    				}
#if DEBUG_INFO
    				else {
    					Debug.Log("iCanScript: Unable to determine the download URL");
    					Debug.Log("iCanScript: JSON root object is: "+rootObject.Encode());
    				}
#endif					
    			}
            }
#if DEBUG_INFO
            catch(System.Exception e) {
    			Debug.LogWarning("iCanScript: JSON exception: "+e.Message);
            }
#else
            catch(System.Exception) {}
#endif        	
    		if(jMajor == null || jMinor == null || jBugFix == null) return null;
    		return new Version((int)jMajor.value, (int)jMinor.value, (int)jBugFix.value);
        }
    
        // ----------------------------------------------------------------------
        // Returns true if the current version is equal or younger then the
    	// version returned by the server.
        static Maybe<bool> IsUpToDate(Version serverVersion) {
            if(serverVersion == null) {
                return new Nothing<bool>();
            }
            return new Just<bool>(
    			serverVersion.IsOlderOrEqualTo(iCS_Config.MajorVersion,
    								  		  iCS_Config.MinorVersion,
    								  		  iCS_Config.BugFixVersion)
    		);
        }
        // ----------------------------------------------------------------------
    	// Returns the given plus the software update interval.
    	static DateTime AddInterval(DateTime date) {
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
    
}