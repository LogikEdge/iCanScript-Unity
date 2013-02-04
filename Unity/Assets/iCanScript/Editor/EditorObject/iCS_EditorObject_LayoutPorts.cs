using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Lays out the ports of this node using the position ratio and node size
    // as basis.
    public void LayoutPorts() {
        var halfSize= 0.5f*DisplaySize;
        var verticalTop    = VerticalPortsTop;
        var verticalBottom = VerticalPortsBottom;
        var horizontalLeft = HorizontalPortsLeft;
        var horizontalRight= HorizontalPortsRight;
        LayoutPortsOnVerticalEdge  (LeftPorts  , verticalTop   , verticalBottom, -halfSize.x);
        LayoutPortsOnVerticalEdge  (RightPorts , verticalTop   , verticalBottom,  halfSize.x);
        LayoutPortsOnHorizontalEdge(TopPorts   , horizontalLeft, horizontalRight, -halfSize.y);
        LayoutPortsOnHorizontalEdge(BottomPorts, horizontalLeft, horizontalRight,  halfSize.y);
    }
    // ----------------------------------------------------------------------
    static void LayoutPortsOnVerticalEdge(iCS_EditorObject[] ports,
                                          float top, float bottom, float x) {
        // Start from the anchor position.
        int nbPorts= ports.Length;
        float[] ys= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            ys[i]= ports[i].LocalAnchorPosition.y-top;
        }
        // Sort the anchor position from top to bottom.
        for(int i= 0; i < nbPorts-1; ++i) {
            for(int j= i+1; j < nbPorts; ++j) {
                var v= ys[i];
                if(v > ys[j]) {
                    ys[i]= ys[j]; ys[j]= v; v= ys[i];
                    var t= ports[i]; ports[i]= ports[j]; ports[j]= t;
                }
            }
        }
        // Resolve port collisions.
        ys= ResolvePortCollisions(ys, bottom-top);
		// Update position from new layout.
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalLayoutPosition= new Vector2(x, top+ys[i]);
		}
    }
    // ----------------------------------------------------------------------
    static void LayoutPortsOnHorizontalEdge(iCS_EditorObject[] ports,
                                            float left, float right, float y) {
        // Start from the anchor position.
        int nbPorts= ports.Length;
        float[] xs= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            xs[i]= ports[i].LocalAnchorPosition.x-left;
        }
        // Sort the anchor position from left to right.
        for(int i= 0; i < nbPorts-1; ++i) {
            for(int j= i+1; j < nbPorts; ++j) {
                var v= xs[i];
                if(v > xs[j]) {
                    xs[i]= xs[j]; xs[j]= v; v= xs[i];
                    var t= ports[i]; ports[i]= ports[j]; ports[j]= t;
                }
            }
        }
        // Resolve port collisions.
        xs= ResolvePortCollisions(xs, right-left);
		// Update position from new layout.
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalLayoutPosition= new Vector2(left+xs[i], y);
		}
    }
    // ----------------------------------------------------------------------
    // Resolves the port layout position for a given edge.
    static float[] ResolvePortCollisions(float[] sortedPosition, float availableLength) {
        // Nothing collision to resolve if we don't have at least two ports.
        int nbPorts= sortedPosition.Length;
        if(nbPorts < 2) return sortedPosition;
        // We can't resolve collision if we don't have enough space to put all ports.
        // In such a situation, we simply distribute the ports has best we can on the
        // available size.
        float minSeparation= iCS_EditorConfig.MinimumPortSeparation;
        if(minSeparation*(nbPorts-1) >= availableLength) {
            var step= availableLength/(nbPorts-1);
            for(int i= 0; i < nbPorts; ++i) {
                sortedPosition[i]= i*step;
            }
            return sortedPosition;
        }
        // We have enough space to resolve the port layout and avoid overlap.
        // Determine min/max position for each port.
        float[] minPositions= new float[nbPorts];
        float[] maxPositions= new float[nbPorts]; 
        for(int i= 0; i < nbPorts; ++i) {
            minPositions[i]= i*minSeparation;
            maxPositions[i]= availableLength-(nbPorts-1-i)*minSeparation;
        }
        // Iterate resolving collisions
        float[] collisions= new float[nbPorts-1];
        bool resolveCollisionNeeded= true;
        float allowedOverlap= 0.01f;
        for(int r= 0; r < nbPorts && resolveCollisionNeeded; ++r) {
            // Apply hard min/max position constraints.
            for(int i= 0; i < nbPorts; ++i) {
                if(Math3D.IsSmaller(sortedPosition[i], minPositions[i])) sortedPosition[i]= minPositions[i];
                if(Math3D.IsGreater(sortedPosition[i], maxPositions[i])) sortedPosition[i]= maxPositions[i];
            }
            // Cummulate collisions penetration.
            resolveCollisionNeeded= false;
            for(int i= 0; i < nbPorts-1; ++i) {
                float overlap= -(sortedPosition[i+1]-sortedPosition[i]-minSeparation);
                collisions[i]= overlap;
                if(Math3D.IsGreater(overlap, allowedOverlap)) resolveCollisionNeeded= true;
            }
            if(!resolveCollisionNeeded) continue;
            // Resolve collisions.
            for(int i= 0; i < nbPorts-1; ++i) {
                float overlap= collisions[i]; 
                if(Math3D.IsGreater(overlap, 0f)) {
                    sortedPosition[i]  -= 0.5f*overlap;
                    sortedPosition[i+1]+= 0.5f*overlap;
                }
            }
        }
        if(resolveCollisionNeeded) {
            Debug.LogWarning("iCanScript: Difficulty stabilizing port layout !!!");
        }
        return sortedPosition;
    }
}
