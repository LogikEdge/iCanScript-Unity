using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public partial class iCS_VisualScriptData : iCS_IVisualScriptData {
    // =======================================================================
    // Simple Queries
    // -----------------------------------------------------------------------
    // Returns 'true' if the given engine object is valid.
    public static bool IsValid(iCS_EngineObject obj, iCS_IVisualScriptData vsd) {
        int id= obj.InstanceId;
        return id >= 0 && id < vsd.EngineObjects.Count;
    }
    // -----------------------------------------------------------------------
    // Returns 'true' if the given EngineObject passed the test.
    public static bool Test(iCS_EngineObject o, Func<iCS_EngineObject, bool> cond, iCS_IVisualScriptData vsd) {
        return IsValid(o, vsd) && cond(o);
    }
    // ----------------------------------------------------------------------
    // Returns 'true' if the given engine object is a public varaiable.
    public static bool IsPublicVariable(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
        if(obj == null) return false;
        if(obj.ParentId != 0) return false;
        return obj.IsConstructor && vsd.EngineObjects[0].IsBehaviour;
    }
    // ----------------------------------------------------------------------
    // Returns 'true' if the given engine object is a user function.
    public static bool IsPublicFunction(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
        if(obj == null) return false;
        if(obj.ParentId != 0) return false;
        return obj.IsPackage && vsd.EngineObjects[0].IsBehaviour;
    }
    
    // =======================================================================
    // General Iterations
    // -----------------------------------------------------------------------
    // Executes for each valid engine object
    public static void ForEach(Action<iCS_EngineObject> fnc, iCS_IVisualScriptData vsd) {
        P.forEach(o=> { if(IsValid(o, vsd)) fnc(o); }, vsd.EngineObjects);
    }
    // -----------------------------------------------------------------------
    // Invokes the given action if the given condition has passed the test.
    public static void FilterWith(Func<iCS_EngineObject,bool> cond, Action<iCS_EngineObject> action, iCS_IVisualScriptData vsd) {
        ForEach( o=> { if(cond(o)) action(o); }, vsd);
    }
    // -----------------------------------------------------------------------
    // Finds the first engine object that passes the test
    public static iCS_EngineObject Find(iCS_IVisualScriptData vsd, Func<iCS_EngineObject,bool> cond) {
        foreach(var o in vsd.EngineObjects) { if(Test(o, cond, vsd)) return o; }
        return null;
    }
}
