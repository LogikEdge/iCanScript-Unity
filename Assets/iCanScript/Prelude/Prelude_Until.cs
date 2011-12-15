using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // until :: (a->bool)->[a]->bool
    public static bool until<A>(Func<A,bool> fnc, A[] lst) {
        foreach(var obj in lst) if(fnc(obj)) return true;
        return false;
    }
    public static bool until<A>(Func<A,bool> fnc, List<A> lst) {
        foreach(var obj in lst) if(fnc(obj)) return true;
        return false;
    }
}
