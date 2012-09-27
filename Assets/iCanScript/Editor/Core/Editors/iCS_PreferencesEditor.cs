using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


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
    const float  kAnimationTime            = 0.35f;
    const float  kScrollSpeed              = 3.0f;
    const float  kEdgeScrollSpeed          = 400.0f;
    const bool   kInverseZoom              = false;
    const string kAnimationTimeKey         = "iCS_AnimationTime";
    const string kScrollSpeedKey           = "iCS_ScrollSpeed";
    const string kEdgeScrollSpeedKey       = "iCS_EdgeScrollSpeed";
    const string kInverseZoomKey           = "iCS_InverseZoom";
    const bool   kShowRuntimePortValue     = false;
    const float  kPortValueRefreshPeriod   = 0.1f;
    const string kShowRuntimePortValueKey  = "iCS_ShowRuntimePortValue";
    const string kPortValueRefreshPeriodKey= "iCS_PortValueRefresh";
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
    const bool   kInstanceAutocreateInThis             = true;
    const bool   kInstanceAutocreateOutThis            = false;
    const bool   kInstanceAutocreateInFields           = false;
    const bool   kInstanceAutocreateOutFields          = false;
    const bool   kInstanceAutocreateInStaticFields     = false;
    const bool   kInstanceAutocreateOutStaticFields    = false;
    const bool   kInstanceAutocreateInProperties       = false;
    const bool   kInstanceAutocreateOutProperties      = false;
    const bool   kInstanceAutocreateInStaticProperties = false;
    const bool   kInstanceAutocreateOutStaticProperties= false;
    const string kInstanceAutocreateInThisKey             = "iCS_InstanceAutocreateInThis";
    const string kInstanceAutocreateOutThisKey            = "iCS_InstanceAutocreateOutThis"; 
    const string kInstanceAutocreateInFieldsKey           = "iCS_InstanceAutocreateInFields"; 
    const string kInstanceAutocreateOutFieldsKey          = "iCS_InstanceAutocreateOutFields"; 
    const string kInstanceAutocreateInStaticFieldsKey     = "iCS_InstanceAutocreateInStaticFields";
    const string kInstanceAutocreateOutStaticFieldsKey    = "iCS_InstanceAutocreateOutStaticFields";
    const string kInstanceAutocreateInPropertiesKey       = "iCS_InstanceAutocreateInProperties";
    const string kInstanceAutocreateOutPropertiesKey      = "iCS_InstanceAutocreateOutProperties"; 
    const string kInstanceAutocreateInStaticPropertiesKey = "iCS_InstanceAutocreateInStaticProperties";
    const string kInstanceAutocreateOutStaticPropertiesKey= "iCS_InstanceAutocreateOutStaticProperties";

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	int         selGridId= 0;
	string[]    selGridStrings= new string[]{"Display Options", "Canvas", "Node Colors", "Type Colors", "Instance Wizard"};
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
    public static bool InstanceAutocreateOutThis {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutThisKey, kInstanceAutocreateOutThis); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutThisKey, value); }        
    }
    public static bool InstanceAutocreateInFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInFieldsKey, kInstanceAutocreateInFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInFieldsKey, value); }        
    }
    public static bool InstanceAutocreateOutFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutFieldsKey, kInstanceAutocreateOutFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutFieldsKey, value); }        
    }
    public static bool InstanceAutocreateInStaticFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInStaticFieldsKey, kInstanceAutocreateInStaticFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInStaticFieldsKey, value); }        
    }
    public static bool InstanceAutocreateOutStaticFields {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutStaticFieldsKey, kInstanceAutocreateOutStaticFields); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutStaticFieldsKey, value); }        
    }
    public static bool InstanceAutocreateInProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInPropertiesKey, kInstanceAutocreateInProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateOutProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutPropertiesKey, kInstanceAutocreateOutProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateInStaticProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateInStaticPropertiesKey, kInstanceAutocreateInStaticProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateInStaticPropertiesKey, value); }        
    }
    public static bool InstanceAutocreateOutStaticProperties {
        get { return EditorPrefs.GetBool(kInstanceAutocreateOutStaticPropertiesKey, kInstanceAutocreateOutStaticProperties); }
        set { EditorPrefs.SetBool(kInstanceAutocreateOutStaticPropertiesKey, value); }        
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
    static iCS_PreferencesEditor() {
        // Canvas colors
        kCanvasBackgroundColor= new Color(0.169f, 0.188f, 0.243f);
        kGridColor            = new Color(0.25f, 0.25f, 0.25f);
        
        // Node colors
        kNodeTitleColor             = Color.black;
        kNodeLabelColor             = Color.white;
        kNodeValueColor             = new Color(1f, 0.8f, 0.4f);
        kEntryStateNodeColor        = new Color(1f, 0.5f, 0.25f);
        kStateNodeColor             = Color.cyan;
        kPackageNodeColor           = Color.yellow;
        kInstanceNodeColor          = new Color(1f, 0.5f, 0f);
        kConstructorNodeColor       = new Color(1f, 0.25f, 0.5f);
        kFunctionNodeColor          = Color.green;
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
            case 4: InstanceWizard(); break;
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
        GUI.Label(pos[0], "Animation Time (seconds)");
        GUI.Label(pos[1], "Scroll Speed");
        GUI.Label(pos[2], "Edge Scroll Speed (pixels)");
        GUI.Label(pos[3], "Inverse Zoom");
        GUI.Label(pos[4], "Show Runtime Values");
        GUI.Label(pos[5], "Refresh Period (seconds)");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
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
        SelectedNodeBackgroundColor= EditorGUI.ColorField(pos[10], SelectedNodeBackgroundColor);
        
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
        InstanceAutocreateInThis            = EditorGUI.Toggle(pos[0], InstanceAutocreateInThis);
        InstanceAutocreateInFields          = EditorGUI.Toggle(pos[1], InstanceAutocreateInFields);
        InstanceAutocreateInStaticFields    = EditorGUI.Toggle(pos[2], InstanceAutocreateInStaticFields);
        InstanceAutocreateInProperties      = EditorGUI.Toggle(pos[3], InstanceAutocreateInProperties);
        InstanceAutocreateInStaticProperties= EditorGUI.Toggle(pos[4], InstanceAutocreateInStaticProperties);
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= 45f;
        }
        InstanceAutocreateOutThis            = EditorGUI.Toggle(pos[0], InstanceAutocreateOutThis);
        InstanceAutocreateOutFields          = EditorGUI.Toggle(pos[1], InstanceAutocreateOutFields);
        InstanceAutocreateOutStaticFields    = EditorGUI.Toggle(pos[2], InstanceAutocreateOutStaticFields);
        InstanceAutocreateOutProperties      = EditorGUI.Toggle(pos[3], InstanceAutocreateOutProperties);
        InstanceAutocreateOutStaticProperties= EditorGUI.Toggle(pos[4], InstanceAutocreateOutStaticProperties);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            InstanceAutocreateInThis            = kInstanceAutocreateInThis;
            InstanceAutocreateInFields          = kInstanceAutocreateInFields;
            InstanceAutocreateInStaticFields    = kInstanceAutocreateInStaticFields;
            InstanceAutocreateInProperties      = kInstanceAutocreateInProperties;
            InstanceAutocreateInStaticProperties= kInstanceAutocreateInStaticProperties;
            InstanceAutocreateOutThis            = kInstanceAutocreateOutThis;
            InstanceAutocreateOutFields          = kInstanceAutocreateOutFields;
            InstanceAutocreateOutStaticFields    = kInstanceAutocreateOutStaticFields;
            InstanceAutocreateOutProperties      = kInstanceAutocreateOutProperties;
            InstanceAutocreateOutStaticProperties= kInstanceAutocreateOutStaticProperties;
        }        
    }

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
}
