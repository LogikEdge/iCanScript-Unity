using UnityEngine;
using System;
using System.Collections;

public class WDEd {
    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    // Executes the given action if the given object matches the T type.
    public static void ExecuteIf(WD_EditorObject obj, Func<WD_EditorObject,bool> cmp, Action<WD_EditorObject> f) {
        if(cmp(obj)) f(obj);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(c5(obj)) f5(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(c5(obj)) f5(obj);
        else if(c6(obj)) f6(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(c5(obj)) f5(obj);
        else if(c6(obj)) f6(obj);
        else if(c7(obj)) f7(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                     Func<WD_EditorObject,bool> c8, Action<WD_EditorObject> f8,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(c5(obj)) f5(obj);
        else if(c6(obj)) f6(obj);
        else if(c7(obj)) f7(obj);
        else if(c8(obj)) f8(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                     Func<WD_EditorObject,bool> c8, Action<WD_EditorObject> f8,
                     Func<WD_EditorObject,bool> c9, Action<WD_EditorObject> f9,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        if(c1(obj)) f1(obj);
        else if(c2(obj)) f2(obj);
        else if(c3(obj)) f3(obj);
        else if(c4(obj)) f4(obj);
        else if(c5(obj)) f5(obj);
        else if(c6(obj)) f6(obj);
        else if(c7(obj)) f7(obj);
        else if(c8(obj)) f8(obj);
        else if(c8(obj)) f9(obj);
        else if(defaultFnc != null) defaultFnc(obj);
    }
}
