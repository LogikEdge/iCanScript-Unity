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
}
