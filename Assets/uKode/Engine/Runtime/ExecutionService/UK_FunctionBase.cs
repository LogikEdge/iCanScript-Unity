using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class UK_FunctionBase : UK_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object[]        myParameters;
    protected bool[]          myParameterIsOuts;
    protected int[]           myInIndexes;
    protected int[]           myOutIndexes;
    protected UK_Connection[] myConnections;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get {
            return idx < myParameters.Length ? myParameters[idx] : DoGetParameter(idx);
        }
        set {
            if(idx < myParameters.Length)  { myParameters[idx]= value; return; }
            DoSetParameter(idx, value);
        }
    }
    protected virtual object DoGetParameter(int idx) {
        Debug.LogError("Invalid parameter index given");        
        return null;
    }
    protected virtual void DoSetParameter(int idx, object value) {
        Debug.LogError("Invalid parameter index given");                
    }
    public bool IsParameterReady(int idx, int frameId) {
        if(idx >= myParameters.Length) return DoIsParameterReady(idx, frameId);
        if(myParameterIsOuts[idx]) return IsCurrent(frameId);
        if(!myConnections[idx].IsConnected) return true;
        return myConnections[idx].IsReady(frameId);
    }
    protected virtual bool DoIsParameterReady(int idx, int frameId) {
        return true;
    }
    public int[] InIndexes  { get { return myInIndexes; }}
    public int[] OutIndexes { get { return myOutIndexes; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_FunctionBase(string name, object[] parameters, bool[] paramIsOuts, Vector2 layout) : base(name, layout) {
        Init(parameters, paramIsOuts);
    }
    public UK_FunctionBase(string name, Vector2 layout) : base(name,layout) {}
    protected void Init(object[] parameters, bool[] paramIsOuts) {
        myParameters= parameters ?? new object[0];
        myParameterIsOuts= paramIsOuts ?? new bool[0];
        myConnections= new UK_Connection[0];
        List<int> inIdx= new List<int>();
        List<int> outIdx= new List<int>();
        for(int i= 0; i < paramIsOuts.Length; ++i) {
            (paramIsOuts[i] ? outIdx : inIdx).Add(i); 
        }
        myInIndexes = inIdx.ToArray();
        myOutIndexes= outIdx.ToArray();        
    }
    public void SetConnections(UK_Connection[] connections) {
        myConnections= connections;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Verify that we are ready to run.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected && !myConnections[id].IsReady(frameId)) {
                IsStalled= true;
                return;
            }
        }
        ForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Fetch all the inputs.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected) {
                myParameters[id]= myConnections[id].Value;
            }
        }
        // Execute function
        DoExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected virtual void DoExecute(int frameId) {}
}
