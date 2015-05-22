using UnityEngine;
using iCanScript.Internal;

namespace iCanScript.Variables {
	
	public static class SimpleVariables {
	    [iCS_Function]  public static bool      _bool  (bool value)    { return value; }
	    [iCS_Function]  public static int       _int   (int value)     { return value; }
	    [iCS_Function]  public static float     _float (float value)   { return value; }
	    [iCS_Function]  public static string    _string(string value)  { return value; }
	}


	[System.Serializable]
	public struct Bool {
	    public bool myValue;
    
	    public bool Value {
	        get { return myValue; }
	        set { myValue= value; }
	    }
	    public bool Not {
	        get { return !myValue; }
	    }
	    public bool Inverse {
	        get { return !myValue; }        
	    }
    
	    public Bool(bool init= false) { myValue= init; }
	    public bool And(bool b) { return myValue & b; }
	    public bool Or(bool b)  { return myValue | b; }
	    public bool Xor(bool b) { return myValue ^ b; }
        public static bool operator &(Bool a, Bool b) { return a.myValue & b.myValue; }
        public static bool operator |(Bool a, Bool b) { return a.myValue | b.myValue; }
        public static bool operator ^(Bool a, Bool b) { return a.myValue | b.myValue; }
	}

	[System.Serializable]
	public struct Int {
	    public int myValue;
    
	    public int Value {
	        [iCS_Function] get { return myValue; }
	        [iCS_Function] set { myValue= value; }
	    }
	    public int Negate {
	        [iCS_Function] get { return -myValue; }
	    }
	    public int Abs {
	        [iCS_Function] get { return myValue < 0 ? -myValue : myValue; }
	    }
	    public int Sign {
	        [iCS_Function] get { return myValue < 0 ? -1 : 1; }
	    }
    
	    [iCS_Function] public Int(int init= 0) { myValue= init; }
	    [iCS_Function(Return="value+b")] public int Add(int b) { return myValue+b; }
	    [iCS_Function(Return="value-b")] public int Sub(int b) { return myValue-b; }
	    [iCS_Function(Return="value*b")] public int Mul(int b) { return myValue*b; }
	    [iCS_Function(Return="value/b")] public int Div(int b) { return myValue/b; }
	    [iCS_Function(Return="value+b")] public int AddAndUpdate(int b) { return myValue= myValue+b; }
	    [iCS_Function(Return="value-b")] public int SubAndUpdate(int b) { return myValue= myValue-b; }
	    [iCS_Function(Return="value*b")] public int MulAndUpdate(int b) { return myValue= myValue*b; }
	    [iCS_Function(Return="value/b")] public int DivAndUpdate(int b) { return myValue= myValue/b; }
	    [iCS_Function(Obsolete="Use 'operator ==' instead.")]  public bool IsEqual(int b)            { return myValue==b; }
	    [iCS_Function(Obsolete="Use 'operator !=' instead.")]  public bool IsNotEqual(int b)         { return myValue!=b; }
	    [iCS_Function(Obsolete="Use 'operator >' instead.")]    public bool IsGreaterThen(int b)      { return myValue > b; }
	    [iCS_Function(Obsolete="Use 'operator <' instead.")]    public bool IsSmallerThen(int b)      { return myValue < b; }
	    [iCS_Function(Obsolete="Use 'operator >=' instead.")]  public bool IsGreaterOrEqualTo(int b) { return myValue >= b; }
	    [iCS_Function(Obsolete="Use 'operator <=' instead.")]  public bool IsSmallerOrEqualTo(int b) { return myValue <= b; }    

		[iCS_Function(Return="a == b")]   public static bool operator ==(Int a, int b)  { return a.myValue == b; }
		[iCS_Function(Return="a != b")]   public static bool operator !=(Int a, int b)  { return a.myValue != b; }
		[iCS_Function(Return="a < b")]    public static bool operator <(Int a, int b)   { return a.myValue < b; }
		[iCS_Function(Return="a > b")]    public static bool operator >(Int a, int b)   { return a.myValue > b; }
		[iCS_Function(Return="a <= b")]   public static bool operator <=(Int a, int b)  { return a.myValue <= b; }
		[iCS_Function(Return="a >= b")]   public static bool operator >=(Int a, int b)  { return a.myValue >= b; }
		[iCS_Function(Return="a + b")]    public static int  operator +(Int a, int b)   { return a.myValue + b; }
		[iCS_Function(Return="a + b")]    public static int  operator +(Int a, Int b)   { return a.myValue + b.myValue; }
		[iCS_Function(Return="a - b")]    public static int  operator -(Int a, int b)   { return a.myValue - b; }
		[iCS_Function(Return="a - b")]    public static int  operator -(int a, Int b)   { return a - b.myValue; }
		[iCS_Function(Return="a - b")]    public static int  operator -(Int a, Int b)   { return a.myValue - b.myValue; }
	
		public override bool Equals(object o) {
			if(o is int) {
				return myValue == (int)o;
			}
			if(o is Int) {
				return myValue == ((Int)o).myValue;
			}
			return false;
		}
		public override int GetHashCode() { return myValue; }
	}

	[System.Serializable]
	public struct Float {
	    public float myValue;
    
	    public float Value {
	        [iCS_Function] get { return myValue; }
	        [iCS_Function] set { myValue= value; }
	    }
	    public float Negate {
	        [iCS_Function] get { return -myValue; }
	    }
	    public float Abs {
	        [iCS_Function] get { return Mathf.Abs(myValue); }
	    }
	    public float Sign {
	        [iCS_Function] get { return Mathf.Sign(myValue); }
	    }
    
	    [iCS_Function] public Float(float init= 0f) { myValue= init; }
	    [iCS_Function(Name="value+b",Return="value+b")] public float Add(float b) { return myValue+b; }
	    [iCS_Function(Name="value-b",Return="value-b")] public float Sub(float b) { return myValue-b; }
	    [iCS_Function(Name="value*b",Return="value*b")] public float Mul(float b) { return myValue*b; }
	    [iCS_Function(Name="value/b",Return="value/b")] public float Div(float b) { return myValue/b; }
	    [iCS_Function(Name="value= value+b",Return="value+b")] public float AddAndUpdate(float b) { return myValue= myValue+b; }
	    [iCS_Function(Name="value= value-b",Return="value-b")] public float SubAndUpdate(float b) { return myValue= myValue-b; }
	    [iCS_Function(Name="value= value*b",Return="value*b")] public float MulAndUpdate(float b) { return myValue= myValue*b; }
	    [iCS_Function(Name="value= value/b",Return="value/b")] public float DivAndUpdate(float b) { return myValue= myValue/b; }
	    [iCS_Function(Name="value == b",Return="value == b")]  public bool IsEqual(float b)            { return Math3D.IsEqual(myValue, b); }
	    [iCS_Function(Name="value != b",Return="value != b")]  public bool IsNotEqual(float b)         { return Math3D.IsNotEqual(myValue, b); }
	    [iCS_Function(Name="value > b",Return="value > b")]   public bool IsGreaterThen(float b)      { return Math3D.IsGreater(myValue, b); }
	    [iCS_Function(Name="value < b",Return="value < b")]   public bool IsSmallerThen(float b)      { return Math3D.IsSmaller(myValue, b); }
	    [iCS_Function(Name="value >= b",Return="value >= b")]  public bool IsGreaterOrEqualTo(float b) { return Math3D.IsGreaterOrEqual(myValue, b); }
	    [iCS_Function(Name="value <= b",Return="value <= b")]  public bool IsSmallerOrEqualTo(float b) { return Math3D.IsSmallerOrEqual(myValue, b); }    
	}

	//namespace System {
	//    [iCS_Class(Company="NET", Library="System")]
	//    public partial struct Int32 {
	//        [iCS_Function]
	//        public static int op_Multiply(int a, int b) { return a * b; }
	//    }    
	//    [iCS_Class(Company="NET", Library="System")]
	//    public partial struct Float {
	//        [iCS_Function]
	//        public static float op_Multiply(float a, float b) { return a * b; }
	//    }    
	//}

}
