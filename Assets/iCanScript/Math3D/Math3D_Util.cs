using UnityEngine;
using System.Collections;

public static partial class Math3D {

    // ======================================================================
    // Float functionality using Epsilon.
    // ----------------------------------------------------------------------
    public static bool IsEqual(float a, float b) {
        return Mathf.Abs(a-b) < Mathf.Epsilon;
    } 
    
    // ----------------------------------------------------------------------
    public static bool IsNotEqual(float a, float b) {
        return !IsEqual(a,b);
    }

    // ----------------------------------------------------------------------
    public static bool IsSmaller(float a, float b) {
        return (a + Mathf.Epsilon) < b;
    }

    // ----------------------------------------------------------------------
    public static bool IsSmallerOrEqual(float a, float b) {
        return (a - Mathf.Epsilon) < b;
    }

    // ----------------------------------------------------------------------
    public static bool IsGreater(float a, float b) {
        return (a - Mathf.Epsilon) > b;
    }

    // ----------------------------------------------------------------------
    public static bool IsGreaterOrEqual(float a, float b) {
        return (a + Mathf.Epsilon) > b;
    }

    // ----------------------------------------------------------------------
    public static bool IsZero(float a) {
        return IsEqual(a,0);
    }
	
    // ----------------------------------------------------------------------
    public static bool IsNotZero(float a) {
        return !IsEqual(a,0);
    }
	
    // ----------------------------------------------------------------------
    public static bool IsWithin(float a, float low, float high) {
        return IsGreater(a, low) && IsSmaller(a, high);
    }

    // ----------------------------------------------------------------------
    public static bool IsWithinOrEqual(float a, float low, float high) {
        return IsGreaterOrEqual(a, low) && IsSmallerOrEqual(a, high);
    }

    // ----------------------------------------------------------------------
    public static bool IsOutside(float a, float low, float high) {
        return IsSmaller(a, low) || IsGreater(a, high);
    }
    
    // ----------------------------------------------------------------------
    public static bool IsOutsideOrEqualWithEpsilon(float a, float low, float high) {
        return IsSmallerOrEqual(a, low) || IsGreaterOrEqual(a, high);
    }
    
    
    // ======================================================================
    // SIMPLE VECTOR FUNCTIONS
    // ----------------------------------------------------------------------
	public static bool IsZero(Vector2 v) {
		return IsZero(v.x) && IsZero(v.y);
	}
	
    // ----------------------------------------------------------------------
	public static bool IsNotZero(Vector2 v) {
		return !IsZero(v);
	}
	
	// ----------------------------------------------------------------------
	public static bool IsZero(Vector3 v) {
		return IsZero(v.x) && IsZero(v.y) && IsZero(v.y);
	}
	
	// ----------------------------------------------------------------------
	public static bool IsNotZero(Vector3 v) {
		return !IsZero(v);
	}
	
	// ----------------------------------------------------------------------
	public static bool IsZero(Vector4 v) {
		return IsZero(v.x) && IsZero(v.y) && IsZero(v.y) && IsZero(v.w);
	}
	
	// ----------------------------------------------------------------------
	public static bool IsNotZero(Vector4 v) {
		return !IsZero(v);
	}
	
	// ----------------------------------------------------------------------
    public static bool IsEqual(Vector2 v1, Vector2 v2) {
        return IsEqual(v1.x, v2.x) && IsEqual(v1.y, v2.y);
    }
    
	// ----------------------------------------------------------------------
    public static bool IsEqual(Vector3 v1, Vector3 v2) {
        return IsEqual(v1.x, v2.x) &&
               IsEqual(v1.y, v2.y) &&
               IsEqual(v1.z, v2.z);
    }
    
	// ----------------------------------------------------------------------
    public static bool IsEqual(Vector4 v1, Vector4 v2) {
        return IsEqual(v1.x, v2.x) &&
               IsEqual(v1.y, v2.y) &&
               IsEqual(v1.z, v2.z) &&
               IsEqual(v1.w, v2.w);
    }
    
	// ----------------------------------------------------------------------
    public static bool IsNotEqual(Vector2 v1, Vector2 v2) {
        return !IsEqual(v1, v2);
    }
    
	// ----------------------------------------------------------------------
    public static bool IsNotEqual(Vector3 v1, Vector3 v2) {
        return !IsEqual(v1, v2);
    }
    
	// ----------------------------------------------------------------------
    public static bool IsNotEqual(Vector4 v1, Vector4 v2) {
        return !IsEqual(v1, v2);
    }
    
	// ----------------------------------------------------------------------
    public static float GetAngle(Vector2 _from, Vector2 _to) {
        Vector2 vector= _to-_from;
        float angle= Vector2.Angle(Vector2.right, vector);
        if(!Math3D.IsZero(angle) && vector.y > 0) {
            angle= 360 - angle;
        }
        return angle;        
    }

	// ----------------------------------------------------------------------
    public static Vector2 ToVector2(Rect rect) {
        return new Vector2(rect.x, rect.y);
    }

	// ----------------------------------------------------------------------
    public static Vector2 Middle(Rect rect) {
        return new Vector2(rect.x+0.5f*rect.width, rect.y+0.5f*rect.height);
    }
	// ----------------------------------------------------------------------
    public static Rect Add(Rect r1, Rect r2) {
        return new Rect(r1.x+r2.x, r1.y+r2.y, r1.width+r2.width, r1.height+r2.height);
    }
	// ----------------------------------------------------------------------
    public static Rect Sub(Rect r1, Rect r2) {
        return new Rect(r1.x-r2.x, r1.y-r2.y, r1.width-r2.width, r1.height-r2.height);
    }
	// ----------------------------------------------------------------------
    public static Rect Mul(Rect r1, float d) {
        return new Rect(d*r1.x, d*r1.y, d*r1.width, d*r1.height);
    }
	// ----------------------------------------------------------------------
    public static Rect Div(Rect r1, float d) {
        if(IsZero(d)) return default(Rect);
        return Mul(r1, 1f/d);
    }
	// ----------------------------------------------------------------------
    public static Rect Lerp(Rect r1, Rect r2, float ratio) {
        return Add(r1, Mul(Sub(r2, r1), ratio));
    }
}
