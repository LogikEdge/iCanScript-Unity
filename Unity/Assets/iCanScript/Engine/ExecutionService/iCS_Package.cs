using UnityEngine;
using System;
using Subspace;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(string name, SSObject parent, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {}
}
