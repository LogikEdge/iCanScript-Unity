using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
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
    
}
