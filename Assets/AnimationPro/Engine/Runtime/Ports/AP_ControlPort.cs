using UnityEngine;
using System.Collections;

public sealed class AP_ControlPort : AP_Port {
    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    public new static AP_ControlPort CreateInstance(string portName, AP_Node parent, DirectionEnum direction) {
        AP_ControlPort instance= CreateInstance<AP_ControlPort>();
        instance.Init(portName, parent, direction);
        return instance;
    }
}
