using UnityEngine;
using System.Collections;

namespace iCanScript.Conversions {

	public static class FromTo {
	//    [iCS_Function(Icon="iCS_SplitIcon_32x32.psd")]
	//	public static void    FromVector(Vector2 v, out float x, out float y)                           { x= v.x; y= v.y; }
	    [iCS_Function(Icon="iCS_SplitIcon_32x32.psd")]
		public static void    FromVector(Vector3 v, out float x, out float y, out float z)              { x= v.x; y= v.y; z= v.z; }
	    [iCS_Function(Icon="iCS_SplitIcon_32x32.psd")]
		public static void    FromVector(Vector4 v, out float x, out float y, out float z, out float w) { x= v.x; y= v.y; z= v.z; w= v.w; }

	    [iCS_Function(Icon="iCS_JoinIcon_32x32.psd")] public static Vector2 ToVector(float x, float y)                   { return new Vector2(x,y); }
	    [iCS_Function(Icon="iCS_JoinIcon_32x32.psd")] public static Vector3 ToVector(float x, float y, float z)          { return new Vector3(x,y,z); }
	    [iCS_Function(Icon="iCS_JoinIcon_32x32.psd")] public static Vector4 ToVector(float x, float y, float z, float w) { return new Vector4(x,y,z,w); }
	}
	
}
