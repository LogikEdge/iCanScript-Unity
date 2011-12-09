using UnityEngine;
using System.Collections;

public static partial class Prelude {
    public static void exchange<A>(ref A v1, ref A v2) {
        A tmp= v1;
        v1= v2;
        v2= tmp;
    }
}
