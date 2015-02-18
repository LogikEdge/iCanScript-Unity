using UnityEngine;
using System;
using Subspace;

public class iCS_Package : iCS_ParallelDispatcher {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public int myIteration= 1;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(string name, SSObject parent, int priority, int nbOfParameters= 0, int nbOfEnables= 0)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Iteration
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        if(myIteration > 1 && (myParent is iCS_Message) == true) {
//            var packageContext= Context.Clone();
//            packageContext.RunId+= 1000;
//            Context= packageContext;
            Context.RunId+= myIteration-1;
            for(int i= 1; i < myIteration; ++i) {
                base.DoEvaluate();
                Context.RunId-= 1;
            }
//            Context= null;
        }
        base.DoEvaluate();
    }
}
