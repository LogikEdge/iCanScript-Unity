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
    public WD_Function(string name, MethodInfo methodInfo, object[] parameters) : base(name) {
        myMethodInfo= methodInfo;
        myParameters= parameters;
        myConnections= new WD_Connection[0];
    }
    public void SetConnections(WD_Connection[] connections, object targetObject= null) {
        Connections= connections;
        TargetObject= targetObject;
    }
    public WD_Connection[]  Connections  { get { return myConnections; }  set { myConnections= value; }}
    public object           TargetObject { get { return myTargetObject; } set { myTargetObject= value; }}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Verify that we are ready to run.
        foreach(var c in myConnections) {
            if(c.IsConnected && !c.IsReady(frameId)) return;
        }
        // Fetch all the inputs.
        for(int i= 0; i < myConnections.Length; ++i) {
            if(myConnections[i].IsConnected) myParameters[i]= myConnections[i].Value; 
        }
        // Execute function
        myReturn= myMethodInfo.Invoke(myTargetObject, myParameters);
        MarkAsCurrent(frameId);
    }
}
