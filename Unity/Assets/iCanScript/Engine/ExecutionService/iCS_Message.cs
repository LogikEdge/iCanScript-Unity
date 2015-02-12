using UnityEngine;
using Subspace;

public class iCS_Message : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(int instanceId, string name, SSContext context, int priority, int nbOfParameters= 0)
    : base(instanceId, name, context, priority, nbOfParameters) {}
}
