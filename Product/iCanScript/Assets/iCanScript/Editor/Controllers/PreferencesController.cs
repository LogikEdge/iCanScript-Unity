// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// PreferencesController.cs
//
// Revised: 2014-01-29
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace iCanScript.Internal.Editor {
    public enum iCS_UpdateInterval { Daily= 0, Weekly= 1, Monthly= 2 };
    
    public static class PreferencesController {
    	// =================================================================================
        // Canvas Preferences
        // ---------------------------------------------------------------------------------
    	//
    	// Default Values
    	//
        static  Color   kCanvasBackgroundColor;
        static  Color   kMinorGridColor;
        static  Color   kMajorGridColor;
        const   float   kGridSpacing = 10.0f;
    
    	//
    	// Database access keys
    	//
        const   string  kCanvasBackgroundColorKey= "iCS_CanvasBackgroundColor";
        const   string  kMinorGridColorKey       = "iCS_MinorGridColor";
        const   string  kMajorGridColorKey       = "iCS_MajorGridColor";
        const   string  kGridSpacingKey          = "iCS_GridSpacing";
    	
        //
        // Cached Values
        //
        static  Color   c_CanvasBackgroundColor;
        static  Color   c_MinorGridColor;
        static  Color   c_MajorGridColor;
        static  float   c_GridSpacing = 10.0f;
        
    	//
    	// Reset to default value functions
    	//
    	public static void ResetCanvasBackgroundColor() {
    		CanvasBackgroundColor= kCanvasBackgroundColor;
    	}
    	public static void ResetGridColor() {
    		MinorGridColor= kMinorGridColor;
    		MajorGridColor= kMajorGridColor;
    	}
    	public static void ResetGridSpacing() {
    		GridSpacing= kGridSpacing;
    	}
    	
    	//
    	// Accessors
    	//
        public static Color CanvasBackgroundColor {
            get { return c_CanvasBackgroundColor; }
            set { SaveColor(kCanvasBackgroundColorKey, value); c_CanvasBackgroundColor= value; }
        }
        public static Color MinorGridColor {
            get { return c_MinorGridColor; }
            set { SaveColor(kMinorGridColorKey, value); c_MinorGridColor= value; }
        }
        public static Color MajorGridColor {
            get { return c_MajorGridColor; }
            set { SaveColor(kMajorGridColorKey, value); c_MajorGridColor= value; }
        }
        public static float GridSpacing {
            get { return c_GridSpacing; }
            set {
                if(value < 5.0f) return;
                EditorPrefs.SetFloat(kGridSpacingKey, value);
                c_GridSpacing= value;
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
    
    	//
    	// Database access keys
    	//
        const string kIsAnimationEnabledKey      = "iCS_IsAnimationEnabled";
        const string kAnimationTimeKey           = "iCS_AnimationTime";
        const string kScrollSpeedKey             = "iCS_ScrollSpeed";
        const string kEdgeScrollSpeedKey         = "iCS_EdgeScrollSpeed";
        const string kInverseZoomKey             = "iCS_InverseZoom";
        const string kZoomSpeedKey               = "iCS_ZoomSpeed";
    
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
        // Chached Values
        //
        static Color   c_NodeTitleColor;
        static Color   c_NodeLabelColor;
        static Color   c_NodeValueColor;
        static Color   c_EntryStateNodeColor;
        static Color   c_StateNodeColor;
        static Color   c_PackageNodeColor;
        static Color   c_InstanceNodeColor;
        static Color   c_ConstructorNodeColor;
        static Color   c_FunctionNodeColor;
        static Color   c_MessageNodeColor;
        static Color   c_UserFunctionNodeColor;
        static Color   c_BackgroundColor;
        static Color   c_SelectedBackgroundColor;            
        
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
            get { return c_NodeTitleColor; }
            set { SaveColor(kNodeTitleColorKey, value); c_NodeTitleColor= value; }
        }
        public static Color NodeLabelColor {
            get { return c_NodeLabelColor; }
            set { SaveColor(kNodeLabelColorKey, value); c_NodeLabelColor= value; }
        }
        public static Color NodeValueColor {
            get { return c_NodeValueColor; }
            set { SaveColor(kNodeValueColorKey, value); c_NodeValueColor= value; }
        }
        public static Color PackageNodeColor {
            get { return c_PackageNodeColor; }
            set { SaveColor(kPackageNodeColorKey, value); c_PackageNodeColor= value; }
        }
        public static Color FunctionNodeColor {
            get { return c_FunctionNodeColor; }
            set { SaveColor(kFunctionNodeColorKey, value); c_FunctionNodeColor= value; }
        }
        public static Color ConstructorNodeColor {
            get { return c_ConstructorNodeColor; }
            set { SaveColor(kConstructorNodeColorKey, value); c_ConstructorNodeColor= value; }
        }
        public static Color InstanceNodeColor {
            get { return c_InstanceNodeColor; }
            set { SaveColor(kInstanceNodeColorKey, value); c_InstanceNodeColor= value; }
        }
        public static Color MessageNodeColor {
            get { return c_MessageNodeColor; }
            set { SaveColor(kMessageNodeColorKey, value); c_MessageNodeColor= value; }        
        }
        public static Color UserFunctionNodeColor {
            get { return c_UserFunctionNodeColor; }
            set { SaveColor(kUserFunctionNodeColorKey, value); c_UserFunctionNodeColor= value; }        
        }
        public static Color StateNodeColor {
            get { return c_StateNodeColor; }
            set { SaveColor(kStateNodeColorKey, value); c_StateNodeColor= value; }
        }
        public static Color EntryStateNodeColor {
            get { return c_EntryStateNodeColor; }
            set { SaveColor(kEntryStateNodeColorKey, value); c_EntryStateNodeColor= value; }
        }
        public static Color BackgroundColor {
            get { return c_BackgroundColor; }
            set { SaveColor(kBackgroundColorKey, value); c_BackgroundColor= value; }
        }
        public static Color SelectedBackgroundColor {
            get { return c_SelectedBackgroundColor; }
            set { SaveColor(kSelectedBackgroundColorKey, value); c_SelectedBackgroundColor= value; }
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
        // Cached Values
        //
        static Color c_BoolTypeColor;
        static Color c_IntTypeColor;
        static Color c_FloatTypeColor;
        static Color c_Vector2TypeColor;
        static Color c_Vector3TypeColor;
        static Color c_Vector4TypeColor;
        static Color c_StringTypeColor;
        static Color c_GameObjectTypeColor;
        static Color c_DefaultTypeColor;
        
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
            get { return c_BoolTypeColor; }
            set { SaveColor(kBoolTypeColorKey, value); c_BoolTypeColor= value; }
        }
        public static Color IntTypeColor {
            get { return c_IntTypeColor; }
            set { SaveColor(kIntTypeColorKey, value); c_IntTypeColor= value; }
        }
        public static Color FloatTypeColor {
            get { return c_FloatTypeColor; }
            set { SaveColor(kFloatTypeColorKey, value); c_FloatTypeColor= value; }
        }
        public static Color StringTypeColor {
            get { return c_StringTypeColor; } 
            set { SaveColor(kStringTypeColorKey, value); c_StringTypeColor= value; }
        }
        public static Color Vector2TypeColor {
            get { return c_Vector2TypeColor; }
            set { SaveColor(kVector2TypeColorKey, value); c_Vector2TypeColor= value; }
        }
        public static Color Vector3TypeColor {
            get { return c_Vector3TypeColor; }
            set { SaveColor(kVector3TypeColorKey, value); c_Vector3TypeColor= value; }
        }
        public static Color Vector4TypeColor {
            get { return c_Vector4TypeColor; }
            set { SaveColor(kVector4TypeColorKey, value); c_Vector4TypeColor= value; }
        }
        public static Color GameObjectTypeColor {
            get { return c_GameObjectTypeColor; }
            set { SaveColor(kGameObjectTypeColorKey, value); c_GameObjectTypeColor= value; }
        }
        public static Color DefaultTypeColor {
            get { return c_DefaultTypeColor; }
            set { SaveColor(kDefaultTypeColorKey, value); c_DefaultTypeColor= value; }
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
    	// Active Project Preferences
        // ---------------------------------------------------------------------------------
    	//
    	// Default Values
    	//
		const string kActiveProjectPath= "";
		
    	//
    	// Database access keys
    	//
    	const string kActiveProjectPathKey= "iCS_ActiveProjectPath";
		
    	//
    	// Accessors
    	//
		public static string ActiveProjectPath {
            get { return EditorPrefs.GetString(kActiveProjectPathKey, kActiveProjectPath); }
            set { EditorPrefs.SetString(kActiveProjectPathKey, value); }        
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
    	// Library Preferences
        // ---------------------------------------------------------------------------------
    	//
    	// Default Values
    	//
    	const bool kLibraryInheritedOption  = true;
        const bool kLibraryProtectedOption  = false;
    	const bool kLibraryUnityEditorOption= true;
    
    	//
    	// Database access keys
    	//
    	const string kLibraryInheritedOptionKey  = "iCS_LibraryInheritedOption";
    	const string kLibraryProtectedOptionKey  = "iCS_LibraryProtectedOption";
    	const string kLibraryUnityEditorOptionKey= "iCS_LibraryUnityEditorOption";

    	//
    	// Reset to default value functions
    	//
    	public static void ResetLibraryInheritedOption() {
    	    LibraryInheritedOption= kLibraryInheritedOption;
    	}
    	public static void ResetLibraryProtectedOption() {
    	    LibraryProtectedOption= kLibraryProtectedOption;
    	}
    	public static void ResetLibraryUnityEditorOption() {
    	    LibraryUnityEditorOption= kLibraryUnityEditorOption;
    	}
    
    	//
    	// Accessors
    	//
        public static bool LibraryInheritedOption {
            get { return EditorPrefs.GetBool(kLibraryInheritedOptionKey, kLibraryInheritedOption); }
            set { EditorPrefs.SetBool(kLibraryInheritedOptionKey, value); }
        }
        public static bool LibraryProtectedOption {
            get { return EditorPrefs.GetBool(kLibraryProtectedOptionKey, kLibraryProtectedOption); }
            set { EditorPrefs.SetBool(kLibraryProtectedOptionKey, value); }
        }
        public static bool LibraryUnityEditorOption {
            get { return EditorPrefs.GetBool(kLibraryUnityEditorOptionKey, kLibraryUnityEditorOption); }
            set { EditorPrefs.SetBool(kLibraryUnityEditorOptionKey, value); }
        }

    	// =================================================================================
    	// Activation
    	// ---------------------------------------------------------------------------------
    	static PreferencesController() {
            var c= new Func<int,float>(i=> ((float)i)/255f);
    
            // Canvas colors
            kCanvasBackgroundColor= new Color(c(40), c(44), c(51));
            kMinorGridColor       = new Color(c(75), c(75), c(75));
            kMajorGridColor       = Color.black;
    		// Initialise Canvas cached values
            c_CanvasBackgroundColor = LoadColor(kCanvasBackgroundColorKey, kCanvasBackgroundColor);
            c_MinorGridColor        = LoadColor(kMinorGridColorKey, kMinorGridColor);
            c_MajorGridColor        = LoadColor(kMajorGridColorKey, kMajorGridColor);
            c_GridSpacing           = EditorPrefs.GetFloat(kGridSpacingKey, kGridSpacing);
            
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
            kSelectedBackgroundColor    = Color.white;
            // Node cached colors
            c_NodeTitleColor         = LoadColor(kNodeTitleColorKey, kNodeTitleColor);
            c_NodeLabelColor         = LoadColor(kNodeLabelColorKey, kNodeLabelColor);
            c_NodeValueColor         = LoadColor(kNodeValueColorKey, kNodeValueColor);
            c_PackageNodeColor       = LoadColor(kPackageNodeColorKey, kPackageNodeColor);
            c_FunctionNodeColor      = LoadColor(kFunctionNodeColorKey, kFunctionNodeColor);
            c_ConstructorNodeColor   = LoadColor(kConstructorNodeColorKey, kConstructorNodeColor);
            c_InstanceNodeColor      = LoadColor(kInstanceNodeColorKey, kInstanceNodeColor);
            c_MessageNodeColor       = LoadColor(kMessageNodeColorKey, kMessageNodeColor);
            c_UserFunctionNodeColor  = LoadColor(kUserFunctionNodeColorKey, kUserFunctionNodeColor);
            c_StateNodeColor         = LoadColor(kStateNodeColorKey, kStateNodeColor);
            c_EntryStateNodeColor    = LoadColor(kEntryStateNodeColorKey, kEntryStateNodeColor);
            c_BackgroundColor        = LoadColor(kBackgroundColorKey, kBackgroundColor);
            c_SelectedBackgroundColor= LoadColor(kSelectedBackgroundColorKey, kSelectedBackgroundColor);
            
            // Type colors
            kBoolTypeColor      = Color.white;
            kIntTypeColor       = Color.magenta;
            kFloatTypeColor     = Color.cyan;
            kVector2TypeColor   = Color.yellow;
            kVector3TypeColor   = Color.green;
            kVector4TypeColor   = new Color(c(64), c(160), c(255));
            kStringTypeColor    = new Color(c(255), c(128), 0);
            kGameObjectTypeColor= new Color(0, c(128), c(255));
            kDefaultTypeColor   = new Color(c(219), c(249), c(194));		
            // Type cached colors
            c_BoolTypeColor      = LoadColor(kBoolTypeColorKey, kBoolTypeColor);
            c_IntTypeColor       = LoadColor(kIntTypeColorKey, kIntTypeColor);
            c_FloatTypeColor     = LoadColor(kFloatTypeColorKey, kFloatTypeColor);
            c_StringTypeColor    = LoadColor(kStringTypeColorKey, kStringTypeColor);
            c_Vector2TypeColor   = LoadColor(kVector2TypeColorKey, kVector2TypeColor);
            c_Vector3TypeColor   = LoadColor(kVector3TypeColorKey, kVector3TypeColor);
            c_Vector4TypeColor   = LoadColor(kVector4TypeColorKey, kVector4TypeColor);
            c_GameObjectTypeColor= LoadColor(kGameObjectTypeColorKey, kGameObjectTypeColor);
            c_DefaultTypeColor   = LoadColor(kDefaultTypeColorKey, kDefaultTypeColor);
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

}
