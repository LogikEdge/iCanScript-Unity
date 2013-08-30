using UnityEngine;
using System;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {}
}
