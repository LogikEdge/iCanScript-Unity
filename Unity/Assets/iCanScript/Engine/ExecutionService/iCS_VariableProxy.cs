using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_VariableProxy : SSActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_VariableProxy(int instanceId, string name, SSObject parent, SSContext context, int priority,
                             int nbOfParameters, int nbOfEnables)
    : base(instanceId, name, parent, context, priority, nbOfParameters, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
        MarkAsExecuted(runId);            
    }

    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int runId) {
        MarkAsExecuted(runId);            
    }
}
