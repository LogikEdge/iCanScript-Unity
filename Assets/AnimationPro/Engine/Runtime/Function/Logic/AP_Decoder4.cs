using UnityEngine;
using System.Collections;

public sealed class AP_Decoder4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[]  xs;
    [AP_OutPort] public int[]  oxs;
    [AP_OutPort] public bool[] os0;
    [AP_OutPort] public bool[] os1;
    [AP_OutPort] public bool[] os2;
    [AP_OutPort] public bool[] os3;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        int len= xs.Length;
        for(int i= 0; i < len; ++i) {
            oxs[i]= xs[i] >> 2;
            switch(xs[i] & 3) {
                case 0:
                    os0[i]= true;
                    os1[i]= false;
                    os2[i]= false;
                    os3[i]= false;
                    break;
                case 1:
                    os0[i]= false;
                    os1[i]= true;
                    os2[i]= false;
                    os3[i]= false;
                    break;
                case 2:
                    os0[i]= false;
                    os1[i]= false;
                    os2[i]= true;
                    os3[i]= false;
                    break;
                case 3:
                    os0[i]= false;
                    os1[i]= false;
                    os2[i]= false;
                    os3[i]= true;
                    break;
            }
        }
    }
}
