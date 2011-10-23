using UnityEngine;
using System;
using System.Collections;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // isEmpty    :: [a]->bool
    // isNotEmpty :: [a]->bool
    public static bool isEmpty<A>(A[] lst)    { return lst.Length == 0; }
    public static bool isNotEmpty<A>(A[] lst) { return !isEmpty(lst); }

    // ----------------------------------------------------------------------
    // emptyLst :: typeof A
    public static A[] empty<A>()    { return new A[0]; }
    
    // ----------------------------------------------------------------------
    // concat :: a->[a]->[a]
    public static A[] concat<A>(A a, A[] lst) {
        A[] newLst= new A[lst.Length+1];
        newLst[0]= a;
        for(int i= 0; i < lst.Length; ++i) newLst[i+1]= lst[i];
        return newLst;
    }
    
    // ----------------------------------------------------------------------
    // head :: [a]->a
    // last :: [a]->a
    // tail :: [a]->[a]
    public static A head<A>(A[] lst) { return lst[0]; }
    public static A last<A>(A[] lst) { return lst[lst.Length-1]; }
    public static A[] tail<A>(A[] lst)  {
        if(isEmpty(lst)) return new A[0];
        A[] newLst= new A[lst.Length-1];
        for(int i= 1; i < lst.Length; ++i) {
            newLst[i-1]= lst[i];
        }
        return newLst;
    }
}
