using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_Iterator : iCS_Package {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Iterator(string name, SSObject parent, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {
    	Debug.Log("Iterator created with=> "+nbOfParameters+" params and=> "+nbOfEnables+" enables");
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
		base.DoEvaluate();
	}
}
