using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public class Maybe<A> {
    	public bool isNothing { get { return this is Nothing<A>; }}
    	public bool isJust    { get { return this is Just<A>; }}
    	public B maybe<B>(B nothingValue, Func<A,B> f) {
    		return isNothing ? nothingValue : f((this as Just<A>).value);
    	}
    	public A Value {
    		get {
    			if(isNothing) {
    				throw new SystemException("Trying to access Maybe value of Nothing !!!");
    			}
    			return (this as Just<A>).value;
    		}
    	}
    }
    public class Nothing<A> : Maybe<A> {}
    public class Just<A>    : Maybe<A> {
    	public A value;
    	public Just(A v) { value= v; }
    }
	
    public static partial class Prelude {
    	public static R maybe<A,R>(R nothingValue, Func<A,R> f, Maybe<A> m) {
    		return m.maybe(nothingValue, f);
    	}
    }

}
