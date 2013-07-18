using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_ISignature  mySignature    = null;
    int             myPortIndex    = -1;
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
    public int PortIndex {
        get { return myPortIndex; }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() { }
    public iCS_Connection(iCS_ISignature signature, int portIndex, bool isAlwaysReady= false) {
        mySignature    = signature;
        myPortIndex    = portIndex;
        myIsAlwaysReady= isAlwaysReady;
    }

    public bool IsConnected             { get{ return mySignature != null; }}
    public object Value                 {
        get { return Signature.GetValue(PortIndex); }
        set { Signature.SetValue(PortIndex, value); }
    }
    public bool IsReady(int frameId)    {
        if(myIsAlwaysReady || !IsConnected) return true;
        return Action.IsCurrent(frameId);
    }
    
    // ----------------------------------------------------------------------
    public override string ToString() {
        return IsConnected ? Signature.GetName(PortIndex) : "Not Connected";
    }
}
