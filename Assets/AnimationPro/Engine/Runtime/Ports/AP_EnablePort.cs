using UnityEngine;
using System.Collections;

public class AP_EnablePort : AP_Port {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	[SerializeField] AP_Port mySource= null;

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_EnablePort CreateInstance(string name, AP_State parent) {
        AP_EnablePort instance= CreateInstance<AP_EnablePort>();
        instance.Init(name, parent);
        return instance;
    }
    protected AP_EnablePort Init(string name, AP_State parent) {
        base.Init(name, parent);
        Edge= EdgeEnum.Top;
        return this;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        if(mySource == null) return true;
        AP_Action sourceAction= mySource.Parent as AP_Action;
        if(sourceAction == null) {
            Debug.LogError("Enable port can only be connected to an Action!!!");
            return true;
        }
        return sourceAction.IsCurrent();
    }
    public override AP_Port GetConnectedPort() {
        return mySource;
    }
}
