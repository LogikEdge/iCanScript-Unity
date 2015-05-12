using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
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
            if(equal<S,T1>())       return new Just<R>(f1());
            else if(equal<S,T2>())  return new Just<R>(f2());
            return new Nothing<R>();
        }
        public static Maybe<R> choice<S,T1,T2,T3,R>(Func<R> f1, Func<R> f2, Func<R> f3) {
            if(equal<S,T1>())       return new Just<R>(f1());
            else if(equal<S,T2>())  return new Just<R>(f2());
            else if(equal<S,T3>())  return new Just<R>(f3());
            return new Nothing<R>();
        }

 
        // ======================================================================
        // Type <-> Instance
        // ----------------------------------------------------------------------
        public static void choice<T1,T2>(object obj, Action<T1> f1,
                                                     Action<T2> f2,
                                                     Action<object> defaultFnc= null) where T1 : class
                                                                                      where T2 : class {
            if(executeIf<T1>(obj, f1)) return;
            if(executeIf<T2>(obj, f2)) return;
            if(defaultFnc != null) defaultFnc(obj);                                                 
        }

        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3>(object obj, Action<T1> f1,
                                                        Action<T2> f2,
                                                        Action<T3> f3,
                                                        Action<object> defaultFnc= null) where T1 : class
                                                                                         where T2 : class
                                                                                         where T3 : class {
            choice<T1,T2>(obj, f1, f2, (_) => {
                if(executeIf<T3>(obj, f3)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
    
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4>(object obj, Action<T1> f1,
                                                           Action<T2> f2,
                                                           Action<T3> f3,
                                                           Action<T4> f4,
                                                           Action<object> defaultFnc= null) where T1 : class
                                                                                            where T2 : class
                                                                                            where T3 : class
                                                                                            where T4 : class {
            choice<T1,T2,T3>(obj, f1, f2, f3, (_) => {
                if(executeIf<T4>(obj, f4)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
    
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5>(object obj, Action<T1> f1,
                                                              Action<T2> f2,
                                                              Action<T3> f3,
                                                              Action<T4> f4,
                                                              Action<T5> f5,
                                                              Action<object> defaultFnc= null) where T1 : class
                                                                                               where T2 : class
                                                                                               where T3 : class
                                                                                               where T4 : class
                                                                                               where T5 : class {
            choice<T1,T2,T3,T4>(obj, f1, f2, f3, f4, (_) => {
                if(executeIf<T5>(obj, f5)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
    
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5,T6>(object obj, Action<T1> f1,
                                                                 Action<T2> f2,
                                                                 Action<T3> f3,
                                                                 Action<T4> f4,
                                                                 Action<T5> f5,
                                                                 Action<T6> f6,
                                                                 Action<object> defaultFnc= null) where T1 : class
                                                                                                  where T2 : class
                                                                                                  where T3 : class
                                                                                                  where T4 : class
                                                                                                  where T5 : class
                                                                                                  where T6 : class {
            choice<T1,T2,T3,T4,T5>(obj, f1, f2, f3, f4, f5, (_) => {
                if(executeIf<T6>(obj, f6)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
    
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5,T6,T7>(object obj, Action<T1> f1,
                                                                    Action<T2> f2,
                                                                    Action<T3> f3,
                                                                    Action<T4> f4,
                                                                    Action<T5> f5,
                                                                    Action<T6> f6,
                                                                    Action<T7> f7,
                                                                    Action<object> defaultFnc= null) where T1 : class
                                                                                                     where T2 : class
                                                                                                     where T3 : class
                                                                                                     where T4 : class
                                                                                                     where T5 : class
                                                                                                     where T6 : class
                                                                                                     where T7 : class {
            choice<T1,T2,T3,T4,T5,T6>(obj, f1, f2, f3, f4, f5, f6, (_) => {
                if(executeIf<T7>(obj, f7)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5,T6,T7,T8>(object obj, Action<T1> f1,
                                                                       Action<T2> f2,
                                                                       Action<T3> f3,
                                                                       Action<T4> f4,
                                                                       Action<T5> f5,
                                                                       Action<T6> f6,
                                                                       Action<T7> f7,
                                                                       Action<T8> f8,
                                                                       Action<object> defaultFnc= null) where T1 : class
                                                                                                        where T2 : class
                                                                                                        where T3 : class
                                                                                                        where T4 : class
                                                                                                        where T5 : class
                                                                                                        where T6 : class
                                                                                                        where T7 : class
                                                                                                        where T8 : class {
            choice<T1,T2,T3,T4,T5,T6,T7>(obj, f1, f2, f3, f4, f5, f6, f7, (_) => {
                if(executeIf<T8>(obj, f8)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5,T6,T7,T8,T9>(object obj, Action<T1> f1,
                                                                          Action<T2> f2,
                                                                          Action<T3> f3,
                                                                          Action<T4> f4,
                                                                          Action<T5> f5,
                                                                          Action<T6> f6,
                                                                          Action<T7> f7,
                                                                          Action<T8> f8,
                                                                          Action<T9> f9,
                                                                          Action<object> defaultFnc= null) where T1 : class
                                                                                                           where T2 : class
                                                                                                           where T3 : class
                                                                                                           where T4 : class
                                                                                                           where T5 : class
                                                                                                           where T6 : class
                                                                                                           where T7 : class
                                                                                                           where T8 : class
                                                                                                           where T9 : class {
            choice<T1,T2,T3,T4,T5,T6,T7,T8>(obj, f1, f2, f3, f4, f5, f6, f7, f8, (_) => {
                if(executeIf<T9>(obj, f9)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }
        // ----------------------------------------------------------------------
        // Executes a case statement according to the type of the object
        public static void choice<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(object obj,
                                                                  Action<T1> f1,
                                                                  Action<T2> f2,
                                                                  Action<T3> f3,
                                                                  Action<T4> f4,
                                                                  Action<T5> f5,
                                                                  Action<T6> f6,
                                                                  Action<T7> f7,
                                                                  Action<T8> f8,
                                                                  Action<T9> f9,
                                                                  Action<T10> f10,
                                                                  Action<object> defaultFnc= null) where T1 : class
                                                                                                           where T2 : class
                                                                                                           where T3 : class
                                                                                                           where T4 : class
                                                                                                           where T5 : class
                                                                                                           where T6 : class
                                                                                                           where T7 : class
                                                                                                           where T8 : class
                                                                                                           where T9 : class
                                                                                                           where T10 : class {
            choice<T1,T2,T3,T4,T5,T6,T7,T8,T9>(obj, f1, f2, f3, f4, f5, f6, f7, f8, f9, (_) => {
                if(executeIf<T10>(obj, f10)) {}
                else if(defaultFnc!= null) defaultFnc(obj);
            });
        }

        // ======================================================================
        // Dynamic
        // ----------------------------------------------------------------------
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                                             Action<A> defaultFnc= null) {
            if(c1(obj)) f1(obj);
            else if(c2(obj)) f2(obj);
            else if(defaultFnc != null) defaultFnc(obj);
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, (_) => {
                if(c3(obj)) f3(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, (_) => {
                if(c4(obj)) f4(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                            Func<A,bool> c5, Action<A> f5,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, c4, f4, (_) => {
                if(c5(obj)) f5(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                            Func<A,bool> c5, Action<A> f5,
                                            Func<A,bool> c6, Action<A> f6,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, (_) => {
                if(c6(obj)) f6(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                            Func<A,bool> c5, Action<A> f5,
                                            Func<A,bool> c6, Action<A> f6,
                                            Func<A,bool> c7, Action<A> f7,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, (_) => {
                if(c7(obj)) f7(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                            Func<A,bool> c5, Action<A> f5,
                                            Func<A,bool> c6, Action<A> f6,
                                            Func<A,bool> c7, Action<A> f7,
                                            Func<A,bool> c8, Action<A> f8,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, (_) => {
                if(c8(obj)) f8(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }
        public static void choice<A>(A obj, Func<A,bool> c1, Action<A> f1,
                                            Func<A,bool> c2, Action<A> f2,
                                            Func<A,bool> c3, Action<A> f3,
                                            Func<A,bool> c4, Action<A> f4,
                                            Func<A,bool> c5, Action<A> f5,
                                            Func<A,bool> c6, Action<A> f6,
                                            Func<A,bool> c7, Action<A> f7,
                                            Func<A,bool> c8, Action<A> f8,
                                            Func<A,bool> c9, Action<A> f9,
                                                             Action<A> defaultFnc= null) {
            choice<A>(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, (_) => {
                if(c9(obj)) f9(obj);
                else if(defaultFnc != null) defaultFnc(obj);
            });
        }

    }

}
