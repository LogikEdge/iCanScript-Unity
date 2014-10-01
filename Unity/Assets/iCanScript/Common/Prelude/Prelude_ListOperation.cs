using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // isEmpty    :: [a]->bool
    // isNotEmpty :: [a]->bool
    public static bool isEmpty<A>(A[] lst)    { return lst.Length == 0; }
    public static bool isNotEmpty<A>(A[] lst) { return !isEmpty(lst); }

    // ----------------------------------------------------------------------
    // emptyLst :: typeof A
    public static A[] empty<A>()    { return new A[0]; }
    
    // ----------------------------------------------------------------------
    // concat :: a->[a]->[a]
    public static A[] concat<A>(A a, A[] lst) {
        A[] newLst= new A[lst.Length+1];
        newLst[0]= a;
        for(int i= 0; i < lst.Length; ++i) newLst[i+1]= lst[i];
        return newLst;
    }
    
    // ----------------------------------------------------------------------
    // head :: [a]->a
    // last :: [a]->a
    // tail :: [a]->[a]
    public static A head<A>(A[] lst) { return lst[0]; }
    public static A last<A>(A[] lst) { return lst[lst.Length-1]; }
    public static A[] tail<A>(A[] lst)  {
        if(isEmpty(lst)) return new A[0];
        A[] newLst= new A[lst.Length-1];
        for(int i= 1; i < lst.Length; ++i) {
            newLst[i-1]= lst[i];
        }
        return newLst;
    }

    // ----------------------------------------------------------------------
    // insertAt :: a->Int->[a]->[a]
    public static A[] insertAt<A>(A a, int idx, A[] lst) {
        var newLst= new A[lst.Length+1];
        if(idx > 0) {
            Array.Copy(lst, newLst, idx);
        }
        newLst[idx]= a;
        if(idx < lst.Length) {
            Array.Copy(lst, idx, newLst, idx+1, lst.Length-idx);
        }
        return newLst;
    }

    // ----------------------------------------------------------------------
    // append :: [a]->[a]->[a]
    public static A[] append<A>(A[] a, A[] b) {
        var aLen= length(a);
        var bLen= length(b);
        var result= new A[aLen+bLen];
        for(int i= 0; i < aLen; ++i) {
            result[i]= a[i];
        }
        for(int i= 0; i < bLen; ++i) {
            result[aLen+i]= b[i];
        }
        return result;
    }
    public static List<A> append<A>(List<A> a, List<A> b) {
        var result= new List<A>(a);
        result.AddRange(b);
        return result;
    }

    // ----------------------------------------------------------------------
    // removeDuplicates :: [a]->[a]
    public static A[] removeDuplicates<A>(A[] a) {
        var aLen= length(a);
        var result= new A[aLen];
        var rLen= 0;
        for(int i= 0; i < aLen; ++i) {
            bool found= false;
            var element= a[i];
            for(int j= 0; j < rLen; ++j) {
                if(element.Equals(result[j])) {
                    found= true;
                    break;
                }
            }
            if(found == false) {
                result[rLen]= element;
                ++rLen;
            }
        }
        Array.Resize(ref result, rLen);
        return result;
    }
    public static List<A> removeDuplicates<A>(List<A> a) {
        var result= new List<A>(a);
        for(int i= 0; i < length(result); ++i) {
            var element= result[i];
            for(int j= i+1; j < length(result);) {
                if(element.Equals(result[j])) {
                    result.RemoveAt(j);
                }
                else {
                    ++j;
                }
            }
        }
        return result;
    }
}
