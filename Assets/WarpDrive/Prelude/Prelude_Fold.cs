using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // fold :: (a->b->b)->b->[a]->b
    public static B fold<A,B>(Func<A,B,B> fnc, B initialValue, A[] l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }
    public static B fold<A,B>(Func<A,B,B> fnc, B initialValue, List<A> l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }

    // ----------------------------------------------------------------------
    // Currying support.
    public static Func<A[],B>         fold<A,B>(Func<A,B,B> f, B v) { return function<Func<A,B,B>,B,A[],B>(fold,f,v); }
    public static Func<B,Func<A[],B>> fold<A,B>(Func<A,B,B> f)      { return function<Func<A,B,B>,B,A[],B>(fold,f); }
}
