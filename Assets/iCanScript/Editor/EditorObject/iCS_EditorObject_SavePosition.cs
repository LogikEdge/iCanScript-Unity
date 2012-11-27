using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  SAVE POSITION
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public void SavePosition() {
        if(IsPort) {
            SavePortPosition();
        } else {
            SaveNodePosition();
        }
    }
    // ----------------------------------------------------------------------
    public void SaveNodePosition() {
        // Save the global position for the root node.
        var engineObject= EngineObject;
        if(!IsParentValid) {
            engineObject.LocalPositionRatio= GlobalPosition;
            return;
        }
        // Compute position ratio using node children area.
        var childArea= Parent.ChildrenArea;
        var topLeftCorner= new Vector2(childArea.x, childArea.y);
        var diff= LocalPosition-topLeftCorner;
        float ratioX= diff.x/childArea.width;
        float ratioY= diff.y/childArea.height;
        if(ratioX < 0f) ratioX= 0f;
        if(ratioX > 1f) ratioX= 1f;
        if(ratioY < 0f) ratioY= 0f;
        if(ratioY > 1f) ratioY= 1f;
        engineObject.LocalPositionRatio= new Vector2(ratioX, ratioY);
    }

    // ======================================================================
    // Save Port Position
    // ----------------------------------------------------------------------
    // Updates port position after successful relocation.
    void SavePortPosition() {
        CleanupPortEdgePosition();
        if(IsOnVerticalEdge) {
            SaveVerticalPortRatioFromLocalPosition(LocalPosition);            
        } else {
            SaveHorizontalPortRatioFromLocalPosition(LocalPosition);                        
        }
        Parent.LayoutPorts();        
    }
    // ----------------------------------------------------------------------
    // Updates the port ratio on the horizontal edage given the port local position.
    void SaveVerticalPortRatioFromLocalPosition(Vector2 localPosition) {
        var parent= Parent;
        var height= parent.AvailableHeightForPorts;
        if(Math3D.IsSmallerOrEqual(height, 0f)) {
            PortPositionRatio= 0.5f;
            return;
        }
        float deltaY= localPosition.y-parent.VerticalPortsTop;
        var ratio= deltaY/height;
        if(ratio < 0f) ratio= 0f;
        if(ratio > 1f) ratio= 1f;
        PortPositionRatio= ratio;
    }
    // ----------------------------------------------------------------------
    // Updates the port ratio on the horizontal edage given the port local position.
    void SaveHorizontalPortRatioFromLocalPosition(Vector2 localPosition) {
        var parent= Parent;
        var width= parent.AvailableWidthForPorts;
        if(Math3D.IsSmallerOrEqual(width, 0f)) {
            PortPositionRatio= 0.5f;
            return;
        }
        float deltaX= localPosition.x-parent.HorizontalPortsLeft;
        var ratio= deltaX/width;
        if(ratio < 0f) ratio= 0f;
        if(ratio > 1f) ratio= 1f;
        PortPositionRatio= ratio;
    }

}
