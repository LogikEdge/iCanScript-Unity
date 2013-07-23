using UnityEngine;
using System.Collections;

public class iCS_Mux : iCS_ActionWithSignature {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Mux(iCS_Storage storage, int instanceId, int priority, int nbOfParameters)
    : base(storage, instanceId, priority, nbOfParameters, true, false) {}

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
        var connections= Connections;
        int nbOfConnections= connections.Length;
		for(int i= 0; i < nbOfConnections; ++i) {
            var c= connections[i];
            if(c.IsReady(frameId)) {
                ReturnValue= c.Value;
                MarkAsExecuted(frameId);
                return true;
            }			
		}
		return false;		
	}
}
