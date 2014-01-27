using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum iCS_UpdateInterval { Day= 0, Week= 1, Month= 2 };

public class iCS_PreferencesEditor : iCS_EditorBase {
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float kMargin      = 10.0f;
    const float kTitleHeight = 40.0f;
    const float kColumn1Width= 120.0f;
    const float kColumn2Width= 180.0f;
    const float kColumn3Width= 180.0f;
    const float kColumn1X    = 0;
    const float kColumn2X    = kColumn1X+kColumn1Width;
    const float kColumn3X    = kColumn2X+kColumn2Width;
    // ---------------------------------------------------------------------------------
    // Display Option Constants
    const bool   kAnimationEnabled           = true;
    const float  kAnimationPixelsPerSecond   = 1500f;
    const float  kMinAnimationTime           = 0.5f;
    const float  kScrollSpeed                = 3.0f;
    const float  kEdgeScrollSpeed            = 400.0f;
    const bool   kInverseZoom                = false;
    const float  kZoomSpeed                  = 1.0f;
    const string kAnimationEnabledKey        = "iCS_AnimationEnabled";
    const string kAnimationPixelsPerSecondKey= "iCS_AnimationPixelsPerSecond";
    const string kMinAnimationTimeKey        = "iCS_MinAnimationTime";
    const string kScrollSpeedKey             = "iCS_ScrollSpeed";
    const string kEdgeScrollSpeedKey         = "iCS_EdgeScrollSpeed";
    const string kInverseZoomKey             = "iCS_InverseZoom";
    const string kZoomSpeedKey               = "iCS_ZoomSpeed";
    const bool   kShowRuntimePortValue       = false;
    const float  kPortValueRefreshPeriod     = 0.1f;
	const bool   kShowRuntimeFrameId         = false;
    const string kShowRuntimePortValueKey    = "iCS_ShowRuntimePortValue";
    const string kPortValueRefreshPeriodKey  = "iCS_PortValueRefresh";
    const string kShowRuntimeFrameIdKey      = "iCS_ShowRuntimeFrameId";
    // ---------------------------------------------------------------------------------
    // Canvas Constants
    static  Color   kCanvasBackgroundColor;
    static  Color   kGridColor;
    const   float   kGridSpacing             = 20.0f;
    const   string  kCanvasBackgroundColorKey= "iCS_CanvasBackgroundColor";
    const   string  kGridColorKey            = "iCS_GridColor";
    const   string  kGridSpacingKey          = "iCS_GridSpacing";
    // ---------------------------------------------------------------------------------
    // Node Color Constants
    const float    kSelectedBrightnessGain= 1.75f;
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
    static Color   kSelectedNodeBackgroundColor;            
    const string   kSelectedBrightnessGainKey     = "iCS_SelectedBrightnessGain";
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
    const string   kSelectedNodeBackgroundColorKey= "iCS_SelectedNodeBackgroundColor";         
    // ---------------------------------------------------------------------------------
    // Type Color Constants
    static Color kBoolTypeColor;
    static Color kIntTypeColor;
    static Color kFloatTypeColor;
    static Color kVector2TypeColor;
    static Color kVector3TypeColor;
    static Color kVector4TypeColor;
    static Color kStringTypeColor;
    static Color kGameObjectTypeColor;
    static Color kDefaultTypeColor;
    const string kBoolTypeColorKey      = "iCS_BoolTypeColor";
    const string kIntTypeColorKey       = "iCS_IntTypeColor";
    const string kFloatTypeColorKey     = "iCS_FloatTypeColor";
    const string kVector2TypeColorKey   = "iCS_Vector2TypeColor";
    const string kVector3TypeColorKey   = "iCS_Vector3TypeColor";
    const string kVector4TypeColorKey   = "iCS_Vector4TypeColor";
    const string kStringTypeColorKey    = "iCS_StringTypeColor";
    const string kGameObjectTypeColorKey= "iCS_GameObjectTypeColor";
    const string kDefaultTypeColorKey   = "iCS_DefaultTypeColor";
    // ---------------------------------------------------------------------------------
    // Instance Node Config Constants
    const bool   kInstanceAutocreateInThis            = true;
    const bool   kInstanceAutocreateInFields          = false;
    const bool   kInstanceAutocreateOutFields         = false;
    const bool   kInstanceAutocreateInClassFields     = false;
    const bool   kInstanceAutocreateOutClassFields    = false;
    const bool   kInstanceAutocreateInProperties      = false;
    const bool   kInstanceAutocreateOutProperties     = false;
    const bool   kInstanceAutocreateInClassProperties = false;
    const bool   kInstanceAutocreateOutClassProperties= false;
    const string kInstanceAutocreateInThisKey            = "iCS_InstanceAutocreateInThis";
    const string kInstanceAutocreateInFieldsKey          = "iCS_InstanceAutocreateInFields"; 
    const string kInstanceAutocreateOutFieldsKey         = "iCS_InstanceAutocreateOutFields"; 
    const string kInstanceAutocreateInClassFieldsKey     = "iCS_InstanceAutocreateInClassFields";
    const string kInstanceAutocreateOutClassFieldsKey    = "iCS_InstanceAutocreateOutClassFields";
    const string kInstanceAutocreateInPropertiesKey      = "iCS_InstanceAutocreateInProperties";
    const string kInstanceAutocreateOutPropertiesKey     = "iCS_InstanceAutocreateOutProperties"; 
    const string kInstanceAutocreateInClassPropertiesKey = "iCS_InstanceAutocreateInClassProperties";
    const string kInstanceAutocreateOutClassPropertiesKey= "iCS_InstanceAutocreateOutClassProperties";
    // ---------------------------------------------------------------------------------
	// Software Updates Config Constants
	const bool	 kSoftwareUpdateWatchEnabled			= true;
	const int    kSoftwareUpdateInterval				= 0;  // 0= day, 1= week, 2= month
	const string kSoftwareUpdateSkippedVersion			= "";
	const string kSoftwareUpdateWatchEnabledKey			= "iCS_SoftwareUpdateWatchEnabled";
	const string kSoftwareUpdateIntervalKey             = "iCS_SoftwareUpdateInterval";
	const string kSoftwareUpdateSkippedVersionKey		= "iCS_SoftwareUpdateSkippedVersion";
#if CODE_GENERATION_CONFIG
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
    // Fields
    // ---------------------------------------------------------------------------------
	GUIStyle    titleStyle= null;
	GUIStyle    selectionStyle= null;
	Texture2D	selectionBackground= null;
	int         selGridId= 0;
	string[]    selGridStrings= new string[]{
	    "Display Options",
	    "Canvas",
	    "Node Colors",
	    "Type Colors",
	    "Instance Wizard",
		"Software Update",
#if CODE_GENERATION_CONFIG
	    "Code Engineering"
#endif
	};
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static bool AnimationEnabled {
        get {
            return EditorPrefs.GetBool(kAnimationEnabledKey, kAnimationEnabled);
        }
        set {
            EditorPrefs.SetBool(kAnimationEnabledKey, value);
        }
    }
    public static float AnimationPixelsPerSecond {
        get {
            return EditorPrefs.GetFloat(kAnimationPixelsPerSecondKey, kAnimationPixelsPerSecond);
        }
        set {
            if(value < 10f) value= 10f;
            EditorPrefs.SetFloat(kAnimationPixelsPerSecondKey, value);
        }
    }
    public static float MinAnimationTime {
        get {
            return EditorPrefs.GetFloat(kMinAnimationTimeKey, kMinAnimationTime);
        }
        set {
            if(value < 0.1f) value= 0.1f;
            EditorPrefs.SetFloat(kMinAnimationTimeKey, value);
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
    public static Color CanvasBackgroundColor {
        get { return LoadColor(kCanvasBackgroundColorKey, kCanvasBackgroundColor); }
        set { SaveColor(kCanvasBackgroundColorKey, value); }
    }
    public static Color GridColor {
        get { return LoadColor(kGridColorKey, kGridColor); }
        set { SaveColor(kGridColorKey, value); }
    }
    public static float GridSpacing {
        get {
            return EditorPrefs.GetFloat(kGridSpacingKey, kGridSpacing);
        }
        set {
            if(value < 5.0f) return;
            EditorPrefs.SetFloat(kGridSpacingKey, value);
        }
    }
    public static float SelectedBrightnessGain {
        get {
            return EditorPrefs.GetFloat(kSelectedBrightnessGainKey, kSelectedBrightnessGain);
        }
        set {
            if(value < 0.25f) return;
            EditorPrefs.SetFloat(kSelectedBrightnessGainKey, value);
        }
    }
    // -------------------------------------------------------------------------
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
    public static Color StateNodeColor {
        get { return LoadColor(kStateNodeColorKey, kStateNodeColor); }
        set { SaveColor(kStateNodeColorKey, value); }
    }
    public static Color EntryStateNodeColor {
        get { return LoadColor(kEntryStateNodeColorKey, kEntryStateNodeColor); }
        set { SaveColor(kEntryStateNodeColorKey, value); }
    }
    public static Color SelectedNodeBackgroundColor {
        get { return LoadColor(kSelectedNodeBackgroundColorKey, kSelectedNodeBackgroundColor); }
        set { SaveColor(kSelectedNodeBackgroundColorKey, value); }
    }
    // -------------------------------------------------------------------------
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
    // -------------------------------------------------------------------------
	// Software Update Config Accessor
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
#if CODE_GENERATION_CONFIG
    // -------------------------------------------------------------------------
	// Code Generation Config Accessor
    public static string CodeGenerationFolder {
        get { return EditorPrefs.GetString(kCodeGenerationFolderKey, kCodeGenerationFolder); }
        set {
            // Rename folder
            var current= CodeGenerationFolder;
            if(current == value) return;
            var path= "Assets/";
            if(String.IsNullOrEmpty(AssetDatabase.RenameAsset(path+current, value))) {
                EditorPrefs.SetString(kCodeGenerationFolderKey, value);                                
            }
        }
    }
    public static string BehaviourGenerationSubfolder {
        get { return EditorPrefs.GetString(kBehaviourGenerationSubFolderKey, kBehaviourGenerationSubFolder); }
        set {
            // Rename folder
            var current= BehaviourGenerationSubfolder;
            if(current == value) return;
            var path= "Assets/"+CodeGenerationFolder+"/";
            if(String.IsNullOrEmpty(AssetDatabase.RenameAsset(path+current, value))) {
                EditorPrefs.SetString(kBehaviourGenerationSubFolderKey, value);                
            }
        }
    }
    public static string CodeGenerationFilePrefix {
        get { return EditorPrefs.GetString(kCodeGenerationFilePrefixKey, kCodeGenerationFilePrefix); }
        set {
            var current= CodeGenerationFilePrefix;
            if(current == value) return;
            var codeGenerationRoot= "Assets/"+iCS_PreferencesEditor.CodeGenerationFolder;
            iCS_FileUtility.ChangeRecursivelyAssetsPrefixInDirectory(codeGenerationRoot, current, value);
            EditorPrefs.SetString(kCodeGenerationFilePrefixKey, value);
        }
    }
#endif
    
	// =================================================================================
    // Storage Utilities
    // ---------------------------------------------------------------------------------
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
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    static iCS_PreferencesEditor() {
        // Canvas colors
        kCanvasBackgroundColor= new Color(0.169f, 0.188f, 0.243f);
        kGridColor            = new Color(0.25f, 0.25f, 0.25f);
        
        // Node colors
        var c= new Func<int,float>(i=> ((float)i)/255f);
        kNodeTitleColor             = Color.black;
        kNodeLabelColor             = Color.white;
        kNodeValueColor             = new Color(1f, 0.8f, 0.4f);
        kEntryStateNodeColor        = new Color(1f, 0.8f, 0.4f);
        kStateNodeColor             = Color.cyan;
        kPackageNodeColor           = Color.yellow;
        kInstanceNodeColor          = new Color(1f, 0.5f, 0f);
        kConstructorNodeColor       = new Color(1f, 0.25f, 0.5f);
        kFunctionNodeColor          = Color.green;
        kMessageNodeColor           = new Color(c(0x36), c(0x8a), c(0xff));
        kSelectedNodeBackgroundColor= Color.white;
        
        // Type colors
        kBoolTypeColor      = Color.red;
        kIntTypeColor       = Color.magenta;
        kFloatTypeColor     = Color.cyan;
        kVector2TypeColor   = Color.yellow;
        kVector3TypeColor   = Color.green;
        kVector4TypeColor   = Color.blue;
        kStringTypeColor    = Color.red;
        kGameObjectTypeColor= Color.blue;
        kDefaultTypeColor   = Color.white;
    }
    public new void OnEnable() {
        base.OnEnable();
        title= "iCanScript Preferences";
        minSize= new Vector2(500f, 400f);
        maxSize= new Vector2(500f, 400f);
    }

    // ---------------------------------------------------------------------------------
	static GUIStyle largeLabelStyleCache= null;
    void RebuildStyles() {
        bool rebuildStyleNeeded= false;
        if(largeLabelStyleCache != EditorStyles.largeLabel) {
            rebuildStyleNeeded= true;
            largeLabelStyleCache= EditorStyles.largeLabel;
        }
		// Build title style
        if(titleStyle == null || rebuildStyleNeeded) {
            titleStyle= new GUIStyle(EditorStyles.largeLabel);                    
	        titleStyle.fontSize= 18;
	        titleStyle.fontStyle= FontStyle.Bold;
		}
		// Build selection grid style
        if(selectionStyle == null || rebuildStyleNeeded) {
            const int additionalPadding= 9;
            selectionStyle= new GUIStyle(EditorStyles.largeLabel);
	        selectionStyle.alignment= TextAnchor.MiddleRight;
	        selectionStyle.padding= new RectOffset(10,10,additionalPadding,additionalPadding);
	        selectionStyle.margin= new RectOffset(0,0,0,0);
			selectionStyle.overflow= new RectOffset(0,0,0,0);
			selectionStyle.border= new RectOffset(0,0,0,0);
			selectionStyle.fixedHeight= 2*additionalPadding+selectionStyle.lineHeight;
			var bkgColor= GUI.skin.settings.selectionColor;
			bkgColor.a= 1f;
			if(selectionBackground == null) {
				selectionBackground= new Texture2D(1,1);				
			}
			selectionBackground.SetPixel(0,0,bkgColor);
			selectionBackground.Apply();
			selectionStyle.onNormal.background= selectionBackground;
			selectionStyle.onNormal.textColor= Color.white;
		}
    }

	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public new void OnGUI() {
        // Draw common stuff for all editors
        base.OnGUI();
        
		// Reset GUI alpha.
		GUI.color= Color.white;
		// Build GUI styles (in case they were changed by user).
        RebuildStyles();
        
        // Outline column 1 area
        Rect column1Rect= new Rect(0,-1,kColumn1Width,position.height+1);
        GUI.Box(column1Rect,"");

        // Show selection grid.
        float lineHeight= selectionStyle.fixedHeight;
        float gridHeight= lineHeight*selGridStrings.Length;
        selGridId= GUI.SelectionGrid(new Rect(0,kTitleHeight,kColumn1Width,gridHeight), selGridId, selGridStrings, 1, selectionStyle);

        // Show title
        if(selGridId >= 0 && selGridId < selGridStrings.Length) {
            string title= selGridStrings[selGridId];            
            GUI.Label(new Rect(kColumn2X+1.5f*kMargin,kMargin, kColumn2Width+kColumn3Width, kTitleHeight), title, titleStyle);
        }

        // Execute option specific panel.
        switch(selGridId) {
            case 0: DisplayOptions(); break;
            case 1: Canvas(); break;
            case 2: NodeColors(); break;
            case 3: TypeColors(); break;
            case 4: InstanceWizard(); break;
			case 5: SoftwareUpdate(); break;
#if CODE_GENERATION_CONFIG
            case 6: CodeEngineering(); break;
#endif
            default: break;
        }

        // Show iCanScript version information.
        string version= iCS_EditorConfig.VersionStr;
        GUIContent versionContent= new GUIContent(version);
        Vector2 versionSize= GUI.skin.label.CalcSize(versionContent);
        float x= column1Rect.x+0.5f*(column1Rect.width-versionSize.x);
        float y= column1Rect.yMax-2.5f*versionSize.y;
        Rect pos= new Rect(x, y, versionSize.x, versionSize.y);
        GUI.Label(pos, versionContent);
        pos.y+= versionSize.y;
        string buildDate= iCS_BuildInfo.kBuildDateStr;
        GUIContent buildDateContent= new GUIContent(buildDate);
        Vector2 buildDateSize= GUI.skin.label.CalcSize(buildDateContent);        
        pos.x= column1Rect.x+0.5f*(column1Rect.width-buildDateSize.x);
		pos.width= buildDateSize.x;
		pos.height= buildDateSize.y;
        GUI.Label(pos, buildDateContent);

        // Show product icon
        var logoWidth= 64f;
        var logoHeight= 64f;
        Texture2D iCanScriptLogo= null;
        if(iCS_TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out iCanScriptLogo)) {
            Rect r= new Rect(0.5f*(kColumn1Width-logoWidth), position.height-logoHeight-10f-2f*versionSize.y, logoWidth, logoHeight);
            GUI.DrawTexture(r, iCanScriptLogo);
        }        		
	}
    // ---------------------------------------------------------------------------------
    void DisplayOptions() {
        // Draw column 2
        Rect p= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        GUI.Label(p, "Animation Controls", EditorStyles.boldLabel);
        Rect[] pos= new Rect[10];
        pos[0]= new Rect(p.x, p.yMax, p.width, p.height);
        for(int i= 1; i < 8; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        pos[7].y+= pos[6].height;
        GUI.Label(pos[7], "Runtime Configuration", EditorStyles.boldLabel);
        pos[7].y+= pos[7].height;
        for(int i= 8; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;            
        }
        GUI.Label(pos[0], "Animation Enabled");
        GUI.Label(pos[1], "Animation Pixels/Second");
        GUI.Label(pos[2], "Minimum Animation Time");
        GUI.Label(pos[3], "Scroll Speed");
        GUI.Label(pos[4], "Edge Scroll Speed (pixels)");
        GUI.Label(pos[5], "Inverse Zoom");
        GUI.Label(pos[6], "Zoom Speed");
        GUI.Label(pos[7], "Show Runtime Values");
        GUI.Label(pos[8], "Refresh Period (seconds)");
		GUI.Label(pos[9], "Show Frame Id");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        AnimationEnabled= EditorGUI.Toggle(pos[0], AnimationEnabled);
        EditorGUI.BeginDisabledGroup(AnimationEnabled==false);
        AnimationPixelsPerSecond= EditorGUI.FloatField(pos[1], AnimationPixelsPerSecond);
        MinAnimationTime= EditorGUI.FloatField(pos[2], MinAnimationTime);
        EditorGUI.EndDisabledGroup();
        ScrollSpeed= EditorGUI.FloatField(pos[3], ScrollSpeed);
        EdgeScrollSpeed= EditorGUI.FloatField(pos[4], EdgeScrollSpeed);
        InverseZoom= EditorGUI.Toggle(pos[5], InverseZoom);
        ZoomSpeed= EditorGUI.FloatField(pos[6], ZoomSpeed);
        ShowRuntimePortValue= EditorGUI.Toggle(pos[7], ShowRuntimePortValue);
        PortValueRefreshPeriod= EditorGUI.FloatField(pos[8], PortValueRefreshPeriod);
		ShowRuntimeFrameId= EditorGUI.Toggle(pos[9], ShowRuntimeFrameId);

        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            AnimationEnabled        = kAnimationEnabled;
            AnimationPixelsPerSecond= kAnimationPixelsPerSecond;
            MinAnimationTime        = kMinAnimationTime;
            ScrollSpeed             = kScrollSpeed;
            EdgeScrollSpeed         = kEdgeScrollSpeed;
            InverseZoom             = kInverseZoom;
            ZoomSpeed               = kZoomSpeed;
            ShowRuntimePortValue    = kShowRuntimePortValue;
            PortValueRefreshPeriod  = kPortValueRefreshPeriod;
			ShowRuntimeFrameId      = kShowRuntimeFrameId;   
        }
    }
    // ---------------------------------------------------------------------------------
    void Canvas() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Grid Spacing");
        GUI.Label(pos[1], "Grid Color");
        GUI.Label(pos[2], "Background Color");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        GridSpacing= EditorGUI.FloatField(pos[0], GridSpacing);
        GridColor= EditorGUI.ColorField(pos[1], GridColor);
        CanvasBackgroundColor= EditorGUI.ColorField(pos[2], CanvasBackgroundColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            GridSpacing= kGridSpacing;
            GridColor= kGridColor;
            CanvasBackgroundColor= kCanvasBackgroundColor;
        }        
    }
    // ---------------------------------------------------------------------------------
    void NodeColors() {
        // Column 2
        Rect[] pos= new Rect[12];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0],  "Selected Brightness Gain");
        GUI.Label(pos[1],  "Title");
        GUI.Label(pos[2],  "Label");
        GUI.Label(pos[3],  "Value");
        GUI.Label(pos[4],  "Package");
        GUI.Label(pos[5],  "Function");
        GUI.Label(pos[6],  "Object Constructor");
        GUI.Label(pos[7],  "Object Instance");
        GUI.Label(pos[8],  "State");
        GUI.Label(pos[9],  "Entry State");
        GUI.Label(pos[10], "Message Handler");
        GUI.Label(pos[11], "Selected Background");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        SelectedBrightnessGain= EditorGUI.FloatField(pos[0], SelectedBrightnessGain);
        NodeTitleColor= EditorGUI.ColorField(pos[1], NodeTitleColor);
        NodeLabelColor= EditorGUI.ColorField(pos[2], NodeLabelColor);
        NodeValueColor= EditorGUI.ColorField(pos[3], NodeValueColor);
        PackageNodeColor= EditorGUI.ColorField(pos[4], PackageNodeColor);
        FunctionNodeColor= EditorGUI.ColorField(pos[5], FunctionNodeColor);
        ConstructorNodeColor= EditorGUI.ColorField(pos[6], ConstructorNodeColor);
        InstanceNodeColor= EditorGUI.ColorField(pos[7], InstanceNodeColor);
        StateNodeColor= EditorGUI.ColorField(pos[8], StateNodeColor);
        EntryStateNodeColor= EditorGUI.ColorField(pos[9], EntryStateNodeColor);
        MessageNodeColor= EditorGUI.ColorField(pos[10], MessageNodeColor);
        SelectedNodeBackgroundColor= EditorGUI.ColorField(pos[11], SelectedNodeBackgroundColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            SelectedBrightnessGain= kSelectedBrightnessGain;
            NodeTitleColor= kNodeTitleColor;
            NodeLabelColor= kNodeLabelColor;
            NodeValueColor= kNodeValueColor;
            PackageNodeColor= kPackageNodeColor;
            FunctionNodeColor= kFunctionNodeColor;
            ConstructorNodeColor= kConstructorNodeColor;
            InstanceNodeColor= kInstanceNodeColor;
            StateNodeColor= kStateNodeColor;
            EntryStateNodeColor= kEntryStateNodeColor;
            MessageNodeColor= kMessageNodeColor;
            SelectedNodeBackgroundColor= kSelectedNodeBackgroundColor;
        }        
    }
    // ---------------------------------------------------------------------------------
    void TypeColors() {
        // Column 2
        Rect[] pos= new Rect[9];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Bool");
        GUI.Label(pos[1], "Int");
        GUI.Label(pos[2], "Float");
        GUI.Label(pos[3], "String");
        GUI.Label(pos[4], "Vector2");
        GUI.Label(pos[5], "Vector3");
        GUI.Label(pos[6], "Vector4");
        GUI.Label(pos[7], "Game Object");
        GUI.Label(pos[8], "Default");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        BoolTypeColor= EditorGUI.ColorField(pos[0], BoolTypeColor);
        IntTypeColor= EditorGUI.ColorField(pos[1], IntTypeColor);
        FloatTypeColor= EditorGUI.ColorField(pos[2], FloatTypeColor);
        StringTypeColor= EditorGUI.ColorField(pos[3], StringTypeColor);
        Vector2TypeColor= EditorGUI.ColorField(pos[4], Vector2TypeColor);
        Vector3TypeColor= EditorGUI.ColorField(pos[5], Vector3TypeColor);
        Vector4TypeColor= EditorGUI.ColorField(pos[6], Vector4TypeColor);
        GameObjectTypeColor= EditorGUI.ColorField(pos[7], GameObjectTypeColor);
        DefaultTypeColor= EditorGUI.ColorField(pos[8], DefaultTypeColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            BoolTypeColor      = kBoolTypeColor;
            IntTypeColor       = kIntTypeColor;
            FloatTypeColor     = kFloatTypeColor;
            StringTypeColor    = kStringTypeColor;
            Vector2TypeColor   = kVector2TypeColor;
            Vector3TypeColor   = kVector3TypeColor;
            Vector4TypeColor   = kVector4TypeColor;
            GameObjectTypeColor= kGameObjectTypeColor;
            DefaultTypeColor   = kDefaultTypeColor;
        }        
    }
    // ---------------------------------------------------------------------------------
    void InstanceWizard() {
        // Header
        Rect p= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        GUI.Label(p, "Auto-Creation", EditorStyles.boldLabel);
        Rect p2= new Rect(kColumn3X+kMargin, p.y, 0.5f*kColumn3Width, 20f);
        GUI.Label(p2, "In", EditorStyles.boldLabel);
        p2.x+= 40f;
        GUI.Label(p2, "Out", EditorStyles.boldLabel);
        
        // Column 2
        Rect[] pos= new Rect[5];
        pos[0]= new Rect(p.x, p.yMax, p.width, p.height);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "this");
        GUI.Label(pos[1], "Instance Fields");
        GUI.Label(pos[2], "Class Fields");
        GUI.Label(pos[3], "Instance Properties");
        GUI.Label(pos[4], "Class Properties");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= 40f;
        }
        InstanceAutocreateInThis           = EditorGUI.Toggle(pos[0], InstanceAutocreateInThis);
        InstanceAutocreateInFields         = EditorGUI.Toggle(pos[1], InstanceAutocreateInFields);
        InstanceAutocreateInClassFields    = EditorGUI.Toggle(pos[2], InstanceAutocreateInClassFields);
        InstanceAutocreateInProperties     = EditorGUI.Toggle(pos[3], InstanceAutocreateInProperties);
        InstanceAutocreateInClassProperties= EditorGUI.Toggle(pos[4], InstanceAutocreateInClassProperties);
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= 45f;
        }
        InstanceAutocreateOutFields         = EditorGUI.Toggle(pos[1], InstanceAutocreateOutFields);
        InstanceAutocreateOutClassFields    = EditorGUI.Toggle(pos[2], InstanceAutocreateOutClassFields);
        InstanceAutocreateOutProperties     = EditorGUI.Toggle(pos[3], InstanceAutocreateOutProperties);
        InstanceAutocreateOutClassProperties= EditorGUI.Toggle(pos[4], InstanceAutocreateOutClassProperties);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            InstanceAutocreateInThis            = kInstanceAutocreateInThis;
            InstanceAutocreateInFields          = kInstanceAutocreateInFields;
            InstanceAutocreateInClassFields     = kInstanceAutocreateInClassFields;
            InstanceAutocreateInProperties      = kInstanceAutocreateInProperties;
            InstanceAutocreateInClassProperties = kInstanceAutocreateInClassProperties;
            InstanceAutocreateOutFields         = kInstanceAutocreateOutFields;
            InstanceAutocreateOutClassFields    = kInstanceAutocreateOutClassFields;
            InstanceAutocreateOutProperties     = kInstanceAutocreateOutProperties;
            InstanceAutocreateOutClassProperties= kInstanceAutocreateOutClassProperties;
        }        
    }
    // ---------------------------------------------------------------------------------
	void SoftwareUpdate() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Watch for Updates");
        GUI.Label(pos[1], "Verification Internal");
        GUI.Label(pos[2], "Skipped Version");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
		SoftwareUpdateWatchEnabled  = EditorGUI.Toggle(pos[0], SoftwareUpdateWatchEnabled);
		SoftwareUpdateInterval      = (iCS_UpdateInterval)EditorGUI.EnumPopup(pos[1], SoftwareUpdateInterval);
		SoftwareUpdateSkippedVersion= EditorGUI.TextField(pos[2], SoftwareUpdateSkippedVersion);

        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
			SoftwareUpdateWatchEnabled  = kSoftwareUpdateWatchEnabled;
			SoftwareUpdateInterval      = kSoftwareUpdateInterval;
			SoftwareUpdateSkippedVersion= kSoftwareUpdateSkippedVersion;
        }        
	}
#if CODE_GENERATION_CONFIG
    // ---------------------------------------------------------------------------------
	void CodeEngineering() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "File Prefix");
        GUI.Label(pos[1], "Root Folder");
        GUI.Label(pos[2], "Behaviour Subfolder");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        CodeGenerationFilePrefix    = EditorGUI.TextField(pos[0], CodeGenerationFilePrefix);
        CodeGenerationFolder        = EditorGUI.TextField(pos[1], CodeGenerationFolder);
        BehaviourGenerationSubfolder= EditorGUI.TextField(pos[2], BehaviourGenerationSubfolder);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            CodeGenerationFilePrefix    = kCodeGenerationFilePrefix;
            CodeGenerationFolder        = kCodeGenerationFolder;
            BehaviourGenerationSubfolder= kBehaviourGenerationSubFolder;
        }        	    
	}
#endif
	
	// =================================================================================
    // Helpers.
    // ---------------------------------------------------------------------------------
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
    // ---------------------------------------------------------------------------------
    public static string RemoveProductPrefix(string name) {
        if(name.StartsWith(iCS_Config.ProductPrefix)) {
            return name.Substring(iCS_Config.ProductPrefix.Length);
        }
        return name;
    }
    // -------------------------------------------------------------------------
    Texture2D LoadIconFromPath(string iconPath) {
        return iCS_TextureCache.GetIcon(iconPath);
    }
    // -------------------------------------------------------------------------
    Texture2D LoadIconFromGUID(string iconGuid) {
        return iCS_TextureCache.GetIconFromGUID(iconGuid);
    }
}
