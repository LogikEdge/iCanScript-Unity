using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_Constructor : iCS_ClassFunction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(string name, SSObject parent, MethodBase methodBase, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, methodBase, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
        if(ReturnValue == null) {
            base.DoExecute();
        } else {
            MarkAsExecuted();
        }
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute() {
        if(ReturnValue == null) {
            base.DoForceExecute();
            if(ReturnValue != null) {
                // TODO: Should remove variable creation for execution queue once done.
                ArePortsAlwaysCurrent= true;
            }
        } else {
            MarkAsExecuted();
        }
    }
    
}
