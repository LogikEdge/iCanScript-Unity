using UnityEngine;
using System.Collections;

public static partial class Math3D {
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Min / Max Utilities.
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public static int Min(int a, int b) { return a < b ? a : b; }
    public static int Max(int a, int b) { return a > b ? a : b; }
    public static int Min(int a, int b, int c) { return a < b ? (a < c ? a : c) : (b < c ? b : c); }
    public static int Max(int a, int b, int c) { return a > b ? (a > c ? a : c) : (b > c ? b : c); }
    public static int Min(int a, int b, int c, int d) { return Min(a, Min(b,c,d)); }
    public static int Max(int a, int b, int c, int d) { return Max(a, Max(b,c,d)); }
    public static int Min(int a, int b, int c, int d, int e) { return Min(a, Min(b,c,d), e); }
    public static int Max(int a, int b, int c, int d, int e) { return Max(a, Max(b,c,d), e); }

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Float conditional statement using Epsilon.
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public static bool IsEqual(float a, float b) {
        return Mathf.Approximately(a,b);
    }  
    // ----------------------------------------------------------------------
    public static bool IsNotEqual(float a, float b) {
        return !IsEqual(a,b);
    }
    // ----------------------------------------------------------------------
    public static bool IsSmaller(float a, float b) {
        return a < b && IsNotEqual(a,b);
    }
    // ----------------------------------------------------------------------
    public static bool IsSmallerOrEqual(float a, float b) {
        return a < b || IsEqual(a,b);
    }
    // ----------------------------------------------------------------------
    public static bool IsGreater(float a, float b) {
        return a > b && IsNotEqual(a,b);
    }
    // ----------------------------------------------------------------------
    public static bool IsGreaterOrEqual(float a, float b) {
        return a > b || IsEqual(a,b);
    }
    // ----------------------------------------------------------------------
    public static bool IsZero(float a) {
        return IsEqual(a,0f);
    }
    // ----------------------------------------------------------------------
    public static bool IsNotZero(float a) {
        return !IsEqual(a,0f);
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
    // ----------------------------------------------------------------------
    public static float SignWithZero(float a) {
		if(IsZero(a)) return 0f;
		return Mathf.Sign(a);
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
    public static bool IsEqual(Rect r1, Rect r2) {
        return IsEqual(r1.x, r2.x) &&
               IsEqual(r1.y, r2.y) &&
               IsEqual(r1.width, r2.width) &&
               IsEqual(r1.height, r2.height);
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
    public static bool IsNotEqual(Rect r1, Rect r2) {
        return !IsEqual(r1, r2);
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

    // ======================================================================
    // Lerp
	// ----------------------------------------------------------------------
    public static int     Lerp(int v1, int v2, float ratio)             { return (int)(v1+(v2-v1)*ratio); }
    public static float   Lerp(float v1, float v2, float ratio)         { return v1+(v2-v1)*ratio; }
    public static Vector2 Lerp(Vector2 v1, Vector2 v2, float ratio)     { return v1+(v2-v1)*ratio; }
    public static Vector3 Lerp(Vector3 v1, Vector3 v2, float ratio)     { return v1+(v2-v1)*ratio; }
    public static Vector4 Lerp(Vector4 v1, Vector4 v2, float ratio)     { return v1+(v2-v1)*ratio; }
    public static Rect    Lerp(Rect r1, Rect r2, float ratio)           { return Add(r1, Mul(Sub(r2, r1), ratio)); }

    // ======================================================================
    // Rectangle utilities
	// ----------------------------------------------------------------------
    public static Rect Union(Rect _rect1, Rect _rect2) {
        float xMin= Mathf.Min(_rect1.xMin, _rect2.xMin);
        float yMin= Mathf.Min(_rect1.yMin, _rect2.yMin);
        float xMax= Mathf.Max(_rect1.xMax, _rect2.xMax);
        float yMax= Mathf.Max(_rect1.yMax, _rect2.yMax);
        return new Rect(xMin, yMin, xMax-xMin, yMax-yMin);
    }
	// ----------------------------------------------------------------------
    public static Rect BuildRect(Vector2 pos, Vector2 size) {
        return new Rect(pos.x, pos.y, size.x, size.y);
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
	public static Vector2 TopLeftCorner(Rect r) {
	    return new Vector2(r.xMin, r.yMin);
	}
    // ----------------------------------------------------------------------
	public static Vector2 TopRightCorner(Rect r) {
	    return new Vector2(r.xMax, r.yMin);
	}
    // ----------------------------------------------------------------------
	public static Vector2 BottomLeftCorner(Rect r) {
	    return new Vector2(r.xMin, r.yMax);
	}
    // ----------------------------------------------------------------------
	public static Vector2 BottomRightCorner(Rect r) {
	    return new Vector2(r.xMax, r.yMax);
	}
	
    // ======================================================================
    // Line segment utilities
	// ----------------------------------------------------------------------
	public static float DistanceFromHorizontalLineSegment(Vector2 point, float x1, float x2, float y) {
		if(x1 > x2) {
			var tmp= x1;
			x1= x2;
			x2= tmp;
		}
		float distance= Mathf.Abs(point.y-y);
		if(point.x >= x1 && point.x <= x2) return distance;
        float x= (point.x < x1) ? x1 : x2;
        float dx= x-point.x;
        float dy= y-point.y;
		return Mathf.Sqrt(dx*dx+dy*dy);
	}
    // ----------------------------------------------------------------------
	public static float DistanceFromVerticalLineSegment(Vector2 point, float y1, float y2, float x) {
		if(y1 > y2) {
			var tmp= y1;
			y1= y2;
			y2= tmp;
		}
		float distance= Mathf.Abs(point.x-x);
		if(point.y >= y1 && point.y <= y2) return distance;
		float y= (point.y < y1) ? y1 : y2;
        float dx= x-point.x;
        float dy= y-point.y;
		return Mathf.Sqrt(dx*dx+dy*dy);
	}

    // ======================================================================
    // Area utilities
	// ----------------------------------------------------------------------
	public static float Area(float x, float y)	{ return x*y; }
	public static float Area(Vector2 v)			{ return Area(v.x, v.y); }
	public static float Area(Rect r)			{ return Area(r.width, r.height); }
}
