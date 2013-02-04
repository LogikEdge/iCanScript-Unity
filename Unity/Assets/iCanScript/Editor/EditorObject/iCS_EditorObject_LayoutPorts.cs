using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Lays out the ports of this node using the position ratio and node size
    // as basis.
    public void LayoutPorts() {
//        var halfSize= 0.5f*LayoutSize;
//        var verticalTop    = VerticalPortsTop;
//        var verticalBottom = VerticalPortsBottom;
//        var horizontalLeft = HorizontalPortsLeft;
//        var horizontalRight= HorizontalPortsRight;
//        LayoutPortsOnVerticalEdge  (LeftPorts  , verticalTop   , verticalBottom, -halfSize.x);
//        LayoutPortsOnVerticalEdge  (RightPorts , verticalTop   , verticalBottom,  halfSize.x);
//        LayoutPortsOnHorizontalEdge(TopPorts   , horizontalLeft, horizontalRight, -halfSize.y);
//        LayoutPortsOnHorizontalEdge(BottomPorts, horizontalLeft, horizontalRight,  halfSize.y);
    }
//    // ----------------------------------------------------------------------
//    static void LayoutPortsOnVerticalEdge(iCS_EditorObject[] ports,
//                                                 float top, float bottom, float x) {
//        // Layout ports on one dimension edge.
//        float[] ys= LayoutPortsOnEdge(ports, top, bottom);
//		// Update position from new layout.
//        int nbPorts= ports.Length;
//		for(int i= 0; i < nbPorts; ++i) {
//			ports[i].LocalPosition= new Vector2(x, top+ys[i]);
//		}
//    }
//    // ----------------------------------------------------------------------
//    static void LayoutPortsOnHorizontalEdge(iCS_EditorObject[] ports,
//                                                 float left, float right, float y) {
//        // Layout ports on one dimension edge.
//        float[] xs= LayoutPortsOnEdge(ports, left, right);
//		// Update position from new layout.
//        int nbPorts= ports.Length;
//		for(int i= 0; i < nbPorts; ++i) {
//			ports[i].LocalPosition= new Vector2(left+xs[i], y);
//		}
//    }
//    // ----------------------------------------------------------------------
//    // Layouts out the ports on an one dimension edge.
//    static float[] LayoutPortsOnEdge(iCS_EditorObject[] ports,
//                                         float minValue, float maxValue) {
//        // Compute position according to ratio.
//        int nbPorts= ports.Length;
//        float diff= maxValue-minValue;
//        float[] xs= GetPortPositionRatios(ref ports);
//        for(int i= 0; i < nbPorts; ++i) {
//            xs[i]*= diff;
//        }
//        // Resolve position according to collisions.
//        ResolvePortCollisions(xs, diff);
//        return xs;
//    }
//    // ----------------------------------------------------------------------
//    // Returns the sorted array of saved port ratios.  The input port array
//    // is also updated to reflect the sort order.
//	static float[] GetPortPositionRatios(ref iCS_EditorObject[] ports) {
//		// Extract ratios.
//        int nbPorts= ports.Length;
//        float[] rs= new float[nbPorts];
//        for(int i= 0; i < nbPorts; ++i) {
//            rs[i]= ports[i].PortPositionRatio;
//        }
//		// Sort port according to ratios
//		for(int i= 0; i < nbPorts-1; ++i) {
//			var v= rs[i];
//			for(int j= i+1; j < nbPorts; ++j) {
//				if(rs[i] > rs[j]) {
//					rs[i]= rs[j]; rs[j]= v; v= rs[i];
//					var tmp= ports[i]; ports[i]= ports[j]; ports[j]= tmp;
//				}
//			}
//		}
//		return rs;
//	}
    // ----------------------------------------------------------------------
    // Resolves the port layout position for a given edge.
    static void ResolvePortCollisionsOnSameEdge(iCS_EditorObject[] ports) {
        int nbPorts= ports.Length;
//        if(nbPorts < 2) return pos;
//        float minSeparation= iCS_EditorConfig.MinimumPortSeparation;
//        // Determine min/max position for each port.
//        float[] minPositions= new float[nbPorts];
//        float[] maxPositions= new float[nbPorts]; 
//        for(int i= 0; i < nbPorts; ++i) {
//            minPositions[i]= i*minSeparation;
//            maxPositions[i]= maxPos-(nbPorts-1-i)*minSeparation;
//        }
//        // Iterate resolving collisions
//        float[] collisions= new float[nbPorts-1];
//        bool resolveCollisionNeeded= true;
//        float allowedOverlap= 0.01f;
//        for(int r= 0; r < nbPorts && resolveCollisionNeeded; ++r) {
//            // Apply hard min/max position constraints.
//            for(int i= 0; i < nbPorts; ++i) {
//                if(Math3D.IsSmaller(pos[i], minPositions[i])) pos[i]= minPositions[i];
//                if(Math3D.IsGreater(pos[i], maxPositions[i])) pos[i]= maxPositions[i];
//            }
//            // Cummulate collisions penetration.
//            resolveCollisionNeeded= false;
//            for(int i= 0; i < nbPorts-1; ++i) {
//                float overlap= -(pos[i+1]-pos[i]-minSeparation);
//                collisions[i]= overlap;
//                if(Math3D.IsGreater(overlap, allowedOverlap)) resolveCollisionNeeded= true;
//            }
//            if(!resolveCollisionNeeded) continue;
//            // Resolve collisions.
//            for(int i= 0; i < nbPorts-1; ++i) {
//                float overlap= collisions[i]; 
//                if(Math3D.IsGreater(overlap, 0f)) {
//                    pos[i]  -= 0.5f*overlap;
//                    pos[i+1]+= 0.5f*overlap;
//                }
//            }
//        }
//        if(resolveCollisionNeeded) {
//            Debug.LogWarning("iCanScript: Difficulty stabilizing port layout !!!");
//        }
//        return pos;        
    }    
}
