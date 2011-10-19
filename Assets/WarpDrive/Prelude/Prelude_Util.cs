using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ======================================================================
    //  List/Array
    // ----------------------------------------------------------------------
    // Returns the length of a list or array.
    public static int length<T>(T[] a)      { return a.Length; }
    public static int length<T>(List<T> l)  { return l.Count; }
    
    // ======================================================================
    //  C# Currying
    // ----------------------------------------------------------------------
    // This function can fixes a problem with C# implementation of the
    // Prelude by allowing a partial function of type 'f(a)(b,c)' to be
    // converted to 'f(a)(b)(c)' as follows 'fix(f)(b,c)'
    public static R fix<A,B,R>(System.Func<A,System.Func<B,R>> f, A a, B b) {
         return f(a)(b);
    }
    public static System.Func<A,B,R>  fix<A,B,R>(System.Func<A,System.Func<B,R>> f) {
        return delegate(A a, B b) { return fix(f,a,b); };
    }
}
