using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_Function : UK_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myReturn      = null;
    protected MethodInfo    myMethodInfo  = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == myParameters.Length ? myReturn : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myReturn= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Function(string name, MethodInfo methodInfo, object[] portValues, bool[] portIsOuts) : base(name) {
        myMethodInfo= methodInfo;
        Init(portValues, portIsOuts);
    }
    public UK_Function(string name, MethodInfo methodInfo) : base(name) {
        myMethodInfo= methodInfo;
    }
    protected new void Init(object[] portValues, bool[] portIsOuts) {
        object[] parameters= new object[portValues.Length-1];
        Array.Copy(portValues, parameters, parameters.Length);
        bool[] paramIsOuts= new bool[portIsOuts.Length-1];
        Array.Copy(portIsOuts, paramIsOuts, paramIsOuts.Length);
        base.Init(parameters, paramIsOuts);
    }
    public new void SetConnections(UK_Connection[] connections) {
        UK_Connection[] paramConnections= new UK_Connection[connections.Length-1];
        Array.Copy(connections, paramConnections, paramConnections.Length);
        base.SetConnections(paramConnections);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myReturn= myMethodInfo.Invoke(null, myParameters);            
        MarkAsCurrent(frameId);
    }
}
