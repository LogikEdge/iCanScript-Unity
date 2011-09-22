using UnityEngine;
using System.Collections;

public sealed class WD_VirtualDataPort : WD_DataPort {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_DataPort ConcretePort= null;

    // ======================================================================
    // Lifetime Management
    // ----------------------------------------------------------------------
    public static WD_VirtualDataPort CreateInstance(string portName, WD_Node parent) {
        WD_VirtualDataPort instance= CreateInstance<WD_VirtualDataPort>();
        instance.Init(portName, parent);
        return instance;
    }

    public override void UpdateValue() {}

}
