using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<iCS_IParams,int>    myConnection= null;

    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    public static iCS_Connection  NoConnection= new iCS_Connection(null, -1);
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_IParams Function  { get { return myConnection != null ? myConnection.Item1 : null; }}
    public int         PortIndex { get { return myConnection != null ? myConnection.Item2 : -1; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() {
        myConnection= NoConnection.myConnection;
    }
    public iCS_Connection(iCS_IParams function, int parameterIndex) {
        myConnection= new Prelude.Tuple<iCS_IParams,int>(function, parameterIndex);
    }

    public bool IsConnected             { get{ return myConnection.Item1 != null; }}
    public object Value                 {
        get { return myConnection.Item1.GetParameter(myConnection.Item2); }
        set { myConnection.Item1.SetParameter(myConnection.Item2, value); }
    }
    public bool IsReady(int frameId)    { return myConnection.Item1.IsParameterReady(myConnection.Item2, frameId); }
    
    // ----------------------------------------------------------------------
    public override string ToString() {
        return IsConnected ? myConnection.Item1.GetParameterName(myConnection.Item2) : "Not Connected";
    }
}
