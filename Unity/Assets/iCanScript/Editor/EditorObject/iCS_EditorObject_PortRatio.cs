using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public Vector2 GetPortLocalAnchorPositionFromRatio() {
        var parent= ParentNode;
        var ratio= PortPositionRatio;
        var parentSize= parent.DisplaySize;
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
        var parent= Parent;
        float ratio;
        if(IsOnVerticalEdge) {
            var a= pos.y-parent.VerticalPortsTop;
            ratio= a/parent.AvailableHeightForPorts;
        } else {
            var a= pos.x-parent.HorizontalPortsLeft;
            ratio= a/parent.AvailableWidthForPorts;
        }
        if(ratio <= 0f) return 0f;
        if(ratio >= 1f) return 1f;
        return ratio;
    }
}
