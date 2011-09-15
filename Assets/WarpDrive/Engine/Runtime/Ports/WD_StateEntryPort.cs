using UnityEngine;
using System.Collections;

public class WD_StateEntryPort : WD_Port {

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_StateEntryPort CreateInstance(string name, WD_State parent) {
        WD_StateEntryPort instance= CreateInstance<WD_StateEntryPort>();
        instance.Init(name, parent);
        return instance;
    }
}
