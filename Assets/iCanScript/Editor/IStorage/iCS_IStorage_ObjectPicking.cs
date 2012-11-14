using UnityEngine;
using System;
using System.Collections;

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
                return !excludeFlag && n.IsNode && n.IsVisible && IsInside(n, pick) && (foundNode == null || n.DisplaySize.x < foundNode.DisplaySize.x);
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
        return (distance < 3f*iCS_Config.PortRadius) ? port : null;
    }
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public iCS_EditorObject GetClosestPortAt(Vector2 pick, Func<iCS_EditorObject,bool> filter= null) {
        iCS_EditorObject bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        if(filter == null) filter= GetPortAtDefaultFilter;
        FilterWith(
            port=> port.IsPort && port.IsVisible && !port.IsFloating && filter(port),
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

}
