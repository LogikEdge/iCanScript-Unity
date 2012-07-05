using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable]
//public class iCS_UserPreferences {
//
//    [System.Serializable]
//    public class UserBackgroundGrid {
//        public Color    BackgroundColor= new Color(0.169f,0.188f,0.243f,1.0f);
//        public Color    GridColor= new Color(0.25f,0.25f,0.25f,1.0f);
//        public float    GridSpacing= 20.0f;
//    }
//    public UserBackgroundGrid   Grid= new UserBackgroundGrid();
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
            case 1: Grid(); break;
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
    void Grid() {
        
    }
    // ---------------------------------------------------------------------------------
    void NodeColors() {
        
    }
    // ---------------------------------------------------------------------------------
    void TypeColors() {
        
    }
    // ---------------------------------------------------------------------------------
    void HiddenPrefixes() {
        
    }
    // ---------------------------------------------------------------------------------
    void Icons() {
        
    }
    // ---------------------------------------------------------------------------------
    void InstanceWizard() {
        
    }
}
