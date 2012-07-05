using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable]
//public class iCS_UserPreferences {
//
//    [System.Serializable]
//    public class UserNodeColors {
//        public float    SelectedBrightness= 1.75f;
//        public Color    TitleColor        = Color.black;
//        public Color    LabelColor        = Color.white;
//        public Color    ValueColor        = new Color(1f,0.8f,0.4f);
//        public Color    EntryStateColor   = new Color(1f,0.5f,0.25f);
//        public Color    StateColor        = Color.cyan;
//        public Color    ModuleColor       = Color.yellow;
//        public Color    ClassColor        = new Color(1f,0.5f,0f);
//        public Color    ConstructorColor  = new Color(1f,0.25f,0.5f);
//        public Color    FunctionColor     = Color.green;
//        public Color    SelectedColor     = Color.white;            
//    }
//    public UserNodeColors   NodeColors= new UserNodeColors();
//
//    [System.Serializable]
//    public class UserTypeColors {
//        [System.Serializable]
//        public class UserTypeColor {
//            public string  TypeName;
//            public Color   TypeColor;
//            public UserTypeColor(Type type, Color color) {
//                TypeName= type.Name;
//                TypeColor= color;
//            }
//        }
//        public UserTypeColor    BoolType      = new UserTypeColor(typeof(bool),       Color.red);
//        public UserTypeColor    IntType       = new UserTypeColor(typeof(int),        Color.magenta);
//        public UserTypeColor    FloatType     = new UserTypeColor(typeof(float),      Color.cyan);
//        public UserTypeColor    Vector2Type   = new UserTypeColor(typeof(Vector2),    Color.yellow);
//        public UserTypeColor    Vector3Type   = new UserTypeColor(typeof(Vector3),    Color.green);
//        public UserTypeColor    Vector4Type   = new UserTypeColor(typeof(Vector4),    Color.blue);
//        public UserTypeColor    StringType    = new UserTypeColor(typeof(string),     Color.red);
//        public UserTypeColor    GameObjectType= new UserTypeColor(typeof(GameObject), Color.blue);
//        public UserTypeColor[]  CustomColors  = new UserTypeColor[0];
//
//        public Color GetColor(Type t) {
//            if(t == null) return Color.white;
//            string typeName= t.HasElementType ? t.GetElementType().Name : t.Name;
//            if(typeName == BoolType.TypeName)       return BoolType.TypeColor;
//            if(typeName == IntType.TypeName)        return IntType.TypeColor;
//            if(typeName == FloatType.TypeName)      return FloatType.TypeColor;
//            if(typeName == Vector2Type.TypeName)    return Vector2Type.TypeColor;
//            if(typeName == Vector3Type.TypeName)    return Vector3Type.TypeColor;
//            if(typeName == Vector4Type.TypeName)    return Vector4Type.TypeColor;
//            if(typeName == StringType.TypeName)     return StringType.TypeColor;
//            if(typeName == GameObjectType.TypeName) return GameObjectType.TypeColor;
//            foreach(var tc in CustomColors) {
//                if(typeName == tc.TypeName) return tc.TypeColor;
//            }
//            return Color.white;
//        }
//    }
//    public UserTypeColors   TypeColors= new UserTypeColors();
//
//    [System.Serializable]
//    public class UserHiddenPrefixes {
//        public const string uCodePrefix= iCS_Config.ProductPrefix;
//        public string[]     CustomPrefixes= new string[0]; 
//
//        public string GetTypeName(Type t)  { return GetName(t.Name); }
//        public string GetName(string name) {
//            int prefixLen= uCodePrefix.Length;
//            int nameLen= name.Length;
//            if(nameLen > prefixLen && name.Substring(0, prefixLen) == uCodePrefix) return name.Substring(prefixLen, nameLen-prefixLen);
//            foreach(var prefix in CustomPrefixes) {
//                prefixLen= prefix.Length;
//                if(nameLen > prefixLen && name.Substring(0, prefixLen) == prefix) return name.Substring(prefixLen, name.Length-prefixLen);                
//            }
//            return name;
//        }
//    }
//    public UserHiddenPrefixes   HiddenPrefixes= new UserHiddenPrefixes();
//
//    [System.Serializable]
//    public class UserIcons {
//        public bool         EnableMinimizedIcons= true;
//        public const string uCodeIconPath= iCS_Config.GuiAssetPath;
//        public string[]     CustomIconPaths= new string[0];
//    }
//    public UserIcons     Icons= new UserIcons();
//    
//    [System.Serializable]
//    public class UserClassWizard {
//        public bool OutputInstanceVariables = true;
//        public bool OutputClassVariables    = false;
//        public bool OutputInstanceProperties= false;
//        public bool OutputClassProperties   = false;
//        public bool InputInstanceVariables  = false;
//        public bool InputClassVariables     = false;
//        public bool InputInstanceProperties = false;
//        public bool InputClassProperties    = false;
//    }
//    public UserClassWizard    ClassWizard= new UserClassWizard();
//
//}
//

public class iCS_PreferencesEditor : iCS_EditorBase {
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float kMargin      = 10.0f;
    const float kTitleHeight = 40.0f;
    const float kColumn1Width= 120.0f;
    const float kColumn2Width= 180.0f;
    const float kColumn3Width= 120.0f;
    const float kColumn1X    = 0;
    const float kColumn2X    = kColumn1X+kColumn1Width;
    const float kColumn3X    = kColumn2X+kColumn2Width;
    // ---------------------------------------------------------------------------------
    // Display Option Constants
    const float  kAnimationTime            = 0.35f;
    const float  kScrollSpeed              = 3.0f;
    const float  kEdgeScrollSpeed          = 400.0f;
    const bool   kInverseZoom              = false;
    const string kAnimationTimeKey         = "iCS_AnimationTime";
    const string kScrollSpeedKey           = "iCS_ScrollSpeed";
    const string kEdgeScrollSpeedKey       = "iCS_EdgeScrollSpeed";
    const string kInverseZoomKey           = "iCS_InverseZoom";
    const bool   kShowRuntimePortValue     = true;
    const float  kPortValueRefreshPeriod   = 0.25f;
    const string kShowRuntimePortValueKey  = "iCS_ShowRuntimePortValue";
    const string kPortValueRefreshPeriodKey= "iCS_PortValueRefresh";
    // ---------------------------------------------------------------------------------
    // Canvas Constants
    const float  kCanvasBackgroundColor_R   = 0.169f;
    const float  kCanvasBackgroundColor_G   = 0.188f;
    const float  kCanvasBackgroundColor_B   = 0.243f;
    const float  kGridColor_R               = 0.25f;
    const float  kGridColor_G               = 0.25f;
    const float  kGridColor_B               = 0.25f;
    const float  kGridSpacing               = 20.0f;
    const string kCanvasBackgroundColor_RKey= "iCS_CanvasBackgroundColor_R";
    const string kCanvasBackgroundColor_GKey= "iCS_CanvasBackgroundColor_G";
    const string kCanvasBackgroundColor_BKey= "iCS_CanvasBackgroundColor_B";
    const string kGridColor_RKey            = "iCS_GridColor_R";
    const string kGridColor_GKey            = "iCS_GridColor_G";
    const string kGridColor_BKey            = "iCS_GridColor_B";
    const string kGridSpacingKey            = "iCS_GridSpacing";
    // ---------------------------------------------------------------------------------
    // Node Color Constants
    const float    kSelectedBrightnessGain      = 1.75f;
    const float    kNodeTitleColor_R            = 0;
    const float    kNodeTitleColor_G            = 0;
    const float    kNodeTitleColor_B            = 0;
    const float    kNodeLabelColor_R            = 1f;
    const float    kNodeLabelColor_G            = 1f;
    const float    kNodeLabelColor_B            = 1f;
    const float    kNodeValueColor_R            = 1f;
    const float    kNodeValueColor_G            = 0.8f;
    const float    kNodeValueColor_B            = 0.4f;
    const float    kEntryStateColor_R           = 1f;
    const float    kEntryStateColor_G           = 0.5f;
    const float    kEntryStateColor_B           = 0.25f;
    const float    kStateColor_R                = 0f;
    const float    kStateColor_G                = 1f;
    const float    kStateColor_B                = 1f;
    const float    kPackageColor_R              = 1f;
    const float    kPackageColor_G              = 0.92f;
    const float    kPackageColor_B              = 0.016f;
    const float    kInstanceColor_R             = 1f;
    const float    kInstanceColor_G             = 0.5f;
    const float    kInstanceColor_B             = 0f;
    const float    kConstructorColor_R          = 1f;
    const float    kConstructorColor_G          = 0.25f;
    const float    kConstructorColor_B          = 0.5f;
    const float    kFunctionColor_R             = 0f;
    const float    kFunctionColor_G             = 1f;
    const float    kFunctionColor_B             = 0f;
    const float    kSelectedBackgroundColor_R   = 1f;            
    const float    kSelectedBackgroundColor_G   = 1f;            
    const float    kSelectedBackgroundColor_B   = 1f;            
    const string   kSelectedBrightnessGainKey   = "iCS_SelectedBrightnessGain";
    const string   kNodeTitleColor_RKey         = "iCS_NodeTitleColor_R";
    const string   kNodeTitleColor_GKey         = "iCS_NodeTitleColor_G";
    const string   kNodeTitleColor_BKey         = "iCS_NodeTitleColor_B";
    const string   kNodeLabelColor_RKey         = "iCS_NodeLabelColor_R";
    const string   kNodeLabelColor_GKey         = "iCS_NodeLabelColor_G";
    const string   kNodeLabelColor_BKey         = "iCS_NodeLabelColor_B";
    const string   kNodeValueColor_RKey         = "iCS_NodeValueColor_R";
    const string   kNodeValueColor_GKey         = "iCS_NodeValueColor_G";
    const string   kNodeValueColor_BKey         = "iCS_NodeValueColor_B";
    const string   kPackageColor_RKey           = "iCS_PackageColor_R";
    const string   kPackageColor_GKey           = "iCS_PackageColor_G";
    const string   kPackageColor_BKey           = "iCS_PackageColor_B";
    const string   kFunctionColor_RKey          = "iCS_FunctionColor_R";
    const string   kFunctionColor_GKey          = "iCS_FunctionColor_G";
    const string   kFunctionColor_BKey          = "iCS_FunctionColor_B";
    const string   kConstructorColor_RKey       = "iCS_ConstructorColor_R";
    const string   kConstructorColor_GKey       = "iCS_ConstructorColor_G";
    const string   kConstructorColor_BKey       = "iCS_ConstructorColor_B";
    const string   kInstanceColor_RKey          = "iCS_InstanceColor_R";
    const string   kInstanceColor_GKey          = "iCS_InstanceColor_G";
    const string   kInstanceColor_BKey          = "iCS_InstanceColor_B";
    const string   kStateColor_RKey             = "iCS_StateColor_R";
    const string   kStateColor_GKey             = "iCS_StateColor_G";
    const string   kStateColor_BKey             = "iCS_StateColor_B";
    const string   kEntryStateColor_RKey        = "iCS_EntryStateColor_R";
    const string   kEntryStateColor_GKey        = "iCS_EntryStateColor_G";
    const string   kEntryStateColor_BKey        = "iCS_EntryStateColor_B";
    const string   kSelectedBackgroundColor_RKey= "iCS_SelectedBackgroundColor_R";         
    const string   kSelectedBackgroundColor_GKey= "iCS_SelectedBackgroundColor_G";         
    const string   kSelectedBackgroundColor_BKey= "iCS_SelectedBackgroundColor_B";         
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	int         selGridId= 0;
	string[]    selGridStrings= new string[]{"Display Options", "Canvas", "Node Colors", "Type Colors", "Hidden Prefixes", "Icons", "Instance Wizard"};
	GUIStyle    titleStyle= null;
	GUIStyle    selectionStyle= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static float AnimationTime {
        get {
            return EditorPrefs.GetFloat(kAnimationTimeKey, kAnimationTime);
        }
        set {
            if(value < 0) return;
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
            if(value < 0) return;
            EditorPrefs.SetFloat(kPortValueRefreshPeriodKey, value);
        }
    }
    public static Color CanvasBackgroundColor {
        get {
            float r= EditorPrefs.GetFloat(kCanvasBackgroundColor_RKey, kCanvasBackgroundColor_R);
            float g= EditorPrefs.GetFloat(kCanvasBackgroundColor_GKey, kCanvasBackgroundColor_G);
            float b= EditorPrefs.GetFloat(kCanvasBackgroundColor_BKey, kCanvasBackgroundColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kCanvasBackgroundColor_RKey, value.r);
            EditorPrefs.SetFloat(kCanvasBackgroundColor_GKey, value.g);
            EditorPrefs.SetFloat(kCanvasBackgroundColor_BKey, value.b);
        }
    }
    public static Color GridColor {
        get {
            float r= EditorPrefs.GetFloat(kGridColor_RKey, kGridColor_R);
            float g= EditorPrefs.GetFloat(kGridColor_GKey, kGridColor_G);
            float b= EditorPrefs.GetFloat(kGridColor_BKey, kGridColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kGridColor_RKey, value.r);
            EditorPrefs.SetFloat(kGridColor_GKey, value.g);
            EditorPrefs.SetFloat(kGridColor_BKey, value.b);
        }
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
            if(value < 5.0f) return;
            EditorPrefs.SetFloat(kSelectedBrightnessGainKey, value);
        }
    }
    public static Color NodeTitleColor {
        get {
            float r= EditorPrefs.GetFloat(kNodeTitleColor_RKey, kNodeTitleColor_R);
            float g= EditorPrefs.GetFloat(kNodeTitleColor_GKey, kNodeTitleColor_G);
            float b= EditorPrefs.GetFloat(kNodeTitleColor_BKey, kNodeTitleColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kNodeTitleColor_RKey, value.r);
            EditorPrefs.SetFloat(kNodeTitleColor_GKey, value.g);
            EditorPrefs.SetFloat(kNodeTitleColor_BKey, value.b);
        }
    }
    public static Color NodeLabelColor {
        get {
            float r= EditorPrefs.GetFloat(kNodeLabelColor_RKey, kNodeLabelColor_R);
            float g= EditorPrefs.GetFloat(kNodeLabelColor_GKey, kNodeLabelColor_G);
            float b= EditorPrefs.GetFloat(kNodeLabelColor_BKey, kNodeLabelColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kNodeLabelColor_RKey, value.r);
            EditorPrefs.SetFloat(kNodeLabelColor_GKey, value.g);
            EditorPrefs.SetFloat(kNodeLabelColor_BKey, value.b);
        }
    }
    public static Color NodeValueColor {
        get {
            float r= EditorPrefs.GetFloat(kNodeValueColor_RKey, kNodeValueColor_R);
            float g= EditorPrefs.GetFloat(kNodeValueColor_GKey, kNodeValueColor_G);
            float b= EditorPrefs.GetFloat(kNodeValueColor_BKey, kNodeValueColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kNodeValueColor_RKey, value.r);
            EditorPrefs.SetFloat(kNodeValueColor_GKey, value.g);
            EditorPrefs.SetFloat(kNodeValueColor_BKey, value.b);
        }
    }
    public static Color PackageColor {
        get {
            float r= EditorPrefs.GetFloat(kPackageColor_RKey, kPackageColor_R);
            float g= EditorPrefs.GetFloat(kPackageColor_GKey, kPackageColor_G);
            float b= EditorPrefs.GetFloat(kPackageColor_BKey, kPackageColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kPackageColor_RKey, value.r);
            EditorPrefs.SetFloat(kPackageColor_GKey, value.g);
            EditorPrefs.SetFloat(kPackageColor_BKey, value.b);
        }
    }
    public static Color FunctionColor {
        get {
            float r= EditorPrefs.GetFloat(kFunctionColor_RKey, kFunctionColor_R);
            float g= EditorPrefs.GetFloat(kFunctionColor_GKey, kFunctionColor_G);
            float b= EditorPrefs.GetFloat(kFunctionColor_BKey, kFunctionColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kFunctionColor_RKey, value.r);
            EditorPrefs.SetFloat(kFunctionColor_GKey, value.g);
            EditorPrefs.SetFloat(kFunctionColor_BKey, value.b);
        }
    }
    public static Color ConstructorColor {
        get {
            float r= EditorPrefs.GetFloat(kConstructorColor_RKey, kConstructorColor_R);
            float g= EditorPrefs.GetFloat(kConstructorColor_GKey, kConstructorColor_G);
            float b= EditorPrefs.GetFloat(kConstructorColor_BKey, kConstructorColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kConstructorColor_RKey, value.r);
            EditorPrefs.SetFloat(kConstructorColor_GKey, value.g);
            EditorPrefs.SetFloat(kConstructorColor_BKey, value.b);
        }
    }
    public static Color InstanceColor {
        get {
            float r= EditorPrefs.GetFloat(kInstanceColor_RKey, kInstanceColor_R);
            float g= EditorPrefs.GetFloat(kInstanceColor_GKey, kInstanceColor_G);
            float b= EditorPrefs.GetFloat(kInstanceColor_BKey, kInstanceColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kInstanceColor_RKey, value.r);
            EditorPrefs.SetFloat(kInstanceColor_GKey, value.g);
            EditorPrefs.SetFloat(kInstanceColor_BKey, value.b);
        }
    }
    public static Color StateColor {
        get {
            float r= EditorPrefs.GetFloat(kStateColor_RKey, kStateColor_R);
            float g= EditorPrefs.GetFloat(kStateColor_GKey, kStateColor_G);
            float b= EditorPrefs.GetFloat(kStateColor_BKey, kStateColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kStateColor_RKey, value.r);
            EditorPrefs.SetFloat(kStateColor_GKey, value.g);
            EditorPrefs.SetFloat(kStateColor_BKey, value.b);
        }
    }
    public static Color EntryStateColor {
        get {
            float r= EditorPrefs.GetFloat(kEntryStateColor_RKey, kEntryStateColor_R);
            float g= EditorPrefs.GetFloat(kEntryStateColor_GKey, kEntryStateColor_G);
            float b= EditorPrefs.GetFloat(kEntryStateColor_BKey, kEntryStateColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kEntryStateColor_RKey, value.r);
            EditorPrefs.SetFloat(kEntryStateColor_GKey, value.g);
            EditorPrefs.SetFloat(kEntryStateColor_BKey, value.b);
        }
    }
    public static Color SelectedBackgroundColor {
        get {
            float r= EditorPrefs.GetFloat(kSelectedBackgroundColor_RKey, kSelectedBackgroundColor_R);
            float g= EditorPrefs.GetFloat(kSelectedBackgroundColor_GKey, kSelectedBackgroundColor_G);
            float b= EditorPrefs.GetFloat(kSelectedBackgroundColor_BKey, kSelectedBackgroundColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kSelectedBackgroundColor_RKey, value.r);
            EditorPrefs.SetFloat(kSelectedBackgroundColor_GKey, value.g);
            EditorPrefs.SetFloat(kSelectedBackgroundColor_BKey, value.b);
        }
    }
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void Initialize() {
        if(titleStyle == null) {
            titleStyle= new GUIStyle(EditorStyles.largeLabel);                    
            titleStyle.fontSize= 18;
            titleStyle.fontStyle= FontStyle.Bold;
        }
        if(selectionStyle == null) {
            selectionStyle= new GUIStyle(EditorStyles.label);
            selectionStyle.alignment= TextAnchor.MiddleRight;
            selectionStyle.padding= new RectOffset(10,10,0,0);
            selectionStyle.active.textColor= Color.white;
            selectionStyle.hover.textColor= Color.blue;
            selectionStyle.onHover.textColor= Color.blue;
        }
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public override void OnGUI() {
        Initialize();
        
        float gridHeight= 20*selGridStrings.Length;
        GUI.Box(new Rect(0,0,kColumn1Width,position.height),"");
        selGridId= GUI.SelectionGrid(new Rect(0,kMargin+kTitleHeight,kColumn1Width,gridHeight), selGridId, selGridStrings, 1, selectionStyle);
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
            case 4: HiddenPrefixes(); break;
            case 5: Icons(); break;
            case 6: InstanceWizard(); break;
            default: break;
        }
	}
    // ---------------------------------------------------------------------------------
    void DisplayOptions() {
        // Draw column 2
        Rect p= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        GUI.Label(p, "Animation Controls", EditorStyles.boldLabel);
        Rect[] pos= new Rect[6];
        pos[0]= new Rect(p.x, p.yMax, p.width, p.height);
        for(int i= 1; i < 5; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        pos[4].y+= pos[3].height;
        GUI.Label(pos[4], "Port Values", EditorStyles.boldLabel);
        pos[4].y+= pos[4].height;
        for(int i= 5; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;            
        }
        GUI.Label(pos[0], "Animation Time");
        GUI.Label(pos[1], "Scroll Speed");
        GUI.Label(pos[2], "Edge Scroll Speed");
        GUI.Label(pos[3], "Inverse Zoom");
        GUI.Label(pos[4], "Show Runtime Values");
        GUI.Label(pos[5], "Refresh Period");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
        }
        AnimationTime= EditorGUI.FloatField(pos[0], AnimationTime);
        ScrollSpeed= EditorGUI.FloatField(pos[1], ScrollSpeed);
        EdgeScrollSpeed= EditorGUI.FloatField(pos[2], EdgeScrollSpeed);
        InverseZoom= EditorGUI.Toggle(pos[3], InverseZoom);
        ShowRuntimePortValue= EditorGUI.Toggle(pos[4], ShowRuntimePortValue);
        PortValueRefreshPeriod= EditorGUI.FloatField(pos[5], PortValueRefreshPeriod);

        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            AnimationTime         = kAnimationTime;
            ScrollSpeed           = kScrollSpeed;
            EdgeScrollSpeed       = kEdgeScrollSpeed;
            InverseZoom           = kInverseZoom;
            ShowRuntimePortValue  = kShowRuntimePortValue;
            PortValueRefreshPeriod= kPortValueRefreshPeriod;            
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
        }
        GridSpacing= EditorGUI.FloatField(pos[0], GridSpacing);
        GridColor= EditorGUI.ColorField(pos[1], GridColor);
        CanvasBackgroundColor= EditorGUI.ColorField(pos[2], CanvasBackgroundColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            GridSpacing= kGridSpacing;
            GridColor= new Color(kGridColor_R, kGridColor_G, kGridColor_B);
            CanvasBackgroundColor= new Color(kCanvasBackgroundColor_R, kCanvasBackgroundColor_G, kCanvasBackgroundColor_B);
        }        
    }
    // ---------------------------------------------------------------------------------
    void NodeColors() {
        // Column 2
        Rect[] pos= new Rect[11];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Selected Brightness Gain");
        GUI.Label(pos[1], "Title");
        GUI.Label(pos[2], "Label");
        GUI.Label(pos[3], "Value");
        GUI.Label(pos[4], "Package");
        GUI.Label(pos[5], "Function");
        GUI.Label(pos[6], "Constructor");
        GUI.Label(pos[7], "Instance");
        GUI.Label(pos[8], "State");
        GUI.Label(pos[9], "Entry State");
        GUI.Label(pos[10], "Selected Background");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
        }
        SelectedBrightnessGain= EditorGUI.FloatField(pos[0], SelectedBrightnessGain);
        NodeTitleColor= EditorGUI.ColorField(pos[1], NodeTitleColor);
        NodeLabelColor= EditorGUI.ColorField(pos[2], NodeLabelColor);
        NodeValueColor= EditorGUI.ColorField(pos[3], NodeValueColor);
        PackageColor= EditorGUI.ColorField(pos[4], PackageColor);
        FunctionColor= EditorGUI.ColorField(pos[5], FunctionColor);
        ConstructorColor= EditorGUI.ColorField(pos[6], ConstructorColor);
        InstanceColor= EditorGUI.ColorField(pos[7], InstanceColor);
        StateColor= EditorGUI.ColorField(pos[8], StateColor);
        EntryStateColor= EditorGUI.ColorField(pos[9], EntryStateColor);
        SelectedBackgroundColor= EditorGUI.ColorField(pos[10], SelectedBackgroundColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            SelectedBrightnessGain= kSelectedBrightnessGain;
            NodeTitleColor= new Color(kNodeTitleColor_R, kNodeTitleColor_G, kNodeTitleColor_B);
            NodeLabelColor= new Color(kNodeLabelColor_R, kNodeLabelColor_G, kNodeLabelColor_B);
            NodeValueColor= new Color(kNodeValueColor_R, kNodeValueColor_G, kNodeValueColor_B);
            PackageColor= new Color(kPackageColor_R, kPackageColor_G, kPackageColor_B);
            FunctionColor= new Color(kFunctionColor_R, kFunctionColor_G, kFunctionColor_B);
            ConstructorColor= new Color(kConstructorColor_R, kConstructorColor_G, kConstructorColor_B);
            InstanceColor= new Color(kInstanceColor_R, kInstanceColor_G, kInstanceColor_B);
            StateColor= new Color(kStateColor_R, kStateColor_G, kStateColor_B);
            EntryStateColor= new Color(kEntryStateColor_R, kEntryStateColor_G, kEntryStateColor_B);
            SelectedBackgroundColor= new Color(kSelectedBackgroundColor_R, kSelectedBackgroundColor_G, kSelectedBackgroundColor_B);
        }        
    }
    // ---------------------------------------------------------------------------------
    void TypeColors() {
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
        }        
    }
    // ---------------------------------------------------------------------------------
    void HiddenPrefixes() {
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
        }         
    }
    // ---------------------------------------------------------------------------------
    void Icons() {
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
        }        
    }
    // ---------------------------------------------------------------------------------
    void InstanceWizard() {
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
        }                
    }
}
