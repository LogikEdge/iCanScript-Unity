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
    public iCS_FunctionBase(MethodBase methodBase, string name, int priority, int nbOfParameters, bool hasReturn, bool hasThis)
    : base(name, priority, nbOfParameters, hasReturn, hasThis) {
        myMethodBase= methodBase;
    }
}
