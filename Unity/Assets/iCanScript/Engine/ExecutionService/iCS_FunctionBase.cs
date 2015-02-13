using UnityEngine;
using System.Reflection;
using System.Collections;
using Subspace;

public abstract class iCS_FunctionBase : SSActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected MethodBase    myMethodBase  = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FunctionBase(int instanceId, string name, SSObject parent, MethodBase methodBase, SSContext context, int priority,
                            int nbOfParameters, int nbOfEnables)
    : base(instanceId, name, parent, context, priority, nbOfParameters, nbOfEnables) {
        myMethodBase= methodBase;
    }
}
