using UnityEngine;
using System.Collections;

public class iCS_Mux : iCS_ActionWithSignature {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Mux(iCS_Storage storage, int instanceId, int priority, int nbOfParameters)
    : base(storage, instanceId, priority, nbOfParameters) {}

    // ======================================================================
    // Execution (not used)
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
		if(!ReadAnyInput(frameId)) {
	        IsStalled= true;
		}
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
		// Use previous output value.
        MarkAsExecuted(frameId);
    }
    // ----------------------------------------------------------------------
	bool ReadAnyInput(int frameId) {
        return !ForEachParameterConnection(
            (_, connection)=> {
                if(connection.IsReady(frameId)) {
                    ReturnValue= connection.Value;
                    MarkAsExecuted(frameId);
                    return false;
                }
                return true;
            }
        );
	}
}
