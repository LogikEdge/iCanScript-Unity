using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Test")]
public class iCS_TestGenericFunction {
    public float myFloat;
    
    [iCS_Function] public iCS_TestGenericFunction(int i) { Debug.Log("Running constructor");}
    [iCS_Function] public bool IsA<T>(T b) { return false; }

    public override string ToString() { return "Working"; }
    
    public class TestClass {
        public int x= 1;
        public int y= 2;
    }
    public enum MyEnum { Red, Blue, Yellow }; 

    [iCS_Function] public int Add(TestClass v, MyEnum e) { return v.x+v.y; }
}
