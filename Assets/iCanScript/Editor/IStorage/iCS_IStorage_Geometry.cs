using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns true if the given point is inside the node coordinates.
    public bool IsInside(iCS_EditorObject node, Vector2 point) {
        // Extend the node range to include the ports.
        float portSize= iCS_Config.PortSize;
        Rect nodePos= GetLayoutPosition(node);
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
        Rect nodeRect= GetLayoutPosition(node);
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
		var pos= GetLayoutPosition(node);
		switch(edge) {
			case iCS_EdgeEnum.Left: {
				distance= GetDistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.x);
				break;
			}
			case iCS_EdgeEnum.Right: {
				distance= GetDistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.xMax);
				break;
			}
			case iCS_EdgeEnum.Bottom: {
				distance= GetDistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.yMax);
				break;
			}
			case iCS_EdgeEnum.Top: {
				distance= GetDistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.y);
				break;
			}
		}
        return distance <= maxDistance;
    }
    // ----------------------------------------------------------------------
    iCS_EdgeEnum GetClosestEdge(iCS_EditorObject node, iCS_EditorObject port) {
        const float kAllowedGap= 2f*iCS_Config.PortRadius;
        var nodePos= GetLayoutPosition(node);
        var portPos= Math3D.ToVector2(GetLayoutPosition(port));
        float topDistance= GetDistanceFromHorizontalLineSegment(portPos, nodePos.x, nodePos.xMax, nodePos.y);
        float bottomDistance= GetDistanceFromHorizontalLineSegment(portPos, nodePos.x, nodePos.xMax, nodePos.yMax);
        float leftDistance= GetDistanceFromVerticalLineSegment(portPos, nodePos.y, nodePos.yMax, nodePos.x);
        float rightDistance= GetDistanceFromVerticalLineSegment(portPos, nodePos.y, nodePos.yMax, nodePos.xMax);
        // Attempt to keep same edge node is shrinked.
        switch(port.Edge) {
			case iCS_EdgeEnum.Left: {
                float left= leftDistance-kAllowedGap;
                if(left < topDistance && left < bottomDistance && left < rightDistance) {
                    return port.Edge;
                }
				break;
			}
			case iCS_EdgeEnum.Right: {
                float right= rightDistance-kAllowedGap;
                if(right < topDistance && right < bottomDistance && right < leftDistance) {
                    return port.Edge;
                }
				break;
			}
			case iCS_EdgeEnum.Bottom: {
                float bottom= bottomDistance-kAllowedGap;
                if(bottom < topDistance && bottom < leftDistance && bottom < rightDistance) {
                    return port.Edge;
                }
				break;
			}
			case iCS_EdgeEnum.Top: {
                float top= topDistance-kAllowedGap;
                if(top < bottomDistance && top < leftDistance && top < rightDistance) {
                    return port.Edge;
                }
				break;
			}            
        }
        if(topDistance < bottomDistance) {
            if(leftDistance < rightDistance) {
                return topDistance < leftDistance ? iCS_EdgeEnum.Top : iCS_EdgeEnum.Left;					
            }
            return topDistance < rightDistance ? iCS_EdgeEnum.Top : iCS_EdgeEnum.Right;
        }
        if(leftDistance < rightDistance) {
            return bottomDistance < leftDistance ? iCS_EdgeEnum.Bottom : iCS_EdgeEnum.Left;
        }
        return bottomDistance < rightDistance ? iCS_EdgeEnum.Bottom : iCS_EdgeEnum.Right;
    }
    // ----------------------------------------------------------------------
	float GetDistanceFromHorizontalLineSegment(Vector2 point, float x1, float x2, float y) {
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
	float GetDistanceFromVerticalLineSegment(Vector2 point, float y1, float y2, float x) {
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
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(iCS_EditorObject port) {
        iCS_EditorObject parentNode= port.Parent;
        Vector2 position= Math3D.ToVector2(GetLayoutPosition(port));
        return GetDistanceFromNode(parentNode, position);
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParentEdge(iCS_EditorObject port, iCS_EdgeEnum edge= iCS_EdgeEnum.None) {
        var parent= port.Parent;
        var pos= Math3D.ToVector2(GetLayoutPosition(port));
        return IsNearNodeEdge(parent, pos, (edge != iCS_EdgeEnum.None ? edge : port.Edge));
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent(iCS_EditorObject port) {
        if(GetNodeAt(Math3D.ToVector2(GetLayoutPosition(port))) != port.Parent) return false;
        return GetDistanceFromParent(port) <= iCS_Config.PortSize*2;
    }
    
}
