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
    const float  kCanvasBackgroundColor_R = 0.169f;
    const float  kCanvasBackgroundColor_G = 0.188f;
    const float  kCanvasBackgroundColor_B = 0.243f;
    const float  kGridColor_R             = 0.25f;
    const float  kGridColor_G             = 0.25f;
    const float  kGridColor_B             = 0.25f;
    const float  kGridSpacing             = 20.0f;
    const string kCanvasBackgroundColorKey= "iCS_CanvasBackgroundColor";
    const string kGridColorKey            = "iCS_GridColor";
    const string kGridSpacingKey          = "iCS_GridSpacing";
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
    const string   kSelectedNodeBackgroundColorKey= "iCS_SelectedNodeBackgroundColor";         
    // ---------------------------------------------------------------------------------
    // Type Color Constants
    const float  kBoolTypeColor_R      = 1f;
    const float  kBoolTypeColor_G      = 0f;
    const float  kBoolTypeColor_B      = 0f;
    const float  kIntTypeColor_R       = 1f;
    const float  kIntTypeColor_G       = 0f;
    const float  kIntTypeColor_B       = 1f;
    const float  kFloatTypeColor_R     = 0f;
    const float  kFloatTypeColor_G     = 1f;
    const float  kFloatTypeColor_B     = 1f;
    const float  kVector2TypeColor_R   = 1f;
    const float  kVector2TypeColor_G   = 0.92f;
    const float  kVector2TypeColor_B   = 0.016f;
    const float  kVector3TypeColor_R   = 0f;
    const float  kVector3TypeColor_G   = 1f;
    const float  kVector3TypeColor_B   = 0f;
    const float  kVector4TypeColor_R   = 0f;
    const float  kVector4TypeColor_G   = 0f;
    const float  kVector4TypeColor_B   = 1f;
    const float  kStringTypeColor_R    = 1f;
    const float  kStringTypeColor_G    = 0f;
    const float  kStringTypeColor_B    = 0f;
    const float  kGameObjectTypeColor_R= 0f;
    const float  kGameObjectTypeColor_G= 0f;
    const float  kGameObjectTypeColor_B= 1f;
    const float  kDefaultTypeColor_R   = 1f;
    const float  kDefaultTypeColor_G   = 1f;
    const float  kDefaultTypeColor_B   = 1f;
    const string kBoolTypeColorKey      = "iCS_BoolTypeColor";
    const string kIntTypeColorKey       = "iCS_IntTypeColor";
    const string kFloatTypeColorKey     = "iCS_FloatTypeColor";
    const string kVector2TypeColorKey   = "iCS_Vector2TypeColor";
    const string kVector3TypeColorKey   = "iCS_Vector3TypeColor";
    const string kVector4TypeColorKey   = "iCS_Vector4TypeColor";
    const string kStringTypeColorKey    = "iCS_StringTypeColor";
    const string kGameObjectTypeColorKey= "iCS_GameObjectTypeColor";
    const string kDefaultTypeColorKey   = "iCS_DefaultTypeColor";
    
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
            return LoadColor(kCanvasBackgroundColorKey, new Color(kCanvasBackgroundColor_R,kCanvasBackgroundColor_G,kCanvasBackgroundColor_B));
        }
        set {
            SaveColor(kCanvasBackgroundColorKey, value);
        }
    }
    public static Color GridColor {
        get {
            return LoadColor(kGridColorKey, new Color(kGridColor_R,kGridColor_G,kGridColor_B));
        }
        set {
            SaveColor(kGridColorKey, value);
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
            if(value < 0.25f) return;
            EditorPrefs.SetFloat(kSelectedBrightnessGainKey, value);
        }
    }
    public static Color NodeTitleColor {
        get {
            return LoadColor(kNodeTitleColorKey, new Color(kNodeTitleColor_R,kNodeTitleColor_G,kNodeTitleColor_B));
        }
        set {
            SaveColor(kNodeTitleColorKey, value);
        }
    }
    public static Color NodeLabelColor {
        get {
            return LoadColor(kNodeLabelColorKey, new Color(kNodeLabelColor_R,kNodeLabelColor_G,kNodeLabelColor_B));
        }
        set {
            SaveColor(kNodeLabelColorKey, value);
        }
    }
    public static Color NodeValueColor {
        get {
            return LoadColor(kNodeValueColorKey, new Color(kNodeValueColor_R,kNodeValueColor_G,kNodeValueColor_B));
        }
        set {
            SaveColor(kNodeValueColorKey, value);
        }
    }
    public static Color PackageNodeColor {
        get {
            return LoadColor(kPackageNodeColorKey, new Color(kPackageNodeColor_R,kPackageNodeColor_G,kPackageNodeColor_B));
        }
        set {
            SaveColor(kPackageNodeColorKey, value);
        }
    }
    public static Color FunctionNodeColor {
        get {
            return LoadColor(kFunctionNodeColorKey, new Color(kFunctionNodeColor_R,kFunctionNodeColor_G,kFunctionNodeColor_B));
        }
        set {
            SaveColor(kFunctionNodeColorKey, value);
        }
    }
    public static Color ConstructorNodeColor {
        get {
            return LoadColor(kConstructorNodeColorKey, new Color(kConstructorNodeColor_R,kConstructorNodeColor_G,kConstructorNodeColor_B));
        }
        set {
            SaveColor(kConstructorNodeColorKey, value);
        }
    }
    public static Color InstanceNodeColor {
        get {
            return LoadColor(kInstanceNodeColorKey, new Color(kInstanceNodeColor_R,kInstanceNodeColor_G,kInstanceNodeColor_B));
        }
        set {
            SaveColor(kInstanceNodeColorKey, value);
        }
    }
    public static Color StateNodeColor {
        get {
            return LoadColor(kStateNodeColorKey, new Color(kStateNodeColor_R,kStateNodeColor_G,kStateNodeColor_B));
        }
        set {
            SaveColor(kStateNodeColorKey, value);
        }
    }
    public static Color EntryStateNodeColor {
        get {
            return LoadColor(kEntryStateNodeColorKey, new Color(kEntryStateNodeColor_R,kEntryStateNodeColor_G,kEntryStateNodeColor_B));
        }
        set {
            SaveColor(kEntryStateNodeColorKey, value);
        }
    }
    public static Color SelectedNodeBackgroundColor {
        get {
            return LoadColor(kSelectedNodeBackgroundColorKey, new Color(kSelectedNodeBackgroundColor_R,kSelectedNodeBackgroundColor_G,kSelectedNodeBackgroundColor_B));
        }
        set {
            SaveColor(kSelectedNodeBackgroundColorKey, value);
        }
    }
    public static Color BoolTypeColor {
        get {
            return LoadColor(kBoolTypeColorKey, new Color(kBoolTypeColor_R,kBoolTypeColor_G,kBoolTypeColor_B));
        }
        set {
            SaveColor(kBoolTypeColorKey, value);
        }
    }
    public static Color IntTypeColor {
        get {
            return LoadColor(kIntTypeColorKey, new Color(kIntTypeColor_R,kIntTypeColor_G,kIntTypeColor_B));
        }
        set {
            SaveColor(kIntTypeColorKey, value);
        }
    }
    public static Color FloatTypeColor {
        get {
            return LoadColor(kFloatTypeColorKey, new Color(kFloatTypeColor_R,kFloatTypeColor_G,kFloatTypeColor_B));
        }
        set {
            SaveColor(kFloatTypeColorKey, value);
        }
    }
    public static Color StringTypeColor {
        get {
            return LoadColor(kStringTypeColorKey, new Color(kStringTypeColor_R,kStringTypeColor_G,kStringTypeColor_B));
        }
        set {
            SaveColor(kStringTypeColorKey, value);
        }
    }
    public static Color Vector2TypeColor {
        get {
            return LoadColor(kVector2TypeColorKey, new Color(kVector2TypeColor_R,kVector2TypeColor_G,kVector2TypeColor_B));
        }
        set {
            SaveColor(kVector2TypeColorKey, value);
        }
    }
    public static Color Vector3TypeColor {
        get {
            return LoadColor(kVector3TypeColorKey, new Color(kVector3TypeColor_R,kVector3TypeColor_G,kVector3TypeColor_B));
        }
        set {
            SaveColor(kVector3TypeColorKey, value);
        }
    }
    public static Color Vector4TypeColor {
        get {
            return LoadColor(kVector4TypeColorKey, new Color(kVector4TypeColor_R,kVector4TypeColor_G,kVector4TypeColor_B));
        }
        set {
            SaveColor(kVector4TypeColorKey, value);
        }
    }
    public static Color GameObjectTypeColor {
        get {
            return LoadColor(kGameObjectTypeColorKey, new Color(kGameObjectTypeColor_R, kGameObjectTypeColor_G, kGameObjectTypeColor_B));
        }
        set {
            SaveColor(kGameObjectTypeColorKey, value);
        }
    }
    public static Color DefaultTypeColor {
        get {
            return LoadColor(kDefaultTypeColorKey, new Color(kDefaultTypeColor_R, kDefaultTypeColor_G, kDefaultTypeColor_B));
        }
        set {
            SaveColor(kDefaultTypeColorKey, value);
        }
    }
    
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
    public new void OnEnable() {
        base.OnEnable();
        MyWindow.title= "iCanScript Preferences";
        MyWindow.minSize= new Vector2(500f, 425f);
        MyWindow.maxSize= new Vector2(500f, 425f);
    }

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
            BoolTypeColor= new Color(kBoolTypeColor_R, kBoolTypeColor_G, kBoolTypeColor_B);
            IntTypeColor= new Color(kIntTypeColor_R, kIntTypeColor_G, kIntTypeColor_B);
            FloatTypeColor= new Color(kFloatTypeColor_R, kFloatTypeColor_G, kFloatTypeColor_B);
            StringTypeColor= new Color(kStringTypeColor_R, kStringTypeColor_G, kStringTypeColor_B);
            Vector2TypeColor= new Color(kVector2TypeColor_R, kVector2TypeColor_G, kVector2TypeColor_B);
            Vector3TypeColor= new Color(kVector3TypeColor_R, kVector3TypeColor_G, kVector3TypeColor_B);
            Vector4TypeColor= new Color(kVector4TypeColor_R, kVector4TypeColor_G, kVector4TypeColor_B);
            GameObjectTypeColor= new Color(kGameObjectTypeColor_R, kGameObjectTypeColor_G, kGameObjectTypeColor_B);
            DefaultTypeColor= new Color(kDefaultTypeColor_R, kDefaultTypeColor_G, kDefaultTypeColor_B);
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
