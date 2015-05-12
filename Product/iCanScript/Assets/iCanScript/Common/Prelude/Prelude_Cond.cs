using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // cond :: (a->Bool)->(a->Void)->a->Void
        public static void cond<A>(Func<A,bool> c, Action<A> f, A x) {
            if(c(x)) { f(x); }
        }

        // ----------------------------------------------------------------------
        // Currying support.
        public static Action<A>             cond<A>(Func<A,bool> c, Action<A> f)  { return delegate (A x)        { cond(c,f,x); }; }
        public static Action<Action<A> >    cond<A>(Func<A,bool> c)               { return delegate(Action<A> f) { cond(c,f); }; }
    }

}
