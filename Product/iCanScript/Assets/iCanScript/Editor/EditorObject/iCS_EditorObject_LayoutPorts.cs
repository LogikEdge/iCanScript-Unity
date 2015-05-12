using UnityEngine;
using System;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ----------------------------------------------------------------------
        // Lays out the ports of this node using the position ratio and node size
        // as basis.
        public void LayoutPorts() {
            if(IsBehaviour) return;
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
    		var displaySize= LocalSize;
            var halfSize= 0.5f*displaySize.x;
    		var x= ports[0].IsOnLeftEdge ? -halfSize : halfSize;
    		for(int i= 0; i < nbPorts; ++i) {
    			ports[i].CollisionOffsetFromLocalPosition= new Vector2(x, top+ys[i]);
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
    		var displaySize= LocalSize;
            var halfSize= 0.5f*displaySize.y;
    		var y= ports[0].IsOnTopEdge ? -halfSize : halfSize;
    		for(int i= 0; i < nbPorts; ++i) {
    			ports[i].CollisionOffsetFromLocalPosition= new Vector2(left+xs[i], y);
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
            float minSeparation= iCS_EditorConfig.kMinimumPortSeparation;
            if(minSeparation*(nbPorts-1) >= availableLength) {
                var step= availableLength/(nbPorts-1);
                for(int i= 0; i < nbPorts; ++i) {
                    sortedPosition[i]= i*step;
                }
                return sortedPosition;
            }
            // We have enough space to resolve the port layout and avoid overlap.
            bool mustRelayout= true;
            while(mustRelayout) {
                mustRelayout= false;
                for (int i= 0; i < nbPorts-1; ++i) {
                    var p1= sortedPosition[i];
                    int j= i+1;
                    var p2= sortedPosition[j];
                    var diff= p2-p1;
                    if(Math3D.IsGreater(diff, minSeparation)) continue;
                    mustRelayout= Math3D.IsSmaller(diff, minSeparation);
                    for (; j < nbPorts-1; ++j) {
                        p1= p2;
                        p2= sortedPosition[j+1];
                        diff= p2-p1;
                        if(Math3D.IsGreater(diff, minSeparation)) break;
                        if(Math3D.IsSmaller(diff, minSeparation)) {
                            mustRelayout= true;
                        }
                    }
                    if(mustRelayout) {
                        var iPos= sortedPosition[i];
                        var jPos= sortedPosition[j];
                        diff= sortedPosition[j]-sortedPosition[i];
                        var minSep= (j-i)*minSeparation;
                        var halfDelta= 0.5f*(minSep-diff);
                        iPos= iPos-halfDelta;
                        jPos= jPos+halfDelta;
                        if(iPos < 0) {
                            jPos+= -iPos;
                            iPos= 0;
                        }
                        if(jPos > availableLength) {
                            iPos-= jPos-availableLength;
                        }
                        for(int k= 0; k <= (j-i); ++k) {
                            sortedPosition[i+k]= iPos+k*minSeparation;
                        }
                        break;
                    }
                }        
            }
            return sortedPosition;
        }
    
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
            return SortPorts(ports, p=> p.LocalPosition.x);
        }
        // ----------------------------------------------------------------------
        static iCS_EditorObject[] SortVerticalPortsOnLayout(iCS_EditorObject[] ports) {
            return SortPorts(ports, p=> p.LocalPosition.y);
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
}
