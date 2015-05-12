using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ----------------------------------------------------------------------
        // Returns true if the given point is inside the node coordinates.
        public bool IsInside(iCS_EditorObject node, Vector2 point) {
            return node.GlobalRect.Contains(point);
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
            float distance= Vector2.Distance(point, Math3D.TopLeftCorner(node.GlobalRect));
            distance= Mathf.Min(distance, Vector2.Distance(point, Math3D.TopRightCorner(node.GlobalRect)));
            distance= Mathf.Min(distance, Vector2.Distance(point, Math3D.BottomLeftCorner(node.GlobalRect)));
            distance= Mathf.Min(distance, Vector2.Distance(point, Math3D.BottomRightCorner(node.GlobalRect)));
            return distance;
        }

        // ----------------------------------------------------------------------
        // Returns true if the distance to parent is less then twice the port size.
        public bool IsNearNode(iCS_EditorObject node, Vector2 point) {
            return GetDistanceFromNode(node, point) <= iCS_EditorConfig.PortDiameter*2;
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
            var parent= port.ParentNode;
            var pos= port.GlobalPosition;
            return parent.IsPositionOnEdge(pos, (edge != iCS_EdgeEnum.None ? edge : port.Edge));
        }
    
    }
    
}
