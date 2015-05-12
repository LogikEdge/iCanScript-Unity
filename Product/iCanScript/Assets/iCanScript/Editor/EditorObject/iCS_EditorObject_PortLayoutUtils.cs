using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ======================================================================
        // Ports layout helpers.
        // ----------------------------------------------------------------------
        /// Returns the available height to layout ports on the vertical edge.
        public float AvailableHeightForPorts {
            get {
                if(!IsVisibleInLayout) return 0f;
    			if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0 : LocalSize.y;
    			}
                return FoldedOrUnfoldedVerticalPortsBottom-FoldedOrUnfoldedVerticalPortsTop;
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the available width to layout ports on the horizontal edge.
        public float AvailableWidthForPorts {
            get {
                if(!IsVisibleInLayout) return 0f;
    			if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0f : LocalSize.x;				
    			}
                return FoldedOrUnfoldedHorizontalPortsRight-FoldedOrUnfoldedHorizontalPortsLeft;
            }
        }

        // ----------------------------------------------------------------------
        /// Returns the top most coordinate for a port on the vertical edge.
        public float VerticalPortsTop {
            get {
                if(!IsVisibleInLayout) return 0f;
                if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0f : -0.25f*LocalSize.y;
    			}
                return FoldedOrUnfoldedVerticalPortsTop;
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the bottom most coordinate for a port on the vertical edge.
        public float VerticalPortsBottom {
            get {
                if(!IsVisibleInLayout) return 0f;
                if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0f : 0.25f*LocalSize.y;
    			}
                return FoldedOrUnfoldedVerticalPortsBottom;
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the top most coordinate for a port on the vertical edge
        /// of a folded or unfolded node.
        float FoldedOrUnfoldedVerticalPortsTop {
            get {
                return NodeTitleHeight+0.5f*(iCS_EditorConfig.kMinimumPortSeparation-LocalSize.y);            
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the bottom most coordinate for a port on the vertical edge
        /// of a folded or unfolded node.
        float FoldedOrUnfoldedVerticalPortsBottom {
            get {
                return 0.5f*(LocalSize.y-iCS_EditorConfig.kMinimumPortSeparation);
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the left most coordinate for a port on the horizontal edge.
        public float HorizontalPortsLeft {
            get {
                if(!IsVisibleInLayout) return 0f;
                if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0f : -0.25f*LocalSize.x;
    			}
                return FoldedOrUnfoldedHorizontalPortsLeft;
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the right most coordinate for a port on the horizontal edge.
        public float HorizontalPortsRight {
            get {
                if(!IsVisibleInLayout) return 0f;
                if(IsIconizedInLayout) {
    				return IsTransitionPackage ? 0f : 0.25f*LocalSize.x;
    			}
                return FoldedOrUnfoldedHorizontalPortsRight;
            }
        }    
        // ----------------------------------------------------------------------
        /// Returns the left most coordinate for a port on the horizontal edge
        /// of a folded/unfolded node.
        float FoldedOrUnfoldedHorizontalPortsLeft {
            get {
                return 0.5f*(iCS_EditorConfig.kMinimumPortSeparation-LocalSize.x);            
            }
        }
        // ----------------------------------------------------------------------
        /// Returns the right most coordinate for a port on the horizontal edge
        /// of a folded/unfolded node.
        float FoldedOrUnfoldedHorizontalPortsRight {
            get {
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
}
