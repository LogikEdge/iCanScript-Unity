using UnityEngine;

public class iCS_Message : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters= 0)
    : base(visualScript, priority, nbOfParameters) {}
}
