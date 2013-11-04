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
            if(connection.DidExecute(frameId)) {
                ReturnValue= connection.Value;
                MarkAsExecuted(frameId);
                return;
            }
        }
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        // Take the last that has executed.
		int smallestDistance= 1000;
		iCS_Connection bestConnection= null;
        foreach(var connection in ParameterConnections) {
			if(connection == null) continue;
			var action= connection.Action;
			if(action == null) continue;
			int frameIdDistance= frameId-action.ExecutionFrameId;
			if(frameIdDistance < smallestDistance) {
				smallestDistance= frameIdDistance;
				bestConnection= connection;
			}
        }
		// Take value from the last that executed.
		if(bestConnection != null) {
            ReturnValue= bestConnection.Value;			
		}
        MarkAsCurrent(frameId);
    }
}
