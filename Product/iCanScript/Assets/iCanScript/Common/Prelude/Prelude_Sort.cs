using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        public static void sort<T>(List<T> toSort, Func<T,T,int> cmp) {
            Comparison<T> compare= (x,y)=> cmp(x,y);
            toSort.Sort(compare);
        }
        // ----------------------------------------------------------------------
        public static void sort<T>(T[] toSort, Func<T,T,int> cmp) {
            Comparison<T> compare= (x,y)=> cmp(x,y);
            Array.Sort<T>(toSort, compare);
        }
    }

}
