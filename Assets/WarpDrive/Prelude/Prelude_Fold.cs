using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // fold :: (f a->b->b)->b->[a]->b
    public static B fold<A,B>(System.Func<A,B,B> fnc, B initialValue, A[] l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }
    public static B fold<A,B>(System.Func<A,B,B> fnc, B initialValue, List<A> l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }

}
