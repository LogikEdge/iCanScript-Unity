using UnityEngine;
using System;
using System.Collections;

public static partial class Prelude {

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
}
