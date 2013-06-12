using UnityEngine;
using System.Collections;

public class iCS_Strings {
    // -----------------------------------------------------------------------
    // Common
    public const string InstanceObjectName= "this";
    
    // -----------------------------------------------------------------------
    // Behaviour Allowed Children
    public const string Start       = "Start";
    public const string Update      = "Update";
    public const string LateUpdate  = "LateUpdate";
    public const string FixedUpdate = "FixedUpdate";
    public const string OnGUI       = "OnGUI";
    public const string OnDrawGizmos= "OnDrawGizmos";
    
    // -----------------------------------------------------------------------
    // State Allowed Children
    public const string OnEntry = "OnEntry";
    public const string OnUpdate= "OnUpdate";
    public const string OnExit  = "OnExit";
    
    // -----------------------------------------------------------------------
    // Special node names.
    public const string TransitionEntryModule= "TransitionEntry";
    public const string TransitionExitModule = "TransitionExit";

    // -----------------------------------------------------------------------
    // Special port names.
    public const string EnablePort= "enable";
    
    // -----------------------------------------------------------------------
    // Reflection methods
    public const string AddChildMethod   = "AddChild";
    public const string RemoveChildMethod= "RemoveChild";
    
    // -----------------------------------------------------------------------
    // Gizmos
    public const string GizmoIcon            = "iCanScriptGizmo.png";

    // -----------------------------------------------------------------------
    public static bool IsEmpty(string s)       { return s == null || s ==""; }
    public static bool IsNotEmpty(string s)    { return !IsEmpty(s); }
}
