using UnityEngine;
using System.Collections;

public class WD_Connection {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Prelude.Tuple<WD_Function,int>    myConnection= null;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Connection(WD_Function function, int parameterIndex) {
        myConnection= new Prelude.Tuple<WD_Function,int>(function, parameterIndex);
    }

    public bool IsConnected             { get{ return myConnection.Item1 != null; }}
    public object Value                 {
        get { return myConnection.Item1[myConnection.Item2]; }
        set { myConnection.Item1[myConnection.Item2]= value; }
    }
    public bool IsReady(int frameId)    { return myConnection.Item1.IsCurrent(frameId); }
}
