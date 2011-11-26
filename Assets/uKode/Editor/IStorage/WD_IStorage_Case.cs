using UnityEngine;
using System;
using System.Collections;

public partial class WD_IStorage {
    public static void ExecuteIf(WD_EditorObject obj, Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> f) {
        Prelude.executeIf<WD_EditorObject>(obj,cond,f);
    }
    public void ExecuteIf(int id, Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> f) {
        if(!IsValid(id)) return;
        ExecuteIf(EditorObjects[id], cond, f);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, defaultFnc);
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
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, defaultFnc);
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
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, defaultFnc);
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
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, c9, f9, defaultFnc);
    }
}
