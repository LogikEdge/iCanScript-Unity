using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Ports layout helpers.
    // ----------------------------------------------------------------------
    // Returns the available height to layout ports on the vertical edge.
    public float AvailableHeightForPorts {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            return VerticalPortsBottom-VerticalPortsTop;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the available width to layout ports on the horizontal edge.
    public float AvailableWidthForPorts {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            return HorizontalPortsRight-HorizontalPortsLeft;
        }
    }

    // ----------------------------------------------------------------------
    // Returns the top most coordinate for a port on the vertical edge.
    public float VerticalPortsTop {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            if(IsIconizedOnDisplay) return -0.25f*DisplaySize.y;
            return NodeTitleHeight+0.5f*(iCS_EditorConfig.MinimumPortSeparation-DisplaySize.y);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the bottom most coordinate for a port on the vertical edge.
    public float VerticalPortsBottom {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            if(IsIconizedOnDisplay) return 0.25f*DisplaySize.y;
            return 0.5f*(DisplaySize.y-iCS_EditorConfig.MinimumPortSeparation);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsLeft {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            if(IsIconizedOnDisplay) return -0.25f*DisplaySize.x;
            return 0.5f*(iCS_EditorConfig.MinimumPortSeparation-DisplaySize.x);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsRight {
        get {
            if(!IsVisibleOnDisplay) return 0f;
            if(IsIconizedOnDisplay) return 0.25f*DisplaySize.x;
            return 0.5f*(DisplaySize.x-iCS_EditorConfig.MinimumPortSeparation);
        }
    }
}
