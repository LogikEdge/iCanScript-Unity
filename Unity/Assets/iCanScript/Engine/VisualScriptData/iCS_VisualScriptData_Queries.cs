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
    public static bool IsPublicVariable(iCS_EngineObject obj) {
        if(obj == null) return false;
        if(obj.ParentId != 0) return false;
        return obj.IsConstructor;
    }
    // ----------------------------------------------------------------------
    // Returns 'true' if the given engine object is a user function.
    public static bool IsUserFunction(iCS_EngineObject obj) {
        if(obj == null) return false;
        if(obj.ParentId != 0) return false;
        return obj.IsPackage;
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
    // -----------------------------------------------------------------------
    // Finds all engine objects that passes the test
    public static iCS_EngineObject[] FindMany(iCS_IVisualScriptData vsd, Func<iCS_EngineObject,bool> cond) {
        List<iCS_EngineObject> result= new List<iCS_EngineObject>();
        foreach(var o in vsd.EngineObjects) { if(Test(o, cond, vsd)) result.Add(o); }
        return result.ToArray();
    }

    // =======================================================================
    // Searching Queries
    // -----------------------------------------------------------------------
    // Finds all the message handlers
    public static iCS_EngineObject[] FindMessageHandlers(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, o=> o.IsMessage);
    }
    // -----------------------------------------------------------------------
    // Finds all the user functions
    public static iCS_EngineObject[] FindUserFunctionCalls(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, o=> o.IsUserFunctionCall);
    }
    // -----------------------------------------------------------------------
    // Finds all the variable proxies
    public static iCS_EngineObject[] FindVariableProxies(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, o=> o.IsVariableProxy);
    }
    // -----------------------------------------------------------------------
    // Finds all the constructors
    public static iCS_EngineObject[] FindConstructors(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, o=> o.IsConstructor);
    }
    // -----------------------------------------------------------------------
    // Finds all user functions
    public static iCS_EngineObject[] FindUserFunctions(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, IsUserFunction);
    }
    // -----------------------------------------------------------------------
    // Finds all user functions
    public static iCS_EngineObject[] FindPublicVariables(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, IsPublicVariable);
    }
    // -----------------------------------------------------------------------
    // Find all public objects
    public static iCS_EngineObject[] FindPublicObjects(iCS_IVisualScriptData vsd) {
        return FindMany(vsd, o=> o.ParentId == 0);
    }
}
