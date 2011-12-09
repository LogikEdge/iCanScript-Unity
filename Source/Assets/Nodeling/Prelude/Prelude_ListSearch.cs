using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // elem    :: a->[a]->bool
    // notElem :: a->[a]->bool
    public static bool elem<A>(A a, A[] lst) where A : IComparable {
        foreach(var o in lst) { if(a.CompareTo(o) == 0) return true; }
        return false;
    }
    public static bool notElem<A>(A a, A[] lst) where A : IComparable {
        return !elem(a, lst);
    }
    
    // ----------------------------------------------------------------------
    // Currying support.
    public static Func<A[],bool>    elem<A>(A a)    where A : IComparable { return function<A,A[],bool>(elem,a); }
    public static Func<A[],bool>    notElem<A>(A a) where A : IComparable { return function<A,A[],bool>(notElem,a); }
}
