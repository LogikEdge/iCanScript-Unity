using UnityEngine;
using System;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(iCS_Storage storage, int instanceId, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(storage, instanceId, priority, nbOfParameters, nbOfEnables) {}
}
