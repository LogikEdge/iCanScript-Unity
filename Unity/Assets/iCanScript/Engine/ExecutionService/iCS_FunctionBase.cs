using UnityEngine;
using System.Reflection;
using System.Collections;

public class iCS_FunctionBase : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected MethodBase    myMethodBase  = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FunctionBase(MethodBase methodBase, iCS_Storage storage, int instanceId, int priority,
                            int nbOfParameters)
    : base(storage, instanceId, priority, nbOfParameters) {
        myMethodBase= methodBase;
    }
}
