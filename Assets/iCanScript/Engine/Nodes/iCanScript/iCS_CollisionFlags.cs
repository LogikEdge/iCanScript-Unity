using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public static class iCS_CollisionFlags {

    [iCS_Function(Return="isTouchingAbove")]
    public static bool IsTouchingAbove(CollisionFlags flags) {
        return (flags & CollisionFlags.Above) != 0;
    }
    [iCS_Function(Return="isTouchingBelow")]
    public static bool IsTouchingBelow(CollisionFlags flags) {
        return (flags & CollisionFlags.Below) != 0;
    }
    [iCS_Function(Return="isTouchingSides")]
    public static bool IsTouchingSides(CollisionFlags flags) {
        return (flags & CollisionFlags.Sides) != 0;
    }

}
