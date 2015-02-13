using UnityEngine;
using System;
using Subspace;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(string name, SSObject parent, SSContext context, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(name, parent, context, priority, nbOfParameters, nbOfEnables) {}
}
