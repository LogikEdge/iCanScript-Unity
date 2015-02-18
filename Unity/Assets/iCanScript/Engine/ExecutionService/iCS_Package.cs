using UnityEngine;
using System;
using Subspace;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public int Repetition= 1;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(string name, SSObject parent, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Iteration
    // ----------------------------------------------------------------------
    public override void Evaluate() {
        base.Evaluate();
    }
    public override void Execute() {
        base.Execute();
    }
}
