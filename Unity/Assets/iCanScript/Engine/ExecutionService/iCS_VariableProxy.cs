using UnityEngine;
using System.Collections;

public class iCS_VariableProxy : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_VariableProxy(iCS_VisualScriptImp visualScript, int priority,
                             int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        MarkAsExecuted(frameId);            
    }

    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        MarkAsExecuted(frameId);            
    }
}
