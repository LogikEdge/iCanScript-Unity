using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Test")]
public class iCS_TestGenericFunction {
    [iCS_Function] public iCS_TestGenericFunction(int i) {}
    [iCS_Function] public bool IsA<T>(T b) { return false; }
}
