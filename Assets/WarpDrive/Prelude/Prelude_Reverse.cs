using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    public static A[] reverse<A>(A[] lst) {
        A[] result= new A[lst.Length];
        for(int i= 0, j= lst.Length-1; i < lst.Length; ++i, --j) {
            result[j]= lst[i];
        }
        return result;
    }
    public static List<A> reverse<A>(List<A> lst) {
        List<A> result= new List<A>();
        for(int i= 0, j= lst.Count-1; i < lst.Count; ++i, --j) {
            result.Add(lst[j]);
        }
        return result;
    }
}
