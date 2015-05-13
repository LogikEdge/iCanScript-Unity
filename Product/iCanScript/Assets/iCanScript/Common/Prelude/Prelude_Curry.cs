using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // curry :: ((a,b)->r)->a->b->r
        // curry :: ((a,b,c)->r)->a->b->c->r
        // curry :: ((a,b,c,d)->r)->a->a->c->d->r
        public static R curry<A,B,R>(Func<Tuple<A,B>,R> fnc, A a, B b) {
            return fnc(new Tuple<A,B>(a,b));
        }
        public static R curry<A,B,C,R>(Func<Tuple<A,B,C>,R> fnc, A a, B b, C c) {
            return fnc(new Tuple<A,B,C>(a,b,c));
        }
        public static R curry<A,B,C,D,R>(Func<Tuple<A,B,C,D>,R> fnc, A a, B b, C c, D d) {
            return fnc(new Tuple<A,B,C,D>(a,b,c,d));
        }
    
        // ----------------------------------------------------------------------
        // uncurry :: (a->b->r)->(a,b)->r
        // uncurry :: (a->b->c->r)->(a,b,c)->r
        // uncurry :: (a->b->c->d->r)->(a,b,c,d)->r
        public static R uncurry<A,B,R>(Func<A,B,R> fnc, Tuple<A,B> tuple) {
            return fnc(tuple.Item1, tuple.Item2);
        }
        public static R uncurry<A,B,C,R>(Func<A,B,C,R> fnc, Tuple<A,B,C> tuple) {
            return fnc(tuple.Item1, tuple.Item2, tuple.Item3);
        }
        public static R uncurry<A,B,C,D,R>(Func<A,B,C,D,R> fnc, Tuple<A,B,C,D> tuple) {
            return fnc(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

    }

}
