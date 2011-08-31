using UnityEngine;
using System.Collections;

public class AP_StateEntryPort : AP_Port {

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_StateEntryPort CreateInstance(string name, AP_State parent) {
        AP_StateEntryPort instance= CreateInstance<AP_StateEntryPort>();
        instance.Init(name, parent);
        return instance;
    }
}
