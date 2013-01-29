using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // and :: bool->bool->bool
    // and :: [bool]->bool
    public static bool and(bool a, bool b) { return a & b; }
    public static bool and(bool[] lst)     { return fold(and, true, lst); }
    public static bool and(List<bool> lst) { return fold(and, true, lst); }

    // ----------------------------------------------------------------------
    // or :: bool->bool->bool
    // or :: [bool]->bool
    public static bool or(bool a, bool b)   { return a | b; }
    public static bool or(bool[] lst)       { return fold(or, false, lst); }
    public static bool or(List<bool> lst)   { return fold(or, false, lst); }

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
