using UnityEngine;
using System.Collections;

public class UK_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<UK_FunctionBase,int>    myConnection= null;

    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    public static UK_Connection  NoConnection= new UK_Connection(null, -1);
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public UK_FunctionBase Function  { get { return myConnection != null ? myConnection.Item1 : null; }}
    public int             PortIndex { get { return myConnection != null ? myConnection.Item2 : -1; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Connection() {
        myConnection= NoConnection.myConnection;
    }
    public UK_Connection(UK_FunctionBase function, int parameterIndex) {
        myConnection= new Prelude.Tuple<UK_FunctionBase,int>(function, parameterIndex);
    }

    public bool IsConnected             { get{ return myConnection.Item1 != null; }}
    public object Value                 {
        get { return myConnection.Item1[myConnection.Item2]; }
        set { myConnection.Item1[myConnection.Item2]= value; }
    }
    public bool IsReady(int frameId)    { return myConnection.Item1.IsParameterReady(myConnection.Item2, frameId); }
}
