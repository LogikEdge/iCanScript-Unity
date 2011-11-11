using UnityEngine;
using System.Collections;

public class WD_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<WD_FunctionBase,int>    myConnection= null;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public WD_FunctionBase Function  { get { return myConnection != null ? myConnection.Item1 : null; }}
    public int             PortIndex { get { return myConnection != null ? myConnection.Item2 : -1; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Connection(WD_Function function, int parameterIndex) {
        myConnection= new Prelude.Tuple<WD_FunctionBase,int>(function, parameterIndex);
    }

    public bool IsConnected             { get{ return myConnection.Item1 != null; }}
    public object Value                 {
        get { return myConnection.Item1[myConnection.Item2]; }
        set { myConnection.Item1[myConnection.Item2]= value; }
    }
    public bool IsReady(int frameId)    { return myConnection.Item1.IsCurrent(frameId); }
}
