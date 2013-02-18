#define NEW_PORT_COLLISION_ALGO
using UnityEngine;
using System;
using System.Collections;

/*
 	FIXME: Port anchorPosition, LayoutPosition and DisplayPosition should all
		   be computed as a block when parent is resized.
*/
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Lays out the ports of this node using the position ratio and node size
    // as basis.
    public void LayoutPorts() {
        LayoutPortsOnVerticalEdge(LeftPorts);
        LayoutPortsOnVerticalEdge(RightPorts);
        LayoutPortsOnHorizontalEdge(TopPorts);
        LayoutPortsOnHorizontalEdge(BottomPorts);
    }
    // ----------------------------------------------------------------------
	public void LayoutPortsOnSameEdge(iCS_EditorObject[] ports) {
		if(ports.Length == 0) return;
		if(ports[0].IsOnHorizontalEdge) {
			LayoutPortsOnHorizontalEdge(ports);
		} else {
			LayoutPortsOnVerticalEdge(ports);
		}
	}
    // ----------------------------------------------------------------------
    public void LayoutPortsOnVerticalEdge(iCS_EditorObject[] ports) {
		if(ports.Length == 0) return;
		// Get vertical top/bottom port range.
        var top    = VerticalPortsTop;
        var bottom = VerticalPortsBottom;
		// Sort ports in ascending display order.
        ports= SortVerticalPortsOnAnchor(ports);
        // Start from the anchor position.
        int nbPorts= ports.Length;
        float[] ys= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            ys[i]= ports[i].LocalAnchorPosition.y-top;
        }
        // Resolve port collisions.
        ys= ResolvePortCollisions(ys, bottom-top);
		// Update position from new layout.
		var displaySize= DisplaySize;
        var halfSize= 0.5f*displaySize.x;
		var x= ports[0].IsOnLeftEdge ? -halfSize : halfSize;
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalLayoutPosition= new Vector2(x, top+ys[i]);
		}
    }
    // ----------------------------------------------------------------------
    public void LayoutPortsOnHorizontalEdge(iCS_EditorObject[] ports) {
		if(ports.Length == 0) return;
		// Get horizontal start/end port range
        var left = HorizontalPortsLeft;
        var right= HorizontalPortsRight;
		// Sort ports in ascendoing display order										
        ports= SortHorizontalPortsOnAnchor(ports);
        // Start from the anchor position.
        int nbPorts= ports.Length;
        float[] xs= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            xs[i]= ports[i].LocalAnchorPosition.x-left;
        }
        // Resolve port collisions.
        xs= ResolvePortCollisions(xs, right-left);
		// Update position from new layout.
		var displaySize= DisplaySize;
        var halfSize= 0.5f*displaySize.y;
		var y= ports[0].IsOnTopEdge ? -halfSize : halfSize;
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalLayoutPosition= new Vector2(left+xs[i], y);
		}
    }
    // IMPROVE: Remove port shake when user drags.
#if NEW_PORT_COLLISION_ALGO
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
    for (int i= 0; i < nbPorts-1; ++i) {
        var iPos= sortedPosition[i];
        for (int j= nbPorts-1; j != i; --j) {
            var jPos= sortedPosition[j];
            var minSep= (j-i)*minSeparation;
            var separation= jPos-iPos;
            if(Math3D.IsSmaller(separation, minSep)) {
                var halfDelta= 0.5f*(minSep-separation);
                iPos= iPos-halfDelta;
                jPos= jPos+halfDelta;
                if(iPos < 0) {
                    jPos+= -iPos;
                    iPos= 0;
                }
                if(jPos > availableLength) {
                    iPos-= jPos-availableLength;
                    jPos= availableLength;
                }
                for(int k= 0; k <= (j-i); ++k) {
                    sortedPosition[i+k]= iPos+k*minSeparation;
                }
            }
        }
    }
    return sortedPosition;
}
#else
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
            if(Math3D.IsSmaller(sortedPosition[i], minPositions[i])) sortedPosition[i]= minPositions[i];
            if(Math3D.IsGreater(sortedPosition[i], maxPositions[i])) sortedPosition[i]= maxPositions[i];
        }
        // Iterate resolving collisions
        float kAllowedOverlap= 0.01f;
		int retry= nbPorts*nbPorts;
        for(int i= 0; i < nbPorts-1 && retry > 0; ++i) {
            float overlap= -(sortedPosition[i+1]-sortedPosition[i]-minSeparation);
            if(Math3D.IsGreater(overlap, kAllowedOverlap)) {
				if(Math3D.IsEqual(sortedPosition[i], minPositions[i])) {
                    sortedPosition[i+1]+= overlap;						
				} else if(Math3D.IsEqual(sortedPosition[i+1], maxPositions[i+1])) {
                    sortedPosition[i]  -= overlap;
				} else {
                    sortedPosition[i]  -= 0.5f*overlap;
                    sortedPosition[i+1]+= 0.5f*overlap;						
				}
	            if(Math3D.IsSmaller(sortedPosition[i], minPositions[i])) sortedPosition[i]= minPositions[i];
	            if(Math3D.IsGreater(sortedPosition[i], maxPositions[i])) sortedPosition[i]= maxPositions[i];
	            if(Math3D.IsSmaller(sortedPosition[i+1], minPositions[i+1])) sortedPosition[i+1]= minPositions[i+1];
	            if(Math3D.IsGreater(sortedPosition[i+1], maxPositions[i+1])) sortedPosition[i+1]= maxPositions[i+1];
				// Restart from previous
				--retry;
				if(i != 0) i-= 2;
			}
        }
        if(retry <= 0) {
            /*
                FIXME: Port layout not stabilizing when node is animated.
            */
            Debug.LogWarning("iCanScript: Difficulty stabilizing port layout !!!");
        }
        return sortedPosition;
    }
#endif

    // ======================================================================
    // Port ordering utilities.
    // ----------------------------------------------------------------------
    public static iCS_EditorObject[] SortPortsOnAnchor(iCS_EditorObject[] ports) {
        if(ports.Length == 0) return ports;
        if(ports[0].IsOnHorizontalEdge) return SortHorizontalPortsOnAnchor(ports);
        return SortVerticalPortsOnAnchor(ports);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortHorizontalPortsOnAnchor(iCS_EditorObject[] ports) {
        return SortPorts(ports, p=> p.LocalAnchorPosition.x);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortVerticalPortsOnAnchor(iCS_EditorObject[] ports) {
        return SortPorts(ports, p=> p.LocalAnchorPosition.y);
    }
    // ----------------------------------------------------------------------
    public static iCS_EditorObject[] SortPortsOnLayout(iCS_EditorObject[] ports) {
        if(ports.Length == 0) return ports;
        if(ports[0].IsOnHorizontalEdge) return SortHorizontalPortsOnLayout(ports);
        return SortVerticalPortsOnLayout(ports);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortHorizontalPortsOnLayout(iCS_EditorObject[] ports) {
        return SortPorts(ports, p=> p.LocalLayoutPosition.x);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortVerticalPortsOnLayout(iCS_EditorObject[] ports) {
        return SortPorts(ports, p=> p.LocalLayoutPosition.y);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortPorts(iCS_EditorObject[] ports, Func<iCS_EditorObject,float> key) {
        // Accumulate keys.
        int nbPorts= ports.Length;
        float[] ks= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            ks[i]= key(ports[i]);
        }
        // Sort the anchor position from left to right.
        return SortPorts(ports, ref ks);
    }
    // ----------------------------------------------------------------------
    static iCS_EditorObject[] SortPorts(iCS_EditorObject[] ports, ref float[] ks) {
        // Sort the anchor position from left to right.
        int nbPorts= ports.Length;
        for(int i= 0; i < nbPorts-1; ++i) {
            for(int j= i+1; j < nbPorts; ++j) {
                var v= ks[i];
                if(v > ks[j]) {
                    ks[i]= ks[j]; ks[j]= v; v= ks[i];
                    var t= ports[i]; ports[i]= ports[j]; ports[j]= t;
                }
            }
        }
        return ports;        
    }
}
