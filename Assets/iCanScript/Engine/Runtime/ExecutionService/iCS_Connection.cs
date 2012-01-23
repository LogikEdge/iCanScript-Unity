using UnityEngine;
using System.Collections;

public class iCS_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<iCS_FunctionBase,int>    myConnection= null;

    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    public static iCS_Connection  NoConnection= new iCS_Connection(null, -1);
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_FunctionBase Function  { get { return myConnection != null ? myConnection.Item1 : null; }}
    public int             PortIndex { get { return myConnection != null ? myConnection.Item2 : -1; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Connection() {
        myConnection= NoConnection.myConnection;
    }
    public iCS_Connection(iCS_FunctionBase function, int parameterIndex) {
        myConnection= new Prelude.Tuple<iCS_FunctionBase,int>(function, parameterIndex);
    }

    public bool IsConnected             { get{ return myConnection.Item1 != null; }}
    public object Value                 {
        get { return myConnection.Item1[myConnection.Item2]; }
        set { myConnection.Item1[myConnection.Item2]= value; }
    }
    public bool IsReady(int frameId)    { return myConnection.Item1.IsParameterReady(myConnection.Item2, frameId); }
    
    // ----------------------------------------------------------------------
    public override string ToString() {
        return IsConnected ? myConnection.Item1.Name+"["+myConnection.Item2+"]" : "Not Connected";
    }
}
