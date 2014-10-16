using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_ISignature  mySignature    = null;
    int             myParameterIndex    = -1;
    bool            myIsAlwaysReady= false;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_Action Action {
        get { return mySignature != null ? mySignature.GetAction() : null; }
    }
    public iCS_SignatureDataSource Signature {
        get { return mySignature != null ? mySignature.GetSignatureDataSource() : null; }
    }
    public int ParameterIndex {
        get { return myParameterIndex; }
    }
//#if UNITY_EDITOR
    public string PortFullName {
        get {
            var nodeName= Action.FullName;
            var port= Action.GetPortWithIndex(myParameterIndex);
            var portName= port == null ? "["+myParameterIndex+"]" : port.Name;
            return nodeName+"."+portName;            
        }
    }
//#endif
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() { }
    public iCS_Connection(iCS_ISignature signature, int portIndex, bool isAlwaysReady= false, bool isControlFlow= false) {
        mySignature    = signature;
        myParameterIndex    = portIndex;
        myIsAlwaysReady= isAlwaysReady;
    }

    public bool IsConnected             { get{ return mySignature != null; }}
    public object Value                 {
        get { return Signature.GetValue(ParameterIndex); }
        set { Signature.SetValue(ParameterIndex, value); }
    }
    public bool IsReady(int frameId)    {
        if(myIsAlwaysReady || !IsConnected) return true;
        return Action.ArePortsCurrent(frameId);
    }
    public bool IsCurrent(int frameId) {
    	return Action.ArePortsCurrent(frameId);
    }
    public bool DidExecute(int frameId) {
    	return Action.ArePortsExecuted(frameId);
    }
 
 
 }
