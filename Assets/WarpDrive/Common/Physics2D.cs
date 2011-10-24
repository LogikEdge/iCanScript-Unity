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
    // Line intersection
    public static bool LineIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2) {
        float x1= l1p1.x;
        float y1= l1p1.y;
        float x2= l1p2.x;
        float y2= l1p2.y;
        float x3= l2p1.x;
        float y3= l2p1.y;
        float x4= l2p2.x;
        float y4= l2p2.y;
        float px= ((x1*y2-y1*x2)*(x3-x4) - (x1-x2)*(x3*y4-y3*x4))/((x1-x2)*(y3-y4) - (y1-y2)*(x3-x4));
        float py= ((x1*y2-y1*x2)*(y3-y4) - (y1-y2)*(x3*y4-y3*x4))/((x1-x2)*(y3-y4) - (y1-y2)*(x3-x4));
        return true;
    }
}
