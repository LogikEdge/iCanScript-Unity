using UnityEngine;
using System;
using Subspace;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(int instanceId, string name, SSContext context, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(instanceId, name, context, priority, nbOfParameters, nbOfEnables) {}
}
