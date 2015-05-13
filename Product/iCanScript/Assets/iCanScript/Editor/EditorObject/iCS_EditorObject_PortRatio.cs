using UnityEngine;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ----------------------------------------------------------------------
        public Vector2 GetPortLocalAnchorPositionFromRatio() {
            var parent= ParentNode;
            var ratio= PortPositionRatio;
            var parentSize= parent.LocalSize;
            if(parent.IsIconizedOnDisplay) {
                parentSize*= (IsTransitionPort ? 0f : 0.5f);
            }
            float x, y;
            if(IsOnVerticalEdge) {
                x= (IsOnLeftEdge ? -0.5f : 0.5f)*parentSize.x;
                y= parent.VerticalPortsTop+parent.AvailableHeightForPorts*ratio;
            } else {
                x= parent.HorizontalPortsLeft+parent.AvailableWidthForPorts*ratio;
                y= (IsOnTopEdge ? -0.5f : 0.5f)*parentSize.y;            
            }
            return new Vector2(x, y);
        }
        // ----------------------------------------------------------------------
        public float GetPortRatioFromLocalAnchorPosition(Vector2 pos) {
            var parent= ParentNode;
    		float offset;
    		float availableSize;
            if(IsOnVerticalEdge) {
                offset= pos.y-parent.VerticalPortsTop;
                availableSize= parent.AvailableHeightForPorts;
            } else {
                offset= pos.x-parent.HorizontalPortsLeft;
                availableSize= parent.AvailableWidthForPorts;
            }
    		if(Math3D.IsSmallerOrEqual(offset, 0f)) return 0f;
    		if(Math3D.IsGreaterOrEqual(offset, availableSize)) return 1f;
            return offset/availableSize;
        }
    }
}
