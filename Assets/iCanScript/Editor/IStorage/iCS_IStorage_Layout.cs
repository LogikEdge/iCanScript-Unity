#define NEW_LAYOUT

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kLayoutEpsilon= 0.01f;
    
    // ======================================================================
    // Node Layout
    // ----------------------------------------------------------------------
    public void Layout(iCS_EditorObject obj) {
        obj.IsDirty= false;
        ExecuteIf(obj, o=> o.IsNode, o=> NodeLayout(o));
    }

    // ----------------------------------------------------------------------
    // Recompute the layout of a parent node.
    // Returns "true" if the new layout is within the window area.
    public void NodeLayout(iCS_EditorObject node, bool needsToBeCentered= false) {
        // Don't layout node if it is not visible.
        if(!node.IsVisible) return;
        
        // Update transition module name
        if(node.IsTransitionModule) {
            GetTransitionName(node);
        }
        
        // Minimized nodes are fully collapsed.
        if(node.IsIconized) {
            node.DisplaySize= iCS_Graphics.GetMaximizeIconSize(node);
            return;
        }

#if NEW_LAYOUT
        node.UpdateNodeSize();
#else
        // Resolve collision on children.
        ResolveCollisionOnChildren(node, Vector2.zero);

        // Determine needed child rect (in global space).
        Rect  childrenGlobalRect= ComputeChildrenRect(node);

        // Determine needed port height.
        float portsHeight= node.NeededPortsHeight;

        // Compute needed width.
        float titleWidth    = node.NodeTitleWidth;
        float titleHeight   = node.NodeTitleHeight;
        float leftPadding   = node.NodeLeftPadding;
        float rightPadding  = node.NodeRightPadding;
        float topPadding    = node.NodeTopPadding;
        float bottomPadding = node.NodeBottomPadding;

        // Process case without child nodes
        var globalPosition= node.GlobalPosition;
        var globalSize    = node.DisplaySize;
        if(Math3D.IsZero(childrenGlobalRect.width) || Math3D.IsZero(childrenGlobalRect.height)) {
            // Apply new width and height.
            float width =  Mathf.Max(titleWidth, leftPadding + rightPadding);
            float height= bottomPadding + Mathf.Max(titleHeight+portsHeight, topPadding);
            if(Math3D.IsNotEqual(width, globalSize.x) || Math3D.IsNotEqual(height, globalSize.y)) {
                Rect newPos= new Rect(globalPosition.x-0.5f*width, globalPosition.y-0.5f*height, width, height);
                SetLayoutPosition(node, newPos);
            }
        }
        // Process case with child nodes.
        else {
            // Adjust children local offset.
            float parentWidthFromChildren = leftPadding+rightPadding+childrenGlobalRect.width;
            float parentHeightFromChildren= bottomPadding+topPadding+childrenGlobalRect.height;
            float parentHeightFromPorts   = bottomPadding+titleHeight+portsHeight;
            float width, height;
            float xMin, yMin;
            if(titleWidth > parentWidthFromChildren) {
                width= titleWidth;
                xMin= 0.5f*(childrenGlobalRect.x+childrenGlobalRect.xMax-width);
            } else {
                width= parentWidthFromChildren;
                xMin= childrenGlobalRect.x-leftPadding;
            }
            if(parentHeightFromPorts > parentHeightFromChildren) {
                height= parentHeightFromPorts;
                yMin= 0.5f*(childrenGlobalRect.y+childrenGlobalRect.yMax-parentHeightFromPorts);
            } else {
                height= parentHeightFromChildren;
                yMin= childrenGlobalRect.y-topPadding;
            }
            var newRect= new Rect(xMin, yMin, width, height);
            var newPosition= Math3D.Middle(newRect);
            if(Math3D.IsNotEqual(newPosition.x, globalPosition.x) ||
               Math3D.IsNotEqual(newPosition.y, globalPosition.y)) {
                   node.AdjustChildLocalPosition(globalPosition-newPosition);
            }

            // Relocate node if centering is needed 
            if(needsToBeCentered) {
                var diff= globalPosition-newPosition; 
                if(Math3D.IsNotZero(diff)) {
                    newRect.x+= diff.x;
                    newRect.y+= diff.y;
                }
            }
            if(!IsSamePosition(node.GlobalRect, newRect)) {
                SetLayoutPosition(node, newRect);
            }
        }

        // Layout child ports
//        UpdatePortPositions(node);
        node.LayoutPorts();
#endif
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    void DeltaMoveInternal(iCS_EditorObject node, Vector2 delta) {
        if(Math3D.IsNotZero(delta)) {
            node.LocalPosition= node.LocalPosition + delta;
            node.IsDirty= true;
        }
    }
    // ----------------------------------------------------------------------
    Vector2 GetTopLeftCorner(iCS_EditorObject node)     {
        Rect position= node.GlobalRect;
        return new Vector2(position.xMin, position.yMin);
    }
    Vector2 GetTopRightCorner(iCS_EditorObject node)    {
        Rect position= node.GlobalRect;
        return new Vector2(position.xMax, position.yMin);
    }
    Vector2 GetBottomLeftCorner(iCS_EditorObject node)  {
        Rect position= node.GlobalRect;
        return new Vector2(position.xMin, position.yMax);
    }
    Vector2 GetBottomRightCorner(iCS_EditorObject node) {
        Rect position= node.GlobalRect;
        return new Vector2(position.xMax, position.yMax);
    }

	// ======================================================================
    // Port Query
	// ----------------------------------------------------------------------
    public iCS_EditorObject GetOverlappingPort(iCS_EditorObject port) {
        iCS_EditorObject foundPort= null;
		float bestDistance= iCS_Config.PortSize;
        Vector2 position= port.GlobalPosition;
        FilterWith(
            p=> p.IsPort && p != port && p.IsVisible,
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
