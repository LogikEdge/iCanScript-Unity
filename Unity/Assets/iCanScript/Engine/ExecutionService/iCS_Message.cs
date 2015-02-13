using UnityEngine;
using Subspace;

public class iCS_Message : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(int instanceId, string name, SSObject parent, SSContext context, int priority, int nbOfParameters= 0)
    : base(instanceId, name, parent, context, priority, nbOfParameters) {}
}
