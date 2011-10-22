using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // add :: a->a->a
    public static float add(float a, float b) { return a+b; }
    public static Func<float,float> add(float a) { return delegate(float b) { return add(a,b); }; }
    public static int add(int a, int b) { return a+b; }
    public static Func<int,int> add(int a) { return delegate(int b) { return add(a,b); }; }
    
    // ----------------------------------------------------------------------
    // sum :: [a]->a
    public static float sum(float[] lst) {
        return fold(add, 0.0f, lst);
    }
    public static float sum(List<float> lst) {
        return fold(add, 0.0f, lst);
    }
}
