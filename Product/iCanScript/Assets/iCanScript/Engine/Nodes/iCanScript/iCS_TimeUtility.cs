using UnityEngine;
using System.Collections;
using iCanScript.Internal;

namespace iCanScript.TimeUtility {

	public static class DeltaTimeUtility {
	    [iCS_Function(Return="value*dt")]
	    public static float ScaleByDeltaTime(float value= 1f) {
	        return Time.deltaTime*value;
	    }
	    [iCS_Function(Return="value*dt")]
	    public static Vector2 ScaleByDeltaTime(Vector2 value) {
	        return Time.deltaTime*value;
	    }
	    [iCS_Function(Return="value*dt")]
	    public static Vector3 ScaleByDeltaTime(Vector3 value) {
	        return Time.deltaTime*value;
	    }
	    [iCS_Function(Return="value*dt")]
	    public static Vector4 ScaleByDeltaTime(Vector4 value) {
	        return Time.deltaTime*value;
	    }
	    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
	    public static float LerpUsingDeltaTime(float v1, float v2, float ratio) {
	        return Mathf.Lerp(v1,v2,ratio*Time.deltaTime);
	    }
	    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
	    public static Vector2 LerpUsingDeltaTime(Vector2 v1, Vector2 v2, float ratio) {
	        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
	    }
	    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
	    public static Vector3 LerpUsingDeltaTime(Vector3 v1, Vector3 v2, float ratio) {
	        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
	    }
	    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
	    public static Vector4 LerpUsingDeltaTime(Vector4 v1, Vector4 v2, float ratio) {
	        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
	    }
	}
	
}
