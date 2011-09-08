using UnityEngine;
using System.Collections;

public sealed class AP_Decoder2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[]  xs;
    [AP_OutPort] public int[]  oxs;
    [AP_OutPort] public bool[] os0;
    [AP_OutPort] public bool[] os1;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Decoder2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Decoder2 instance= CreateInstance<AP_Decoder2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        int len= xs.Length;
        for(int i= 0; i < len; ++i) {
            oxs[i]= xs[i] >> 1;
            if((xs[i] & 1) == 0) {
                os0[i]= true;
                os1[i]= false;
            }
            else {
                os0[i]= false;
                os1[i]= true;
            }
        }
    }
}
