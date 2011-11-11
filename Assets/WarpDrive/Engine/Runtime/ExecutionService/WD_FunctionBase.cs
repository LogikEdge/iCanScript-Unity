using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public abstract class WD_FunctionBase : WD_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object[]        myParameters;
    protected int[]           myInIndexes;
    protected int[]           myOutIndexes;
    protected int[]           myParameterFrameIds;
    protected WD_Connection[] myConnections;
    protected object          myReturn;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get {
            if(idx < myParameters.Length)  return myParameters[idx];
            if(idx == myParameters.Length) return myReturn;
            Debug.LogError("Invalid parameter index given");
            return null;
        }
        set {
            if(idx < myParameters.Length)  { myParameters[idx]= value; return; }
            if(idx == myParameters.Length) { myReturn= value; return; }
            Debug.LogError("Invalid parameter index given");            
        }
    }
    public int GetParameterFrameId(int idx) {
        return idx < myParameters.Length ? myParameterFrameIds[idx] : FrameId;
    }
    public bool IsParameterReady(int idx, int frameId) {
        return GetParameterFrameId(idx) == frameId;
    }
    public void SetParameterFrameId(int idx, int frameId) {
        if(idx < myParameters.Length)  myParameterFrameIds[idx]= frameId;        
    }
    public new void MarkAsCurrent(int frameId) {
        foreach(var id in myOutIndexes) myParameterFrameIds[id]= frameId;
        base.MarkAsCurrent(frameId);
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_FunctionBase(string name, object[] parameters, bool[] paramIsOuts) : base(name) {
        myParameters= parameters;
        myParameterFrameIds= new int[parameters.Length];
        myConnections= new WD_Connection[0];
        List<int> inIdx= new List<int>();
        List<int> outIdx= new List<int>();
        for(int i= 0; i < paramIsOuts.Length; ++i) {
            (paramIsOuts[i] ? outIdx : inIdx).Add(i); 
        }
        myInIndexes = inIdx.ToArray();
        myOutIndexes= outIdx.ToArray();
        myReturn= null;
    }
    public void SetConnections(WD_Connection[] connections) {
        myConnections= connections;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Verify that we are ready to run.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected && !myConnections[id].IsReady(frameId)) return;
        }
        // Fetch all the inputs.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected) {
                myParameters[id]= myConnections[id].Value;
            }
            myParameterFrameIds[id]= frameId;
        }
        // Execute function
        DoExecute(frameId);
    }
    protected abstract void DoExecute(int frameId);
}
