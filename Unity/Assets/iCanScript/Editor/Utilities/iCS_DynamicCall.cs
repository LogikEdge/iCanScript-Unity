using UnityEngine;
using System.Collections;

/// This class is used to dynamically find and invoke
/// functionality that is not directly reachable from
/// the current assembly.
public static class iCS_DynamicCall {
    // ----------------------------------------------------------------------
    /// Adds a viusal script on the givne GameObject.
    /// @param go   The GameObject on which to add a new viusal script.
    public static void AddVisualScript(GameObject go) {
        if(myAddVisualScriptFnc == null) {
            myAddVisualScriptFnc= new DynamicInvoke("iCS_MenuInterface", "AddVisualScript");
        }
        myAddVisualScriptFnc.Invoke(null, go);
    }
    static DynamicInvoke    myAddVisualScriptFnc= null;
    // ----------------------------------------------------------------------
    /// Adds an iCS behaviour component on the givne GameObject.
    /// @param go   The GameObject on which to add a new iCS behaviour.
    public static void AddBehaviour(GameObject go) {
        if(myAddBehaviourFnc == null) {
            myAddBehaviourFnc= new DynamicInvoke("iCS_MenuInterface", "AddBehaviour");
        }
        myAddBehaviourFnc.Invoke(null, go);
    }
    static DynamicInvoke    myAddBehaviourFnc= null;
}
