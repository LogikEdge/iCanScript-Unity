using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // Type<->Type
        public static bool equal<T1,T2>() {
            return typeof(T1) == typeof(T2);
        }
        public static bool notEqual<T1,T2>() {
            return !equal<T1,T2>();
        }

        // ----------------------------------------------------------------------
        // Type<->Instance
        public static bool equal<T>(object obj) {
            return obj.GetType() == typeof(T);
        }
        public static bool notEqual<T>(object obj) {
            return !equal<T>(obj);
        }
    }

}
