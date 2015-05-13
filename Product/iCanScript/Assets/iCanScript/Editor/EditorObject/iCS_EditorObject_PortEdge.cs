using UnityEngine;
using System.Collections;
using iCanScript;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ======================================================================
        // Port layout attributes.
    	public bool IsOnTopEdge         { get { return Edge == iCS_EdgeEnum.Top; }}
        public bool IsOnBottomEdge      { get { return Edge == iCS_EdgeEnum.Bottom; }}
        public bool IsOnRightEdge       { get { return Edge == iCS_EdgeEnum.Right; }}
        public bool IsOnLeftEdge        { get { return Edge == iCS_EdgeEnum.Left; }}
        public bool IsOnHorizontalEdge  { get { return IsOnTopEdge   || IsOnBottomEdge; }}
        public bool IsOnVerticalEdge    { get { return IsOnRightEdge || IsOnLeftEdge; }}
        // ----------------------------------------------------------------------
        public iCS_EdgeEnum Edge {
    		get {
    			if(IsNestedPort) {
    				return Parent.Edge;
    			}
    			return EngineObject.Edge;
    		}
    		set {
                var engineObject= EngineObject;
    //            if(engineObject.Edge == value) return;
                engineObject.Edge= value;
                if(!IsFloating) CleanupPortEdgePosition();
    		}
    	}

        // ----------------------------------------------------------------------
        // Updates the port edge information from the port type.
        public void UpdatePortEdge() {
    		UpdatePortEdge(LocalPosition);
        }
    	// ----------------------------------------------------------------------
        // Updates the port edge information from the port type.
        public void UpdatePortEdge(Vector2 localPosition) {
            Edge= GetClosestEdge(localPosition);
        }
        // ----------------------------------------------------------------------
        public void CleanupPortEdgePosition() {
            var size= Parent.LocalSize;
            var lp= LocalPosition;
            switch(Edge) {
                case iCS_EdgeEnum.Top:      lp.y= -0.5f*size.y; break; 
                case iCS_EdgeEnum.Bottom:   lp.y=  0.5f*size.y; break;
                case iCS_EdgeEnum.Left:     lp.x= -0.5f*size.x; break;
                case iCS_EdgeEnum.Right:    lp.x=  0.5f*size.x; break;
            }
    //		LocalPosition= lp;
            CollisionOffsetFromLocalPosition= lp;
        }
        // ----------------------------------------------------------------------
        public bool IsPortOnParentEdge {
            get {
                var edge= IsStatePort || IsTransitionPort ? ClosestEdge : Edge;
    			return IsPortOnNodeEdge(Parent, edge);
            }
        }
        // ----------------------------------------------------------------------
        public bool IsPortOnNodeEdge(iCS_EditorObject node, iCS_EdgeEnum edge) {
    		return IsPortOnRectEdge(node.GlobalRect, edge);
        }
        // ----------------------------------------------------------------------
        public bool IsPortOnRectEdge(Rect r, iCS_EdgeEnum edge) {
    		return IsPositionOnRectEdge(GlobalPosition, r, edge);
        }
        // ----------------------------------------------------------------------
    	// Return true if the position is on the edge of the node.
    	public bool IsPositionOnEdge(Vector2 position, iCS_EdgeEnum edge) {
    		return IsPositionOnRectEdge(position, GlobalRect, edge);
    	}
        // ----------------------------------------------------------------------
        public static bool IsPositionOnRectEdge(Vector2 pos, Rect r, iCS_EdgeEnum edge) {
    		float maxDistance= iCS_EditorConfig.PortDiameter;
            float distance= 2f*maxDistance;
            float leftX  = r.xMin;
            float rightX = r.xMax;
            float topY   = r.yMin;
            float bottomY= r.yMax;
            switch(edge) {
                case iCS_EdgeEnum.Top:
                    distance= Math3D.DistanceFromHorizontalLineSegment(pos, leftX, rightX, topY);
                    break; 
                case iCS_EdgeEnum.Bottom:
                    distance= Math3D.DistanceFromHorizontalLineSegment(pos, leftX, rightX, bottomY);
                    break;
                case iCS_EdgeEnum.Left:
                    distance= Math3D.DistanceFromVerticalLineSegment(pos, topY, bottomY, leftX);
                    break;
                case iCS_EdgeEnum.Right:
                    distance= Math3D.DistanceFromVerticalLineSegment(pos, topY, bottomY, rightX);
                    break;                
            }
            return distance <= maxDistance;
        }
        // ----------------------------------------------------------------------
    	public iCS_EdgeEnum ClosestEdge {
    		get { return GetClosestEdge(AnimatedLocalPosition); }
    	}
        // ----------------------------------------------------------------------
    	public iCS_EdgeEnum GetClosestEdge(Vector2 localPosition) {
    		// Don't change edge if parent is iconized.
    		var parent= ParentNode;
    		if(parent.IsIconizedOnDisplay) return Edge;
            var parentSize= parent.LocalSize;
            float leftX  = -0.5f*parentSize.x;
            float rightX =  0.5f*parentSize.x;
            float topY   = -0.5f*parentSize.y;
            float bottomY=  0.5f*parentSize.y;
    		var edge= iCS_EdgeEnum.Top;
    		float distance= Math3D.DistanceFromHorizontalLineSegment(localPosition, leftX, rightX, topY);
    		float d= Math3D.DistanceFromHorizontalLineSegment(localPosition, leftX, rightX, bottomY);
    		if(d < distance) {
    			distance= d;
    			edge= iCS_EdgeEnum.Bottom;
    		}
    		d= Math3D.DistanceFromVerticalLineSegment(localPosition, topY, bottomY, leftX);
    		if(d < distance) {
    			distance= d;
    			edge= iCS_EdgeEnum.Left;
    		}
    		d= Math3D.DistanceFromVerticalLineSegment(localPosition, topY, bottomY, rightX); 
    		if(d < distance) {
    			edge= iCS_EdgeEnum.Right;
    		}
    		return edge;
    	}

    }
}
