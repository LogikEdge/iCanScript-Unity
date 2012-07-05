using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable]
//public class iCS_UserPreferences {
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
    const float    kSelectedBrightnessGain       = 1.75f;
    const float    kNodeTitleColor_R             = 0;
    const float    kNodeTitleColor_G             = 0;
    const float    kNodeTitleColor_B             = 0;
    const float    kNodeLabelColor_R             = 1f;
    const float    kNodeLabelColor_G             = 1f;
    const float    kNodeLabelColor_B             = 1f;
    const float    kNodeValueColor_R             = 1f;
    const float    kNodeValueColor_G             = 0.8f;
    const float    kNodeValueColor_B             = 0.4f;
    const float    kEntryStateNodeColor_R        = 1f;
    const float    kEntryStateNodeColor_G        = 0.5f;
    const float    kEntryStateNodeColor_B        = 0.25f;
    const float    kStateNodeColor_R             = 0f;
    const float    kStateNodeColor_G             = 1f;
    const float    kStateNodeColor_B             = 1f;
    const float    kPackageNodeColor_R           = 1f;
    const float    kPackageNodeColor_G           = 0.92f;
    const float    kPackageNodeColor_B           = 0.016f;
    const float    kInstanceNodeColor_R          = 1f;
    const float    kInstanceNodeColor_G          = 0.5f;
    const float    kInstanceNodeColor_B          = 0f;
    const float    kConstructorNodeColor_R       = 1f;
    const float    kConstructorNodeColor_G       = 0.25f;
    const float    kConstructorNodeColor_B       = 0.5f;
    const float    kFunctionNodeColor_R          = 0f;
    const float    kFunctionNodeColor_G          = 1f;
    const float    kFunctionNodeColor_B          = 0f;
    const float    kSelectedNodeBackgroundColor_R= 1f;            
    const float    kSelectedNodeBackgroundColor_G= 1f;            
    const float    kSelectedNodeBackgroundColor_B= 1f;            
    const string   kSelectedBrightnessGainKey       = "iCS_SelectedBrightnessGain";
    const string   kNodeTitleColor_RKey             = "iCS_NodeTitleColor_R";
    const string   kNodeTitleColor_GKey             = "iCS_NodeTitleColor_G";
    const string   kNodeTitleColor_BKey             = "iCS_NodeTitleColor_B";
    const string   kNodeLabelColor_RKey             = "iCS_NodeLabelColor_R";
    const string   kNodeLabelColor_GKey             = "iCS_NodeLabelColor_G";
    const string   kNodeLabelColor_BKey             = "iCS_NodeLabelColor_B";
    const string   kNodeValueColor_RKey             = "iCS_NodeValueColor_R";
    const string   kNodeValueColor_GKey             = "iCS_NodeValueColor_G";
    const string   kNodeValueColor_BKey             = "iCS_NodeValueColor_B";
    const string   kPackageNodeColor_RKey           = "iCS_PackageNodeColor_R";
    const string   kPackageNodeColor_GKey           = "iCS_PackageNodeColor_G";
    const string   kPackageNodeColor_BKey           = "iCS_PackageNodeColor_B";
    const string   kFunctionNodeColor_RKey          = "iCS_FunctionNodeColor_R";
    const string   kFunctionNodeColor_GKey          = "iCS_FunctionNodeColor_G";
    const string   kFunctionNodeColor_BKey          = "iCS_FunctionNodeColor_B";
    const string   kConstructorNodeColor_RKey       = "iCS_ConstructorNodeColor_R";
    const string   kConstructorNodeColor_GKey       = "iCS_ConstructorNodeColor_G";
    const string   kConstructorNodeColor_BKey       = "iCS_ConstructorNodeColor_B";
    const string   kInstanceNodeColor_RKey          = "iCS_InstanceNodeColor_R";
    const string   kInstanceNodeColor_GKey          = "iCS_InstanceNodeColor_G";
    const string   kInstanceNodeColor_BKey          = "iCS_InstanceNodeColor_B";
    const string   kStateNodeColor_RKey             = "iCS_StateNodeColor_R";
    const string   kStateNodeColor_GKey             = "iCS_StateNodeColor_G";
    const string   kStateNodeColor_BKey             = "iCS_StateNodeColor_B";
    const string   kEntryStateNodeColor_RKey        = "iCS_EntryStateNodeColor_R";
    const string   kEntryStateNodeColor_GKey        = "iCS_EntryStateNodeColor_G";
    const string   kEntryStateNodeColor_BKey        = "iCS_EntryStateNodeColor_B";
    const string   kSelectedNodeBackgroundColor_RKey= "iCS_SelectedNodeBackgroundColor_R";         
    const string   kSelectedNodeBackgroundColor_GKey= "iCS_SelectedNodeBackgroundColor_G";         
    const string   kSelectedNodeBackgroundColor_BKey= "iCS_SelectedNodeBackgroundColor_B";         
    
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
    public static Color PackageNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kPackageNodeColor_RKey, kPackageNodeColor_R);
            float g= EditorPrefs.GetFloat(kPackageNodeColor_GKey, kPackageNodeColor_G);
            float b= EditorPrefs.GetFloat(kPackageNodeColor_BKey, kPackageNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kPackageNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kPackageNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kPackageNodeColor_BKey, value.b);
        }
    }
    public static Color FunctionNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kFunctionNodeColor_RKey, kFunctionNodeColor_R);
            float g= EditorPrefs.GetFloat(kFunctionNodeColor_GKey, kFunctionNodeColor_G);
            float b= EditorPrefs.GetFloat(kFunctionNodeColor_BKey, kFunctionNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kFunctionNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kFunctionNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kFunctionNodeColor_BKey, value.b);
        }
    }
    public static Color ConstructorNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kConstructorNodeColor_RKey, kConstructorNodeColor_R);
            float g= EditorPrefs.GetFloat(kConstructorNodeColor_GKey, kConstructorNodeColor_G);
            float b= EditorPrefs.GetFloat(kConstructorNodeColor_BKey, kConstructorNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kConstructorNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kConstructorNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kConstructorNodeColor_BKey, value.b);
        }
    }
    public static Color InstanceNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kInstanceNodeColor_RKey, kInstanceNodeColor_R);
            float g= EditorPrefs.GetFloat(kInstanceNodeColor_GKey, kInstanceNodeColor_G);
            float b= EditorPrefs.GetFloat(kInstanceNodeColor_BKey, kInstanceNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kInstanceNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kInstanceNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kInstanceNodeColor_BKey, value.b);
        }
    }
    public static Color StateNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kStateNodeColor_RKey, kStateNodeColor_R);
            float g= EditorPrefs.GetFloat(kStateNodeColor_GKey, kStateNodeColor_G);
            float b= EditorPrefs.GetFloat(kStateNodeColor_BKey, kStateNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kStateNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kStateNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kStateNodeColor_BKey, value.b);
        }
    }
    public static Color EntryStateNodeColor {
        get {
            float r= EditorPrefs.GetFloat(kEntryStateNodeColor_RKey, kEntryStateNodeColor_R);
            float g= EditorPrefs.GetFloat(kEntryStateNodeColor_GKey, kEntryStateNodeColor_G);
            float b= EditorPrefs.GetFloat(kEntryStateNodeColor_BKey, kEntryStateNodeColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kEntryStateNodeColor_RKey, value.r);
            EditorPrefs.SetFloat(kEntryStateNodeColor_GKey, value.g);
            EditorPrefs.SetFloat(kEntryStateNodeColor_BKey, value.b);
        }
    }
    public static Color SelectedNodeBackgroundColor {
        get {
            float r= EditorPrefs.GetFloat(kSelectedNodeBackgroundColor_RKey, kSelectedNodeBackgroundColor_R);
            float g= EditorPrefs.GetFloat(kSelectedNodeBackgroundColor_GKey, kSelectedNodeBackgroundColor_G);
            float b= EditorPrefs.GetFloat(kSelectedNodeBackgroundColor_BKey, kSelectedNodeBackgroundColor_B);
            return new Color(r,g,b);
        }
        set {
            EditorPrefs.SetFloat(kSelectedNodeBackgroundColor_RKey, value.r);
            EditorPrefs.SetFloat(kSelectedNodeBackgroundColor_GKey, value.g);
            EditorPrefs.SetFloat(kSelectedNodeBackgroundColor_BKey, value.b);
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
        PackageNodeColor= EditorGUI.ColorField(pos[4], PackageNodeColor);
        FunctionNodeColor= EditorGUI.ColorField(pos[5], FunctionNodeColor);
        ConstructorNodeColor= EditorGUI.ColorField(pos[6], ConstructorNodeColor);
        InstanceNodeColor= EditorGUI.ColorField(pos[7], InstanceNodeColor);
        StateNodeColor= EditorGUI.ColorField(pos[8], StateNodeColor);
        EntryStateNodeColor= EditorGUI.ColorField(pos[9], EntryStateNodeColor);
        SelectedNodeBackgroundColor= EditorGUI.ColorField(pos[10], SelectedNodeBackgroundColor);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            SelectedBrightnessGain= kSelectedBrightnessGain;
            NodeTitleColor= new Color(kNodeTitleColor_R, kNodeTitleColor_G, kNodeTitleColor_B);
            NodeLabelColor= new Color(kNodeLabelColor_R, kNodeLabelColor_G, kNodeLabelColor_B);
            NodeValueColor= new Color(kNodeValueColor_R, kNodeValueColor_G, kNodeValueColor_B);
            PackageNodeColor= new Color(kPackageNodeColor_R, kPackageNodeColor_G, kPackageNodeColor_B);
            FunctionNodeColor= new Color(kFunctionNodeColor_R, kFunctionNodeColor_G, kFunctionNodeColor_B);
            ConstructorNodeColor= new Color(kConstructorNodeColor_R, kConstructorNodeColor_G, kConstructorNodeColor_B);
            InstanceNodeColor= new Color(kInstanceNodeColor_R, kInstanceNodeColor_G, kInstanceNodeColor_B);
            StateNodeColor= new Color(kStateNodeColor_R, kStateNodeColor_G, kStateNodeColor_B);
            EntryStateNodeColor= new Color(kEntryStateNodeColor_R, kEntryStateNodeColor_G, kEntryStateNodeColor_B);
            SelectedNodeBackgroundColor= new Color(kSelectedNodeBackgroundColor_R, kSelectedNodeBackgroundColor_G, kSelectedNodeBackgroundColor_B);
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
