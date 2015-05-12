using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Object Picking
        // ----------------------------------------------------------------------
        // Returns the node at the given position
        public iCS_EditorObject GetNodeAt(Vector2 pick, iCS_EditorObject exclude= null) {
            iCS_EditorObject foundNode= null;
            FilterWith(
                n=> {
                    bool excludeFlag= false;
                    if(exclude != null) {
                        excludeFlag= n == exclude || IsChildOf(n, exclude);
                    }
                    return !excludeFlag && n.IsNode && n.IsVisibleInLayout && IsInside(n, pick) && (foundNode == null || n.LocalSize.x < foundNode.LocalSize.x);
                },
                n=> foundNode= n
            );
            return foundNode;
        }
        // ----------------------------------------------------------------------
        // Returns the connection at the given position.
        public iCS_EditorObject GetPortAt(Vector2 pick, Func<iCS_EditorObject,bool> filter= null) {
            iCS_EditorObject port= GetClosestPortAt(pick, filter);
            if(port == null) return port;
            float distance= Vector2.Distance(port.GlobalPosition, pick);
            return (distance < iCS_EditorConfig.PortRadius*1.2f) ? port : null;
        }
        // ----------------------------------------------------------------------
        // Returns the connection at the given position.
        public iCS_EditorObject GetClosestPortAt(Vector2 pick, Func<iCS_EditorObject,bool> filter= null) {
            iCS_EditorObject bestPort= null;
            float bestDistance= 100000;     // Simply a big value
            if(filter == null) filter= GetPortAtDefaultFilter;
            FilterWith(
                port=> port.IsPort && port.IsVisibleInLayout && !port.IsFloating && filter(port),
                port=> {
                    Vector2 position= port.GlobalPosition;
                    float distance= Vector2.Distance(position, pick);
                    if(distance < bestDistance) {
                        bestDistance= distance;
                        bestPort= port;
                    }                                
                } 
            );
            return bestPort;
        }
        bool GetPortAtDefaultFilter(iCS_EditorObject port) { return true; }

        // ----------------------------------------------------------------------
        // Returns the node at the given position
        public iCS_EditorObject GetNodeWithEdgeAt(Vector2 pick, iCS_EditorObject exclude= null) {
            iCS_EditorObject foundNode= null;
            FilterWith(
                n=> {
                    bool excludeFlag= false;
                    if(exclude != null) {
                        excludeFlag= n == exclude || IsChildOf(n, exclude);
                    }
                    if(excludeFlag || !n.IsNode || !n.IsVisibleInLayout) return false;
                    var portRadius= iCS_EditorConfig.PortRadius;
                    var portSize= 2f*portRadius;
                    var globalRect= n.GlobalRect;
                    var outterEdge= new Rect(globalRect.x-portRadius, globalRect.y-portRadius, globalRect.width+portSize, globalRect.height+portSize);
                    var innerEdge = new Rect(globalRect.x+portRadius, globalRect.y+portRadius, globalRect.width-portSize, globalRect.height-portSize);
                    return outterEdge.Contains(pick) && !innerEdge.Contains(pick) &&
                           (foundNode == null || n.LocalSize.x < foundNode.LocalSize.x);
                },
                n=> foundNode= n
            );
            return foundNode;
        }
    	// ----------------------------------------------------------------------
        public iCS_EditorObject GetOverlappingPort(iCS_EditorObject port) {
            iCS_EditorObject foundPort= null;
    		float bestDistance= iCS_EditorConfig.PortDiameter;
            Vector2 position= port.GlobalPosition;
            FilterWith(
                p=> p.IsPort && p != port && p.IsVisibleInLayout,
                p=> {
                    float distance= Vector2.Distance(p.GlobalPosition, position);
                    if(distance < bestDistance) {
    					bestDistance= distance;
                        foundPort= p;
                    }
                }
            );
            return foundPort;
        }	

    }

}
