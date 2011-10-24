using UnityEngine;
using System.Collections;

public class Physics2D {

    // ======================================================================
    // COLLISION FUNCTIONS
	// ----------------------------------------------------------------------
    // Returns true if the given rectangles overlap.
    public static bool DoesCollide(Rect _rect1, Rect _rect2) {
        Rect merge= Merge(_rect1, _rect2);
        return merge.width < (_rect1.width + _rect2.width) && merge.height < (_rect1.height + _rect2.height);        
    }

    // ----------------------------------------------------------------------
    // Returns the smallest rectangle that incompasses the two given
    // rectangles.
    public static Rect Merge(Rect _rect1, Rect _rect2) {
        float xMin= Mathf.Min(_rect1.xMin, _rect2.xMin);
        float yMin= Mathf.Min(_rect1.yMin, _rect2.yMin);
        float xMax= Mathf.Max(_rect1.xMax, _rect2.xMax);
        float yMax= Mathf.Max(_rect1.yMax, _rect2.yMax);
        return new Rect(xMin, yMin, xMax-xMin, yMax-yMin);
    }

    // ----------------------------------------------------------------------
    // Returns the intersection between two rectangles.
    public static Rect Intersection(Rect _rect1, Rect _rect2) {
        float xMin= Mathf.Max(_rect1.xMin, _rect2.xMin);
        float yMin= Mathf.Max(_rect1.yMin, _rect2.yMin);
        float xMax= Mathf.Min(_rect1.xMax, _rect2.xMax);
        float yMax= Mathf.Min(_rect1.yMax, _rect2.yMax);
        float width= xMax-xMin;
        float height= yMax-yMin;
        if(MathfExt.IsSmallerOrEqual(width,0.0f)) width= 0.0f;
        if(MathfExt.IsSmallerOrEqual(height, 0.0f)) height= 0.0f;
        return new Rect(xMin, yMin, width, height);
    }

    // ----------------------------------------------------------------------
    // Determine the interscetion point of two infinit size lines if
    // they are not parallel.  "true" is returned if an intersection
    // point is found, otherwise 'false' is return for parallel lines.
    public static bool LineIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2, out Vector2 intersection) {
        float x1= l1p1.x;
        float y1= l1p1.y;
        float x2= l1p2.x;
        float y2= l1p2.y;
        float x3= l2p1.x;
        float y3= l2p1.y;
        float x4= l2p2.x;
        float y4= l2p2.y;
        // Parallel lines don't intersect.
        float divider= (x1-x2)*(y3-y4) - (y1-y2)*(x3-x4);
        if(MathfExt.IsZero(divider)) { intersection= Vector2.zero; return false; }
        // Compute interscetion
        float t1= x1*y2-y1*x2;
        float t2= x3*y4-y3*x4;
        float px= (t1*(x3-x4) - (x1-x2)*t2)/divider;
        float py= (t1*(y3-y4) - (y1-y2)*t2)/divider;
        intersection= new Vector2(px,py);
        return true;
    }
    // ----------------------------------------------------------------------
    // Determine the interscetion point of two line segments if
    // they are not parallel.  "true" is returned if an intersection
    // point is found, otherwise 'false' is return for parallel lines.
    public static bool LineSegmentIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2, out Vector2 intersection) {
        bool isParallel= !LineIntersection(l1p1, l1p2, l2p1, l2p2, out intersection);
        if(isParallel) return false;
        // Determine if intersection point is on the given line segment.
        bool isValidX= ((intersection.x <= l1p1.x && intersection.x >= l1p1.x) ||
                        (intersection.x >= l1p1.x && intersection.x <= l1p1.x));
        bool isValidY= ((intersection.y <= l1p1.y && intersection.y >= l1p1.y) ||
                        (intersection.y >= l1p1.y && intersection.y <= l1p1.y));
        return isValidX && isValidY;                
    }
}
