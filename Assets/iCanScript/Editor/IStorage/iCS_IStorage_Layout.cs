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
    // Moves the node without changing its size.
    public void SetInitialPosition(iCS_EditorObject obj, Vector2 initialPosition) {
        if(IsValid(obj.ParentId)) {
            Rect position= GetLayoutPosition(EditorObjects[obj.ParentId]);
            obj.LocalRect= new Rect(initialPosition.x-position.x, initialPosition.y-position.y,
                                    obj.DisplaySize.x, obj.DisplaySize.y);
        }
        else {
            obj.LocalRect= new Rect(initialPosition.x, initialPosition.y, obj.DisplaySize.x, obj.DisplaySize.y);
        }
        SetDirty(obj);
    }

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

        // Refresh children that have not yet performed a layout.
        ForEachChild(node, c=> { if(c.LocalRect.x == 0 && c.LocalRect.y == 0) SetDirty(c); });
        
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
        float childrenWidth = childrenGlobalRect.width;
        float childrenHeight= childrenGlobalRect.height; 
        float width         =  Mathf.Max(titleWidth, leftPadding + rightPadding + childrenWidth);
        float height        = bottomPadding + Mathf.Max(titleHeight+portsHeight, topPadding+childrenHeight);

        // Process case without child nodes
        Rect globalPosition= GetLayoutPosition(node);
        if(Math3D.IsZero(childrenGlobalRect.width) || Math3D.IsZero(childrenGlobalRect.height)) {
            // Apply new width and height.
            if(Math3D.IsNotEqual(height, globalPosition.height) || Math3D.IsNotEqual(width, globalPosition.width)) {
                float deltaWidth = width - globalPosition.width;
                float deltaHeight= height - globalPosition.height;
                Rect newPos= new Rect(globalPosition.xMin-0.5f*deltaWidth, globalPosition.yMin-0.5f*deltaHeight, width, height);
                SetLayoutPosition(node, newPos);
            }
        }
        // Process case with child nodes.
        else {
            // Adjust children local offset.
            float neededChildXOffset= leftPadding;
            float neededChildYOffset= topPadding;
            float neededNodeGlobalX= childrenGlobalRect.x-neededChildXOffset;
            float neededNodeGlobalY= childrenGlobalRect.y-neededChildYOffset;
            if(Math3D.IsNotEqual(neededNodeGlobalX, globalPosition.x) ||
               Math3D.IsNotEqual(neededNodeGlobalY, globalPosition.y)) {
                   node.AdjustChildPosition(new Vector2(globalPosition.x-neededNodeGlobalX, globalPosition.y-neededNodeGlobalY));
            }

            // Relocate node if centering is needed 
            Rect newPos= new Rect(neededNodeGlobalX, neededNodeGlobalY, width, height);
            if(needsToBeCentered) {
                var currentCenter= Math3D.Middle(globalPosition);
                var newCenter= Math3D.Middle(newPos);
                var diff= currentCenter-newCenter; 
                if(Math3D.IsNotZero(diff)) {
                    newPos.x+= diff.x;
                    newPos.y+= diff.y;
                }
            }
            if(!IsSamePosition(globalPosition, newPos)) {
                SetLayoutPosition(node, newPos);
            }
        }

        // Layout child ports
        UpdatePortPositions(node);
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
    public void MoveTo(iCS_EditorObject node, Vector2 _newPos) {
        Rect position = GetLayoutPosition(node);
        var delta= new Vector2(_newPos.x - position.x, _newPos.y - position.y);
        DeltaMove(node, delta);
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void DeltaMove(iCS_EditorObject node, Vector2 _delta) {
        // Move the node
        DeltaMoveInternal(node, _delta);
        // Resolve collision between siblings.
        LayoutParent(node, _delta);
	}
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    void DeltaMoveInternal(iCS_EditorObject node, Vector2 _delta) {
        if(Math3D.IsNotZero(_delta)) {
            node.LocalRect= new Rect(node.LocalRect.x+_delta.x, node.LocalRect.y+_delta.y,
                                     node.DisplaySize.x, node.DisplaySize.y);
            SetDirty(node);
        }
    }
    // ----------------------------------------------------------------------
    Vector2 GetTopLeftCorner(iCS_EditorObject node)     {
        Rect position= GetLayoutPosition(node);
        return new Vector2(position.xMin, position.yMin);
    }
    Vector2 GetTopRightCorner(iCS_EditorObject node)    {
        Rect position= GetLayoutPosition(node);
        return new Vector2(position.xMax, position.yMin);
    }
    Vector2 GetBottomLeftCorner(iCS_EditorObject node)  {
        Rect position= GetLayoutPosition(node);
        return new Vector2(position.xMin, position.yMax);
    }
    Vector2 GetBottomRightCorner(iCS_EditorObject node) {
        Rect position= GetLayoutPosition(node);
        return new Vector2(position.xMax, position.yMax);
    }
    // ----------------------------------------------------------------------
    void LayoutParent(iCS_EditorObject node, Vector2 _deltaMove) {
        if(!IsValid(node.ParentId)) return;
        iCS_EditorObject parentNode= node.Parent;
        ResolveCollision(parentNode, _deltaMove);
        Layout(parentNode);
    }
//    // ----------------------------------------------------------------------
//    // Returns the space used by all children.
//    Rect ComputeChildRect(iCS_EditorObject node) {
//        // Compute child space.
//        Rect childRect= new Rect(0.5f*node.DisplaySize.x,0.5f*node.DisplaySize.y,0,0);
//        ForEachChild(node,
//            (child)=> {
//                if(child.IsNode && child.IsVisible && !child.IsFloating) {
//                    childRect= Math3D.Merge(childRect, child.LocalRect);
//                }
//            }
//        );
//        return childRect;
//    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildrenRect(iCS_EditorObject node) {
        // Compute child space.
        Rect childrenRect= new Rect(0,0,0,0);
        node.ForEachChildNode(
            child=> {
                if(child.IsVisible && !child.IsFloating) {
                    var childPos= GetLayoutPosition(child);
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
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        iCS_EditorObject[] ports= node.TopPorts;
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? cp.y : -cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortBottomPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        iCS_EditorObject[] ports= node.BottomPorts;
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? -cp.y : cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortLeftPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= node.LeftPorts;                             
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? cp.x : -cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortRightPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= node.RightPorts;
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
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
    void ResolveCollision(iCS_EditorObject node, Vector2 _delta) {
        ResolveCollisionOnChildren(node, _delta);
        if(!IsValid(node.ParentId)) return;
        ResolveCollision(node.Parent, _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren(iCS_EditorObject node, Vector2 _delta) {
        bool didCollide= false;
        iCS_EditorObject[] children= BuildListOfChildren(c=> c.IsVisible && c.IsNode && !c.IsFloating,node);
        for(int i= 0; i < children.Length-1; ++i) {
            for(int j= i+1; j < children.Length; ++j) {
                didCollide |= ResolveCollisionBetweenTwoNodes(children[i], children[j], _delta);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildren(node, _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject node, iCS_EditorObject otherNode, Vector2 _delta) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithGutter(node, otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(node, GetLayoutPosition(otherNode));
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

		// Seperate using the known movement.
        if( !Math3D.IsZero(_delta) ) {
    		if(Vector2.Dot(_delta, penetration) > 0) {
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
        return Math3D.DoesCollide(GetLayoutPosition(node), GetLayoutPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithGutter(iCS_EditorObject node, iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(iCS_EditorObject.AddMargins(GetLayoutPosition(node)), GetLayoutPosition(otherNode));
    }

    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(iCS_EditorObject node, Rect _rect) {
        Rect myRect= iCS_EditorObject.AddMargins(GetLayoutPosition(node));
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
            float cx= 0.5f*node.DisplaySize.x;
            float cy= 0.5f*node.DisplaySize.y;
            ForEachChildPort(node, port=> {
                port.LocalRect= new Rect(cx,cy, port.DisplaySize.x, port.DisplaySize.y);
            });
            return;
        }

        // Relayout top ports.
        var size= node.DisplaySize;        
        iCS_EditorObject[] ports= SortTopPorts(node);
        if(ports.Length != 0) {
            float xStep= size.x / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2((i+0.5f)*xStep, 0);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout bottom ports.
        ports= SortBottomPorts(node);
        if(ports.Length != 0) {
            float xStep= size.x / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2((i+0.5f)*xStep, size.y);
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
            float yStep= (size.y-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(0, topOffset+(i+0.5f) * yStep);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout right ports.
        ports= SortRightPorts(node);
        if(ports.Length != 0) {
            float yStep= (size.y-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalPosition= new Vector2(size.x, topOffset+(i+0.5f)*yStep);
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
        Rect tmp= GetLayoutPosition(port);
        Vector2 position= new Vector2(tmp.x, tmp.y);
        FilterWith(
            p=> p.IsPort && p != port && p.IsVisible,
            p=> {
                tmp= GetLayoutPosition(p);
                Vector2 pPos= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(pPos, position);
                if(distance <= 1.5*iCS_Config.PortSize) {
                    foundPort= p;
                }
            }
        );
        return foundPort;
    }	
}
