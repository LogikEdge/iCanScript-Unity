using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // reverse :: [a]->[a]
        public static A[] reverse<A>(A[] lst) {
            A[] result= new A[length(lst)];
            for(int i= 0, j= length(lst)-1; i < length(lst); ++i, --j) {
                result[j]= lst[i];
            }
            return result;
        }
        public static List<A> reverse<A>(List<A> lst) {
            List<A> result= new List<A>();
            for(int i= 0, j= length(lst)-1; i < length(lst); ++i, --j) {
                result.Add(lst[j]);
            }
            return result;
        }
    }

}
