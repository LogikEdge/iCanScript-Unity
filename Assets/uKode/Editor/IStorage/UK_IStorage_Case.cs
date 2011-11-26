using UnityEngine;
using System;
using System.Collections;

public partial class UK_IStorage {
    public static void ExecuteIf(UK_EditorObject obj, Func<UK_EditorObject,bool> cond, Action<UK_EditorObject> f) {
        Prelude.executeIf<UK_EditorObject>(obj,cond,f);
    }
    public void ExecuteIf(int id, Func<UK_EditorObject,bool> cond, Action<UK_EditorObject> f) {
        if(!IsValid(id)) return;
        ExecuteIf(EditorObjects[id], cond, f);
    }
    public static void Case(UK_EditorObject obj,
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, defaultFnc);
    }
    public static void Case(UK_EditorObject obj, 
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, defaultFnc);
    }
    public static void Case(UK_EditorObject obj, 
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, defaultFnc);
    }
    public static void Case(UK_EditorObject obj, 
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                     Func<UK_EditorObject,bool> c5, Action<UK_EditorObject> f5,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, defaultFnc);
    }
    public static void Case(UK_EditorObject obj, 
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                     Func<UK_EditorObject,bool> c5, Action<UK_EditorObject> f5,
                     Func<UK_EditorObject,bool> c6, Action<UK_EditorObject> f6,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, defaultFnc);
    }
    public static void Case(UK_EditorObject obj,
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                     Func<UK_EditorObject,bool> c5, Action<UK_EditorObject> f5,
                     Func<UK_EditorObject,bool> c6, Action<UK_EditorObject> f6,
                     Func<UK_EditorObject,bool> c7, Action<UK_EditorObject> f7,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, defaultFnc);
    }
    public static void Case(UK_EditorObject obj,
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                     Func<UK_EditorObject,bool> c5, Action<UK_EditorObject> f5,
                     Func<UK_EditorObject,bool> c6, Action<UK_EditorObject> f6,
                     Func<UK_EditorObject,bool> c7, Action<UK_EditorObject> f7,
                     Func<UK_EditorObject,bool> c8, Action<UK_EditorObject> f8,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, defaultFnc);
    }
    public static void Case(UK_EditorObject obj, 
                     Func<UK_EditorObject,bool> c1, Action<UK_EditorObject> f1,
                     Func<UK_EditorObject,bool> c2, Action<UK_EditorObject> f2,
                     Func<UK_EditorObject,bool> c3, Action<UK_EditorObject> f3,
                     Func<UK_EditorObject,bool> c4, Action<UK_EditorObject> f4,
                     Func<UK_EditorObject,bool> c5, Action<UK_EditorObject> f5,
                     Func<UK_EditorObject,bool> c6, Action<UK_EditorObject> f6,
                     Func<UK_EditorObject,bool> c7, Action<UK_EditorObject> f7,
                     Func<UK_EditorObject,bool> c8, Action<UK_EditorObject> f8,
                     Func<UK_EditorObject,bool> c9, Action<UK_EditorObject> f9,
                                                    Action<UK_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, c9, f9, defaultFnc);
    }
}
