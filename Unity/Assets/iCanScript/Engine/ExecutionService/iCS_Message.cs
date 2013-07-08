using UnityEngine;

public class iCS_Message : iCS_Aggregate{
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Message(string name, int priority, int nbOfParameters= 0)
    : base(name, priority, nbOfParameters) {}
}
