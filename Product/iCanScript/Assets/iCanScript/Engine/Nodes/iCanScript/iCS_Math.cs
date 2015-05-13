using UnityEngine;
using System.Collections;

namespace iCanScript.MathUtility {

	[iCS_Class(Company="iCanScript", Library="Math", HideClassFromLibrary=true)]
	public static class Math {
	    [iCS_Function(Return="a+b",Icon="iCS_CalculatorIcon.psd")]
	        public static int     Add(int a, int b)         { return a+b; }
	    [iCS_Function(Return="a+b",Icon="iCS_CalculatorIcon.psd")]
	        public static float   Add(float a, float b)     { return a+b; }
	    [iCS_Function(Return="a+b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector2 Add(Vector2 a, Vector2 b) { return a+b; }
	    [iCS_Function(Return="a+b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector3 Add(Vector3 a, Vector3 b) { return a+b; }
	    [iCS_Function(Return="a+b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector4 Add(Vector4 a, Vector4 b) { return a+b; }

	    [iCS_Function(Return="a-b",Icon="iCS_CalculatorIcon.psd")]
	        public static int     Sub(int a, int b)         { return a-b; }
	    [iCS_Function(Return="a-b",Icon="iCS_CalculatorIcon.psd")]
	        public static float   Sub(float a, float b)     { return a-b; }
	    [iCS_Function(Return="a-b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector2 Sub(Vector2 a, Vector2 b) { return a-b; }
	    [iCS_Function(Return="a-b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector3 Sub(Vector3 a, Vector3 b) { return a-b; }
	    [iCS_Function(Return="a-b",Icon="iCS_CalculatorIcon.psd")]
	        public static Vector4 Sub(Vector4 a, Vector4 b) { return a-b; }

	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static int     Mul(int a, int b)         { return a*b; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static float   Mul(float a, float b)     { return a*b; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static float   op_Multiply(float a, float b)     { return a*b; }

	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static int     Div(int a, int div)       { return a/div; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static float   Div(float a, float div)   { return a/div; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static Vector2 Div(Vector2 v, float div) { return v/div; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static Vector3 Div(Vector3 v, float div) { return v/div; }
	    [iCS_Function(Icon="iCS_CalculatorIcon.psd")] public static Vector4 Div(Vector4 v, float div) { return v/div; }

	    [iCS_Function] public static int     Lerp(int v1, int v2, float ratio)            { return (int)(v1+(v2-v1)*ratio); }
	    [iCS_Function] public static float   Lerp(float v1, float v2, float ratio)        { return v1+(v2-v1)*ratio; }
	    [iCS_Function] public static Vector2 Lerp(Vector2 v1, Vector2 v2, float ratio)    { return v1+(v2-v1)*ratio; }
	    [iCS_Function] public static Vector3 Lerp(Vector3 v1, Vector3 v2, float ratio)    { return v1+(v2-v1)*ratio; }
	    [iCS_Function] public static Vector4 Lerp(Vector4 v1, Vector4 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    
	    [iCS_Function(Name="Random",Icon="iCS_RandomIcon_32x32.png")] public static float   Random(float scale= 1f)         { return scale*UnityEngine.Random.value; }
	    [iCS_Function(Name="Random",Icon="iCS_RandomIcon_32x32.png")] public static Vector2 RandomVector2(float scale= 1f)  { return scale*UnityEngine.Random.insideUnitCircle; }
	    [iCS_Function(Name="Random",Icon="iCS_RandomIcon_32x32.png")] public static Vector3 RandomVector3(float scale= 1f)  { return scale*UnityEngine.Random.insideUnitSphere; }
    
	    [iCS_Function] public static Vector2 ScaleVector(float scale, Vector2 v) { return scale*v; }
	    [iCS_Function] public static Vector3 ScaleVector(float scale, Vector3 v) { return scale*v; }
	    [iCS_Function] public static Vector4 ScaleVector(float scale, Vector4 v) { return scale*v; }

	    [iCS_Function] public static Vector2 Scale2Vector(float s1, float s2, Vector2 v) { return s1*s2*v; }
	    [iCS_Function] public static Vector3 Scale3Vector(float s1, float s2, Vector3 v) { return s1*s2*v; }
	    [iCS_Function] public static Vector4 Scale4Vector(float s1, float s2, Vector4 v) { return s1*s2*v; }
    
	    [iCS_Function(Description="Returns the normalized cross product.")]
	    public static Vector3 NormalizedCross(Vector3 v1, Vector3 v2) { return Vector3.Cross(v1,v2).normalized; }
	}
	
}

