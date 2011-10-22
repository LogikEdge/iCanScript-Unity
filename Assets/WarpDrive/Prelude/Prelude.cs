using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Functional Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public static partial class Prelude {
    // ----------------------------------------------------------------------
    // id :: a->a
    public static A id<A>(A a)                 { return a; }

    // ----------------------------------------------------------------------
    // constant :: a->b->a
    public static A constant<A,B>(A a, B b)    { return a; }
    public static Func<B,A> constant<A,B>(A a) { return delegate(B b) { return constant(a,b); }; }

    // ----------------------------------------------------------------------
    // dot :: (b->c)->(a->b)->a->c
    public static C dot<A,B,C>(Func<B,C> f1, Func<A,B> f2, A a) {
        return f1(f2(a));
    }
    public static Func<A,R> dot<A,B,R>(Func<B,R> f1, Func<A,B> f2) {
        return delegate(A a) { return dot(f1,f2,a); };
    }
    public static Func<Func<A,B>, Func<A,R>> dot<A,B,R>(Func<B,R> f1) {
        return delegate(Func<A,B> f2) { return dot(f1,f2); };
    }
    
    // ----------------------------------------------------------------------
    // flip :: (a->b->c)->b->a->c
    public static C flip<A,B,C>(Func<A,B,C> f, B b, A a) { return f(a,b); }
    public static Func<A,C> flip<A,B,C>(Func<A,B,C> f, B b) {
        return delegate(A a) { return flip(f,b,a); };
    }
    public static Func<B,Func<A,C>> flip<A,B,C>(Func<A,B,C> f) {
        return delegate(B b) { return flip(f,b); };
    }

}
