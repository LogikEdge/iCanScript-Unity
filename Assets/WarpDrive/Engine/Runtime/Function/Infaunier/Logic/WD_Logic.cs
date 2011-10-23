using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public class WD_Logic {
    [WD_Function] public static bool And(bool a, bool b) { return a & b; }
    [WD_Function] public static bool Or (bool a, bool b) { return a | b; }
    [WD_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [WD_Function] public static bool Not(bool a)         { return !a; }

    [WD_Function] public static void Decoder2(int In, out bool a, out bool b) {
        a= (In & 1) == 0;
        b= (In & 1) == 1;
    }
    [WD_Function] public static void Decoder4(int In, out bool a, out bool b, out bool c, out bool d) {
        a= (In & 3) == 0;
        b= (In & 3) == 1;
        c= (In & 3) == 2;
        d= (In & 3) == 3;
    }
}
