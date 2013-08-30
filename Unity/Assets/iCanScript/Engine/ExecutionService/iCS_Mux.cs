using UnityEngine;
using System.Collections;

public class iCS_Mux : iCS_ActionWithSignature {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Mux(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters)
    : base(visualScript, priority, nbOfParameters, 0) {}

    // ======================================================================
    // Execution (not used)
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Take the first valid connection.
        foreach(var connection in ParameterConnections) {
            if(connection.IsReady(frameId)) {
                ReturnValue= connection.Value;
                MarkAsExecuted(frameId);
                return;
            }
        }
	    IsStalled= true;
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
		// Use previous output value.
        MarkAsExecuted(frameId);
    }
}
