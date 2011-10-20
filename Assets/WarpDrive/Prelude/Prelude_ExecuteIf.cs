using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ======================================================================
    // Type <-> Type
    // ----------------------------------------------------------------------
    // executeIf :: Typeof a->Typeof b->(f ()->())->bool
    // executeIf :: Typeof a->Typeof b->(f ()->r)->r->r
    // executeIf :: Typeof a->Typeof b->(f ()->r)->Maybe r
    public static bool executeIf<T1,T2>(Action fnc) {
        if(notEqual<T1,T2>()) return false;
        fnc();
        return true;
    }
    public static R executeIf<T1,T2,R>(System.Func<R> fnc, R defaultValue) {
        return equal<T1,T2>() ? fnc() : defaultValue;
    }
    public static Maybe<R> executeIf<T1,T2,R>(Func<R> fnc) {
        return equal<T1,T2>() ? new Maybe<R>(new Just<R>(fnc())) : new Maybe<R>(new Nothing());
    }
    // ----------------------------------------------------------------------
    // executeIfNot :: Typeof a->Typeof b->(f ()->())->bool
    // executeIfNot :: Typeof a->Typeof b->(f ()->r)->r->r
    // executeIfNot :: Typeof a->Typeof b->(f ()->r)->Maybe r
    public static bool executeIfNot<T1,T2>(Action fnc) {
        if(equal<T1,T2>()) return false;
        fnc();
        return true;
    }
    public static Maybe<R> executeIfNot<T1,T2,R>(Func<R> fnc) {
        return notEqual<T1,T2>() ? new Maybe<R>(new Just<R>(fnc())) : new Maybe<R>(new Nothing());
    }
    public static R executeIf<T1,T2,R>(System.Func<R> fnc, R defaultValue) {
        return notEqual<T1,T2>() ? fnc() : defaultValue;
    }

    // ======================================================================
    // Type <-> Instance
    // ----------------------------------------------------------------------
    // executeIf :: Typeof a->b->(f a->())->bool
    // executeIf :: Typeof a->b->(f a->r)->r->r
    // executeIf :: Typeof a->b->(f a->r)->Maybe r
    public static bool executeIf<T>(object obj, System.Action<T> fnc) where T : class {
        if(notEqual<T>(obj)) return false;
        fnc(obj as T);
        return true;
    }
    public static R executeIf<T,R>(object obj, System.Func<T,R> fnc, R defaultReturn) where T : class {
        return equal<T>(obj) ? fnc(obj as T) : defaultReturn;
    }
    public static Maybe<R> executeIf<T,R>(object obj, System.Func<T,R> fnc) where T : class {
        return equal<T>(obj) ? new Maybe<R>(new Just<R>(fnc(obj as T))) : new Maybe<R>(new Nothing());
    }
    // ----------------------------------------------------------------------
    // executeIfNot :: Typeof a->b->(f a->())->bool
    // executeIfNot :: Typeof a->b->(f a->r)->r->r
    // executeIfNot :: Typeof a->b->(f a->r)->Maybe r
    public static bool executeIfNot<T>(object obj, System.Action<T> fnc) where T : class {
        if(equal<T>(obj)) return false;
        fnc(obj as T);
        return true;
    }
    public static R executeIfNot<T,R>(object obj, System.Func<T,R> fnc, R defaultReturn) where T : class {
        return notEqual<T>(obj) ? fnc(obj as T) : defaultReturn;
    }
    public static Maybe<R> executeIfNot<T,R>(object obj, System.Func<T,R> fnc) where T : class {
        return notEqual<T>(obj) ? new Maybe<R>(new Just<R>(fnc(obj as T))) : new Maybe<R>(new Nothing());
    }

    // ======================================================================
    // Dynamic
    // ----------------------------------------------------------------------
    // executeIf :: a->(f a->bool)->(f a->())->bool
    // executeIf :: a->(f a->bool)->(f a->b)->b->b
    // executeIf :: a->(f a->bool)->(f a->b)->Maybe b
    public static bool executeIf<A>(A obj, Func<A,bool> cond, Action<A> action) {
        if(!cond(obj)) return false;
        action(obj);
        return true;
    }
    public static B executeIf<A,B>(A obj, Func<A,bool> cond, Func<A,B> fnc, B defaultValue) {
        return cond(obj) ? fnc(obj) : defaultValue;
    }
    public static Maybe<B> executeIf<A,B>(A obj, Func<A,bool> cond, Func<A,B> fnc) {
        return cond(obj) ? new Maybe<B>(new Just<B>(fnc(obj))) : new Maybe<B>(new Nothing());
    }
    // ----------------------------------------------------------------------
    // executeIfNot :: a->(f a->bool)->(f a->())->bool
    // executeIfNot :: a->(f a->bool)->(f a->b)->b->b
    // executeIfNot :: a->(f a->bool)->(f a->b)->Maybe b
    public static bool executeIfNot<A>(A obj, Func<A,bool> cond, Action<A> action) {
        if(cond(obj)) return false;
        action(obj);
        return true;
    }
    public static B executeIfNot<A,B>(A obj, Func<A,bool> cond, Func<A,B> fnc, B defaultValue) {
        return (!cond(obj)) ? fnc(obj) : defaultValue;
    }
    public static Maybe<B> executeIfNot<A,B>(A obj, Func<A,bool> cond, Func<A,B> fnc) {
        return (!cond(obj)) ? new Maybe<B>(new Just<B>(fnc(obj))) : new Maybe<B>(new Nothing());
    }

}
