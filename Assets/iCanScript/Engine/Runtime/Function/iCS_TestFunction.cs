using UnityEngine;
using System;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Test")]
public class iCS_TestGenericFunction {
    public float myFloat;
	public string myString= "working";
    public GameObject	myGameObject= null;

    [iCS_Function] public iCS_TestGenericFunction() { myFloat= 0f;}
    [iCS_Function] public iCS_TestGenericFunction(char[] v) { foreach(var c in v) myString+= c; }
    [iCS_Function] public bool IsA<T>(T b) { return false; }

    public override string ToString() { return myString; }
    
    public class TestClass {
        public int x= 1;
        public int y= 2;
    }
    public enum MyEnum { Red, Blue, Yellow }; 

    [iCS_Function] public int Add(TestClass[] testArray, TestClass v, MyEnum e, TestClass[] boolArray) { return v.x+v.y; }
    [iCS_Function] public static int TypeNameLen(Type t) { return t.Name.Length; }

	[iCS_Function] public void ArrayTest(UnityEngine.Object[] uo, Type[] types) { }

	public bool IsValid {
		[iCS_Function] get { return true; }
		[iCS_Function] set { }
	}
	public static bool StaticIsValid {
		[iCS_Function] get { return true; }
		[iCS_Function] set { }
	}
	[iCS_InPort] public bool myInPort= false;
	[iCS_OutPort] public bool myOutPort= false;
	[iCS_InOutPort] public bool myInOutPort= true;
}
