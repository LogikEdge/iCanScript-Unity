using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Test")]
public class iCS_TestGenericFunction {
    [iCS_Function] public iCS_TestGenericFunction(int i) { Debug.Log("Running constructor");}
    [iCS_Function] public bool IsA<T>(T b) { return false; }

    public override string ToString() { return "Working"; }
}
