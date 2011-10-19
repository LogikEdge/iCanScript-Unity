using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ======================================================================
    // Type <-> Type
    // ----------------------------------------------------------------------
    // choice :: Typeof s->Typeof t1->Typeof t2->(f1 ()->())->(f2 ()->())->()
    // choice :: Typeof s->Typeof t1->Typeof t2->(f1 ()->c)->(f2 ()->c)->c->c
    // choice :: Typeof s->Typeof t1->Typeof t2->(f1 ()->c)->(f2 ()->c)->Maybe c
    // ...
    public static void choice<S,T1,T2>(Action f1, Action f2) {
        if(equal<S,T1>()) f1();
        else if(equal<S,T2>()) f2();
    }
    public static void choice<S,T1,T2,T3>(Action f1, Action f2, Action f3) {
        if(equal<S,T1>()) f1();
        else if(equal<S,T2>()) f2();
        else if(equal<S,T3>()) f3();
    }
    // ----------------------------------------------------------------------
    public static R choice<S,T1,T2,R>(Func<R> f1, Func<R> f2, R defaultValue) {
        if(equal<S,T1>())       return f1();
        else if(equal<S,T2>())  return f2();
        return defaultValue;
    }
    public static R choice<S,T1,T2,T3,R>(Func<R> f1, Func<R> f2, Func<R> f3, R defaultValue) {
        if(equal<S,T1>())       return f1();
        else if(equal<S,T2>())  return f2();
        else if(equal<S,T3>())  return f3();
        return defaultValue;
    }
    // ----------------------------------------------------------------------
    public static Maybe<R> choice<S,T1,T2,R>(Func<R> f1, Func<R> f2) {
        if(equal<S,T1>())       return new Maybe<R>(new Just<R>(f1()));
        else if(equal<S,T2>())  return new Maybe<R>(new Just<R>(f2()));
        return new Maybe<R>(new Nothing());
    }
    public static Maybe<R> choice<S,T1,T2,T3,R>(Func<R> f1, Func<R> f2, Func<R> f3) {
        if(equal<S,T1>())       return new Maybe<R>(new Just<R>(f1()));
        else if(equal<S,T2>())  return new Maybe<R>(new Just<R>(f2()));
        else if(equal<S,T3>())  return new Maybe<R>(new Just<R>(f3()));
        return new Maybe<R>(new Nothing());
    }

 
    // ======================================================================
    // Type <-> Instance
    // ----------------------------------------------------------------------
    public static void choice<T1,T2>(object obj, Action<T1> f1,
                                                 Action<T2> f2,
                                                 Action<object> defaultFnc) where T1 : class
                                                                            where T2 : class {
        T1 t1= obj as T1;
        if(t1 != null) { f1(t1); return; }
        T2 t2= obj as T2;
        if(t2 != null) { f2(t2); return; }
        defaultFnc(obj);
    }

    // ----------------------------------------------------------------------
    // Executes a case statement according to the type of the object
    public static void choice<T1,T2,T3>(object obj, Action<T1> f1,
                                                    Action<T2> f2,
                                                    Action<T3> f3,
                                                    Action<object> defaultFnc) where T1 : class
                                                                               where T2 : class
                                                                               where T3 : class {
        T1 t1= obj as T1;
        if(t1 != null) { f1(t1); return; }
        T2 t2= obj as T2;
        if(t2 != null) { f2(t2); return; }
        T3 t3= obj as T3;
        if(t3 != null) { f3(t3); return; }
        defaultFnc(obj);
    }
    
    // ----------------------------------------------------------------------
    // Executes a case statement according to the type of the object
    public static void choice<T1,T2,T3,T4>(object obj, Action<T1> f1,
                                                       Action<T2> f2,
                                                       Action<T3> f3,
                                                       Action<T4> f4,
                                                       Action<object> defaultFnc) where T1 : class
                                                                                  where T2 : class
                                                                                  where T3 : class
                                                                                  where T4 : class {
        T1 t1= obj as T1;
        if(t1 != null) { f1(t1); return; }
        T2 t2= obj as T2;
        if(t2 != null) { f2(t2); return; }
        T3 t3= obj as T3;
        if(t3 != null) { f3(t3); return; }
        T4 t4= obj as T4;
        if(t4 != null) { f4(t4); return; }
        defaultFnc(obj);
    }
    
    // ----------------------------------------------------------------------
    // Executes a case statement according to the type of the object
    public static void choice<T1,T2,T3,T4,T5>(object obj, Action<T1> f1,
                                                          Action<T2> f2,
                                                          Action<T3> f3,
                                                          Action<T4> f4,
                                                          Action<T5> f5,
                                                          Action<object> defaultFnc) where T1 : class
                                                                                     where T2 : class
                                                                                     where T3 : class
                                                                                     where T4 : class
                                                                                     where T5 : class {
        T1 t1= obj as T1;
        if(t1 != null) { f1(t1); return; }
        T2 t2= obj as T2;
        if(t2 != null) { f2(t2); return; }
        T3 t3= obj as T3;
        if(t3 != null) { f3(t3); return; }
        T4 t4= obj as T4;
        if(t4 != null) { f4(t4); return; }
        T5 t5= obj as T5;
        if(t5 != null) { f5(t5); return; }
        defaultFnc(obj);
    }
    
    // ======================================================================
    // Dynamic
    // ----------------------------------------------------------------------

}
