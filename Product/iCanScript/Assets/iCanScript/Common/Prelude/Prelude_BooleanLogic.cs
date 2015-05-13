using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // and :: bool->bool->bool
        // and :: [bool]->bool
        public static bool and(bool a, bool b) 					  { return a & b; }
        public static bool and(bool[] lst)     					  { return all(id, lst); }
        public static bool and(List<bool> lst) 					  { return all(id, lst); }
    	public static bool and<A>(Func<A,bool> cond, A[] lst)     { return all(cond, lst); }
    	public static bool and<A>(Func<A,bool> cond, List<A> lst) { return all(cond, lst); }

        // ----------------------------------------------------------------------
        // or :: bool->bool->bool
        // or :: [bool]->bool
        public static bool or(bool a, bool b)   					{ return a | b; }
        public static bool or(bool[] lst)       					{ return any(id, lst); }
        public static bool or(List<bool> lst)   					{ return any(id, lst); }
    	public static bool or<A>(Func<A,bool> cond, A[] lst)		{ return any(cond, lst); }
    	public static bool or<A>(Func<A,bool> cond, List<A> lst)	{ return any(cond, lst); }

        // ----------------------------------------------------------------------
        // all :: (f a->bool)->[a]->bool
        public static bool all<A>(System.Func<A,bool> cond, A[] lst) {
            foreach(var obj in lst) {
                if(!cond(obj)) {
                    return false;
                }
            }
            return true;
        }
        public static bool all<A>(System.Func<A,bool> cond, List<A> lst) {
            foreach(var obj in lst) {
                if(!cond(obj)) {
                    return false;
                }
            }
            return true;
        }

        // ----------------------------------------------------------------------
        // any :: (f a->bool)->[a]->bool
        public static bool any<A>(System.Func<A,bool> cond, A[] lst) {
            foreach(var obj in lst) {
                if(cond(obj)) {
                    return true;
                }
            }
            return false;
        }
        public static bool any<A>(System.Func<A,bool> cond, List<A> lst) {
            foreach(var obj in lst) {
                if(cond(obj)) {
                    return true;
                }
            }
            return false;
        }
    
    }

}
