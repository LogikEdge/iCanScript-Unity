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
    // Determine if position are the same.
    public bool IsSamePosition(Rect r1, Rect r2) {
        return Math3D.IsWithin(r1.x, r2.x-kLayoutEpsilon, r2.x+kLayoutEpsilon) &&
               Math3D.IsWithin(r1.y, r2.y-kLayoutEpsilon, r2.y+kLayoutEpsilon) &&
               Math3D.IsWithin(r1.width, r2.width-kLayoutEpsilon, r2.width+kLayoutEpsilon) &&
               Math3D.IsWithin(r1.height, r2.height-kLayoutEpsilon, r2.height+kLayoutEpsilon);
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void MoveTo(iCS_EditorObject node, Vector2 newPos) {
        var delta= newPos - node.GlobalPosition;
        DeltaMove(node, delta);
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void DeltaMove(iCS_EditorObject node, Vector2 delta) {
        // Move the node
        DeltaMoveInternal(node, delta);
        // Resolve collision between siblings.
        LayoutParent(node, delta);
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
    // ----------------------------------------------------------------------
    void LayoutParent(iCS_EditorObject node, Vector2 deltaMove) {
        if(!IsValid(node.ParentId)) return;
        iCS_EditorObject parentNode= node.Parent;
        ResolveCollision(parentNode, deltaMove);
        Layout(parentNode);
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildrenRect(iCS_EditorObject node) {
        // Compute child space.
        Rect childrenRect= new Rect(0,0,0,0);
        node.ForEachChildNode(
            child=> {
                if(child.IsVisible && !child.IsFloating) {
                    var childPos= child.GlobalRect;
                    if(Math3D.IsZero(childrenRect.width)) {
                        childrenRect= childPos;
                    } else {
                        childrenRect= Math3D.Merge(childrenRect, childPos);                        
                    }
                }
            }
        );
        return childrenRect;
    }

    // ======================================================================
    // Port Layout
    // ----------------------------------------------------------------------
    bool AreChildrenInSameOrder(iCS_EditorObject node, iCS_EditorObject[] orderedChildren) {
        return node.AreChildrenInSameOrder(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    void ReorderChildren(iCS_EditorObject node, iCS_EditorObject[] orderedChildren) {
        node.ReorderChildren(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortTopPorts(iCS_EditorObject node) {
        Rect nodePos= node.GlobalRect;
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        iCS_EditorObject[] ports= node.TopPorts;
        Vector2[] connectedPos= Prelude.map(p=> p.GlobalPosition, ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? cp.y : -cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortBottomPorts(iCS_EditorObject node) {
        Rect nodePos= node.GlobalRect;
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        iCS_EditorObject[] ports= node.BottomPorts;
        Vector2[] connectedPos= Prelude.map(p=> p.GlobalPosition, ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? -cp.y : cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortLeftPorts(iCS_EditorObject node) {
        Rect nodePos= node.GlobalRect;
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= node.LeftPorts;                             
        Vector2[] connectedPos= Prelude.map(p=> p.GlobalPosition, ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? cp.x : -cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortRightPorts(iCS_EditorObject node) {
        Rect nodePos= node.GlobalRect;
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= node.RightPorts;
        Vector2[] connectedPos= Prelude.map(p=> p.GlobalPosition, ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? -cp.x : cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    // Sorts the given port according to their relative positions.
    iCS_EditorObject[] SortPorts(iCS_EditorObject[] ports, float[] keys1, float[] keys2) {
        for(int i= 0; i < ports.Length-1; ++i) {
            for(int j= i+1; j < ports.Length; ++j) {
                if(Math3D.IsGreater(keys1[i], keys1[j])) {
                    Exchange(ref ports[i], ref ports[j]);
                    Exchange(ref keys1[i], ref keys1[j]);
                    Exchange(ref keys2[i], ref keys2[j]);
                } else if(Math3D.IsEqual(keys1[i], keys1[j])) {                
                    if(Math3D.IsGreater(keys2[i], keys2[j])) {
                        Exchange(ref ports[i], ref ports[j]);
                        Exchange(ref keys1[i], ref keys1[j]);
                        Exchange(ref keys2[i], ref keys2[j]);                    
                    }
                }
            }
        }
        return ports;
    }
    void Exchange(ref iCS_EditorObject a, ref iCS_EditorObject b) {
        iCS_EditorObject tmp= a;
        a= b;
        b= tmp;
    }
    void Exchange(ref float a, ref float b) {
        float tmp= a;
        a= b;
        b= tmp;
    }

    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolve collision on parents.
    void ResolveCollision(iCS_EditorObject node, Vector2 delta) {
        ResolveCollisionOnChildren(node, delta);
        if(!IsValid(node.ParentId)) return;
        ResolveCollision(node.Parent, delta);
    }

    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren(iCS_EditorObject node, Vector2 delta) {
        bool didCollide= false;
        iCS_EditorObject[] children= BuildListOfChildren(c=> c.IsVisible && c.IsNode && !c.IsFloating, node);
        for(int i= 0; i < children.Length-1; ++i) {
            for(int j= i+1; j < children.Length; ++j) {
                didCollide |= ResolveCollisionBetweenTwoNodes(children[i], children[j], delta);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildren(node, delta);
    }

    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject node, iCS_EditorObject otherNode, Vector2 delta) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithGutter(node, otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(node, otherNode.GlobalRect);
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

		// Seperate using the known movement.
        if( !Math3D.IsZero(delta) ) {
    		if(Vector2.Dot(delta, penetration) > 0) {
    		    DeltaMoveInternal(otherNode, penetration);
    		}
    		else {
    		    DeltaMoveInternal(node, -penetration);
    		}            
    		return true;
        }

		// Seperate nodes by the penetration that is not a result of movement.
        penetration*= 0.5f;
        DeltaMoveInternal(otherNode, penetration);
        DeltaMoveInternal(node, -penetration);
        return true;
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollide(iCS_EditorObject node, iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(node.GlobalRect, otherNode.GlobalRect);
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithGutter(iCS_EditorObject node, iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(iCS_EditorObject.AddMargins(node.GlobalRect), otherNode.GlobalRect);
    }

    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(iCS_EditorObject node, Rect _rect) {
        Rect myRect= iCS_EditorObject.AddMargins(node.GlobalRect);
        Rect otherRect= _rect;
        float xMin= Mathf.Min(myRect.xMin, otherRect.xMin);
        float yMin= Mathf.Min(myRect.yMin, otherRect.yMin);
        float xMax= Mathf.Max(myRect.xMax, otherRect.xMax);
        float yMax= Mathf.Max(myRect.yMax, otherRect.yMax);
        float xDistance= xMax-xMin;
        float yDistance= yMax-yMin;
        float totalWidth= myRect.width+otherRect.width;
        float totalHeight= myRect.height+otherRect.height;
        if(xDistance >= totalWidth) return Vector2.zero;
        if(yDistance >= totalHeight) return Vector2.zero;
        if((totalWidth-xDistance) < (totalHeight-yDistance)) {
            if(myRect.xMin < otherRect.xMin) {
                return new Vector2(totalWidth-xDistance, 0);
            }
            else {
                return new Vector2(xDistance-totalWidth, 0);
            }
        }
        else {
            if(myRect.yMin < otherRect.yMin) {
                return new Vector2(0, totalHeight-yDistance);
            }
            else {
                return new Vector2(0, yDistance-totalHeight);                
            }            
        }
	}


	// ======================================================================
    // Layout from iCS_Port
    // ----------------------------------------------------------------------
    // Updates the port position.
    public void UpdatePortPositions(iCS_EditorObject node) {
        // Special case for minimized nodes.
        if(node.IsIconized) {
            ForEachChildPort(node, port=> {
                port.LocalPosition= Vector2.zero;
            });
            return;
        }

        // Parent node edge offsets.
        var parentSize= node.DisplaySize;
        float leftX  = -0.5f*parentSize.x;
        float rightX =  0.5f*parentSize.x;
        float topY   = -0.5f*parentSize.y;
        float bottomY=  0.5f*parentSize.y;

        // Relayout top ports.                
        iCS_EditorObject[] ports= SortTopPorts(node);
        if(ports.Length != 0) {
            float xStep= parentSize.x / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(leftX+(i+0.5f)*xStep, topY);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout bottom ports.
        ports= SortBottomPorts(node);
        if(ports.Length != 0) {
            float xStep= parentSize.x / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(leftX+(i+0.5f)*xStep, bottomY);
                }
            }            
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout left ports.
        float topOffset= node.NodeTitleHeight;
        ports= SortLeftPorts(node);
        if(ports.Length != 0) {
            float yStep= (parentSize.y-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(leftX, topY+topOffset+(i+0.5f) * yStep);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout right ports.
        ports= SortRightPorts(node);
        if(ports.Length != 0) {
            float yStep= (parentSize.y-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(rightX, topY+topOffset+(i+0.5f)*yStep);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }        
    }

    // ======================================================================
    // Port Query
	// ----------------------------------------------------------------------
    public iCS_EditorObject GetOverlappingPort(iCS_EditorObject port) {
        iCS_EditorObject foundPort= null;
        Vector2 position= port.GlobalPosition;
        FilterWith(
            p=> p.IsPort && p != port && p.IsVisible,
            p=> {
                float distance= Vector2.Distance(p.GlobalPosition, position);
                if(distance <= 1.5*iCS_Config.PortSize) {
                    foundPort= p;
                }
            }
        );
        return foundPort;
    }	
}
