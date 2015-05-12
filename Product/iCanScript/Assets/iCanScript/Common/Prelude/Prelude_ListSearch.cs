using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // find :: (a->bool)->[a]->Maybe<a>
        public static A find<A>(Func<A,bool> cond, A[] lst) {
            foreach(var o in lst) {
                if(cond(o)) {
                    return o;
                }
            }
            return default(A);
        }
        public static A find<A>(Func<A,bool> cond, List<A> lst) {
            foreach(var o in lst) {
                if(cond(o)) {
                    return o;
                }
            }
            return default(A);
        }
        // findFirst :: (a->bool)->(int->a->b)->b->[a]->b
        public static R findFirst<A,R>(Func<A,bool> cond, Func<int,A,R> fnc, R defResult, A[] lst) {
            var len= length(lst);
            for(int i= 0; i < len; ++i) {
                var a= lst[i];
                if(cond(a)) {
                    return fnc(i,a);
                }
            }
            return defResult;
        }
        public static R findFirst<A,R>(Func<A,bool> cond, Func<int,A,R> fnc, R defResult, List<A> lst) {
            var len= length(lst);
            for(int i= 0; i < len; ++i) {
                var a= lst[i];
                if(cond(a)) {
                    return fnc(i,a);
                }
            }
            return defResult;
        }
    
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

}
