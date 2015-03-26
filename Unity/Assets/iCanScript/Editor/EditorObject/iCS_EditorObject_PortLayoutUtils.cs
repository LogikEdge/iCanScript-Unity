using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Ports layout helpers.
    // ----------------------------------------------------------------------
    // Returns the available height to layout ports on the vertical edge.
    public float AvailableHeightForPorts {
        get {
            if(!IsVisibleInLayout) return 0f;
			if(IsTransitionPackage && IsIconizedInLayout) return 0f;
            return VerticalPortsBottom-VerticalPortsTop;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the available width to layout ports on the horizontal edge.
    public float AvailableWidthForPorts {
        get {
            if(!IsVisibleInLayout) return 0f;
			if(IsTransitionPackage && IsIconizedInLayout) return 0f;
            return HorizontalPortsRight-HorizontalPortsLeft;
        }
    }

    // ----------------------------------------------------------------------
    // Returns the top most coordinate for a port on the vertical edge.
    public float VerticalPortsTop {
        get {
            if(!IsVisibleInLayout) return 0f;
            if(IsIconizedInLayout) {
				return IsTransitionPackage ? 0f : -0.25f*LocalSize.y;
			}
            return NodeTitleHeight+0.5f*(iCS_EditorConfig.kMinimumPortSeparation-LocalSize.y);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the bottom most coordinate for a port on the vertical edge.
    public float VerticalPortsBottom {
        get {
            if(!IsVisibleInLayout) return 0f;
            if(IsIconizedInLayout) {
				return IsTransitionPackage ? 0f : 0.25f*LocalSize.y;
			}
            return 0.5f*(LocalSize.y-iCS_EditorConfig.kMinimumPortSeparation);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsLeft {
        get {
            if(!IsVisibleInLayout) return 0f;
            if(IsIconizedInLayout) {
				return IsTransitionPackage ? 0f : -0.25f*LocalSize.x;
			}
            return 0.5f*(iCS_EditorConfig.kMinimumPortSeparation-LocalSize.x);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsRight {
        get {
            if(!IsVisibleInLayout) return 0f;
            if(IsIconizedInLayout) {
				return IsTransitionPackage ? 0f : 0.25f*LocalSize.x;
			}
            return 0.5f*(LocalSize.x-iCS_EditorConfig.kMinimumPortSeparation);
        }
    }    
    // ----------------------------------------------------------------------
    // Returns the minimium height needed for the left / right ports.
    public float MinimumHeightForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            return nbOfPorts*iCS_EditorConfig.kMinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimum width needed for the top / bottom ports.
    public float MinimumWidthForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfTopPorts, NbOfBottomPorts);
            return nbOfPorts*iCS_EditorConfig.kMinimumPortSeparation;                                            
        }
    }
    
}
