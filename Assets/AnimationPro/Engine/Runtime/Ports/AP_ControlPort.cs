using UnityEngine;
using System.Collections;

public class AP_ControlPort : AP_Port {

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ControlPort CreateInstance(string name, AP_State parent) {
        AP_ControlPort instance= CreateInstance<AP_ControlPort>();
        instance.Init(name, parent);
        return instance;
    }

}
