using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // fold :: (a->b->a)->a->[b]->a
        public static A fold<A,B>(Func<A,B,A> fnc, A initialValue, B[] lst) {
            return foldl<A,B>(fnc, initialValue, lst);
        }
        public static A fold<A,B>(Func<A,B,A> fnc, A initialValue, List<B> lst) {
            return foldl<A,B>(fnc, initialValue, lst);
        }
        // ----------------------------------------------------------------------
        // foldl :: (a->b->a)->a->[b]->a
        public static A foldl<A,B>(Func<A,B,A> fnc, A initialValue, B[] lst) {
            A result= initialValue;
            foreach(var b in lst) {
                result= fnc(result, b);
            }
            return result;
        }
        public static A foldl<A,B>(Func<A,B,A> fnc, A initialValue, List<B> lst) {
            A result= initialValue;
            foreach(var b in lst) {
                result= fnc(result, b);
            }
            return result;
        }
        // ----------------------------------------------------------------------
        // foldr :: (a->b->b)->b->[a]->b
        public static B foldr<A,B>(Func<A,B,B> fnc, B initialValue, A[] lst) {
            B result= initialValue;
            for(int i= length(lst)-1; i >= 0; --i) {
                result= fnc(lst[i], result);
            }
            return result;
        }
        public static B foldr<A,B>(Func<A,B,B> fnc, B initialValue, List<A> lst) {
            B result= initialValue;
            for(int i= length(lst)-1; i >= 0; --i) {
                result= fnc(lst[i], result);
            }
            return result;
        }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Func<B[],A>         fold<A,B>(Func<A,B,A> f, A v)  { return function<Func<A,B,A>,A,B[],A>(fold,f,v); }
        public static Func<A,Func<B[],A>> fold<A,B>(Func<A,B,A> f)       { return function<Func<A,B,A>,A,B[],A>(fold,f); }
        public static Func<B[],A>         foldl<B,A>(Func<A,B,A> f, A v) { return function<Func<A,B,A>,A,B[],A>(foldl,f,v); }
        public static Func<A,Func<B[],A>> foldl<B,A>(Func<A,B,A> f)      { return function<Func<A,B,A>,A,B[],A>(foldl,f); }
        public static Func<A[],B>         foldr<A,B>(Func<A,B,B> f, B v) { return function<Func<A,B,B>,B,A[],B>(foldr,f,v); }
        public static Func<B,Func<A[],B>> foldr<A,B>(Func<A,B,B> f)      { return function<Func<A,B,B>,B,A[],B>(foldr,f); }
    }

}
