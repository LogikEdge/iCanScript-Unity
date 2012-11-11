using UnityEngine;
using System.Collections;

public static class iCS_Config {
    // ----------------------------------------------------------------------
    public const string Version= "Version 0.8.6";
    public const string VersionLabel= "(Beta #6)";
    public const string ProductName= "iCanScript";
    public const string ProductAcronym= "iCS";
	public const string EditorPath= "Assets/"+ProductName+"/Editor";
	public const string EnginePath= "Assets/"+ProductName+"/Engine";
	public const string GizmosPath= EnginePath+"/Gizmos";
	public const string ResourcePath= EditorPath + "/Resources";
	public const string GuiAssetPath= ResourcePath;
	public const string ProductPrefix= ProductAcronym+"_";
	public const char   PrivateStringPrefix= '$';

//#if UNITY_EDITOR
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// BEGIN EDITOR SECTION
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    // ----------------------------------------------------------------------
    // Initial values that MUST NOT DEPEND ON ANY SERVICES
    public const float  NodeInitialOffset  = 15.0f;
    public const float  NodeInitialWidth   = 30.0f;
    public const float  NodeInitialHeight  = 30.0f;
    public const float  ExtraIconWidth     = 12.0f;
    
    // ----------------------------------------------------------------------
    public static GUIStyle NodeStyle        { get { return GUI.skin.button; }}
    public static GUIStyle PortLabelStyle   { get { return GUI.skin.label; }}
    
    // ----------------------------------------------------------------------
    public const  float MarginSize = 15.0f;
    public const  float PaddingSize= 15.0f;
    public static float ButtonHeight      { get { return GetButtonHeight("A"); }}
    public static float NodeTitleHeight   { get { return GetNodeHeight("A"); }}
    public static float MinimumNodeHeight { get { return 2.0f*NodeTitleHeight; }}

    // ----------------------------------------------------------------------
    public const float EditorWindowMarginSize = MarginSize;
    public const float EditorWindowPaddingSize= PaddingSize;
    public const float EditorWindowToolbarHeight= 16.0f;
    public const float EditorWindowMinX= EditorWindowMarginSize;
    public const float EditorWindowMinY= EditorWindowMarginSize + EditorWindowToolbarHeight;
    
    // ----------------------------------------------------------------------
    public static float ScrollBarWidth { get { return ButtonHeight; }}

    // ----------------------------------------------------------------------
    public const  float PortRadius= 3.0f;
    public const  float PortSize= 2.0f * PortRadius;
    public static float MinimumPortSeparation { get { return GetPortLabelHeight("A"); }}

    // ----------------------------------------------------------------------
	public const float NodeShadowSize= 5.0f;
	
    // ======================================================================
    // Node info.
    public static Vector2 GetNodeSize(string _label) {
        return NodeStyle.CalcSize(new GUIContent(_label));
    }
    public static float GetNodeWidth(string _label) {
        return GetNodeSize(_label).x;
    }
    public static float GetNodeHeight(string _label) {
        return GetNodeSize(_label).y;
    }
    
    // ======================================================================
    // Port info.
    public static Vector2 GetPortLabelSize(string _label) {
        return PortLabelStyle.CalcSize(new GUIContent(_label));
    }
    public static float GetPortLabelWidth(string _label) {
        return GetPortLabelSize(_label).x;
    }
    public static float GetPortLabelHeight(string _label) {
        return GetPortLabelSize(_label).y;
    }
    
    // ======================================================================
    public static Vector2 GetLabelSize(string _label) {
        return GUI.skin.label.CalcSize(new GUIContent(_label));
    }
    public static float GetLabelWidth(string _label) {
        return GetLabelSize(_label).x;
    }
    public static float GetLabelHeight(string _label) {
        return GetLabelSize(_label).y;
    }

    // ======================================================================
    public static Vector2 GetButtonSize(string _label) {
        return GUI.skin.button.CalcSize(new GUIContent(_label));
    }
    public static float GetButtonWidth(string _label) {
        return GetButtonSize(_label).x;
    }
    public static float GetButtonHeight(string _label) {
        return GetButtonSize(_label).y;
    }

    // ======================================================================
    public static Vector2 GetWindowSize(string _label) {
        return GUI.skin.button.CalcSize(new GUIContent(_label));
    }
    public static float GetWindowWidth(string _label) {
        return GetButtonSize(_label).x;
    }
    public static float GetWindowHeight(string _label) {
        return GetButtonSize(_label).y;
    }
//#endif
}
