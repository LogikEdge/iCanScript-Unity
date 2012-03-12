using UnityEngine;
using System.Collections;

public class iCS_MuxPort : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override bool DoIsParameterReady(int idx, int frameId) {
		if(idx != 0) return base.DoIsParameterReady(idx, frameId);
		if(IsCurrent(frameId)) return true;
		return ReadAnyInput(frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MuxPort(string name, bool[] paramIsOuts, Vector2 layout) : base(name, paramIsOuts, layout) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
		if(!ReadAnyInput(frameId)) {
	        IsStalled= true;
		}
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
		// Use previous output value.
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
	bool ReadAnyInput(int frameId) {
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected && myConnections[id].IsReady(frameId)) {
                myParameters[0]= myConnections[id].Value;
                MarkAsCurrent(frameId);
                return true;
            }
        }
		return false;		
	}
}
