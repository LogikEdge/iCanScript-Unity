// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// iCS_PreferencesController.cs
//
// Revised: 2014-01-29
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//#define CODE_GENERATION_CONFIG

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public enum iCS_UpdateInterval { Daily= 0, Weekly= 1, Monthly= 2 };


public static class iCS_PreferencesController {
//	// =================================================================================
//    // Debug Preferences
//    // ---------------------------------------------------------------------------------
//	//
//	// Default Values
//	//
//    const bool  kDebugTrace= false;
//
//	//
//	// Database access keys
//	//
//    const string kDebugTraceKey= "iCS_DebugTrace";
//    
//	//
//	// Reset to default value functions
//	//
//    public static void ResetDebugTrace() {
//        DebugTrace= kDebugTrace;
//    }
//    
//	//
//	// Accessors
//	//
//    public static bool DebugTrace {
//        get { return EditorPrefs.GetBool(kDebugTraceKey, kDebugTrace); }
//        set { EditorPrefs.SetBool(kDebugTraceKey, value); }        
//    }
    
	// =================================================================================
    // Canvas Preferences
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    static  Color   kCanvasBackgroundColor;
    static  Color   kGridColor;
    const   float   kGridSpacing = 40.0f;

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
    // Display Option Constants
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    const bool   kIsAnimationEnabled         = true;
    const float  kAnimationTime              = 0.5f;
    const float  kScrollSpeed                = 3.0f;
    const float  kEdgeScrollSpeed            = 400.0f;
    const bool   kInverseZoom                = false;
    const float  kZoomSpeed                  = 1.0f;
    const bool   kShowRuntimePortValue       = false;
    const float  kPortValueRefreshPeriod     = 0.1f;
	const bool   kShowRuntimeFrameId         = false;
    const bool   kShowNodeStereotype         = false;

	//
	// Database access keys
	//
    const string kIsAnimationEnabledKey      = "iCS_IsAnimationEnabled";
    const string kAnimationTimeKey           = "iCS_AnimationTime";
    const string kScrollSpeedKey             = "iCS_ScrollSpeed";
    const string kEdgeScrollSpeedKey         = "iCS_EdgeScrollSpeed";
    const string kInverseZoomKey             = "iCS_InverseZoom";
    const string kZoomSpeedKey               = "iCS_ZoomSpeed";
    const string kShowRuntimePortValueKey    = "iCS_ShowRuntimePortValue";
    const string kPortValueRefreshPeriodKey  = "iCS_PortValueRefresh";
    const string kShowRuntimeFrameIdKey      = "iCS_ShowRuntimeFrameId";
    const string kShowNodeStereotypeKey      = "iCS_ShowNodeStereotype";

	//
	// Reset to default value functions
	//
    public static void ResetIsAnimationEnabled() {
	    IsAnimationEnabled= kIsAnimationEnabled;
    }
    public static void ResetAnimationTime() {
	    AnimationTime= kAnimationTime;
    }
    public static void ResetScrollSpeed() {
	    ScrollSpeed= kScrollSpeed;
    }
    public static void ResetEdgeScrollSpeed() {
	    EdgeScrollSpeed= kEdgeScrollSpeed;
    }
    public static void ResetInverseZoom() {
	    InverseZoom= kInverseZoom;
    }
    public static void ResetZoomSpeed() {
	    ZoomSpeed= kZoomSpeed;
    }
    public static void ResetShowRuntimePortValue() {
	    ShowRuntimePortValue= kShowRuntimePortValue;
    }
    public static void ResetPortValueRefreshPeriod() {
	    PortValueRefreshPeriod= kPortValueRefreshPeriod;
    }
	public static void ResetShowRuntimeFrameId() {
		ShowRuntimeFrameId= kShowRuntimeFrameId;    	
	}
    public static void ResetShowNodeStereotype() {
        ShowNodeStereotype= kShowNodeStereotype;
    }

	//
	// Accessors
	//
    public static bool IsAnimationEnabled {
        get {
            return EditorPrefs.GetBool(kIsAnimationEnabledKey, kIsAnimationEnabled);
        }
        set {
            EditorPrefs.SetBool(kIsAnimationEnabledKey, value);
        }
    }
    public static float AnimationTime {
        get {
            return EditorPrefs.GetFloat(kAnimationTimeKey, kAnimationTime);
        }
        set {
            if(value < 0.1f) value= 0.1f;
            EditorPrefs.SetFloat(kAnimationTimeKey, value);
        }
    }
    public static float ScrollSpeed {
        get {
            return EditorPrefs.GetFloat(kScrollSpeedKey, kScrollSpeed);
        }
        set {
            if(value < 0) return;
            EditorPrefs.SetFloat(kScrollSpeedKey, value);
        }
    }
    public static float EdgeScrollSpeed {
        get {
            return EditorPrefs.GetFloat(kEdgeScrollSpeedKey, kEdgeScrollSpeed);
        }
        set {
            if(value < 0) return;
            EditorPrefs.SetFloat(kEdgeScrollSpeedKey, value);
        }
    }
    public static bool InverseZoom {
        get {
            return EditorPrefs.GetBool(kInverseZoomKey, kInverseZoom);
        }
        set {
            EditorPrefs.SetBool(kInverseZoomKey, value);
        }
    }
    public static float ZoomSpeed {
        get {
            return EditorPrefs.GetFloat(kZoomSpeedKey, kZoomSpeed);
        }
        set {
            if(value < 0.1f || value >5.0f) return;
            EditorPrefs.SetFloat(kZoomSpeedKey, value);
        }
    }
    public static bool ShowRuntimePortValue {
        get {
            return EditorPrefs.GetBool(kShowRuntimePortValueKey, kShowRuntimePortValue);
        }
        set {
            EditorPrefs.SetBool(kShowRuntimePortValueKey, value);
        }
    }
    public static float PortValueRefreshPeriod {
        get {
            return EditorPrefs.GetFloat(kPortValueRefreshPeriodKey, kPortValueRefreshPeriod);
        }
        set {
            if(value < 0.1f) value= 0.1f;
            if(value > 2.0f) value= 2.0f;
            EditorPrefs.SetFloat(kPortValueRefreshPeriodKey, value);
        }
    }
    public static bool ShowRuntimeFrameId {
        get {
            return EditorPrefs.GetBool(kShowRuntimeFrameIdKey, kShowRuntimeFrameId);
        }
        set {
            EditorPrefs.SetBool(kShowRuntimeFrameIdKey, value);
        }
    }
    public static bool ShowNodeStereotype {
        get {
            return EditorPrefs.GetBool(kShowNodeStereotypeKey, kShowNodeStereotype);
        }
        set {
            EditorPrefs.SetBool(kShowNodeStereotypeKey, value);
        }
    }


	// =================================================================================
    // Node Color Constants
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    static Color   kNodeTitleColor;
    static Color   kNodeLabelColor;
    static Color   kNodeValueColor;
    static Color   kEntryStateNodeColor;
    static Color   kStateNodeColor;
    static Color   kPackageNodeColor;
    static Color   kInstanceNodeColor;
    static Color   kConstructorNodeColor;
    static Color   kFunctionNodeColor;
    static Color   kMessageNodeColor;
    static Color   kUserFunctionNodeColor;
    static Color   kBackgroundColor;
    static Color   kSelectedBackgroundColor;            

	//
	// Database access keys
	//
    const string   kNodeTitleColorKey             = "iCS_NodeTitleColor";
    const string   kNodeLabelColorKey             = "iCS_NodeLabelColor";
    const string   kNodeValueColorKey             = "iCS_NodeValueColor";
    const string   kPackageNodeColorKey           = "iCS_PackageNodeColor";
    const string   kFunctionNodeColorKey          = "iCS_FunctionNodeColor";
    const string   kConstructorNodeColorKey       = "iCS_ConstructorNodeColor";
    const string   kInstanceNodeColorKey          = "iCS_InstanceNodeColor";
    const string   kStateNodeColorKey             = "iCS_StateNodeColor";
    const string   kEntryStateNodeColorKey        = "iCS_EntryStateNodeColor";
    const string   kMessageNodeColorKey           = "iCS_MessageNodeColor";
    const string   kUserFunctionNodeColorKey      = "iCS_UserFunctionNodeColor";
    const string   kBackgroundColorKey            = "iCS_BackgroundColor";         
    const string   kSelectedBackgroundColorKey    = "iCS_SelectedBackgroundColor";         

	//
	// Reset to default value functions
	//
	public static void ResetNodeTitleColor() {
		NodeTitleColor= kNodeTitleColor;
	}
	public static void ResetNodeLabelColor() {
		NodeLabelColor= kNodeLabelColor;
	}
    public static void ResetNodeValueColor() {
		NodeValueColor= kNodeValueColor;
    }
    public static void ResetPackageNodeColor() {
		PackageNodeColor= kPackageNodeColor;
    }
    public static void ResetFunctionNodeColor() {
		FunctionNodeColor= kFunctionNodeColor;
    }
    public static void ResetConstructorNodeColor() {
		ConstructorNodeColor= kConstructorNodeColor;
    }
    public static void ResetInstanceNodeColor() {
		InstanceNodeColor= kInstanceNodeColor;
    }
    public static void ResetMessageNodeColor() {
		MessageNodeColor= kMessageNodeColor;
    }
    public static void ResetUserFunctionNodeColor() {
		UserFunctionNodeColor= kUserFunctionNodeColor;
    }
    public static void ResetStateNodeColor() {
		StateNodeColor= kStateNodeColor;
    }
    public static void ResetEntryStateNodeColor() {
		EntryStateNodeColor= kEntryStateNodeColor;
    }
    public static void ResetBackgroundColor() {
		BackgroundColor= kBackgroundColor;
    }
    public static void ResetSelectedBackgroundColor() {
		SelectedBackgroundColor= kSelectedBackgroundColor;
    }
	
	//
	// Accessors
	//
    public static Color NodeTitleColor {
        get { return LoadColor(kNodeTitleColorKey, kNodeTitleColor); }
        set { SaveColor(kNodeTitleColorKey, value); }
    }
    public static Color NodeLabelColor {
        get { return LoadColor(kNodeLabelColorKey, kNodeLabelColor); }
        set { SaveColor(kNodeLabelColorKey, value); }
    }
    public static Color NodeValueColor {
        get { return LoadColor(kNodeValueColorKey, kNodeValueColor); }
        set { SaveColor(kNodeValueColorKey, value); }
    }
    public static Color PackageNodeColor {
        get { return LoadColor(kPackageNodeColorKey, kPackageNodeColor); }
        set { SaveColor(kPackageNodeColorKey, value); }
    }
    public static Color FunctionNodeColor {
        get { return LoadColor(kFunctionNodeColorKey, kFunctionNodeColor); }
        set { SaveColor(kFunctionNodeColorKey, value); }
    }
    public static Color ConstructorNodeColor {
        get { return LoadColor(kConstructorNodeColorKey, kConstructorNodeColor); }
        set { SaveColor(kConstructorNodeColorKey, value); }
    }
    public static Color InstanceNodeColor {
        get { return LoadColor(kInstanceNodeColorKey, kInstanceNodeColor); }
        set { SaveColor(kInstanceNodeColorKey, value); }
    }
    public static Color MessageNodeColor {
        get { return LoadColor(kMessageNodeColorKey, kMessageNodeColor); }
        set { SaveColor(kMessageNodeColorKey, value); }        
    }
    public static Color UserFunctionNodeColor {
        get { return LoadColor(kUserFunctionNodeColorKey, kUserFunctionNodeColor); }
        set { SaveColor(kUserFunctionNodeColorKey, value); }        
    }
    public static Color StateNodeColor {
        get { return LoadColor(kStateNodeColorKey, kStateNodeColor); }
        set { SaveColor(kStateNodeColorKey, value); }
    }
    public static Color EntryStateNodeColor {
        get { return LoadColor(kEntryStateNodeColorKey, kEntryStateNodeColor); }
        set { SaveColor(kEntryStateNodeColorKey, value); }
    }
    public static Color BackgroundColor {
        get { return LoadColor(kBackgroundColorKey, kBackgroundColor); }
        set { SaveColor(kBackgroundColorKey, value); }
    }
    public static Color SelectedBackgroundColor {
        get { return LoadColor(kSelectedBackgroundColorKey, kSelectedBackgroundColor); }
        set { SaveColor(kSelectedBackgroundColorKey, value); }
    }


	// =================================================================================
    // Type Color Constants
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    static Color kBoolTypeColor;
    static Color kIntTypeColor;
    static Color kFloatTypeColor;
    static Color kVector2TypeColor;
    static Color kVector3TypeColor;
    static Color kVector4TypeColor;
    static Color kStringTypeColor;
    static Color kGameObjectTypeColor;
    static Color kDefaultTypeColor;

	//
	// Database access keys
	//
    const string kBoolTypeColorKey      = "iCS_BoolTypeColor";
    const string kIntTypeColorKey       = "iCS_IntTypeColor";
    const string kFloatTypeColorKey     = "iCS_FloatTypeColor";
    const string kVector2TypeColorKey   = "iCS_Vector2TypeColor";
    const string kVector3TypeColorKey   = "iCS_Vector3TypeColor";
    const string kVector4TypeColorKey   = "iCS_Vector4TypeColor";
    const string kStringTypeColorKey    = "iCS_StringTypeColor";
    const string kGameObjectTypeColorKey= "iCS_GameObjectTypeColor";
    const string kDefaultTypeColorKey   = "iCS_DefaultTypeColor";

	//
	// Reset to default value functions
	//
    public static void ResetBoolTypeColor() {
	    BoolTypeColor= kBoolTypeColor;
    }
    public static void ResetIntTypeColor() {
	    IntTypeColor= kIntTypeColor;
    }
    public static void ResetFloatTypeColor() {
	    FloatTypeColor= kFloatTypeColor;
    }
    public static void ResetVector2TypeColor() {
	    Vector2TypeColor= kVector2TypeColor;
    }
    public static void ResetVector3TypeColor() {
	    Vector3TypeColor= kVector3TypeColor;
    }
    public static void ResetVector4TypeColor() {
	    Vector4TypeColor= kVector4TypeColor;
    }
    public static void ResetStringTypeColor() {
	    StringTypeColor= kStringTypeColor;
    }
    public static void ResetGameObjectTypeColor() {
	    GameObjectTypeColor= kGameObjectTypeColor;
    }
    public static void ResetDefaultTypeColor() {
	    DefaultTypeColor= kDefaultTypeColor;    	
    }

	//
	// Accessors
	//
    public static Color BoolTypeColor {
        get { return LoadColor(kBoolTypeColorKey, kBoolTypeColor); }
        set { SaveColor(kBoolTypeColorKey, value); }
    }
    public static Color IntTypeColor {
        get { return LoadColor(kIntTypeColorKey, kIntTypeColor); }
        set { SaveColor(kIntTypeColorKey, value); }
    }
    public static Color FloatTypeColor {
        get { return LoadColor(kFloatTypeColorKey, kFloatTypeColor); }
        set { SaveColor(kFloatTypeColorKey, value); }
    }
    public static Color StringTypeColor {
        get { return LoadColor(kStringTypeColorKey, kStringTypeColor);} 
        set { SaveColor(kStringTypeColorKey, value); }
    }
    public static Color Vector2TypeColor {
        get { return LoadColor(kVector2TypeColorKey, kVector2TypeColor); }
        set { SaveColor(kVector2TypeColorKey, value); }
    }
    public static Color Vector3TypeColor {
        get { return LoadColor(kVector3TypeColorKey, kVector3TypeColor); }
        set { SaveColor(kVector3TypeColorKey, value); }
    }
    public static Color Vector4TypeColor {
        get { return LoadColor(kVector4TypeColorKey, kVector4TypeColor); }
        set { SaveColor(kVector4TypeColorKey, value); }
    }
    public static Color GameObjectTypeColor {
        get { return LoadColor(kGameObjectTypeColorKey, kGameObjectTypeColor); }
        set { SaveColor(kGameObjectTypeColorKey, value); }
    }
    public static Color DefaultTypeColor {
        get { return LoadColor(kDefaultTypeColorKey, kDefaultTypeColor); }
        set { SaveColor(kDefaultTypeColorKey, value); }
    }

	//
    // Helpers
	//
    public static Color GetTypeColor(Type type) {
        Type t= iCS_Types.GetElementType(type);
        if(t == typeof(bool))       return BoolTypeColor;
        if(t == typeof(int))        return IntTypeColor;
        if(t == typeof(float))      return FloatTypeColor;
        if(t == typeof(string))     return StringTypeColor;
        if(t == typeof(GameObject)) return GameObjectTypeColor;
        if(t == typeof(Vector2))    return Vector2TypeColor;
        if(t == typeof(Vector3))    return Vector3TypeColor;
        if(t == typeof(Vector4))    return Vector4TypeColor;
        return DefaultTypeColor;
    }


	// =================================================================================
    // Instance Node Config Constants
    // ---------------------------------------------------------------------------------
	//
	// Default Values
	//
    const bool   kInstanceAutocreateInThis            = true;
    const bool   kInstanceAutocreateInFields          = false;
    const bool   kInstanceAutocreateOutFields         = false;
    const bool   kInstanceAutocreateInClassFields     = false;
    const bool   kInstanceAutocreateOutClassFields    = false;
    const bool   kInstanceAutocreateInProperties      = false;
    const bool   kInstanceAutocreateOutProperties     = false;
    const bool   kInstanceAutocreateInClassProperties = false;
    const bool   kInstanceAutocreateOutClassProperties= false;

	//
	// Database access keys
	//
    const string kInstanceAutocreateInThisKey            = "iCS_InstanceAutocreateInThis";
    const string kInstanceAutocreateInFieldsKey          = "iCS_InstanceAutocreateInFields"; 
    const string kInstanceAutocreateOutFieldsKey         = "iCS_InstanceAutocreateOutFields"; 
    const string kInstanceAutocreateInClassFieldsKey     = "iCS_InstanceAutocreateInClassFields";
    const string kInstanceAutocreateOutClassFieldsKey    = "iCS_InstanceAutocreateOutClassFields";
    const string kInstanceAutocreateInPropertiesKey      = "iCS_InstanceAutocreateInProperties";
    const string kInstanceAutocreateOutPropertiesKey     = "iCS_InstanceAutocreateOutProperties"; 
    const string kInstanceAutocreateInClassPropertiesKey = "iCS_InstanceAutocreateInClassProperties";
    const string kInstanceAutocreateOutClassPropertiesKey= "iCS_InstanceAutocreateOutClassProperties";

	//
	// Reset to default value functions
	//
    public static void ResetInstanceAutocreateInThis() {
	    InstanceAutocreateInThis= kInstanceAutocreateInThis;
    }
    public static void ResetInstanceAutocreateInFields() {
	    InstanceAutocreateInFields= kInstanceAutocreateInFields;
    }
    public static void ResetInstanceAutocreateOutFields() {
	    InstanceAutocreateOutFields= kInstanceAutocreateOutFields;
    }
    public static void ResetInstanceAutocreateInClassFields() {
	    InstanceAutocreateInClassFields= kInstanceAutocreateInClassFields;
    }
    public static void ResetInstanceAutocreateOutClassFields() {
	    InstanceAutocreateOutClassFields= kInstanceAutocreateOutClassFields;
    }
    public static void ResetInstanceAutocreateInProperties() {
	    InstanceAutocreateInProperties= kInstanceAutocreateInProperties;
    }
    public static void ResetInstanceAutocreateOutProperties() {
	    InstanceAutocreateOutProperties= kInstanceAutocreateOutProperties;
    }
    public static void ResetInstanceAutocreateInClassProperties() {
	    InstanceAutocreateInClassProperties= kInstanceAutocreateInClassProperties;
    }
    public static void ResetInstanceAutocreateOutClassProperties() {
	    InstanceAutocreateOutClassProperties= kInstanceAutocreateOutClassProperties;    	
    }

	//
	// Accessors
	//
    public static bool InstanceAutocreateInThis {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInThisKey, kInstanceAutocreateInThis); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInThisKey, value); }        
    }
    public static bool InstanceAutocreateInFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInFieldsKey, kInstanceAutocreateInFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInFieldsKey, value); }        
    }
    public static bool InstanceAutocreateOutFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutFieldsKey, kInstanceAutocreateOutFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutFieldsKey, value); }        
    }
    public static bool InstanceAutocreateInClassFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInClassFieldsKey, kInstanceAutocreateInClassFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInClassFieldsKey, value); }        
    }
    public static bool InstanceAutocreateOutClassFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutClassFieldsKey, kInstanceAutocreateOutClassFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutClassFieldsKey, value); }        
    }
    public static bool InstanceAutocreateInProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInPropertiesKey, kInstanceAutocreateInProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateOutProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutPropertiesKey, kInstanceAutocreateOutProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateInClassProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInClassPropertiesKey, kInstanceAutocreateInClassProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInClassPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateOutClassProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutClassPropertiesKey, kInstanceAutocreateOutClassProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutClassPropertiesKey, value); }        
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
	

#if CODE_GENERATION_CONFIG
	// =================================================================================
	// Code Engineering Preferences
    // ---------------------------------------------------------------------------------
	// Code Engineering Config Constants
	const string kCodeGenerationFolder           = "iCanScript_GeneratedCode";
	const string kBehaviourGenerationSubFolder   = "Behaviours";
    const string kCodeGenerationFilePrefix       = "";
	const string kCodeGenerationFolderKey        = "iCS_CodeGenerationFolder";
	const string kBehaviourGenerationSubFolderKey= "iCS_BehaviourGenerationSubFolder";
	const string kCodeGenerationFilePrefixKey    = "iCS_CodeGenerationFilePrefix";
#endif


	// =================================================================================
	// Activation
	// ---------------------------------------------------------------------------------
	static iCS_PreferencesController() {
        var c= new Func<int,float>(i=> ((float)i)/255f);

        // Canvas colors
        kCanvasBackgroundColor= new Color(c(9), c(69), c(167));
        kGridColor            = new Color(c(160), c(160), c(160));
		
        // Node colors
        kNodeTitleColor             = Color.black;
        kNodeLabelColor             = Color.white;
        kNodeValueColor             = new Color(1f, 0.8f, 0.4f);
        kEntryStateNodeColor        = new Color(1f, 0.8f, 0.4f);
        kStateNodeColor             = Color.cyan;
        kPackageNodeColor           = Color.yellow;
        kInstanceNodeColor          = new Color(1f, 0.5f, 0f);
        kConstructorNodeColor       = new Color(1f, 0.25f, 0.75f);
        kFunctionNodeColor          = Color.green;
        kMessageNodeColor           = new Color(c(0x36), c(0x8a), c(0xff));
        kUserFunctionNodeColor      = new Color(c(0x80), c(0xff), c(0x80));
        kBackgroundColor            = new Color(c(41), c(41), c(41));
        kSelectedBackgroundColor    = new Color(c(116), c(116), c(116));
        
        // Type colors
//        kBoolTypeColor      = Color.white;
        kBoolTypeColor      = Color.red;
        kIntTypeColor       = Color.magenta;
        kFloatTypeColor     = Color.cyan;
        kVector2TypeColor   = Color.yellow;
        kVector3TypeColor   = Color.green;
        kVector4TypeColor   = new Color(c(64), c(160), c(255));
//        kStringTypeColor    = new Color(c(255), c(128), 0);
        kStringTypeColor    = Color.red;
        kGameObjectTypeColor= new Color(0, c(128), c(255));
        kDefaultTypeColor   = new Color(c(219), c(249), c(194));		
	}
	
	// =================================================================================
    // License Information
	// ---------------------------------------------------------------------------------
	//
	// Database access keys
	//
    const string kUserLicenseKey= "iCS_UserLicenseKey";
    
	//
	// Reset to default value functions
	//
    public static void ResetUserLicense() {
        EditorPrefs.SetString(kUserLicenseKey, "");
    }
    
	//
	// Accessors
	//
    public static string UserLicense {
        get { return EditorPrefs.GetString(kUserLicenseKey, ""); }
        set { EditorPrefs.SetString(kUserLicenseKey, value); }
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
