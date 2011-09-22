using UnityEngine;
using System.Collections;

public abstract class WD_Port : WD_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public virtual bool     IsReady()           { return true; }
    public virtual WD_Port  GetConnectedPort()  { return null; }
    
}
