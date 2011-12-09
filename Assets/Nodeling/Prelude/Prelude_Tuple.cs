using UnityEngine;
using System;
using System.Collections;

public static partial class Prelude {
    public class Tuple<A, B> {
        public Tuple() {
        }

        public Tuple(A first, B second) {
            this.Item1 = first;
            this.Item2 = second;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
    };
    public class Tuple<A,B,C> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
    };
    public class Tuple<A,B,C,D> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3, D item4) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
        public D Item4 { get; set; }
    };
    public class Tuple<A,B,C,D,E> {
        public Tuple() {
        }

        public Tuple(A item1, B item2, C item3, D item4, E item5) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }

        public A Item1 { get; set; }
        public B Item2 { get; set; }
        public C Item3 { get; set; }
        public D Item4 { get; set; }
        public E Item5 { get; set; }
    };
}
