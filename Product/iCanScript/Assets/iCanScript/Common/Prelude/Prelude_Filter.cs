using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // filterWith :: (a->bool)->(a->())->[a]->()
        // filter     :: (a->bool)->[a]->[a]
        public static void filterWith<A>(Func<A,bool> cond, Action<A> fnc, A[] l) {
            foreach(var obj in l) {
                if(cond(obj)) {
                    fnc(obj);
                }
            }
        }
        public static void filterWith<A>(Func<A,bool> cond, Action<A> fnc, List<A> l) {
            foreach(var obj in l) {
                if(cond(obj)) {
                    fnc(obj);
                }
            }
        }
        public static A[] filter<A>(Func<A,bool> cond, A[] l) {
            List<A> result= new List<A>();
            filterWith<A>(cond, (obj)=> { result.Add(obj); }, l);
            return result.ToArray();
        }
        public static List<A> filter<A>(Func<A,bool> cond, List<A> l) {
            List<A> result= new List<A>();
            filterWith<A>(cond, (obj)=> { result.Add(obj); }, l);
            return result;
        }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Func<A[],A[]> filter<A>(Func<A,bool> cond) { return function<Func<A,bool>,A[],A[]>(filter,cond); }
    }

}
