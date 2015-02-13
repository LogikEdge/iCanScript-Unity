using UnityEngine;
using System.Reflection;
using System.Collections;
using Subspace;

public abstract class iCS_FunctionBase : SSActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected MethodBase    myMethodBase  = null;

    public MethodBase methodBase { get { return myMethodBase; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FunctionBase(string name, SSObject parent, MethodBase methodBase, SSContext context, int priority,
                            int nbOfParameters, int nbOfEnables)
    : base(name, parent, context, priority, nbOfParameters, nbOfEnables) {
        myMethodBase= methodBase;
    }
}
