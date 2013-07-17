using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<iCS_ISignature,int>    myConnection= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_Action Action {
        get { return myConnection != null ? myConnection.Item1.GetAction() : null; }
    }
    public iCS_SignatureDataSource Signature {
        get { return myConnection != null ? myConnection.Item1.GetSignatureDataSource() : null; }
    }
    public int PortIndex {
        get { return myConnection != null ? myConnection.Item2 : -1; }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() { myConnection= null; }
    public iCS_Connection(iCS_ISignature signature, int parameterIndex) {
        if(signature == null) {
            myConnection= null;
        } else {
            myConnection= new Prelude.Tuple<iCS_ISignature,int>(signature, parameterIndex);            
        }
    }

    public bool IsConnected             { get{ return myConnection != null; }}
    public object Value                 {
        get { return Signature.GetValue(PortIndex); }
        set { Signature.SetValue(PortIndex, value); }
    }
    public bool IsReady(int frameId)    {
        if(!IsConnected) return true;
        return Action.DidExecute(frameId);
    }
    
    // ----------------------------------------------------------------------
    public override string ToString() {
        return IsConnected ? Signature.GetName(PortIndex) : "Not Connected";
    }
}
