using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ConnectionParams {
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
    public iCS_ConnectionParams(iCS_EditorObject port, Rect portPos, iCS_EditorObject source, Rect sourcePos, iCS_IStorage storage) {
        Start= new Vector2(sourcePos.x, sourcePos.y);
        End= new Vector2(portPos.x, portPos.y);

        // Compute Bezier tangents.
        Vector2 startDir= ConnectionDirectionFromTo(source, port, storage);
        Vector2 endDir= ConnectionDirectionFromTo(port, source, storage);
		Vector2 vertex= End-Start;
        StartTangent= Start + startDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector2.Dot(startDir, vertex)));
        EndTangent  = End + endDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector2.Dot(endDir, vertex)));

        // Compute connection center point.
        Center= BezierCenter(Start, End, StartTangent, EndTangent);
    }
    // ----------------------------------------------------------------------
    public iCS_ConnectionParams(iCS_EditorObject port, iCS_EditorObject source, iCS_IStorage storage) : this(port, port.GlobalRect, source, source.GlobalRect, storage) {}
    // ----------------------------------------------------------------------
    public iCS_ConnectionParams(iCS_EditorObject port, iCS_IStorage storage) : this(port, port.Source, storage) {}
    // ----------------------------------------------------------------------
    static Vector2 ConnectionDirectionFromTo(iCS_EditorObject port, iCS_EditorObject to, iCS_IStorage storage) {
        // Don't compute complex tangents if we don't have a proper parent.
        iCS_EditorObject portParent= port.Parent;
        if(port.IsFloating || to.IsFloating) {
            if(port.IsDataPort || !storage.IsNearNodeEdge(portParent, port.GlobalPosition, port.Edge)) {
                Vector2 fromPos= port.GlobalPosition;
                Vector2 toPos= to.GlobalPosition;
                return GetBestDirectionFrom((toPos-fromPos).normalized);                
            }
        }

        if(port.IsOutTransitionPort && portParent.IsIconized) {
            return storage.GetTransitionModuleVector(portParent);
        }
        if(port.IsInTransitionPort && portParent.IsIconized) {
            return -storage.GetTransitionModuleVector(portParent);
        }
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
        iCS_EditorObject toParent= to.Parent;
        if(storage.IsChildOf(toParent, portParent) && !port.IsInStatePort) {
            direction= -direction;
        }
        return direction;
    }
    // ----------------------------------------------------------------------
    static Vector2 GetBestDirectionFrom(Vector2 dir) {
        float up= Vector2.Dot(dir, UpDirection);
        float right= Vector2.Dot(dir, RightDirection);
        if(Mathf.Abs(up) > Mathf.Abs(right)) {
            return up > 0 ? UpDirection : DownDirection;
        }
        return right > 0 ? RightDirection : LeftDirection;
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
