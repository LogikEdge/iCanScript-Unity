using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Resolve collision between children then wrap node around the children.
    public void LayoutNode() {
        UpdateNodeLayoutSize();
//        // Attempt to predict size.
//        bool shouldComputeSize= true;
//        ForEachChildNode(n=> { if(n.IsSticky) shouldComputeSize= false; });
//        if(shouldComputeSize) {
//            LayoutSize= ComputeNodeSizeFromChildrenRatio();
//        }
//        // Resolve collision with siblings.
//        ResolveCollisionOnChildrenNodes();
//        // Adjust parent to wrap children.
//        WrapAroundChildrenNodes();        
    }


//    // Storage accessors ====================================================
//    public Vector2 NodePositionRatio {
//        get { return LocalAnchorPosition; }
//		set { LocalAnchorPosition= value; }
//    }
//
//    // ----------------------------------------------------------------------
//	public Rect NodeGlobalChildRect {
//		get {
//            if(!IsVisible || IsIconized || IsFolded) {
//                Debug.LogWarning("iCanScript: NodeGlobalChildRect should not be called when the node is iconized or folded");
//                var gp= GlobalPosition;
//                return new Rect(gp.x, gp.y, 0, 0);
//            }
//			var r= GlobalRect;
//			var top= NodeTopPadding;
//			r.y+= top;
//			r.height-= top+NodeBottomPadding;
//			var left= NodeLeftPadding;
//			r.x+= left;
//			r.width-= left+NodeRightPadding;
//			return r;
//		}
//		set {
//			var top= NodeTopPadding;
//			value.y-= top;
//			value.height+= top+NodeBottomPadding;
//			var left= NodeLeftPadding;
//			value.x-= left;
//			value.width+= left+NodeRightPadding;
//            // Assure that the width is sufficent for the title.
//            float titleWidth= NodeTitleWidth;
//            if(titleWidth > value.width) {
//                value.x-= 0.5f*(titleWidth-value.width);
//                value.width= titleWidth;
//            }
//			GlobalRect= value;
//		}
//	}
//    // ----------------------------------------------------------------------
//    Rect ChildrenLocalRect {
//        get {
//            var globalPosition= GlobalPosition;
//            var childGlobalRect= NodeGlobalChildRect;
//            return new Rect(childGlobalRect.x-globalPosition.x, childGlobalRect.y-globalPosition.y, childGlobalRect.width, childGlobalRect.height);
//        }
//    }
//    // ----------------------------------------------------------------------
//	// Initializes port position ratio of all edges.
//	public void InitialPortLayout() {
//		InitialPortLayout(LeftPorts);
//		InitialPortLayout(RightPorts);
//		InitialPortLayout(TopPorts);
//		InitialPortLayout(BottomPorts);
//	}
//    // ----------------------------------------------------------------------
//	// Initializes port position ratio for ports on the same edge.
//	void InitialPortLayout(iCS_EditorObject[] ports) {
//		// Sort according to port index.
//		int len= ports.Length;
//		if(len == 0) return;
//		// Sort port according to port index.
//		for(int i= 0; i < len-1; ++i) {
//			for(int j= i+1; j < len; ++j) {
//				if(ports[i].PortIndex > ports[j].PortIndex) {
//					var tmp= ports[i];
//					ports[i]= ports[j];
//					ports[j]= tmp;
//				}
//			}
//		}
//		// Update port position ratio.
//		float step= 1f/(float)(len);
//		for(int i= 0; i < len; ++i) {
//			float ratio= ((float)(i)+0.5f)*step;
//			ports[i].PortPositionRatio= ratio;
//		}
//	}
//    // ----------------------------------------------------------------------
//	// Updates the port position ratio of all edges.
//    public void UpdatePortRatios() {
//        foreach(var port in LeftPorts) {
//            port.SaveVerticalPortRatioFromLocalPosition(port.LocalPosition);
//        }
//        foreach(var port in RightPorts) {
//            port.SaveVerticalPortRatioFromLocalPosition(port.LocalPosition);
//        }
//        foreach(var port in TopPorts) {
//            port.SaveHorizontalPortRatioFromLocalPosition(port.LocalPosition);
//        }
//        foreach(var port in BottomPorts) {
//            port.SaveHorizontalPortRatioFromLocalPosition(port.LocalPosition);
//        }
//    }
//
//    // ----------------------------------------------------------------------
//    // Returns true if given child is outside the existing children area.
//    bool IsOutsideChildArea(iCS_EditorObject child) {
//        var childrenArea= NodeGlobalChildRect;
//        var childRect= child.GlobalRect;
//        var intersection= Math3D.Intersection(childrenArea, childRect);
//        return Math3D.IsNotEqual(intersection, childRect);
//    }
//
}

