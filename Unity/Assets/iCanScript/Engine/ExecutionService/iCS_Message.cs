using UnityEngine;

public class iCS_Message : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(iCS_Storage storage, int instanceId, int priority, int nbOfParameters= 0)
    : base(storage, instanceId, priority, nbOfParameters) {}
}
