using UnityEngine;
using Object=System.Object;
using iCanScript.Internal;

namespace iCanScript.Logic {

	public static class Conditions {
	    // Object comparaison.
	    public static bool IsNull(object obj, out bool isNotNull)
	        { isNotNull= obj != null; return !isNotNull; }
	
	    // Float comparaison.
	    public static bool IsZero(float a, out bool isNotZero)
	        { isNotZero= Math3D.IsNotZero(a); return !isNotZero; }
	    public static bool IsEqual(float a, float b, out bool isNotEqual)
	        { isNotEqual= Math3D.IsNotEqual(a,b); return !isNotEqual; }
	    public static bool IsGreater(float value, float bias, out bool isSmallerOrEqual)
	        { isSmallerOrEqual= Math3D.IsSmallerOrEqual(value,bias); return !isSmallerOrEqual; }
	    public static bool IsSmaller(float value, float bias, out bool isGreaterOrEqual)
	        { isGreaterOrEqual= Math3D.IsGreaterOrEqual(value,bias); return !isGreaterOrEqual; }
	    public static bool IsGreaterOrEqual(float value, float bias, out bool isSmaller)
	        { isSmaller= Math3D.IsSmaller(value,bias); return !isSmaller; }
	    public static bool IsSmallerOrEqual(float value, float bias, out bool isGreater)
	        { isGreater= Math3D.IsGreater(value,bias); return !isGreater; }
	    [iCS_Function(Return="isEqual")]
	    public static bool Compare(float value, float bias, out bool isSmaller,
	                                                        out bool isGreater,
	                                                        out bool isSmallerOrEqual,
	                                                        out bool isGreaterOrEqual) {
	        bool isEqual= Math3D.IsEqual(value, bias);
	        isSmaller= Math3D.IsSmaller(value, bias);
	        isGreater= Math3D.IsGreater(value, bias);
	        isSmallerOrEqual= isSmaller || isEqual;
	        isGreaterOrEqual= isGreater || isEqual;
	        return isEqual;
	    }
	
	    // Int comparaison.
	    public static bool IsZero(int a, out bool isNotZero)
	        { isNotZero= a != 0; return !isNotZero; }
	    public static bool IsEqual(int a, int b, out bool isNotEqual)
	        { isNotEqual= a != b; return !isNotEqual; }
	    public static bool IsGreater(int value, int bias, out bool isSmallerOrEqual)
	        { isSmallerOrEqual= value <= bias; return !isSmallerOrEqual; }
	    public static bool IsSmaller(int value, int bias, out bool isGreaterOrEqual)
	        { isGreaterOrEqual= value >= bias; return !isGreaterOrEqual; }
	    public static bool IsGreaterOrEqual(int value, int bias, out bool isSmaller)
	        { isSmaller= value < bias; return !isSmaller; }
	    public static bool IsSmallerOrEqual(int value, int bias, out bool isGreater)
	        { isGreater= value > bias; return !isGreater; }
	    [iCS_Function(Return="isEqual")]
	    public static bool Compare(int value, int bias, out bool isSmaller,
	                                                    out bool isGreater,
	                                                    out bool isSmallerOrEqual,
	                                                    out bool isGreaterOrEqual) {
	        bool isEqual  = value == bias;
	        isSmaller= value < bias;
	        isGreater= value > bias;
	        isSmallerOrEqual= isSmaller || isEqual;
	        isGreaterOrEqual= isGreater || isEqual;
	        return isEqual;
	    }
	
	    // String comparaison.
	    [iCS_Function(Return="isNullOrEmpty")]
	    public static bool IsNullOrEmpty(string a, out bool Else)
	        { bool isNullOrEmpty= string.IsNullOrEmpty(a); Else= !isNullOrEmpty; return isNullOrEmpty; }
	    [iCS_Function(Return="isEqual")]
	    public static bool IsEqual(string value, string bias, out bool isNotEqual)
	        { isNotEqual= string.Compare(value,bias) != 0; return !isNotEqual; }
	    [iCS_Function(Return="isGreater")]
	    public static bool IsGreater(string value, string bias, out bool isSmallerOrEqual)
	        { isSmallerOrEqual= string.Compare(value,bias) <= 0; return !isSmallerOrEqual; }
	    [iCS_Function(Return="isSmaller")]
	    public static bool IsSmaller(string value, string bias, out bool isGreaterOrEqual)
	        { isGreaterOrEqual= string.Compare(value,bias) >= 0; return !isGreaterOrEqual; }
	    [iCS_Function(Return="isGreaterOrEqual")]
	    public static bool IsGreaterOrEqual(string value, string bias, out bool isSmaller)
	        { isSmaller= string.Compare(value,bias) < 0; return !isSmaller; }
	    [iCS_Function(Return="isSmallerOrEqual")]
	    public static bool IsSmallerOrEqual(string value, string bias, out bool isGreater)
	        { isGreater= string.Compare(value,bias) > 0; return !isGreater; }
	    [iCS_Function(Return="isEqual")]
	    public static bool Compare(string value, string bias, out bool isSmaller,
	                                                          out bool isGreater,
	                                                          out bool isSmallerOrEqual,
	                                                          out bool isGreaterOrEqual) {
	        int result= string.Compare(value, bias);
	        bool isEqual= result == 0;
	        isSmaller= result < 0;
	        isGreater= result > 0;
	        isSmallerOrEqual= isSmaller || isEqual;
	        isGreaterOrEqual= isGreater || isEqual;
	        return isEqual;
	    }
	}
	
	public static class Choices {
		public static bool   Choice(bool trueValue,  bool falseValue,  bool sel) { return sel ? trueValue : falseValue; }
		public static float  Choice(float trueValue,  float falseValue,  bool sel) { return sel ? trueValue : falseValue; }
		public static int    Choice(int trueValue,    int falseValue,    bool sel) { return sel ? trueValue : falseValue; }
		public static string Choice(string trueValue, string falseValue, bool sel) { return sel ? trueValue : falseValue; }
		public static Object Choice(Object trueValue, Object falseValue, bool sel) { return sel ? trueValue : falseValue; }
	}
}