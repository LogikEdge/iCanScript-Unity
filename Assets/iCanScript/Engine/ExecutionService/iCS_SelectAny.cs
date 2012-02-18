using UnityEngine;
using System.Collections;

public class iCS_SelectAny : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myReturn      = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == myParameters.Length ? myReturn : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myReturn= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SelectAny(string name, bool[] paramIsOuts, Vector2 layout) : base(name, paramIsOuts, layout) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Verify that we are ready to run.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected && myConnections[id].IsReady(frameId)) {
                myReturn= myConnections[id].Value;
                MarkAsCurrent(frameId);
                return;
            }
        }
        IsStalled= true;
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Use previous output value.
        MarkAsCurrent(frameId);
    }

}
