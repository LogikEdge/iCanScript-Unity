using UnityEngine;
using System.Collections;

namespace iCanScript.Internal {
    
    public static partial class Math3D {

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
            if(Math3D.IsSmallerOrEqual(width,0.0f) || Math3D.IsSmallerOrEqual(height, 0.0f)) {
                width= 0.0f;
                height= 0.0f;
            }
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
            if(Math3D.IsZero(divider)) { intersection= Vector2.zero; return false; }
            // Compute interscetion
            float t1= x1*y2-y1*x2;
            float t2= x3*y4-y3*x4;
            float px= (t1*(x3-x4) - (x1-x2)*t2)/divider;
            float py= (t1*(y3-y4) - (y1-y2)*t2)/divider;
            intersection= new Vector2(px,py);
            return true;
        }
        // ----------------------------------------------------------------------
        // Determine the intersection point of two line segments if
        // they are not parallel.  "true" is returned if an intersection
        // point is found, otherwise 'false' is return for parallel lines.
        public static bool LineSegmentIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2, out Vector2 intersection) {
            bool isParallel= !LineIntersection(l1p1, l1p2, l2p1, l2p2, out intersection);
            if(isParallel) return false;
            // Determine if intersection point is on the given line segment.
            float l1xMin= l1p1.x; if(l1p2.x < l1p1.x) l1xMin= l1p2.x;
            float l1xMax= l1p1.x; if(l1p2.x > l1p1.x) l1xMax= l1p2.x;
            float l1yMin= l1p1.y; if(l1p2.y < l1p1.y) l1yMin= l1p2.y;
            float l1yMax= l1p1.y; if(l1p2.y > l1p1.y) l1yMax= l1p2.y;
            float l2xMin= l2p1.x; if(l2p2.x < l2p1.x) l2xMin= l2p2.x;
            float l2xMax= l2p1.x; if(l2p2.x > l2p1.x) l2xMax= l2p2.x;
            float l2yMin= l2p1.y; if(l2p2.y < l2p1.y) l2yMin= l2p2.y;
            float l2yMax= l2p1.y; if(l2p2.y > l2p1.y) l2yMax= l2p2.y;
            bool isValidX= Math3D.IsWithinOrEqual(intersection.x, l1xMin, l1xMax) &&
                           Math3D.IsWithinOrEqual(intersection.x, l2xMin, l2xMax);
            bool isValidY= Math3D.IsWithinOrEqual(intersection.y, l1yMin, l1yMax) &&
                           Math3D.IsWithinOrEqual(intersection.y, l2yMin, l2yMax);
            return isValidX && isValidY;                
        }
        // ----------------------------------------------------------------------
        // Determines the intersection point of a line segment and the edge
        // of a rectangle. "true" is returned if an intersection is found.
        public static bool LineSegmentAndRectEdgeIntersection(Vector2 lp1, Vector2 lp2, Rect rect, out Vector2 intersection) {
            if(LineSegmentIntersection(lp1, lp2, TopLeftCorner(rect), TopRightCorner(rect), out intersection)) {
                return true;
            }
            if(LineSegmentIntersection(lp1, lp2, BottomLeftCorner(rect), BottomRightCorner(rect), out intersection)) {
                return true;
            }
            if(LineSegmentIntersection(lp1, lp2, TopLeftCorner(rect), BottomLeftCorner(rect), out intersection)) {
                return true;
            }
            if(LineSegmentIntersection(lp1, lp2, TopRightCorner(rect), BottomRightCorner(rect), out intersection)) {
                return true;
            }
            return false;
        }
    }

}
