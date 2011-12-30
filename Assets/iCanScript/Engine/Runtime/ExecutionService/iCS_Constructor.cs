using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Constructor : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myThis      = null;
    protected MethodBase    myMethodBase= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == myParameters.Length ? myThis : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(string name, MethodBase methodBase, bool[] portIsOuts, Vector2 layout) : base(name, layout) {
        myMethodBase= methodBase;
        Init(portIsOuts);
    }
    public iCS_Constructor(string name, MethodBase methodBase, Vector2 layout) : base(name, layout) {
        myMethodBase= methodBase;
    }
    protected new void Init(bool[] portIsOuts) {
        bool[] paramIsOuts= new bool[portIsOuts.Length-1];
        Array.Copy(portIsOuts, paramIsOuts, paramIsOuts.Length);
        base.Init(paramIsOuts);
    }
    public new void SetConnections(iCS_Connection[] connections, object[] initValues) {
        iCS_Connection[] baseConnections= new iCS_Connection[connections.Length-1];
        Array.Copy(connections, baseConnections, baseConnections.Length);
        object[] baseInitValues= new object[initValues.Length-1];
        Array.Copy(initValues, baseInitValues, baseInitValues.Length);
        base.SetConnections(baseConnections, baseInitValues);
    }
    public new void SetConnection(int id, iCS_Connection connection) {
        if(id < myParameters.Length) base.SetConnection(id, connection);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        if(myThis == null) {
            myThis= myMethodBase.Invoke(null, myParameters);            
        }
        MarkAsCurrent(frameId);
    }
}
