using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_ConnectionParams {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Vector2 Start       = Vector2.zero;
    public Vector2 End         = Vector2.zero;
    public Vector2 StartTangent= Vector2.zero;
    public Vector2 EndTangent  = Vector2.zero;
    public Vector2 Center      = Vector2.zero;

    // ======================================================================
	// Constants
    // ----------------------------------------------------------------------
    public static readonly Vector2 UpDirection   = new Vector2(0,-1);
    public static readonly Vector2 DownDirection = new Vector2(0,1);
    public static readonly Vector2 RightDirection= new Vector2(1,0);
    public static readonly Vector2 LeftDirection = new Vector2(-1,0);
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public WD_ConnectionParams(WD_EditorObject port, WD_IStorage storage) {
        // Can only get Bezier parameters from port with a source.
        if(!storage.IsValid(port.Source)) {
            Debug.LogError(port.Name+": Cannot obtain connection parameters for a port without a source !!!");
            return;
        }

        WD_EditorObject source= storage.GetSource(port);
        Rect sourcePos= storage.GetPosition(source);
        Rect portPos  = storage.GetPosition(port);
        Start= new Vector2(sourcePos.x, sourcePos.y);
        End= new Vector2(portPos.x, portPos.y);

        // Compute Bezier tangents.
        Vector2 startDir= ConnectionDirectionForTo(source, port, storage);
        Vector2 endDir= ConnectionDirectionForTo(port, source, storage);
		Vector2 vertex= End-Start;
        StartTangent= Start + startDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector2.Dot(startDir, vertex)));
        EndTangent  = End + endDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector2.Dot(endDir, vertex)));

        // Compute connection center point.
        Center= BezierCenter(Start, End, StartTangent, EndTangent);
    }
    // ----------------------------------------------------------------------
    static Vector2 ConnectionDirectionForTo(WD_EditorObject port, WD_EditorObject to, WD_IStorage storage) {
        Vector2 direction;
        if(port.IsOnLeftEdge) {
            direction= LeftDirection;
        } else if(port.IsOnRightEdge) {
            direction= RightDirection;
        } else if(port.IsOnTopEdge) {
            direction= UpDirection;
        } else {
            direction= DownDirection;
        }
        // Inverse direction for connection between nested nodes.
        WD_EditorObject portParent= storage.GetParent(port);
        WD_EditorObject toParent= storage.GetParent(to);
        if(storage.IsChildOf(toParent, portParent)) {
            direction= -direction;
        }
        return direction;
    }
    // ----------------------------------------------------------------------
    public static Vector3 BezierCenter(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent) {
        // A simple linear interpolation suffices for facing tangents.
        Vector3 point= 0.5f*(start+end);
        return ClosestPointBezier(point, start, end, startTangent, endTangent);
    }
    // ----------------------------------------------------------------------
    public static Vector3 ClosestPointBezier(Vector3 point, Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, int iteration= 1) {
        // Have we finished iterating ?
        if(iteration == 0) return point;
        
        // Get distance from bezier curve.
        float distance= HandleUtility.DistancePointBezier(point, start, end, startTangent, endTangent);

        // Point is on Bezier so just return it.
        if(Math3D.IsZero(distance)) return point;
        
        Vector3 px= point;
        px.x+= distance;
        Vector3 py= point;
        py.y+= distance;
        float dx= HandleUtility.DistancePointBezier(px, start, end, startTangent, endTangent);
        float dy= HandleUtility.DistancePointBezier(py, start, end, startTangent, endTangent);
        float xSign= 1f;
        float ySign= 1f;
        if(dx > distance) {
            dx= 2f*distance-dx;
            xSign= -1f;
        }
        if(dy > distance) {
            dy= 2f*distance-dy;
            ySign= -1f;
        }
        float fx= 1f-dx/distance;
        float fy= 1f-dy/distance;
        point.x+= xSign*fx*distance;
        point.y+= ySign*fy*distance;
        return ClosestPointBezier(point, start, end, startTangent, endTangent, iteration-1);        
    }

}
