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
                            int nbOfParameters, bool hasReturn, bool hasThis)
    : base(storage, instanceId, priority, nbOfParameters, hasReturn, hasThis) {
        myMethodBase= methodBase;
    }
}
