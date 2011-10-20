using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // forEach :: (f a->())->[a]->()
    public static void forEach<A>(Action<A> action, A[] lst) {
        foreach(var obj in lst) action(obj);
    }
    public static void forEach<A>(Action<A> action, List<A> lst) {
        foreach(var obj in lst) action(obj);
    }
}
