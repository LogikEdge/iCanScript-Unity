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
    public iCS_FunctionBase(MethodBase methodBase, iCS_VisualScriptImp visualScript, int priority,
                            int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {
        myMethodBase= methodBase;
    }
}
