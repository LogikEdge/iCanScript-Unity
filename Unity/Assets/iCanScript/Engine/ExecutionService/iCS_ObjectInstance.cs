using UnityEngine;
using System.Collections;

public class iCS_ObjectInstance : iCS_Package {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	int myInInstanceIdx = -1;
	int myOutInstanceIdx= -1;
	
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ObjectInstance(iCS_Storage storage, int instanceId, int priority, int nbOfParameters= 0)
    : base(storage, instanceId, priority, nbOfParameters) {}

    // -------------------------------------------------------------------------
    public void ActivateInstancePorts(int inPortIdx, int outPortIdx) {
		myInInstanceIdx = inPortIdx;
		myOutInstanceIdx= outPortIdx;
    }
}
