using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_Method : UK_Function {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myThis          = null;
    UK_Connection   myThisConnection= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return (idx == myParameters.Length+1 || idx == myParameters.Length+2) ? myThis : base.DoGetParameter(idx); 
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length+1 || idx == myParameters.Length+2) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        if(idx == myParameters.Length+2) return IsCurrent(frameId);
        if(idx == myParameters.Length+1) {
            return !myThisConnection.IsConnected ? true : myThisConnection.IsReady(frameId);
        }
        return base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Method(string name, MethodInfo methodInfo, object[] portValues, bool[] portIsOuts) : base(name, methodInfo) {
        Init(portValues, portIsOuts);
    }
    protected new void Init(object[] portValues, bool[] portIsOuts) {
        myThis= portValues[portValues.Length-2];
        object[] paramValues= new object[portValues.Length-2];
        Array.Copy(portValues, paramValues, paramValues.Length);
        bool[] paramIsOuts= new bool[portIsOuts.Length-2];
        Array.Copy(portIsOuts, paramIsOuts, paramIsOuts.Length);
        base.Init(paramValues, paramIsOuts);        
    }
    public new void SetConnections(UK_Connection[] connections) {
        myThisConnection= connections[connections.Length-2];
        UK_Connection[] paramConnections= new UK_Connection[connections.Length-2];
        Array.Copy(connections, paramConnections, paramConnections.Length);
        base.SetConnections(paramConnections);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Validate that this is ready.
        if(myThisConnection.IsConnected && !myThisConnection.IsReady(frameId)) return;
        base.Execute(frameId);        
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Fetch this.
        if(myThisConnection.IsConnected) {
            myThis= myThisConnection.Value;
        }
        // Execute function
        if(myThis != null) myReturn= myMethodInfo.Invoke(myThis, myParameters);
        MarkAsCurrent(frameId);
    }
}
