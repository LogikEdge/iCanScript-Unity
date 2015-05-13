using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static partial class Prelude {

        public class Left<A> {
            public A Value {
                get { return myValue; }
                set { myValue = value; }
            }
            [SerializeField] A myValue= default(A);

            public Left(A a)    { Value= a; }
        }
        public class Right<A> {
            public A Value {
                get { return myValue; }
                set { myValue = value; }
            }
            [SerializeField] A myValue= default(A);

            public Right(A a)    { Value= a; }
        }
        public class Either<LEFT,RIGHT> {
            [SerializeField] System.Object  myValue= null;

            public bool IsLeft  { get { return (myValue as Left<LEFT>) != null; } }
            public bool IsRight { get { return (myValue as Right<RIGHT>) != null; } }
            public Left<LEFT> Left {
                get {
                    Left<LEFT> l= myValue as Left<LEFT>;
                    if(l == null) Debug.LogWarning("Trying to get Left of a right Either !!!");
                    return l;
                }
            }
            public Right<RIGHT> Right {
                get {
                    Right<RIGHT> r= myValue as Right<RIGHT>;
                    if(r == null) Debug.LogWarning("Trying to get Right of a left Either !!!");
                    return r;
                }
            }

            public Either(Right<RIGHT> r)   { Set(r); }
            public Either(Left<LEFT> l)     { Set(l); }
            public void Set(Right<RIGHT> r) { myValue= r; }
            public void Set(Left<LEFT> l)   { myValue= l; }
        }
        public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Either<LEFT,RIGHT> e) {
            return e.IsLeft ? f1(e.Left.Value) : f2(e.Right.Value);
        }
        public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Left<LEFT> e) {
            return f1(e.Value);
        }
        public static R either<LEFT,RIGHT,R>(Func<LEFT,R> f1, Func<RIGHT,R> f2, Right<RIGHT> e) {
            return f2(e.Value);
        }
    
    }

}
