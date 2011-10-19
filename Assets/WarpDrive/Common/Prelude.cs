using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Functional Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public static class Prelude {
    
    // ======================================================================
    // Tuple
    // ----------------------------------------------------------------------
    public class Tuple<A, B> {
        public Tuple() {
        }

        public Tuple(A first, B second) {
            this.Item1 = first;
            this.Item2 = second;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
    };
    public class Tuple<A,B,C> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
    };
    public class Tuple<A,B,C,D> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3, D item4) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
        public D Item4 { get; set; }
    };
    public class Tuple<A,B,C,D,E> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3, D item4, E item5) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
        public D Item4 { get; set; }
        public E Item5 { get; set; }
    };
    
    // ======================================================================
    // Maybe
    // ----------------------------------------------------------------------
    public class Nothing {}
    public class Just<A> {
        public A Value {
            get { return myValue; }
            set { myValue = value; }
        }
        [SerializeField] A myValue= default(A);

        public Just(A a) { Value= a; }
    }
    public class Maybe<A> {
        [SerializeField] System.Object  myValue= null;

        public bool IsNothing { get { return (myValue as Nothing) != null; }}
        public bool IsJustA   { get { return (myValue as Just<A>) != null; }}

        public Just<A> Just {
            get {
                Just<A> j= myValue as Just<A>;
                if(j == null) Debug.LogWarning("Trying to get Just<A> of a nothing Maybe !!!");
                return j;                
            }
        }
        
        public Maybe()             { Set(new Nothing()); }
        public Maybe(Nothing n)    { Set(n); }
        public Maybe(Just<A> j)    { Set(j); }
        public void Set(Nothing n) { myValue= n; }
        public void Set(Just<A> j) { myValue= j; }
    }
    public static R maybe<A,R>(R nothingValue, Func<A,R> f, Maybe<A> m) {
        return m.IsNothing ? nothingValue : f(m.Just.Value);
    }
    public static R maybe<A,R>(R nothingValue, Func<A,R> f, Nothing m) {
        return nothingValue;
    }
    public static R maybe<A,R>(R nothingValue, Func<A,R> f, Just<A> j) {
        return f(j.Value);
    }
    
    
    // ======================================================================
    // Either
    // ----------------------------------------------------------------------
    public class Left<A> {
        public A Value {
            get { return myValue; }
            set { myValue = value; }
        }
        [SerializeField] A myValue= default(A);

        public Left(A a)    { Value= a; }
    }
    public class Right<A> {
        public A Value {
            get { return myValue; }
            set { myValue = value; }
        }
        [SerializeField] A myValue= default(A);

        public Right(A a)    { Value= a; }
    }
    public class Either<LEFT,RIGHT> {
        [SerializeField] System.Object  myValue= null;

        public bool IsLeft  { get { return (myValue as Left<LEFT>) != null; } }
        public bool IsRight { get { return (myValue as Right<RIGHT>) != null; } }
        public Left<LEFT> Left {
            get {
                Left<LEFT> l= myValue as Left<LEFT>;
                if(l == null) Debug.LogWarning("Trying to get Left of a right Either !!!");
                return l;
            }
        }
        public Right<RIGHT> Right {
            get {
                Right<RIGHT> r= myValue as Right<RIGHT>;
                if(r == null) Debug.LogWarning("Trying to get Right of a left Either !!!");
                return r;
            }
        }

        public Either(Right<RIGHT> r)   { Set(r); }
        public Either(Left<LEFT> l)     { Set(l); }
        public void Set(Right<RIGHT> r) { myValue= r; }
        public void Set(Left<LEFT> l)   { myValue= l; }
    }
    public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Either<LEFT,RIGHT> e) {
        return e.IsLeft ? f1(e.Left.Value) : f2(e.Right.Value);
    }
    public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Left<LEFT> e) {
        return f1(e.Value);
    }
    public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Right<RIGHT> e) {
        return f2(e.Value);
    }

    
    // ----------------------------------------------------------------------
    // This function can fixes a problem with C# implementation of the
    // Prelude by allowing a partial function of type 'f(a)(b,c)' to be
    // converted to 'f(a)(b)(c)' as follows 'fix(f)(b,c)'
    public static R fix<A,B,R>(System.Func<A,System.Func<B,R>> f, A a, B b) {
         return f(a)(b);
    }
    public static System.Func<A,B,R>  fix<A,B,R>(System.Func<A,System.Func<B,R>> f) {
        return delegate(A a, B b) { return fix(f,a,b); };
    }

    // ----------------------------------------------------------------------
    public static A id<A>(A a) { return a; }
    
    // ----------------------------------------------------------------------
    public static R dot<A,B,R>(System.Func<B,R> f1, System.Func<A,B> f2, A a) {
        return f1(f2(a));
    }
    public static System.Func<A,R> dot<A,B,R>(System.Func<B,R> f1, System.Func<A,B> f2) {
        return delegate(A a) { return dot(f1,f2,a); };
    }
    public static System.Func<System.Func<A,B>,System.Func<A,R>> dot<A,B,R>(System.Func<B,R> f1) {
        return delegate(System.Func<A,B> f2) { return dot(f1,f2); };
    }
    
    // ----------------------------------------------------------------------
    public static R flip<A,B,R>(System.Func<B,A,R> f, A a, B b) { return f(b,a); }
    public static Func<B,R> flip<A,B,R>(Func<B,A,R> f, A a) {
        return delegate(B b) { return flip(f,a,b); };
    }
    public static Func<A,Func<B,R>> flip<A,B,R>(Func<B,A,R> f) {
        return delegate(A a) { return flip(f,a); };
    }

    // ----------------------------------------------------------------------
    // Excutes the given action if the given object matches the T type.
    public static void executeIf<T>(System.Object _obj, System.Action<T> fnc) where T : class {
        T obj= _obj as T;
        if(obj != null) fnc(obj);
    }
    
    // ----------------------------------------------------------------------
    // Excutes the given function if the given object matches the T type.
    public static R executeIf<T,R>(System.Object _obj, System.Func<T,R> fnc, R defaultReturn) where T : class {
        T obj= _obj as T;
        return obj != null ? fnc(obj) : defaultReturn;
    }
    
    // ----------------------------------------------------------------------
    // Executes a case statement according to the type of the object
    public static void choice<T1,T2>(System.Object obj, System.Action<T1> f1,
                                                        System.Action<T2> f2,
                                                        System.Action<System.Object> defaultFnc) where T1 : class
                                                                                                 where T2 : class {
        T1 t1= obj as T1;
        if(t1 != null) { f1(t1); return; }
        T2 t2= obj as T2;
        if(t2 != null) { f2(t2); return; }
        defaultFnc(obj);
    }

    // ----------------------------------------------------------------------
    // Executes a case statement according to the type of the object
    public static void choice<T1,T2,T3>(System.Object obj, System.Action<T1> f1,
                                                           System.Action<T2> f2,
                                                           System.Action<T3> f3,
                                                           System.Action<System.Object> defaultFnc) where T1 : class
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
    public static void choice<T1,T2,T3,T4>(System.Object obj, System.Action<T1> f1,
                                                              System.Action<T2> f2,
                                                              System.Action<T3> f3,
                                                              System.Action<T4> f4,
                                                              System.Action<System.Object> defaultFnc) where T1 : class
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
    public static void choice<T1,T2,T3,T4,T5>(System.Object obj, System.Action<T1> f1,
                                                                 System.Action<T2> f2,
                                                                 System.Action<T3> f3,
                                                                 System.Action<T4> f4,
                                                                 System.Action<T5> f5,
                                                                 System.Action<System.Object> defaultFnc) where T1 : class
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
    //  List associated functions
    // ----------------------------------------------------------------------
    // Returns the length of a list or array.
    public static int length<T>(T[] a)      { return a.Length; }
    public static int length<T>(List<T> l)  { return l.Count; }
    
    // ----------------------------------------------------------------------
    // Filters a list according to the given condition.
    public static void filterWith<T>(System.Action<T> fnc, System.Func<T,bool> cond, T[] l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static void filterWith<T>(System.Action<T> fnc, System.Func<T,bool> cond, List<T> l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static T[] filter<T>(System.Func<T,bool> cond, T[] l) {
        List<T> result= new List<T>();
        filterWith<T>( (obj)=> { result.Add(obj); }, cond, l);
        return result.ToArray();
    }
    public static List<T> filter<T>(List<T> l, System.Func<T,bool> cond) {
        List<T> result= new List<T>();
        filterWith<T>( (obj)=> { result.Add(obj); }, cond, l);
        return result;
    }
    
    // ======================================================================
    // zip & zipWith
    // ----------------------------------------------------------------------
    // zipWith :: (f a->b->r)->[a]->[b]->[r]
    public static R[] zipWith_<A,B,R>(R[] result, System.Func<A,B,R> fnc, A[] l1, B[] l2) {
        int len= Inf.Min(length(l1), length(l2));
        if(length(result) != len) result= new R[len];
        for(int i= 0; i < len; ++i) {
            result[i]= fnc(l1[i], l2[i]);
        }
        return result;
    }
    public static List<R> zipWith_<A,B,R>(List<R> result, System.Func<A,B,R> fnc, List<A> l1, List<B> l2) {
        int len= Inf.Min(length(l1), length(l2));
        result.Clear();
        if(result.Capacity < len) result.Capacity= len;
        for(int i= 0; i < len; ++i) {
            result.Add(fnc(l1[i], l2[i]));
        }
        return result;
    }
    public static R[] zipWith<A,B,R>(System.Func<A,B,R> fnc, A[] l1, B[] l2) {
        int len= Inf.Min(length(l1), length(l2));
        R[] result= new R[len];
        for(int i= 0; i < len; ++i) {
            result[i]= fnc(l1[i], l2[i]);
        }
        return result;
    }
    public static List<R> zipWith<A,B,R>(System.Func<A,B,R> fnc, List<A> l1, List<B> l2) {
        return zipWith_(new List<R>(), fnc, l1, l2);
    }
    // ----------------------------------------------------------------------
    // zip :: [a]->[b]->[(a,b)]
    public static Tuple<A,B>[] zip<A,B>(A[] l1, B[] l2) {
        return zipWith<A,B,Tuple<A,B>>((a,b)=> new Tuple<A,B>(a,b), l1, l2);
    }
    public static List<Tuple<A,B>> zip<A,B>(List<A> l1, List<B> l2) {
        return zipWith<A,B,Tuple<A,B>>((a,b)=> new Tuple<A,B>(a,b), l1, l2);
    }
    // ----------------------------------------------------------------------
    // zipWith :: (f a->b->c->r)->[a]->[b]->[c]->[r]
    public static R[] zipWith_<A,B,C,R>(R[] result, System.Func<A,B,C,R> fnc, A[] l1, B[] l2, C[] l3) {
        int len= Inf.Min(length(l1), length(l2), length(l3));
        if(length(result) != len) result= new R[len];
        for(int i= 0; i < len; ++i) {
            result[i]= fnc(l1[i], l2[i], l3[i]);
        }
        return result;
    }
    public static List<R> zipWith_<A,B,C,R>(List<R> result, System.Func<A,B,C,R> fnc, List<A> l1, List<B> l2, List<C> l3) {
        int len= Inf.Min(length(l1), length(l2), length(l3));
        result.Clear();
        if(result.Capacity < len) result.Capacity= len;
        for(int i= 0; i < len; ++i) {
            result[i]= fnc(l1[i], l2[i], l3[i]);
        }
        return result;
    }
    public static R[] zipWith<A,B,C,R>(System.Func<A,B,C,R> fnc, A[] l1, B[] l2, C[] l3) {
        int len= Inf.Min(length(l1), length(l2), length(l3));
        R[] result= new R[len];
        for(int i= 0; i < len; ++i) {
            result[i]= fnc(l1[i], l2[i], l3[i]);
        }
        return result;
    }
    public static List<R> zipWith<A,B,C,R>( System.Func<A,B,C,R> fnc, List<A> l1, List<B> l2, List<C> l3) {
        return zipWith_(new List<R>(), fnc, l1, l2, l3);
    }
    // ----------------------------------------------------------------------
    // zip :: [a]->[b]->[c]->[(a,b,c)]
    public static Tuple<A,B,C>[] zip<A,B,C>(A[] l1, B[] l2, C[] l3) {
        return zipWith<A,B,C,Tuple<A,B,C>>((a,b,c)=> new Tuple<A,B,C>(a,b,c), l1, l2, l3);
    }
    public static List<Tuple<A,B,C>> zip<A,B,C>(List<A> l1, List<B> l2, List<C> l3) {
        return zipWith<A,B,C,Tuple<A,B,C>>((a,b,c)=> new Tuple<A,B,C>(a,b,c), l1, l2, l3);
    }
    // ----------------------------------------------------------------------
    // zipWith :: (f a->b->c->d->r)->[a]->[b]->[c]->[d]->[r]
    public static R[] zipWith_<A,B,C,D,R>(R[] result, System.Func<A,B,C,D,R> fnc, A[] l1, B[] l2, C[] l3, D[] l4) {
        int len= Inf.Min(length(l1), length(l2), length(l3), length(l4));
        if(length(result) != len) result= new R[len];
        for(int i= 0; i < len; ++i) {
            fnc(l1[i], l2[i], l3[i], l4[i]);
        }
        return result;
    }
    public static List<R> zipWith_<A,B,C,D,R>(List<R> result, System.Func<A,B,C,D,R> fnc, List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
        int len= Inf.Min(length(l1), length(l2), length(l3), length(l4));
        result.Clear();
        if(result.Capacity < len) result.Capacity= len;
        for(int i= 0; i < len; ++i) {
            result.Add(fnc(l1[i], l2[i], l3[i], l4[i]));
        }
        return result;
    }
    public static R[] zipWith<A,B,C,D,R>(System.Func<A,B,C,D,R> fnc, A[] l1, B[] l2, C[] l3, D[] l4) {
        int end= Inf.Min(length(l1), length(l2), length(l3), length(l4));
        R[] result= new R[end];
        for(int i= 0; i < end; ++i) {
            fnc(l1[i], l2[i], l3[i], l4[i]);
        }
        return result;
    }
    public static List<R> zipWith<A,B,C,D,R>(System.Func<A,B,C,D,R> fnc, List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
        int end= Inf.Min(length(l1), length(l2), length(l3), length(l4));
        List<R> result= new List<R>();
        for(int i= 0; i < end; ++i) {
            result.Add(fnc(l1[i], l2[i], l3[i], l4[i]));
        }
        return result;
    }
    // ----------------------------------------------------------------------
    // zip :: [a]->[b]->[c]->[d]->[(a,b,c,d)]
    public static Tuple<A,B,C,D>[] zip<A,B,C,D>(A[] l1, B[] l2, C[] l3, D[] l4) {
        return zipWith<A,B,C,D,Tuple<A,B,C,D>>((a,b,c,d)=> new Tuple<A,B,C,D>(a,b,c,d), l1, l2, l3, l4);
    }
    public static List<Tuple<A,B,C,D>> zip<A,B,C,D>(List<A> l1, List<B> l2, List<C> l3, List<D> l4) {
        return zipWith<A,B,C,D,Tuple<A,B,C,D>>((a,b,c,d)=> new Tuple<A,B,C,D>(a,b,c,d), l1, l2, l3, l4);
    }
    
    // ======================================================================
    // map
    // ----------------------------------------------------------------------
    // map :: (f a->b->c)->[a]->[b]->[c]
    public static B[] map_<A,B>(B[] result, System.Func<A,B> fnc, A[] l1) {
        int len= length(l1);
        if(len != length(result)) result= new B[len];
        for(int i= 0; i < len; ++i)
            result[i]= fnc(l1[i]);
        return result;
    }
    public static List<B> map_<A,B>(List<B> result, System.Func<A,B> fnc, List<A> l1) {
        int len= length(l1);
        result.Clear();
        if(result.Capacity < len) result.Capacity= len;        
        for(int i= 0; i < len; ++i)
            result[i]= fnc(l1[i]);
        return result;
    }
    public static B[] map<A,B>(System.Func<A,B> fnc, A[] l1) {
        int len= length(l1);
        B[] result= new B[len];
        for(int i= 0; i < len; ++i)
            result[i]= fnc(l1[i]);
        return result;
    }
    public static List<B> map<A,B>(System.Func<A,B> fnc, List<A> l1) {
        return map_(new List<B>(), fnc, l1);
    }
    
    // ======================================================================
    // fold
    // ----------------------------------------------------------------------
    public static B fold<A,B>(System.Func<A,B,B> fnc, B initialValue, A[] l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }
    public static B fold<A,B>(System.Func<A,B,B> fnc, B initialValue, List<A> l1) {
        B result= initialValue;
        foreach(var obj in l1)
            result= fnc(obj, result);
        return result;
    }

    // ======================================================================
    // curry & uncurry
    // ----------------------------------------------------------------------
    // curry :: (f (a,b)->r)->a->b->r
    // curry :: (f (a,b,c)->r)->a->b->c->r
    // curry :: (f (a,b,c,d)->r)->a->a->c->d->r
    public static R curry<A,B,R>(System.Func<Tuple<A,B>,R> fnc, A a, B b) {
        return fnc(new Tuple<A,B>(a,b));
    }
    public static R curry<A,B,C,R>(System.Func<Tuple<A,B,C>,R> fnc, A a, B b, C c) {
        return fnc(new Tuple<A,B,C>(a,b,c));
    }
    public static R curry<A,B,C,D,R>(System.Func<Tuple<A,B,C,D>,R> fnc, A a, B b, C c, D d) {
        return fnc(new Tuple<A,B,C,D>(a,b,c,d));
    }
    
    // ----------------------------------------------------------------------
    // uncurry :: (f a->b->r))->(a,b)->r
    // uncurry :: (f a->b->c->r)->(a,b,c)->r
    // uncurry :: (f a->b->c->d->r)->(a,b,c,d)->r
    public static R uncurry<A,B,R>(System.Func<A,B,R> fnc, Tuple<A,B> tuple) {
        return fnc(tuple.Item1, tuple.Item2);
    }
    public static R uncurry<A,B,C,R>(System.Func<A,B,C,R> fnc, Tuple<A,B,C> tuple) {
        return fnc(tuple.Item1, tuple.Item2, tuple.Item3);
    }
    public static R uncurry<A,B,C,D,R>(System.Func<A,B,C,D,R> fnc, Tuple<A,B,C,D> tuple) {
        return fnc(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
    }

    // ======================================================================
    // boolean logic
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
