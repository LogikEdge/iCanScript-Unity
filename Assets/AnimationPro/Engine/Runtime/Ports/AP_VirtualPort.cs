using UnityEngine;
using System.Collections;

public sealed class AP_VirtualPort : AP_Port {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_Port ConcretePort= null;

    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    public new static AP_VirtualPort CreateInstance(string portName, AP_Node parent, DirectionEnum direction) {
        AP_VirtualPort instance= CreateInstance<AP_VirtualPort>();
        instance.Init(portName, parent, direction);
        return instance;
    }

    public override void UpdateValue() {}

}
