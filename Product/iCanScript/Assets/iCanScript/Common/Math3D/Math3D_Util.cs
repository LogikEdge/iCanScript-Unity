using UnityEngine;
using System.Collections;

namespace iCanScript.Internal {
    using P=iCanScript.Internal.Prelude;
    
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

        public static float Min(float a, float b) { return a < b ? a : b; }
        public static float Max(float a, float b) { return a > b ? a : b; }
        public static float Min(float a, float b, float c) { return a < b ? (a < c ? a : c) : (b < c ? b : c); }
        public static float Max(float a, float b, float c) { return a > b ? (a > c ? a : c) : (b > c ? b : c); }
        public static float Min(float a, float b, float c, float d) { return Min(a, Min(b,c,d)); }
        public static float Max(float a, float b, float c, float d) { return Max(a, Max(b,c,d)); }
        public static float Min(float a, float b, float c, float d, float e) { return Min(a, Min(b,c,d), e); }
        public static float Max(float a, float b, float c, float d, float e) { return Max(a, Max(b,c,d), e); }

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
    
        // ======================================================================
        // Direction utilities
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
    	public static Vector2 QuantizeAt90Degrees(Vector2 v) {
    		if(Mathf.Abs(v.x) > Mathf.Abs(v.y)) {
    			return v.x > 0 ? new Vector2(1f,0) : new Vector2(-1f,0);
    		}
    		return v.y > 0 ? new Vector2(0,1f) : new Vector2(0,-1f);
    	}
    	// ----------------------------------------------------------------------
        // Determines if the two given directions are pointing on the same side
    	public static bool IsSameSide(Vector2 d1, Vector2 d2) {
    	    return Vector2.Dot(d1,d2) > 0;
    	}
    	// ----------------------------------------------------------------------
        // Determines if the two given directions are pointing on the same side
    	public static bool IsSameSide(Vector3 d1, Vector3 d2) {
    	    return Vector3.Dot(d1,d2) > 0;
    	}
    	// ----------------------------------------------------------------------
        // Determines if the two given directions are pointing on opposite sides
    	public static bool IsOppositeSide(Vector2 d1, Vector2 d2) {
    	    return !IsSameSide(d1,d2);
    	}
    	// ----------------------------------------------------------------------
        // Determines if the two given directions are pointing on opposite sides
    	public static bool IsOppositeDirection(Vector3 d1, Vector3 d2) {
    	    return !IsSameSide(d1,d2);
    	}
    	// ----------------------------------------------------------------------
        public static bool IsSameSide(Vector2 refPoint, Vector2 point1, Vector2 point2) {
            var dir1= point1-refPoint;
            var dir2= point2-refPoint;
            return IsSameSide(dir1, dir2);
        }
    	// ----------------------------------------------------------------------
        public static bool IsSameSide(Vector3 refPoint, Vector3 point1, Vector3 point2) {
            var dir1= point1-refPoint;
            var dir2= point2-refPoint;
            return IsSameSide(dir1, dir2);
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
        public static Rect Union(Rect[] rs) {
            if(rs.Length == 0) return new Rect(0,0,0,0);
            return P.fold((acc,r) => Union(acc,r), rs[0], rs);
        }
    	// ----------------------------------------------------------------------
        public static Rect BuildRect(Vector2 pos, Vector2 size) {
            return new Rect(pos.x, pos.y, size.x, size.y);
        }
    	// ----------------------------------------------------------------------
        public static Rect BuildRectCenteredAt(Vector2 center, Vector2 size) {
            return new Rect(center.x-0.5f*size.x, center.y-0.5f*size.y, size.x, size.y);
        }
    	// ----------------------------------------------------------------------
        public static Rect BuildRectCenteredAt(Vector2 center, float width, float height) {
            return new Rect(center.x-0.5f*width, center.y-0.5f*height, width, height);
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
        // ----------------------------------------------------------------------
        public static Vector3 ClosestPointOnLineToPoint(Vector3 origin, Vector3 direction, Vector3 point) {
            var w= point-origin;
            float vsq= Vector3.Dot(direction, direction);
            float proj= Vector3.Dot(w, direction);
            return origin+(proj/vsq)*direction;
        }
        // ----------------------------------------------------------------------
        public static Vector3 ClosestPointOnLineSegmentToPoint(Vector3 origin, Vector3 end, Vector3 point) {
            var direction= end-origin;
            var closestPoint= ClosestPointOnLineToPoint(origin, direction, point);
            // Determine if closest point is within line segment.
            var toOrigin= closestPoint-origin;
            var toEnd= closestPoint-end;
            if(Vector3.Dot(toOrigin, toEnd) < 0) {
                return closestPoint;
            }
            // Closest point is outside line segment. so return either origin or end...
            if(Vector3.Dot(toOrigin, toOrigin) > Vector3.Dot(toEnd, toEnd)) {
                return end;
            }
            return origin;
        }
    
        // ======================================================================
        // Area utilities
    	// ----------------------------------------------------------------------
    	public static float Area(float x, float y)	{ return x*y; }
    	public static float Area(Vector2 v)			{ return Area(v.x, v.y); }
    	public static float Area(Rect r)			{ return Area(r.width, r.height); }

        // ======================================================================
        // Vector list utilities
    	// ----------------------------------------------------------------------
        public static Vector2 Sum(Vector2[] lst) {
            return P.fold((v, acc)=> v+acc, Vector2.zero, lst);
        }
    	// ----------------------------------------------------------------------
        public static Vector3 Sum(Vector3[] lst) {
            return P.fold((v, acc)=> v+acc, Vector3.zero, lst);
        }
    	// ----------------------------------------------------------------------
        public static Vector2 Average(Vector2[] lst) {
            if(lst.Length == 0) return Vector2.zero;
            return Sum(lst)/lst.Length;
        }
    	// ----------------------------------------------------------------------
        public static Vector3 Average(Vector3[] lst) {
            if(lst.Length == 0) return Vector3.zero;
            return Sum(lst)/lst.Length;
        }

        // ======================================================================
        // Polygon utilities
    	// ----------------------------------------------------------------------
        public static Vector2[] ScaleAndTranslatePolygon(Vector2[] polygon, Vector2 scale, Vector2 translate) {
            var newPolygon= new Vector2[polygon.Length];
            for(int i= 0; i < polygon.Length; ++i) {
                newPolygon[i]= new Vector2(translate.x+polygon[i].x*scale.x,
                                           translate.y+polygon[i].y*scale.y);
            }
            return newPolygon;
        }
    	// ----------------------------------------------------------------------
        public static Vector2[] FlipPolygonVertically(Vector2[] polygon, float flipPoint) {
            var newPolygon= new Vector2[polygon.Length];
            for(int i= 0; i < polygon.Length; ++i) {
                newPolygon[i]= new Vector2(-(polygon[i].x-flipPoint), polygon[i].y);
            }
            return newPolygon;
        }    
    	// ----------------------------------------------------------------------
        public static Vector2[] FlipPolygonHorizontally(Vector2[] polygon, float flipPoint) {
            var newPolygon= new Vector2[polygon.Length];
            for(int i= 0; i < polygon.Length; ++i) {
                newPolygon[i]= new Vector2(polygon[i].x, -(polygon[i].y-flipPoint));
            }
            return newPolygon;
        }
        // ----------------------------------------------------------------------
        public static Vector2[] Rotate90DegreesPolygon(Vector2[] polygon) {
            var newPolygon= new Vector2[polygon.Length];
            for(int i= 0; i < polygon.Length; ++i) {
                newPolygon[i]= new Vector2(polygon[i].y, -polygon[i].x);
            }
            return newPolygon;        
        }
        // ----------------------------------------------------------------------
        public static Vector2 ClosestPointOnPolygonToPoint(Vector2[] polygon, Vector2 point) {
            Vector2 closestPoint= Vector2.zero;
            if(polygon.Length < 3) return closestPoint;
            float minSqrMagnitude= Mathf.Infinity;
            Vector2 segmentStart= polygon[polygon.Length-1];
            for(int i= 0; i < polygon.Length; ++i) {
                // Return if point is outside polygon.
                Vector2 segmentEnd= polygon[i];
                Vector2 pointOnPolygon= ClosestPointOnLineSegmentToPoint(segmentStart, segmentEnd, point);
                // Determine smallest distance to all polygon edges.
                var sqrMagnitude= (point-pointOnPolygon).sqrMagnitude;
                if(sqrMagnitude < minSqrMagnitude) {
                    minSqrMagnitude= sqrMagnitude;
                    closestPoint= pointOnPolygon;
                }
                segmentStart= segmentEnd;
            }
            return closestPoint;
        }
    }

}
