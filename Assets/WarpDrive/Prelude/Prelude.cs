using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Functional Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public static partial class Prelude {
    // ----------------------------------------------------------------------
    public static A id<A>(A a) { return a; }
    
    // ----------------------------------------------------------------------
    public static R dot<A,B,R>(System.Func<B,R> f1, System.Func<A,B> f2, A a) {
        return f1(f2(a));
    }
    public static System.Func<A,R> dot<A,B,R>(System.Func<B,R> f1, System.Func<A,B> f2) {
        return delegate(A a) { return dot(f1,f2,a); };
    }
    public static System.Func<System.Func<A,B>,System.Func<A,R>> dot<A,B,R>(System.Func<B,R> f1) {
        return delegate(System.Func<A,B> f2) { return dot(f1,f2); };
    }
    
    // ----------------------------------------------------------------------
    public static R flip<A,B,R>(System.Func<B,A,R> f, A a, B b) { return f(b,a); }
    public static Func<B,R> flip<A,B,R>(Func<B,A,R> f, A a) {
        return delegate(B b) { return flip(f,a,b); };
    }
    public static Func<A,Func<B,R>> flip<A,B,R>(Func<B,A,R> f) {
        return delegate(A a) { return flip(f,a); };
    }


}
