using UnityEngine;
using iCanScript.Internal;

namespace iCanScript.Variables {
	
	public static class SimpleVariables {
	    public static bool      _bool  (bool value)    { return value; }
	    public static int       _int   (int value)     { return value; }
	    public static float     _float (float value)   { return value; }
	    public static string    _string(string value)  { return value; }
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
	    int myValue;
    
	    public int Value {
	        get { return myValue; }
	        set { myValue= value; }
	    }
	    public int Negate {
	        get { return -myValue; }
	    }
	    public int Abs {
	        get { return myValue < 0 ? -myValue : myValue; }
	    }
	    public int Sign {
	        get { return myValue < 0 ? -1 : 1; }
	    }
    
	    public Int(int init= 0) { myValue= init; }
	    public int Add(int b) { return myValue+b; }
	    public int Sub(int b) { return myValue-b; }
	    public int Mul(int b) { return myValue*b; }
	    public int Div(int b) { return myValue/b; }
	    public int AddAndUpdate(int b) { return myValue= myValue+b; }
	    public int SubAndUpdate(int b) { return myValue= myValue-b; }
	    public int MulAndUpdate(int b) { return myValue= myValue*b; }
	    public int DivAndUpdate(int b) { return myValue= myValue/b; }
	    public bool IsEqual(int b)            { return myValue==b; }
	    public bool IsNotEqual(int b)         { return myValue!=b; }
	    public bool IsGreaterThen(int b)      { return myValue > b; }
	    public bool IsSmallerThen(int b)      { return myValue < b; }
	    public bool IsGreaterOrEqualTo(int b) { return myValue >= b; }
	    public bool IsSmallerOrEqualTo(int b) { return myValue <= b; }    

		public static bool operator ==(Int a, int b)  { return a.myValue == b; }
		public static bool operator !=(Int a, int b)  { return a.myValue != b; }
		public static bool operator <(Int a, int b)   { return a.myValue < b; }
		public static bool operator >(Int a, int b)   { return a.myValue > b; }
		public static bool operator <=(Int a, int b)  { return a.myValue <= b; }
		public static bool operator >=(Int a, int b)  { return a.myValue >= b; }
		public static int  operator +(Int a, int b)   { return a.myValue + b; }
		public static int  operator +(Int a, Int b)   { return a.myValue + b.myValue; }
		public static int  operator -(Int a, int b)   { return a.myValue - b; }
		public static int  operator -(int a, Int b)   { return a - b.myValue; }
		public static int  operator -(Int a, Int b)   { return a.myValue - b.myValue; }
	
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
	    float myValue;
    
	    public float Value {
	        get { return myValue; }
	        set { myValue= value; }
	    }
	    public float Negate {
	        get { return -myValue; }
	    }
	    public float Abs {
	        get { return Mathf.Abs(myValue); }
	    }
	    public float Sign {
	        get { return Mathf.Sign(myValue); }
	    }
    
	    public Float(float init= 0f) 			{ myValue= init; }
	    public float Add(float b) 				{ return myValue+b; }
	    public float Sub(float b) 				{ return myValue-b; }
	    public float Mul(float b) 				{ return myValue*b; }
	    public float Div(float b) 				{ return myValue/b; }
	    public float AddAndUpdate(float b) 		{ return myValue= myValue+b; }
	    public float SubAndUpdate(float b) 		{ return myValue= myValue-b; }
	    public float MulAndUpdate(float b) 		{ return myValue= myValue*b; }
	    public float DivAndUpdate(float b) 		{ return myValue= myValue/b; }
	    public bool IsEqual(float b)            { return Math3D.IsEqual(myValue, b); }
	    public bool IsNotEqual(float b)         { return Math3D.IsNotEqual(myValue, b); }
	    public bool IsGreaterThen(float b)      { return Math3D.IsGreater(myValue, b); }
	    public bool IsSmallerThen(float b)      { return Math3D.IsSmaller(myValue, b); }
	    public bool IsGreaterOrEqualTo(float b) { return Math3D.IsGreaterOrEqual(myValue, b); }
	    public bool IsSmallerOrEqualTo(float b) { return Math3D.IsSmallerOrEqual(myValue, b); }    
	}

}
