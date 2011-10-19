using UnityEngine;
using System;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Programming constructs using types as base variables
public partial class Util {
    // ======================================================================
    // Comparaisons
    // ----------------------------------------------------------------------
    public static bool equal<T1,T2>() {
        return typeof(T1) == typeof(T2);
    }
    public static bool notEqual<T1,T2>() {
        return !equal<T1,T2>();
    }

    // ======================================================================
    // Simple executions
    // ----------------------------------------------------------------------
    public static void executeIf<T1,T2>(Action fnc) {
        if(equal<T1,T2>()) fnc();
    }
    public static void executeIfNot<T1,T2>(Action fnc) {
        if(notEqual<T1,T2>()) fnc();
    }
    
    // ----------------------------------------------------------------------
    public static Maybe<R> executeIf<T1,T2,R>(Func<R> fnc) {
        return equal<T1,T2>() ? new Maybe<R>(new Just<R>(fnc())) : new Maybe<R>(new Nothing());
    }
    public static void choice<S,T1,T2>(Action f1, Action f2) {
        if(equal<S,T1>()) f1();
        else if(equal<S,T2>()) f2();
    }
    public static Maybe<R> choice<S,T1,T2,R>(Func<R> f1, Func<R> f2) {
        if(equal<S,T1>())       return new Maybe<R>(new Just<R>(f1()));
        else if(equal<S,T2>())  return new Maybe<R>(new Just<R>(f2()));
        return new Maybe<R>(new Nothing());
    }
    public static void choice<S,T1,T2,T3>(Action f1, Action f2, Action f3) {
        if(equal<S,T1>()) f1();
        else if(equal<S,T2>()) f2();
        else if(equal<S,T3>()) f3();
    }
    public static Maybe<R> choice<S,T1,T2,T3,R>(Func<R> f1, Func<R> f2, Func<R> f3) {
        if(equal<S,T1>())       return new Maybe<R>(new Just<R>(f1()));
        else if(equal<S,T2>())  return new Maybe<R>(new Just<R>(f2()));
        else if(equal<S,T3>())  return new Maybe<R>(new Just<R>(f3()));
        return new Maybe<R>(new Nothing());
    }

}
