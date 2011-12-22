using UnityEngine;
using System.Collections;

public static class iCS_AllowedChildren {
    public static readonly string[]    Behaviour= null;
    public static readonly string[]    State= null;
    
    static iCS_AllowedChildren() {
        Behaviour= new string[]{
            iCS_EngineStrings.BehaviourChildStart,
            iCS_EngineStrings.BehaviourChildUpdate,
            iCS_EngineStrings.BehaviourChildLateUpdate,
            iCS_EngineStrings.BehaviourChildFixedUpdate,
            iCS_EngineStrings.BehaviourChildOnGUI,
            iCS_EngineStrings.BehaviourChildOnDrawGizmos
        };
        State= new string[]{
            iCS_EngineStrings.StateChildOnEntry,
            iCS_EngineStrings.StateChildOnUpdate,
            iCS_EngineStrings.StateChildOnExit
        };
    }
}
