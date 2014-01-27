using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public enum iCS_UpdateInterval { Daily= 0, Weekly= 1, Monthly= 2 };

public static class iCS_PreferencesController {
	// =================================================================================
	// Software Updates Preferences
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
	const bool	 kSoftwareUpdateWatchEnabled			= true;
	const int    kSoftwareUpdateInterval				= 0;  // 0= day, 1= week, 2= month
	const string kSoftwareUpdateSkippedVersion			= "";

	//
	// Database access keys
	//
	const string kSoftwareUpdateWatchEnabledKey			= "iCS_SoftwareUpdateWatchEnabled";
	const string kSoftwareUpdateIntervalKey             = "iCS_SoftwareUpdateInterval";
	const string kSoftwareUpdateSkippedVersionKey		= "iCS_SoftwareUpdateSkippedVersion";
	const string kSoftwareUpdateLastWatchDateKey        = "iCS_SoftwareUpdateLastWatchDate";

	//
	// Reset to default value functions
	//
	public static void ResetSoftwareUpdateWatchEnabled() {
		SoftwareUpdateWatchEnabled= kSoftwareUpdateWatchEnabled;
	}
	public static void ResetSoftwareUpdateInterval() {
		SoftwareUpdateInterval= kSoftwareUpdateInterval;
	}
	public static void ResetSoftwareUpdateSkippedVersion() {
		SoftwareUpdateSkippedVersion= kSoftwareUpdateSkippedVersion;
	}

	//
	// Accessors
	//
    public static bool SoftwareUpdateWatchEnabled {
        get { return EditorPrefs.GetBool(kSoftwareUpdateWatchEnabledKey, kSoftwareUpdateWatchEnabled); }
        set { EditorPrefs.SetBool(kSoftwareUpdateWatchEnabledKey, value); }        
    }
    public static iCS_UpdateInterval SoftwareUpdateInterval {
        get { return (iCS_UpdateInterval)EditorPrefs.GetInt(kSoftwareUpdateIntervalKey, kSoftwareUpdateInterval); }
        set { EditorPrefs.SetInt(kSoftwareUpdateIntervalKey, (int)value); }        
    }
    public static string SoftwareUpdateSkippedVersion {
        get { return EditorPrefs.GetString(kSoftwareUpdateSkippedVersionKey, kSoftwareUpdateSkippedVersion); }
        set { EditorPrefs.SetString(kSoftwareUpdateSkippedVersionKey, value); }        
    }
	public static DateTime SoftwareUpdateLastWatchDate {
		get { return GetDateTime(kSoftwareUpdateLastWatchDateKey, DateTime.Now); }
		set { SetDateTime(kSoftwareUpdateLastWatchDateKey, value); }
	}
	
//	public static void ResetSoftwareUpdateWatchDate() {
//		SoftwareUpdateLastWatchDate= GetDefaultSoftwareUpdateWatchTime();
//	}
//	public static DateTime GetDefaultSoftwareUpdateWatchTime() {
//		DateTime lastWatch= DateTime.Now;
//		switch(SoftwareUpdateInterval) {
//			case Daily:
//				break;
//			case Weekly:
//				break;
//			case Montly:
//				break;
//		}
//		return lastWatch;
//	}


	// =================================================================================
	// Utilities
	// -------------------------------------------------------------------------
	public static void SetDateTime(string key, DateTime dateTime) {
		long binaryTime= dateTime.ToBinary();
		int low= (int)(binaryTime & 0xffff);
		int high= (int)((binaryTime >> 32) & 0xffff);
		EditorPrefs.SetInt(key+"Low", low);
		EditorPrefs.SetInt(key+"High", high);
	}
	public static DateTime GetDateTime(string key, DateTime defaultDateTime) {
		long binaryTime= defaultDateTime.ToBinary();
		int low= (int)(binaryTime & 0xffff);
		int high= (int)((binaryTime >> 32) & 0xffff);
		low= EditorPrefs.GetInt(key+"Low", low);
		high= EditorPrefs.GetInt(key+"High", high);
		binaryTime= high & 0xffff;
		binaryTime <<= 32;
		binaryTime |= low;
		return new DateTime(binaryTime);
	}
}
