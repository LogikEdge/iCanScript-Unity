using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // forEach :: (a->())->[a]->()
    // forEach :: (a->bool)->[a]->bool
    public static void forEach<A>(Action<A> action, A[] lst) {
        foreach(var obj in lst) action(obj);
    }
    public static void forEach<A>(Action<A> action, List<A> lst) {
        foreach(var obj in lst) action(obj);
    }
    public static bool forEach<A>(Func<A,bool> fnc, A[] lst) {
        foreach(var obj in lst) if(fnc(obj)) return true;
        return false;
    }
    public static bool forEach<A>(Func<A,bool> fnc, List<A> lst) {
        foreach(var obj in lst) if(fnc(obj)) return true;
        return false;
    }
}
