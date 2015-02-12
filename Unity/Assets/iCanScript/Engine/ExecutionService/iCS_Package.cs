using UnityEngine;
using System;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(int instanceId, string name, iCS_VisualScriptImp visualScript, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(instanceId, name, visualScript, priority, nbOfParameters, nbOfEnables) {}
}
