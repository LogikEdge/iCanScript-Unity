using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_FunctionBase : iCS_Action, iCS_IParams {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object[]         myParameters;
    protected bool[]           myParameterIsOuts;
    protected int[]            myInIndexes;
    protected int[]            myOutIndexes;
    protected iCS_Connection[] myConnections;
    
    // ======================================================================
    // IParams implementation
    // ----------------------------------------------------------------------
	public string GetParameterName(int idx) { return Name+"["+idx+"]"; }
	public object GetParameter(int idx) {
        return idx < myParameters.Length ? myParameters[idx] : DoGetParameter(idx);		
	}
	public void SetParameter(int idx, object value) {
        if(idx < myParameters.Length)  { myParameters[idx]= value; return; }
        DoSetParameter(idx, value);		
	}
    public bool IsParameterReady(int idx, int frameId) {
        if(idx >= myParameters.Length) return DoIsParameterReady(idx, frameId);
        if(myParameterIsOuts[idx]) return IsCurrent(frameId);
        if(!myConnections[idx].IsConnected) return true;
        return myConnections[idx].IsReady(frameId);
    }
	public void SetParameterConnection(int idx, iCS_Connection connection) {
		myConnections[idx]= connection;
	}
	
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get { return GetParameter(idx); }
        set { SetParameter(idx, value); }
    }
    protected virtual object DoGetParameter(int idx) {
        Debug.LogError("Invalid parameter index given");        
        return null;
    }
    protected virtual void DoSetParameter(int idx, object value) {
        Debug.LogError("Name: "+Name+"=> Invalid parameter index: "+idx+" parameter length: "+myParameters.Length);                
    }
    protected virtual bool DoIsParameterReady(int idx, int frameId) {
        return true;
    }
    public int[] InIndexes  { get { return myInIndexes; }}
    public int[] OutIndexes { get { return myOutIndexes; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FunctionBase(string name, bool[] paramIsOuts, int priority) : base(name, priority) {
        myParameterIsOuts= paramIsOuts;
        List<int> inIdx= new List<int>();
        List<int> outIdx= new List<int>();
        for(int i= 0; i < paramIsOuts.Length; ++i) {
            (paramIsOuts[i] ? outIdx : inIdx).Add(i); 
        }
        myInIndexes = inIdx.ToArray();
        myOutIndexes= outIdx.ToArray();        
        // Allocate parameters & connections
        myParameters= new object[paramIsOuts.Length];
        myConnections= new iCS_Connection[paramIsOuts.Length];
        for(int i= 0; i < myConnections.Length; ++i) myConnections[i]= null;
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
