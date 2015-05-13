using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // until :: (a->bool)->[a]->bool
        public static bool until<A>(Func<A,bool> fnc, A[] lst) {
            foreach(var obj in lst) if(fnc(obj)) return true;
            return false;
        }
        public static bool until<A>(Func<A,bool> fnc, List<A> lst) {
            foreach(var obj in lst) if(fnc(obj)) return true;
            return false;
        }
        // ----------------------------------------------------------------------
        // until :: (a->bool)->(a->a)->a->a
        public static A until<A>(Func<A,bool> cond, Func<A,A> fnc, A value) {
            while(cond(value)) value= fnc(value);
            return value;
        }
    }

}
