using UnityEngine;
using System.Collections;

public class AP_EnablePort : AP_Port {

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_EnablePort CreateInstance(string name, AP_State parent) {
        AP_EnablePort instance= CreateInstance<AP_EnablePort>();
        instance.Init(name, parent);
        return instance;
    }

}
