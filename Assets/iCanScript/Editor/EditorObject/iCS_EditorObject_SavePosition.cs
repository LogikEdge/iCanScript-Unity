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
            engineObject.LocalAnchorPosition= GlobalPosition;
            return;
        }
        // Compute position ratio using node children area.
        var childArea= Parent.ChildrenLocalRect;
        var topLeftCorner= new Vector2(childArea.x, childArea.y);
        var diff= LocalPosition-topLeftCorner;
        float ratioX= diff.x/childArea.width;
        float ratioY= diff.y/childArea.height;
        if(ratioX < 0f) ratioX= 0f;
        if(ratioX > 1f) ratioX= 1f;
        if(ratioY < 0f) ratioY= 0f;
        if(ratioY > 1f) ratioY= 1f;
        engineObject.LocalAnchorPosition= new Vector2(ratioX, ratioY);
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
        PortPositionRatio= ratio;
        CleanupPortRatios();
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
        PortPositionRatio= ratio;
        CleanupPortRatios();
    }
    // ----------------------------------------------------------------------
    void CleanupPortRatios() {
        var parent= Parent;
        iCS_EditorObject[] ports;
        if(IsOnVerticalEdge) {
            ports= IsOnLeftEdge ? parent.LeftPorts : parent.RightPorts;            
        } else {
            ports= IsOnTopEdge ? parent.TopPorts : parent.BottomPorts;                        
        }
        float[] ratios= GetPortPositionRatios(ref ports);
        var len= ratios.Length;
        // Flatten edge values.
        for(int i= 0; i < len; ++i) {
            if(ratios[i] < 0f) {
                ratios[i]= 0f;
                ports[i].PortPositionRatio= 0f;
            }
            if(ratios[i] > 1f) {
                ratios[i]= 1f;
                ports[i].PortPositionRatio= 1f;
            }
        }
        // Need to readjust ratio to avoid multiple ports at same ratio.
        for(int i= 0; i < len-1; ++i) {
            if(ratios[i] > 0.5f) continue; 
            if(Math3D.IsGreaterOrEqual(ratios[i], ratios[i+1])) {
                ratios[i+1]= ratios[i]+0.01f;
                ports[i+1].PortPositionRatio= ratios[i+1];
            }
        }
        for(int i= len-1; i > 0; --i) {
            if(ratios[i] < 0.5f) continue;
            if(Math3D.IsGreaterOrEqual(ratios[i-1], ratios[i])) {
                ratios[i-1]= ratios[i]-0.01f;
                ports[i-1].PortPositionRatio= ratios[i-1];
            }
         }
    }
}
