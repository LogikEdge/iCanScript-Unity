using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public Vector2 GetPortLocalAnchorPositionFromRatio() {
        var parentSize= Parent.DisplaySize;
        if(IsOnVerticalEdge) {
            var y= VerticalPortYCoordinateFromRatio();
            if(IsOnLeftEdge) {
                return new Vector2(-0.5f*parentSize.x, y);
            }
            return new Vector2(0.5f*parentSize.x, y);
        }
        var x= HorizontalPortXCoordinateFromRatio();
        if(IsOnTopEdge) {
            return new Vector2(x, -0.5f*parentSize.y);
        }
        return new Vector2(x, 0.5f*parentSize.y);
    }
    // ----------------------------------------------------------------------
    // Returns Y coordinate for a port on the vertical edge given its ratio.
    public float VerticalPortYCoordinateFromRatio() {
        var parent= Parent;
        var ratio= PortPositionRatio;
        return parent.VerticalPortsTop+parent.AvailableHeightForPorts*ratio;
    }
    // ----------------------------------------------------------------------
    // Returns X coordinate for a port on the horizontal edge given its ratio.
    public float HorizontalPortXCoordinateFromRatio() {
        var parent= Parent;
        var ratio= PortPositionRatio;
        return parent.HorizontalPortsLeft+parent.AvailableWidthForPorts*ratio;
    }
}
