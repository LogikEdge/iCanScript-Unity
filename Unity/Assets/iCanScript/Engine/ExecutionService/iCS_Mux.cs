using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_Mux : SSNodeAction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Mux(string name, SSObject parent, int priority, int nbOfParameters)
    : base(name, parent, priority, nbOfParameters, 0) {}

    // ======================================================================
    // Execution (not used)
    // ----------------------------------------------------------------------
    // FIXME: Mux should prefer running over current frame nodes.
    protected override void DoEvaluate() {
        // Take the first valid connection.
        foreach(var connection in ParameterConnections) {
            if(connection.IsExecuted) {
                ReturnValue= connection.Value;
                MarkAsExecuted();
                return;
            }
        }
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
        // Take the last that has executed.
		int smallestDistance= 100000;
		SSBinding bestConnection= null;
        foreach(var connection in ParameterConnections) {
			if(connection == null) continue;
			var action= connection.Action;
			if(action == null) continue;
			int runIdDistance= Context.RunId-action.ExecutedRunId;
			if(runIdDistance < smallestDistance) {
				smallestDistance= runIdDistance;
				bestConnection= connection;
			}
        }
		// Take value from the last that executed.
		if(bestConnection != null) {
            ReturnValue= bestConnection.Value;			
		}
        MarkAsEvaluated();
    }
}
