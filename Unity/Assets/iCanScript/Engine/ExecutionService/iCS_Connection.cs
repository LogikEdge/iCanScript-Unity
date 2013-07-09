using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<iCS_IParameters,int>    myConnection= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_IParameters Function  { get { return myConnection != null ? myConnection.Item1 : null; }}
    public int         PortIndex { get { return myConnection != null ? myConnection.Item2 : -1; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() {
        myConnection= null;
    }
    public iCS_Connection(iCS_IParameters function, int parameterIndex) {
        if(function == null) {
            myConnection= null;
        } else {
            myConnection= new Prelude.Tuple<iCS_IParameters,int>(function, parameterIndex);            
        }
    }

    public bool IsConnected             { get{ return myConnection != null; }}
    public object Value                 {
        get { return myConnection.Item1.GetParameter(myConnection.Item2); }
        set { myConnection.Item1.SetParameter(myConnection.Item2, value); }
    }
    public bool IsReady(int frameId)    {
        if(!IsConnected) return true;
        return myConnection.Item1.IsParameterReady(myConnection.Item2, frameId);
    }
    
    // ----------------------------------------------------------------------
    public override string ToString() {
        return IsConnected ? myConnection.Item1.GetParameterName(myConnection.Item2) : "Not Connected";
    }
}
