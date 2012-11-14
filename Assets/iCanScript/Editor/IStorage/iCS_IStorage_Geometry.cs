using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns true if the given point is inside the node coordinates.
    public bool IsInside(iCS_EditorObject node, Vector2 point) {
        // Extend the node range to include the ports.
        float portSize= iCS_Config.PortSize;
        Rect nodePos= node.GlobalRect;
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
        Rect nodeRect= node.GlobalRect;
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
    public bool IsNearNodeEdge(iCS_EditorObject node, Vector2 point, iCS_EdgeEnum edge) {
		float maxDistance= 2f*iCS_Config.PortSize;
        float distance= maxDistance+1f;
		var pos= node.GlobalRect;
		switch(edge) {
			case iCS_EdgeEnum.Left: {
				distance= Math3D.DistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.x);
				break;
			}
			case iCS_EdgeEnum.Right: {
				distance= Math3D.DistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.xMax);
				break;
			}
			case iCS_EdgeEnum.Bottom: {
				distance= Math3D.DistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.yMax);
				break;
			}
			case iCS_EdgeEnum.Top: {
				distance= Math3D.DistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.y);
				break;
			}
		}
        return distance <= maxDistance;
    }
    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(iCS_EditorObject port) {
        iCS_EditorObject parentNode= port.Parent;
        Vector2 position= port.GlobalPosition;
        return GetDistanceFromNode(parentNode, position);
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParentEdge(iCS_EditorObject port, iCS_EdgeEnum edge= iCS_EdgeEnum.None) {
        var parent= port.Parent;
        var pos= port.GlobalPosition;
        return IsNearNodeEdge(parent, pos, (edge != iCS_EdgeEnum.None ? edge : port.Edge));
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent(iCS_EditorObject port) {
        if(GetNodeAt(port.GlobalPosition) != port.Parent) return false;
        return GetDistanceFromParent(port) <= iCS_Config.PortSize*2;
    }
    
}
