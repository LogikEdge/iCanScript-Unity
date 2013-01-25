using UnityEngine;
using System;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE NEEDED SIZE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
//    // ----------------------------------------------------------------------
//    // Updates the size of the node.  We assume that all children have been
//    // previously layed out prior to updating the node size.
//    public void UpdateNodeSizeFromChildrenGlobalRect() {
//        DisplaySize= ComputeNodeSizeFromChildrenGlobalRect();
//    }
//    // ----------------------------------------------------------------------
//    // Returns the size of the node using the current children global position.
//	Vector2 ComputeNodeSizeFromChildrenGlobalRect() {
//	    return ComputeNodeSize(o=> o.ChildrenSizeFromGlobalRect);
//	}
//    // ----------------------------------------------------------------------
//    // Returns the size of the node using the current children position ratio.
//	Vector2 ComputeNodeSizeFromChildrenRatio() {
//	    return ComputeNodeSize(o=> o.ChildrenSizeFromRatio);
//	}
//    // ----------------------------------------------------------------------
//    // Computes the children area size using the children global rect.  Note
//    // that child collision resolution is not performed by this function.
//    public Vector2 ChildrenSizeFromGlobalRect {
//        get {
//            Rect childRect= ChildrenGlobalRectFromGlobalRect;
//            return new Vector2(childRect.width, childRect.height);
//        }
//    }
//    // ----------------------------------------------------------------------
//    // Computes the children area size using the children size and position
//    // ratio.  Note that child collision resolution is not performed by this
//    // function.
//    public Vector2 ChildrenSizeFromRatio {
//        get {
//            Vector2 childrenSize= Vector2.zero;
//            ForEachChildNode(
//                c=> {
//                    if(!c.IsFloating) {
//                        var ratio= c.NodePositionRatio;
//                        var rx= 0.5f-Mathf.Abs(0.5f-ratio.x);
//                        var ry= 0.5f-Mathf.Abs(0.5f-ratio.y);
//                        if(Math3D.IsZero(rx) || Math3D.IsZero(ry)) {
//                            Debug.LogWarning("iCanScript: Invalid node position ratio: "+ratio+" for: "+c.Name);
//                        } else {
//                            var childHalfSize= 0.5f*c.DisplaySize; 
//                            var sx= childHalfSize.x/rx;
//                            var sy= childHalfSize.y/ry;
//                            if(sx > childrenSize.x) childrenSize.x= sx;
//                            if(sy > childrenSize.y) childrenSize.y= sy;
//                        }
//                    }
//                }
//            );
//            return childrenSize;
//        }
//    }
}
