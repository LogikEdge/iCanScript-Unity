using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public enum iCS_UpdateInterval { Daily= 0, Weekly= 1, Monthly= 2 };

public static class iCS_PreferencesController {
	// =================================================================================
    // Canvas Preferences
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    static  Color   kCanvasBackgroundColor;
    static  Color   kGridColor;
    const   float   kGridSpacing             = 20.0f;

	//
	// Database access keys
	//
    const   string  kCanvasBackgroundColorKey= "iCS_CanvasBackgroundColor";
    const   string  kGridColorKey            = "iCS_GridColor";
    const   string  kGridSpacingKey          = "iCS_GridSpacing";

	//
	// Reset to default value functions
	//
	public static void ResetCanvasBackgroundColor() {
		CanvasBackgroundColor= kCanvasBackgroundColor;
	}
	public static void ResetGridColor() {
		GridColor= kGridColor;
	}
	public static void ResetGridSpacing() {
		GridSpacing= kGridSpacing;
	}
	
	//
	// Accessors
	//
    public static Color CanvasBackgroundColor {
        get { return LoadColor(kCanvasBackgroundColorKey, kCanvasBackgroundColor); }
        set { SaveColor(kCanvasBackgroundColorKey, value); }
    }
    public static Color GridColor {
        get { return LoadColor(kGridColorKey, kGridColor); }
        set { SaveColor(kGridColorKey, value); }
    }
    public static float GridSpacing {
        get { return EditorPrefs.GetFloat(kGridSpacingKey, kGridSpacing); }
        set {
            if(value < 5.0f) return;
            EditorPrefs.SetFloat(kGridSpacingKey, value);
        }
    }


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
	public static void ResetSoftwareUpdateLastWatchDate() {
		SoftwareUpdateLastWatchDate= DateTime.Now;
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
	

	// =================================================================================
	// Activation
	// ---------------------------------------------------------------------------------
	static iCS_PreferencesController() {
        // Canvas colors
        kCanvasBackgroundColor= new Color(0.169f, 0.188f, 0.243f);
        kGridColor            = new Color(0.25f, 0.25f, 0.25f);
	}
	
	// =================================================================================
	// Utilities
	// ---------------------------------------------------------------------------------
	//
	// Saving / Loading Colors
	//
    static void SaveColor(string name, Color color) {
        EditorPrefs.SetFloat(name+"_R", color.r);
        EditorPrefs.SetFloat(name+"_G", color.g);
        EditorPrefs.SetFloat(name+"_B", color.b);
        EditorPrefs.SetFloat(name+"_A", color.a);        
    }
    static Color LoadColor(string name, Color defaultColor) {
        float r= EditorPrefs.GetFloat(name+"_R", defaultColor.r);
        float g= EditorPrefs.GetFloat(name+"_G", defaultColor.g);
        float b= EditorPrefs.GetFloat(name+"_B", defaultColor.b);
        float a= EditorPrefs.GetFloat(name+"_A", defaultColor.a);
        return new Color(r,g,b,a);        
    }

	//
	// Saving / Loading Date & Time
	//
	public static void SetDateTime(string key, DateTime dateTime) {
		long binaryTime= dateTime.ToBinary();
		int low= (int)(binaryTime & 0xffffffff);
		int high= (int)((binaryTime >> 32) & 0xffffffff);
		EditorPrefs.SetInt(key+"Low", low);
		EditorPrefs.SetInt(key+"High", high);
	}
	public static DateTime GetDateTime(string key, DateTime defaultDateTime) {
		long binaryTime= defaultDateTime.ToBinary();
		int low= (int)(binaryTime & 0xffffffff);
		int high= (int)((binaryTime >> 32) & 0xffffffff);
		low= EditorPrefs.GetInt(key+"Low", low);
		high= EditorPrefs.GetInt(key+"High", high);
		binaryTime= high & 0xffffffff;
		binaryTime <<= 32;
		binaryTime |= ((long)(low)) & 0xffffffff;
		return DateTime.FromBinary(binaryTime);
	}
}
