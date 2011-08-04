using UnityEngine;
using System.Collections;

public class AP_TransitionTest : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public  bool o= false;
                 private int  c= 0;
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_TransitionTest CreateInstance(string theFunctionName, AP_Aggregate theParent) {
        AP_TransitionTest instance= CreateInstance<AP_TransitionTest>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        if(++c > 2) {
            c= 0;
            o= true;
        }
        else {
            o= false;
        }
    }

}
