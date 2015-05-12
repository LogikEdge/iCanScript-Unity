using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        public static void ExecuteIf(iCS_EditorObject obj, Func<iCS_EditorObject,bool> cond, Action<iCS_EditorObject> f) {
            Prelude.executeIf<iCS_EditorObject>(obj,cond,f);
        }
        public void ExecuteIf(int id, Func<iCS_EditorObject,bool> cond, Action<iCS_EditorObject> f) {
            if(!IsValid(id)) return;
            ExecuteIf(EditorObjects[id], cond, f);
        }
        public static void Case(iCS_EditorObject obj,
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj, 
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj, 
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj, 
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                         Func<iCS_EditorObject,bool> c5, Action<iCS_EditorObject> f5,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj, 
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                         Func<iCS_EditorObject,bool> c5, Action<iCS_EditorObject> f5,
                         Func<iCS_EditorObject,bool> c6, Action<iCS_EditorObject> f6,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj,
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                         Func<iCS_EditorObject,bool> c5, Action<iCS_EditorObject> f5,
                         Func<iCS_EditorObject,bool> c6, Action<iCS_EditorObject> f6,
                         Func<iCS_EditorObject,bool> c7, Action<iCS_EditorObject> f7,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj,
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                         Func<iCS_EditorObject,bool> c5, Action<iCS_EditorObject> f5,
                         Func<iCS_EditorObject,bool> c6, Action<iCS_EditorObject> f6,
                         Func<iCS_EditorObject,bool> c7, Action<iCS_EditorObject> f7,
                         Func<iCS_EditorObject,bool> c8, Action<iCS_EditorObject> f8,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, defaultFnc);
        }
        public static void Case(iCS_EditorObject obj, 
                         Func<iCS_EditorObject,bool> c1, Action<iCS_EditorObject> f1,
                         Func<iCS_EditorObject,bool> c2, Action<iCS_EditorObject> f2,
                         Func<iCS_EditorObject,bool> c3, Action<iCS_EditorObject> f3,
                         Func<iCS_EditorObject,bool> c4, Action<iCS_EditorObject> f4,
                         Func<iCS_EditorObject,bool> c5, Action<iCS_EditorObject> f5,
                         Func<iCS_EditorObject,bool> c6, Action<iCS_EditorObject> f6,
                         Func<iCS_EditorObject,bool> c7, Action<iCS_EditorObject> f7,
                         Func<iCS_EditorObject,bool> c8, Action<iCS_EditorObject> f8,
                         Func<iCS_EditorObject,bool> c9, Action<iCS_EditorObject> f9,
                                                        Action<iCS_EditorObject> defaultFnc= null) {
            Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, c9, f9, defaultFnc);
        }
    }

}
