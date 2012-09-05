using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the absolute position of the given object.
    public Rect GetLayoutPosition(iCS_EditorObject eObj) {
        return Storage.GetPosition(eObj);
    }
    public Rect GetLayoutPosition(int id) {
        return GetLayoutPosition(EditorObjects[id]);
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
    public bool IsNearNodeEdge(iCS_EditorObject node, Vector2 point, iCS_EditorObject.EdgeEnum edge) {
		float maxDistance= 2f*iCS_Config.PortSize;
        float distance= maxDistance+1f;
		var pos= GetLayoutPosition(node);
		switch(edge) {
			case iCS_EditorObject.EdgeEnum.Left: {
				distance= GetDistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.x);
				break;
			}
			case iCS_EditorObject.EdgeEnum.Right: {
				distance= GetDistanceFromVerticalLineSegment(point, pos.y, pos.yMax, pos.xMax);
				break;
			}
			case iCS_EditorObject.EdgeEnum.Bottom: {
				distance= GetDistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.yMax);
				break;
			}
			case iCS_EditorObject.EdgeEnum.Top: {
				distance= GetDistanceFromHorizontalLineSegment(point, pos.x, pos.xMax, pos.y);
				break;
			}
		}
        return distance <= maxDistance;
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject.EdgeEnum GetClosestEdge(iCS_EditorObject node, Vector2 point) {
        var pos= GetLayoutPosition(node);
        float xDistance   = Mathf.Abs(point.x-pos.x);
        float xMaxDistance= Mathf.Abs(point.x-pos.xMax);
        float yDistance   = Mathf.Abs(point.y-pos.y);
        float yMaxDistance= Mathf.Abs(point.y-pos.yMax);
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
	float GetDistanceFromHorizontalLineSegment(Vector2 point, float x1, float x2, float y) {
		if(x1 > x2) {
			var tmp= x1;
			x1= x2;
			x2= tmp;
		}
		float distance= Mathf.Abs(point.y-y);
		if(point.x >= x1 && point.x <= x2) return distance;
		if(point.x < x1) return Mathf.Max(distance, x1-point.x);
		return Mathf.Max(distance, point.x-x2);
		
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
		if(point.y < y1) return Mathf.Max(distance, y1-point.y);
		return Mathf.Max(distance, point.y-y2);
		
	}
    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(iCS_EditorObject port) {
        iCS_EditorObject parentNode= GetParent(port);
        Vector2 position= Math3D.ToVector2(GetLayoutPosition(port));
        return GetDistanceFromNode(parentNode, position);
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParentEdge(iCS_EditorObject port, iCS_EditorObject.EdgeEnum edge= iCS_EditorObject.EdgeEnum.None) {
        var parent= GetParent(port);
        var pos= Math3D.ToVector2(GetLayoutPosition(port));
        return IsNearNodeEdge(parent, pos, (edge != iCS_EditorObject.EdgeEnum.None ? edge : port.Edge));
    }
    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent(iCS_EditorObject port) {
        if(GetNodeAt(Math3D.ToVector2(GetLayoutPosition(port))) != GetParent(port)) return false;
        return GetDistanceFromParent(port) <= iCS_Config.PortSize*2;
    }
    
}
