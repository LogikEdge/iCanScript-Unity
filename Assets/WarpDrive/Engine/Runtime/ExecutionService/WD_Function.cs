using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_Function : WD_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myTargetObject= null;
    MethodInfo      myMethodInfo  = null;
    object[]        myParameters  = null;
    object          myReturn      = null;
    WD_Connection[] myConnections = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get {
            if(idx < myParameters.Length) return myParameters[idx];
            if(idx == myParameters.Length) return myReturn;
            Debug.LogError("Invalid parameter index given");
            return null;
        }
        set {
            if(idx < myParameters.Length) { myParameters[idx]= value; return; }
            Debug.LogError("Invalid parameter index given");            
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Function(MethodInfo methodInfo, WD_Connection[] connections, object obj= null) {
        myTargetObject= obj;
        myMethodInfo= methodInfo;
        myParameters= new object[methodInfo.GetParameters().Length];
        myConnections= connections;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        foreach(var c in myConnections) {
            if(c.IsConnected && !c.IsReady(frameId)) return;
        }
        for(int i= 0; i < myConnections.Length; ++i) {
            if(myConnections[i].IsConnected) myParameters[i]= myConnections[i].Value; 
        }
        myReturn= myMethodInfo.Invoke(myTargetObject, myParameters);
        MarkAsCurrent(frameId);
    }
}
