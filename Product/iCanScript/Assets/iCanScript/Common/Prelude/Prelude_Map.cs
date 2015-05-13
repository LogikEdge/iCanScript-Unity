using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // map_ :: [b]->(a->b)->[a]->[b]
        public static B[] map_<A,B>(B[] result, Func<A,B> fnc, A[] l1) {
            int len= length(l1);
            if(len != length(result)) result= new B[len];
            for(int i= 0; i < len; ++i)
                result[i]= fnc(l1[i]);
            return result;
        }
        public static List<B> map_<A,B>(List<B> result, Func<A,B> fnc, List<A> l1) {
            int len= length(l1);
            result.Clear();
            if(result.Capacity < len) result.Capacity= len;        
    		forEach(i=> result.Add(fnc(i)), l1);
            return result;
        }

        // ----------------------------------------------------------------------
        // map :: (a->b)->[a]->[b]
        public static B[] map<A,B>(Func<A,B> fnc, A[] l1) {
            int len= length(l1);
            B[] result= new B[len];
            for(int i= 0; i < len; ++i)
                result[i]= fnc(l1[i]);
            return result;
        }
        public static List<B> map<A,B>(Func<A,B> fnc, List<A> l1) {
            return map_(new List<B>(), fnc, l1);
        }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Func<A[],B[]>         map<A,B>(Func<A,B> f) { return function<Func<A,B>,A[],B[]>(map,f); }
    }

}
