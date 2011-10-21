using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // sum :: [a]->a
    public static float sum(float[] lst) {
        return fold((a,b)=> a+b, 0.0f, lst);
    }
    public static float sum(List<float> lst) {
        return fold((a,b)=> a+b, 0.0f, lst);
    }
}
