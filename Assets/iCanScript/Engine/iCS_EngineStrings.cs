using UnityEngine;
using System.Collections;

public class iCS_EngineStrings {
    // Behaviour Allowed Children
    public const string BehaviourChildStart       = "Start";
    public const string BehaviourChildUpdate      = "Update";
    public const string BehaviourChildLateUpdate  = "LateUpdate";
    public const string BehaviourChildFixedUpdate = "FixedUpdate";
    public const string BehaviourChildOnGUI       = "OnGUI";
    public const string BehaviourChildOnDrawGizmos= "OnDrawGizmos";
    
    // State Allowed Children
    public const string StateChildOnEntry = "OnEntry";
    public const string StateChildOnUpdate= "OnUpdate";
    public const string StateChildOnExit  = "OnExit";
    
    // Special node names.
    public const string TransitionEntryModule= "TransitionEntry";
    public const string TransitionExitModule = "TransitionExit";

    // Special port names.
    public const string EnablePort= "enable";
    
    // Reflection methods
    public const string AddChildMethod   = "AddChild";
    public const string RemoveChildMethod= "RemoveChild";

}
