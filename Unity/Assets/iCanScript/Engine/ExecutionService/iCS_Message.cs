using UnityEngine;

public class iCS_Message : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(int instanceId, string name, iCS_VisualScriptImp visualScript, int priority, int nbOfParameters= 0)
    : base(instanceId, name, visualScript, priority, nbOfParameters) {}
}
