using UnityEngine;
using System.Collections;

public class WD_EnablePort : WD_Port {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	[SerializeField] WD_Port mySource= null;

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_EnablePort CreateInstance(string name, WD_State parent) {
        WD_EnablePort instance= CreateInstance<WD_EnablePort>();
        instance.Init(name, parent);
        return instance;
    }
    protected WD_EnablePort Init(string name, WD_State parent) {
        base.Init(name, parent);
        return this;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        if(mySource == null) return true;
        WD_Action sourceAction= mySource.Parent as WD_Action;
        if(sourceAction == null) {
            Debug.LogError("Enable port can only be connected to an Action!!!");
            return true;
        }
        return sourceAction.IsCurrent();
    }
    public override WD_Port GetConnectedPort() {
        return mySource;
    }
}
