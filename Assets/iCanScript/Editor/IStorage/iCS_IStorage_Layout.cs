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
            Vector2 iconSize= iCS_Graphics.GetMaximizeIconSize(node);
            if(node.DisplaySize.x != iconSize.x || node.DisplaySize.y != iconSize.y) {
                if(IsValid(node.ParentId)) {
                    if(node.LocalRect.x == 0) {
                        node.LocalRect= new Rect(0.5f*node.Parent.DisplaySize.x, node.LocalRect.y,
                                                 node.DisplaySize.x, node.DisplaySize.y);
                    }
                    if(node.LocalRect.y == 0) {
                        node.LocalRect= new Rect(node.LocalRect.x, 0.5f*node.Parent.DisplaySize.y,
                                                 node.DisplaySize.x, node.DisplaySize.y);
                    }
                }
                node.LocalRect= new Rect(node.LocalRect.x+0.5f*(node.DisplaySize.x-iconSize.x),
                                         node.LocalRect.y+0.5f*(node.DisplaySize.y-iconSize.y),
                                         iconSize.x, iconSize.y);
                UpdatePortPositions(node);
            }
            return;
        }

        // Refresh children that have not yet performed a layout.
        ForEachChild(node, c=> { if(c.LocalRect.x == 0 && c.LocalRect.y == 0) SetDirty(c); });
        
        // Resolve collision on children.
        ResolveCollisionOnChildren(node, Vector2.zero);

        // Determine needed child rect (in global space).
        Rect  childrenGlobalRect= ComputeChildrenRect(node);

        // Compute needed width.
        float titleWidth  = iCS_Config.GetNodeWidth(node.Name)+iCS_Config.ExtraIconWidth;
        float leftMargin  = ComputeLeftMargin(node);
        float rightMargin = ComputeRightMargin(node);
        float width       = 2.0f*iCS_Config.GutterSize + Mathf.Max(titleWidth, leftMargin + rightMargin + childrenGlobalRect.width);

        // Process case without child nodes
        Rect globalPosition= GetLayoutPosition(node);
        if(Math3D.IsZero(childrenGlobalRect.width) || Math3D.IsZero(childrenGlobalRect.height)) {
            // Compute needed height.
            iCS_EditorObject[] leftPorts= GetLeftPorts(node);
            iCS_EditorObject[] rightPorts= GetRightPorts(node);
            int nbOfPorts= leftPorts.Length > rightPorts.Length ? leftPorts.Length : rightPorts.Length;
            float height= Mathf.Max(iCS_Config.NodeTitleHeight+nbOfPorts*iCS_Config.MinimumPortSeparation, iCS_Config.MinimumNodeHeight);                                
            
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
            float neededChildXOffset= iCS_Config.GutterSize+leftMargin;
            float neededChildYOffset= iCS_Config.GutterSize+iCS_Config.NodeTitleHeight;
            float neededNodeGlobalX= childrenGlobalRect.x-neededChildXOffset;
            float neededNodeGlobalY= childrenGlobalRect.y-neededChildYOffset;
            if(Math3D.IsNotEqual(neededNodeGlobalX, globalPosition.x) ||
               Math3D.IsNotEqual(neededNodeGlobalY, globalPosition.y)) {
                   AdjustChildLocalPosition(node, new Vector2(globalPosition.x-neededNodeGlobalX, globalPosition.y-neededNodeGlobalY));
            }

            // Compute needed height.
            int nbOfLeftPorts = GetNbOfLeftPorts(node);
            int nbOfRightPorts= GetNbOfRightPorts(node);
            int nbOfPorts= nbOfLeftPorts > nbOfRightPorts ? nbOfLeftPorts : nbOfRightPorts;
            float portHeight= nbOfPorts*iCS_Config.MinimumPortSeparation;                                
            float height= iCS_Config.NodeTitleHeight+Mathf.Max(portHeight, childrenGlobalRect.height+2.0f*iCS_Config.GutterSize);
            
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
    // ----------------------------------------------------------------------
    void AdjustChildLocalPosition(iCS_EditorObject node, Vector2 _delta) {
        ForEachChild(node, (child)=> { if(child.IsNode) DeltaMoveInternal(child, _delta); } );
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildRect(iCS_EditorObject node) {
        // Compute child space.
        Rect childRect= new Rect(0.5f*node.DisplaySize.x,0.5f*node.DisplaySize.y,0,0);
        ForEachChild(node,
            (child)=> {
                if(child.IsNode && child.IsVisible && !child.IsFloating) {
                    childRect= Math3D.Merge(childRect, child.LocalRect);
                }
            }
        );
        return childRect;
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildrenRect(iCS_EditorObject node) {
        // Compute child space.
        Rect childrenRect= new Rect(0,0,0,0);
        ForEachChild(node,
            (child)=> {
                if(child.IsNode && child.IsVisible && !child.IsFloating) {
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
    // ----------------------------------------------------------------------
    // Returns the inner left margin.
    float ComputeLeftMargin(iCS_EditorObject node) {
        float LeftMargin= 0;
        ForEachLeftPort(node,
            port=> {
                if(!port.IsStatePort && IsPortOnParent(port)) {
                    Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+iCS_Config.PortSize;
                    if(LeftMargin < nameSize) LeftMargin= nameSize;
                }
            }
        );
        return LeftMargin;
    }
    // ----------------------------------------------------------------------
    // Returns the inner right margin.
    float ComputeRightMargin(iCS_EditorObject node) {
        float RightMargin= 0;
        ForEachRightPort(node,
            port => {
                if(!port.IsStatePort && IsPortOnParent(port)) {
                    Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+iCS_Config.PortSize;
                    if(RightMargin < nameSize) RightMargin= nameSize;                    
                }
            }
        );
        return RightMargin;
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
        iCS_EditorObject[] ports= GetTopPorts(node);
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? cp.y : -cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortBottomPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        iCS_EditorObject[] ports= GetBottomPorts(node);
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? -cp.y : cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortLeftPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= GetLeftPorts(node);                             
        Vector2[] connectedPos= Prelude.map(p=> Math3D.ToVector2(GetLayoutPosition(p)), ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? cp.x : -cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] SortRightPorts(iCS_EditorObject node) {
        Rect nodePos= GetLayoutPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        iCS_EditorObject[] ports= GetRightPorts(node);
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
    // ----------------------------------------------------------------------
    // Returns all ports position on the top edge.
    public iCS_EditorObject[] GetTopPorts(iCS_EditorObject node) {
        List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
        ForEachTopPort(node, port=> { if(IsPortOnParent(port)) ports.Add(port);});
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the bottom edge.
    public iCS_EditorObject[] GetBottomPorts(iCS_EditorObject node) {
        List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
        ForEachBottomPort(node, port=> { if(IsPortOnParent(port)) ports.Add(port);});
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the left edge.
    public iCS_EditorObject[] GetLeftPorts(iCS_EditorObject node) {
        List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
        ForEachLeftPort(node, port=> { if(IsPortOnParent(port)) ports.Add(port);});
        return ports.ToArray();        
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the right edge.
    public iCS_EditorObject[] GetRightPorts(iCS_EditorObject node) {
        List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
        ForEachRightPort(node, port=> { if(IsPortOnParent(port)) ports.Add(port);});
        return ports.ToArray();
    }
    // ----------------------------------------------------------------------
    bool IsPortOnParent(iCS_EditorObject port) {
        if(!port.IsFloating) return true;
        if(port.IsDataPort) {
            return IsNearNodeEdge(port.Parent, Math3D.ToVector2(GetLayoutPosition(port)), port.Edge);
        }
        if(port.IsStatePort) {
            var parent= port.Parent;
            var bestEdge= GetClosestEdge(parent, port);
            return IsNearNodeEdge(parent, Math3D.ToVector2(GetLayoutPosition(port)), bestEdge);
        }
        return false;
    }
    // ----------------------------------------------------------------------
    // Returns the number of ports on the top edge.
    public int GetNbOfTopPorts(iCS_EditorObject node) {
        int nbOfPorts= 0;
        ForEachTopPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the bottom edge.
    public int GetNbOfBottomPorts(iCS_EditorObject node) {
        int nbOfPorts= 0;
        ForEachBottomPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the left edge.
    public int GetNbOfLeftPorts(iCS_EditorObject node) {
        int nbOfPorts= 0;
        ForEachLeftPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the right edge.
    public int GetNbOfRightPorts(iCS_EditorObject node) {
        int nbOfPorts= 0;
        ForEachRightPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    public void ForEachTopPort(iCS_EditorObject node, System.Action<iCS_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnTopEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachBottomPort(iCS_EditorObject node, System.Action<iCS_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnBottomEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachLeftPort(iCS_EditorObject node, System.Action<iCS_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnLeftEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachRightPort(iCS_EditorObject node, System.Action<iCS_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnRightEdge, fnc));
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
        return Math3D.DoesCollide(RectWithGutter(GetLayoutPosition(node)), GetLayoutPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    static Rect RectWithGutter(Rect _rect) {
        float gutterSize= iCS_Config.GutterSize;
        float gutterSize2= 2.0f*gutterSize;
        return new Rect(_rect.x-gutterSize, _rect.y-gutterSize, _rect.width+gutterSize2, _rect.height+gutterSize2);        
    }
    
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(iCS_EditorObject node, Rect _rect) {
        Rect myRect= RectWithGutter(GetLayoutPosition(node));
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
    public void UpdatePortEdge(iCS_EditorObject port) {
        var edgeBeforeUpdate= port.Edge;
        // Enable ports are always on top of the node.
        if(port.IsEnablePort) {
            port.Edge= iCS_EdgeEnum.Top;
        }
        // Data ports are always on the left or right depending on input/output direction.
        else if(port.IsDataPort) {
            port.Edge= port.IsInputPort ? iCS_EdgeEnum.Left : iCS_EdgeEnum.Right;
        }
        // Selected closest edge.
        else {
            port.Edge= GetClosestEdge(port.Parent, port);            
        }
        // Set dirty flag if port edge has changed.
        if(!port.IsFloating) CleanupPortPositionOnEdge(port);
        if(port.Edge != edgeBeforeUpdate) SetDirty(port);
    }
    // ----------------------------------------------------------------------
    public void CleanupPortPositionOnEdge(iCS_EditorObject port) {
        var parent= port.Parent;
        var parentPos= GetLayoutPosition(parent);
        Rect lp= port.LocalRect;
        switch(port.Edge) {
            case iCS_EdgeEnum.Top:      lp.y= 0; port.LocalRect= lp; break; 
            case iCS_EdgeEnum.Bottom:   lp.y= parentPos.height; port.LocalRect= lp; break;
            case iCS_EdgeEnum.Left:     lp.x= 0; port.LocalRect= lp; break;
            case iCS_EdgeEnum.Right:    lp.x= parentPos.width; port.LocalRect= lp; break;
        }
    }
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
        Rect position= GetLayoutPosition(node);        
        iCS_EditorObject[] ports= SortTopPorts(node);
        if(ports.Length != 0) {
            float xStep= position.width / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalRect= new Rect((i+0.5f)*xStep, 0, 0, 0);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout bottom ports.
        ports= SortBottomPorts(node);
        if(ports.Length != 0) {
            float xStep= position.width / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalRect= new Rect((i+0.5f)*xStep, position.height, 0, 0);
                }
            }            
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout left ports.
        ports= SortLeftPorts(node);
        if(ports.Length != 0) {
            float topOffset= iCS_Config.NodeTitleHeight-2;
            float yStep= (position.height-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalRect= new Rect(0, topOffset+(i+0.5f) * yStep, 0, 0);
                }
            }
            if(!AreChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
            }            
        }

        // Relayout right ports.
        ports= SortRightPorts(node);
        if(ports.Length != 0) {
            float topOffset= iCS_Config.NodeTitleHeight-2;
            float yStep= (position.height-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(!ports[i].IsFloating) {
                    ports[i].LocalRect= new Rect(position.width, topOffset+(i+0.5f)*yStep, 0, 0);
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
