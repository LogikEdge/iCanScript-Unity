using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
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

        // ----------------------------------------------------------------------
        // dot :: (b->c)->(a->b)->a->c
        public static C dot<A,B,C>(Func<B,C> f1, Func<A,B> f2, A a) {
            return f1(f2(a));
        }
    
        // ----------------------------------------------------------------------
        // flip :: (a->b->c)->b->a->c
        public static C flip<A,B,C>(Func<A,B,C> f, B b, A a) { return f(a,b); }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Func<B,A> constant<A,B>(A a) {
            return function<A,B,A>(constant, a);
        }
        public static Func<A,C> dot<A,B,C>(Func<B,C> f1, Func<A,B> f2) {
            return function<Func<B,C>,Func<A,B>,A,C>(dot,f1,f2);
        }
        public static Func<Func<A,B>,Func<A,C>> dot<A,B,C>(Func<B,C> f1) {
            return function<Func<B,C>,Func<A,B>,A,C>(dot,f1);
        }
        public static Func<A,C> flip<A,B,C>(Func<A,B,C> f, B b) {
            return function<Func<A,B,C>,B,A,C>(flip,f,b);
        }
        public static Func<B,Func<A,C>> flip<A,B,C>(Func<A,B,C> f) {
            return function<Func<A,B,C>,B,A,C>(flip,f);
        }

    }

}
