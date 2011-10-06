using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public sealed class WD_Decoder4 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public int[]  xs;
    [WD_OutPort] public int[]  oxs;
    [WD_OutPort] public bool[] os0;
    [WD_OutPort] public bool[] os1;
    [WD_OutPort] public bool[] os2;
    [WD_OutPort] public bool[] os3;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
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
