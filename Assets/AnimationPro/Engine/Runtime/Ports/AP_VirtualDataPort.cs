using UnityEngine;
using System.Collections;

public sealed class AP_VirtualDataPort : AP_DataPort {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_DataPort ConcretePort= null;

    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    public static AP_VirtualDataPort CreateInstance(string portName, AP_Node parent, DirectionEnum direction) {
        AP_VirtualDataPort instance= CreateInstance<AP_VirtualDataPort>();
        instance.Init(portName, parent, direction);
        return instance;
    }

    public override void UpdateValue() {}

}
