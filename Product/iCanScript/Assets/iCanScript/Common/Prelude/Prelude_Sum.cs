using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // add :: a->a->a
        public static float add(float a, float b) { return a+b; }
        public static int   add(int   a, int   b) { return a+b; }
    
        // ----------------------------------------------------------------------
        // sum :: [a]->a
        public static float sum(float[] l) {
            return fold(add, 0.0f, l);
        }
        public static float sum(List<float> l) {
            return fold(add, 0.0f, l);
        }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Func<float,float> add(float a) { return function<float,float,float>(add, a); }
        public static Func<int,int>     add(int   a) { return function<int,  int,  int>  (add, a); }

    }

}
