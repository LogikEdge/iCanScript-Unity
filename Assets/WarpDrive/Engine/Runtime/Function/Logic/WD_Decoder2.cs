using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public sealed class WD_Decoder2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public int[]  xs;
    [WD_OutPort] public int[]  oxs;
    [WD_OutPort] public bool[] os0;
    [WD_OutPort] public bool[] os1;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
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
