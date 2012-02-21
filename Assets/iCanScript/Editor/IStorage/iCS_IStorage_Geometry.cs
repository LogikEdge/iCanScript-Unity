using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the absolute position of the given object.
    public Rect GetPosition(iCS_EditorObject eObj) {
        return Storage.GetPosition(eObj);
    }
    public Rect GetPosition(int id) {
        return GetPosition(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    // Returns the local position of the given object.
    public Rect GetLocalPosition(iCS_EditorObject eObj) {
        return eObj.LocalPosition;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given point is inside the node coordinates.
    public bool IsInside(iCS_EditorObject node, Vector2 point) {
        // Extend the node range to include the ports.
        float portSize= iCS_Config.PortSize;
        Rect nodePos= GetPosition(node);
        nodePos.x-= portSize;
        nodePos.y-= portSize;
        nodePos.width+= 2f*portSize;
        nodePos.height+= 2f*portSize;
        return nodePos.Contains(point);
    }

    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromNode(iCS_EditorObject node, Vector2 point) {
        if(IsInside(node, point)) return 0;
        Rect nodeRect= GetPosition(node);
        if(point.x > nodeRect.xMin && point.x < nodeRect.xMax) {
            return Mathf.Min(Mathf.Abs(point.y-nodeRect.yMin),
                             Mathf.Abs(point.y-nodeRect.yMax));
        }
        if(point.y > nodeRect.yMin && point.y < nodeRect.yMax) {
            return Mathf.Min(Mathf.Abs(point.x-nodeRect.xMin),
                             Mathf.Abs(point.x-nodeRect.xMax));
        }
        float distance= Vector2.Distance(point, GetTopLeftCorner(node));
        distance= Mathf.Min(distance, Vector2.Distance(point, GetTopRightCorner(node)));
        distance= Mathf.Min(distance, Vector2.Distance(point, GetBottomLeftCorner(node)));
        distance= Mathf.Min(distance, Vector2.Distance(point, GetBottomRightCorner(node)));
        return distance;
    }

    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearNode(iCS_EditorObject node, Vector2 point) {
        return GetDistanceFromNode(node, point) <= iCS_Config.PortSize*2;
    }

    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearNodeEdge(iCS_EditorObject node, Vector2 point, iCS_EditorObject.EdgeEnum edge) {
        float distance;
        iCS_EditorObject.EdgeEnum closestEdge= GetClosestEdge(node, point, out distance);
        if(distance > 2f*iCS_Config.PortSize) return false;
        return closestEdge == edge;
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject.EdgeEnum GetClosestEdge(iCS_EditorObject node, Vector2 point, out float distance) {
        var pos= GetPosition(node);
        float xDistance   = Mathf.Abs(point.x-pos.x);
        float xMaxDistance= Mathf.Abs(point.x-pos.xMax);
        float yDistance   = Mathf.Abs(point.y-pos.y);
        float yMaxDistance= Mathf.Abs(point.y-pos.yMax);
        distance= Mathf.Min(Mathf.Min(xDistance, xMaxDistance), Mathf.Min(yDistance, yMaxDistance));
        if(xDistance < xMaxDistance) {
            if(yDistance < yMaxDistance) {
                return xDistance < yDistance ? iCS_EditorObject.EdgeEnum.Left : iCS_EditorObject.EdgeEnum.Top;
            } else {
                return xDistance < yMaxDistance ? iCS_EditorObject.EdgeEnum.Left : iCS_EditorObject.EdgeEnum.Bottom;
            }
        } else {
            if(yDistance < yMaxDistance) {
                return xMaxDistance < yDistance ? iCS_EditorObject.EdgeEnum.Right : iCS_EditorObject.EdgeEnum.Top;
            } else {
                return xMaxDistance < yMaxDistance ? iCS_EditorObject.EdgeEnum.Right : iCS_EditorObject.EdgeEnum.Bottom;
            }            
        }
    }

    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(iCS_EditorObject port) {
        iCS_EditorObject parentNode= GetParent(port);
        Vector2 position= Math3D.ToVector2(GetPosition(port));
        return GetDistanceFromNode(parentNode, position);
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParentEdge(iCS_EditorObject port, iCS_EditorObject.EdgeEnum edge= iCS_EditorObject.EdgeEnum.None) {
        var parent= GetParent(port);
        var pos= Math3D.ToVector2(GetPosition(port));
        return IsNearNodeEdge(parent, pos, (edge != iCS_EditorObject.EdgeEnum.None ? edge : port.Edge));
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent(iCS_EditorObject port) {
        if(GetNodeAt(Math3D.ToVector2(GetPosition(port))) != GetParent(port)) return false;
        return GetDistanceFromParent(port) <= iCS_Config.PortSize*2;
    }
    
}
