using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
    // ======================================================================
    // Node Layout
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void SetInitialPosition(UK_EditorObject obj, Vector2 initialPosition) {
        if(IsValid(obj.ParentId)) {
            Rect position= GetPosition(EditorObjects[obj.ParentId]);
            obj.LocalPosition.x= initialPosition.x - position.x;
            obj.LocalPosition.y= initialPosition.y - position.y;            
        }
        else {
            obj.LocalPosition.x= initialPosition.x;
            obj.LocalPosition.y= initialPosition.y;                        
        }
        SetDirty(obj);
    }

    // ----------------------------------------------------------------------
    public void Layout(UK_EditorObject obj) {
        obj.IsDirty= false;
        ExecuteIf(obj, WD.IsNode, NodeLayout);
    }
    public void Layout(int id) {
        if(IsInvalid(id)) return;
        Layout(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    // Recompute the layout of a parent node.
    // Returns "true" if the new layout is within the window area.
    public void NodeLayout(UK_EditorObject node) {
        // Don't layout node if it is not visible.
        if(!IsVisible(node)) return;

        // Update transition module name
        if(node.IsTransitionModule) {
            GetTransitionName(node);
        }
        
        // Minimized nodes are fully collapsed.
        if(node.IsMinimized) {
            Texture2D icon= UK_Graphics.GetMaximizeIcon(node, null, this);
            if(node.LocalPosition.width != icon.width || node.LocalPosition.height != icon.height) {
                node.LocalPosition.x+= 0.5f*(node.LocalPosition.width-icon.width);
                node.LocalPosition.y+= 0.5f*(node.LocalPosition.height-icon.height);
                node.LocalPosition.width= icon.width;
                node.LocalPosition.height= icon.height;
                LayoutPorts(node);
            }
            return;
        }

        // Resolve collision on children.
        ResolveCollisionOnChildren(node, Vector2.zero);

        // Determine needed child rect.
        Rect  childRect   = ComputeChildRect(node);

        // Compute needed width.
        float titleWidth  = UK_EditorConfig.GetNodeWidth(node.Name)+UK_EditorConfig.ExtraIconWidth;
        float leftMargin  = ComputeLeftMargin(node);
        float rightMargin = ComputeRightMargin(node);
        float width       = 2.0f*UK_EditorConfig.GutterSize + Mathf.Max(titleWidth, leftMargin + rightMargin + childRect.width);

        // Process case without child nodes
        Rect position= GetPosition(node);
        if(Math3D.IsZero(childRect.width) || Math3D.IsZero(childRect.height)) {
            // Compute needed height.
            UK_EditorObject[] leftPorts= GetLeftPorts(node);
            UK_EditorObject[] rightPorts= GetRightPorts(node);
            int nbOfPorts= leftPorts.Length > rightPorts.Length ? leftPorts.Length : rightPorts.Length;
            float height= Mathf.Max(UK_EditorConfig.NodeTitleHeight+nbOfPorts*UK_EditorConfig.MinimumPortSeparation, UK_EditorConfig.MinimumNodeHeight);                                
            
            // Apply new width and height.
            if(Math3D.IsNotEqual(height, position.height) || Math3D.IsNotEqual(width, position.width)) {
                float deltaWidth = width - position.width;
                float deltaHeight= height - position.height;
                Rect newPos= new Rect(position.xMin-0.5f*deltaWidth, position.yMin-0.5f*deltaHeight, width, height);
                SetPosition(node, newPos);
            }
        }
        // Process case with child nodes.
        else {
            // Adjust children local offset.
            float neededChildXOffset= UK_EditorConfig.GutterSize+leftMargin;
            float neededChildYOffset= UK_EditorConfig.GutterSize+UK_EditorConfig.NodeTitleHeight;
            if(Math3D.IsNotEqual(neededChildXOffset, childRect.x) ||
               Math3D.IsNotEqual(neededChildYOffset, childRect.y)) {
                   AdjustChildLocalPosition(node, new Vector2(neededChildXOffset-childRect.x, neededChildYOffset-childRect.y));
            }

            // Compute needed height.
            int nbOfLeftPorts = GetNbOfLeftPorts(node);
            int nbOfRightPorts= GetNbOfRightPorts(node);
            int nbOfPorts= nbOfLeftPorts > nbOfRightPorts ? nbOfLeftPorts : nbOfRightPorts;
            float portHeight= nbOfPorts*UK_EditorConfig.MinimumPortSeparation;                                
            float height= UK_EditorConfig.NodeTitleHeight+Mathf.Max(portHeight, childRect.height+2.0f*UK_EditorConfig.GutterSize);
            
            float deltaWidth = width - node.LocalPosition.width;
            float deltaHeight= height - node.LocalPosition.height;
            float xMin= position.xMin-0.5f*deltaWidth;
            float yMin= position.yMin-0.5f*deltaHeight;
            if(Math3D.IsNotEqual(xMin, position.xMin) || Math3D.IsNotEqual(yMin, position.yMin) ||
               Math3D.IsNotEqual(width, node.LocalPosition.width) || Math3D.IsNotEqual(height, node.LocalPosition.height)) {
                Rect newPos= new Rect(xMin, yMin, width, height);
                SetPosition(node, newPos);
            }
        }

        // Layout child ports
        LayoutPorts(node);
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void MoveTo(UK_EditorObject node, Vector2 _newPos) {
        Rect position = GetPosition(node);
        DeltaMove(node, new Vector2(_newPos.x - position.x, _newPos.y - position.y));
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void DeltaMove(UK_EditorObject node, Vector2 _delta) {
        // Move the node
        DeltaMoveInternal(node, _delta);
        // Resolve collision between siblings.
        LayoutParent(node, _delta);
	}
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    void DeltaMoveInternal(UK_EditorObject node, Vector2 _delta) {
        if(Math3D.IsNotZero(_delta)) {
            node.LocalPosition.x+= _delta.x;
            node.LocalPosition.y+= _delta.y;
            SetDirty(node);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the absolute position of the node.
    public Rect GetPosition(UK_EditorObject node) {
        if(!IsValid(node.ParentId)) return node.LocalPosition;
        Rect position= GetPosition(EditorObjects[node.ParentId]);
        if(!IsVisible(node)) {
            Vector2 midPoint= Math3D.Middle(position);
            return new Rect(midPoint.x, midPoint.y, 0, 0);
        }
        return new Rect(position.x+node.LocalPosition.x,
                        position.y+node.LocalPosition.y,
                        node.LocalPosition.width,
                        node.LocalPosition.height);
    }
    public Rect GetPosition(int id) {
        return GetPosition(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    public void SetPosition(UK_EditorObject node, Rect _newPos) {
        // Adjust node size.
        node.LocalPosition.width = _newPos.width;
        node.LocalPosition.height= _newPos.height;
        // Reposition node.
        if(!IsValid(node.ParentId)) {
            node.LocalPosition.x= _newPos.x;
            node.LocalPosition.y= _newPos.y;            
        }
        else {
            Rect position= GetPosition(node);
            Rect deltaMove= new Rect(_newPos.xMin-position.xMin, _newPos.yMin-position.yMin, _newPos.width-position.width, _newPos.height-position.height);
            node.LocalPosition.x+= deltaMove.x;
            node.LocalPosition.y+= deltaMove.y;
            node.LocalPosition.width= _newPos.width;
            node.LocalPosition.height= _newPos.height;
            float separationX= Mathf.Abs(deltaMove.x) > Mathf.Abs(deltaMove.width) ? deltaMove.x : deltaMove.width;
            float separationY= Mathf.Abs(deltaMove.y) > Mathf.Abs(deltaMove.height) ? deltaMove.y : deltaMove.height;
            LayoutParent(node, new Vector2(separationX, separationY));
        }
    }    
    // ----------------------------------------------------------------------
    Vector2 GetTopLeftCorner(UK_EditorObject node)     {
        Rect position= GetPosition(node);
        return new Vector2(position.xMin, position.yMin);
    }
    Vector2 GetTopRightCorner(UK_EditorObject node)    {
        Rect position= GetPosition(node);
        return new Vector2(position.xMax, position.yMin);
    }
    Vector2 GetBottomLeftCorner(UK_EditorObject node)  {
        Rect position= GetPosition(node);
        return new Vector2(position.xMin, position.yMax);
    }
    Vector2 GetBottomRightCorner(UK_EditorObject node) {
        Rect position= GetPosition(node);
        return new Vector2(position.xMax, position.yMax);
    }
    // ----------------------------------------------------------------------
    void LayoutParent(UK_EditorObject node, Vector2 _deltaMove) {
        if(!IsValid(node.ParentId)) return;
        UK_EditorObject parentNode= GetParent(node);
        ResolveCollision(parentNode, _deltaMove);
        Layout(parentNode);
    }
    // ----------------------------------------------------------------------
    void AdjustChildLocalPosition(UK_EditorObject node, Vector2 _delta) {
        ForEachChild(node, (child)=> { if(child.IsNode) DeltaMoveInternal(child, _delta); } );
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildRect(UK_EditorObject node) {
        // Compute child space.
        Rect childRect= new Rect(0.5f*node.LocalPosition.width,0.5f*node.LocalPosition.height,0,0);
        ForEachChild(node,
            (child)=> {
                if(child.IsNode && IsVisible(child)) {
                    childRect= Math3D.Merge(childRect, child.LocalPosition);
                }
            }
        );
        return childRect;
    }
    // ----------------------------------------------------------------------
    // Returns the inner left margin.
    float ComputeLeftMargin(UK_EditorObject node) {
        float LeftMargin= 0;
        ForEachLeftPort(node,
            port=> {
                if(!port.IsStatePort) {
                    Vector2 labelSize= UK_EditorConfig.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+UK_EditorConfig.PortSize;
                    if(LeftMargin < nameSize) LeftMargin= nameSize;
                }
            }
        );
        return LeftMargin;
    }
    // ----------------------------------------------------------------------
    // Returns the inner right margin.
    float ComputeRightMargin(UK_EditorObject node) {
        float RightMargin= 0;
        ForEachRightPort(node,
            port => {
                if(!port.IsStatePort) {
                    Vector2 labelSize= UK_EditorConfig.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+UK_EditorConfig.PortSize;
                    if(RightMargin < nameSize) RightMargin= nameSize;                    
                }
            }
        );
        return RightMargin;
    }


    // ======================================================================
    // Port Layout
    // ----------------------------------------------------------------------
    // Recomputes the port position.
    public void LayoutPorts(UK_EditorObject node) {
        // Update all port edges.
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, port=> UpdatePortEdge(port)));
        
        // Special case for minimized nodes.
        if(node.IsMinimized) {
            float cx= 0.5f*node.LocalPosition.width;
            float cy= 0.5f*node.LocalPosition.height;
            ForEachChild(node, child=> ExecuteIf(child, p=> p.IsPort, port=> { port.LocalPosition.x= cx; port.LocalPosition.y= cy; }));
            return;
        }

		// Gather all ports.
        Rect position= GetPosition(node);
        
        // Relayout top ports.
        UK_EditorObject[] ports= SortTopPorts(node);
        if(ports.Length != 0) {
            float xStep= position.width / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(ports[i].IsBeingDragged == false) {
                    ports[i].LocalPosition.x= (i+0.5f) * xStep;
                    ports[i].LocalPosition.y= 0;
                }
            }
            if(!IsChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
                SetAllConnectedPortsDirty(ports);
            }            
        }

        // Relayout bottom ports.
        ports= SortBottomPorts(node);
        if(ports.Length != 0) {
            float xStep= position.width / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(ports[i].IsBeingDragged == false) {
                    ports[i].LocalPosition.x= (i+0.5f) * xStep;
                    ports[i].LocalPosition.y= position.height;                
                }
            }            
            if(!IsChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
                SetAllConnectedPortsDirty(ports);
            }            
        }

        // Relayout left ports.
        ports= SortLeftPorts(node);
        if(ports.Length != 0) {
            float topOffset= UK_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(ports[i].IsBeingDragged == false) {
                    ports[i].LocalPosition.x= 0;
                    ports[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;                
                }
            }
            if(!IsChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
                SetAllConnectedPortsDirty(ports);
            }            
        }

        // Relayout right ports.
        ports= SortRightPorts(node);
        if(ports.Length != 0) {
            float topOffset= UK_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / ports.Length;
            for(int i= 0; i < ports.Length; ++i) {
                if(ports[i].IsBeingDragged == false) {
                    ports[i].LocalPosition.x= position.width;
                    ports[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;
                }
            }
            if(!IsChildrenInSameOrder(node, ports)) {
                ReorderChildren(node, ports);
                SetAllConnectedPortsDirty(ports);
            }            
        }        
    }

    // ----------------------------------------------------------------------
    bool IsChildrenInSameOrder(UK_EditorObject node, UK_EditorObject[] orderedChildren) {
        return TreeCache[node.InstanceId].IsChildrenInSameOrder(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    void ReorderChildren(UK_EditorObject node, UK_EditorObject[] orderedChildren) {
        TreeCache[node.InstanceId].ReorderChildren(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    // ----------------------------------------------------------------------
    void SetAllConnectedPortsDirty(UK_EditorObject[] ports) {
        foreach(var p in ports) {
            if(IsValid(p.Source)) SetDirty(p);
            UK_EditorObject[] connectedPorts= FindConnectedPorts(p);
            foreach(var cp in connectedPorts) SetDirty(cp);
        }        
    }
    // ----------------------------------------------------------------------
    Vector2 GetAverageConnectionPosition(UK_EditorObject port) {
        UK_EditorObject[] connectedPorts= FindConnectedPorts(port);
        Vector2 posSum= Prelude.fold(
            (remotePort,sum)=> sum+Math3D.ToVector2(GetPosition(remotePort)),
            Vector2.zero,
            connectedPorts
        );
        int nbOfPorts= connectedPorts.Length;
        if(IsValid(port.Source)) {
            posSum+= Math3D.ToVector2(GetPosition(EditorObjects[port.Source]));
            ++nbOfPorts;
        }
        return nbOfPorts != 0 ? (1.0f/nbOfPorts)*posSum : Math3D.ToVector2(GetPosition(port));        
    }
    // ----------------------------------------------------------------------
    UK_EditorObject[] SortTopPorts(UK_EditorObject node) {
        Rect nodePos= GetPosition(node);
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        UK_EditorObject[] ports= GetTopPorts(node);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? cp.y : -cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    UK_EditorObject[] SortBottomPorts(UK_EditorObject node) {
        Rect nodePos= GetPosition(node);
        float refPos= 0.5f*(nodePos.xMin+nodePos.xMax);
        UK_EditorObject[] ports= GetBottomPorts(node);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(cp=> cp.x, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.x ? -cp.y : cp.y, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    UK_EditorObject[] SortLeftPorts(UK_EditorObject node) {
        Rect nodePos= GetPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        UK_EditorObject[] ports= GetLeftPorts(node);                             
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? cp.x : -cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    UK_EditorObject[] SortRightPorts(UK_EditorObject node) {
        Rect nodePos= GetPosition(node);
        float refPos= 0.5f*(nodePos.yMin+nodePos.yMax);
        UK_EditorObject[] ports= GetRightPorts(node);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(cp=> cp.y, connectedPos); 
        float[] secondKeys= Prelude.map(cp=> refPos < cp.y ? -cp.x : cp.x, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    // Sorts the given port according to their relative positions.
    UK_EditorObject[] SortPorts(UK_EditorObject[] ports, float[] keys1, float[] keys2) {
        for(int i= 0; i < ports.Length-1; ++i) {
            for(int j= i+1; j < ports.Length; ++j) {
                if(Math3D.IsGreater(keys1[i], keys1[j])) {
                    Prelude.exchange(ref ports[i], ref ports[j]);
                    Prelude.exchange(ref keys1[i], ref keys1[j]);
                    Prelude.exchange(ref keys2[i], ref keys2[j]);
                } else if(Math3D.IsEqual(keys1[i], keys1[j])) {                
                    if(Math3D.IsGreater(keys2[i], keys2[j])) {
                        Prelude.exchange(ref ports[i], ref ports[j]);
                        Prelude.exchange(ref keys1[i], ref keys1[j]);
                        Prelude.exchange(ref keys2[i], ref keys2[j]);                    
                    }
                }
            }
        }
        return ports;
    }
    // ----------------------------------------------------------------------
    // Returns all ports position on the top edge.
    public UK_EditorObject[] GetTopPorts(UK_EditorObject node) {
        List<UK_EditorObject> ports= new List<UK_EditorObject>();
        ForEachTopPort(node, ports.Add);
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the bottom edge.
    public UK_EditorObject[] GetBottomPorts(UK_EditorObject node) {
        List<UK_EditorObject> ports= new List<UK_EditorObject>();
        ForEachBottomPort(node, ports.Add);
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the left edge.
    public UK_EditorObject[] GetLeftPorts(UK_EditorObject node) {
        List<UK_EditorObject> ports= new List<UK_EditorObject>();
        ForEachLeftPort(node, ports.Add);
        return ports.ToArray();        
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the right edge.
    public UK_EditorObject[] GetRightPorts(UK_EditorObject node) {
        List<UK_EditorObject> ports= new List<UK_EditorObject>();
        ForEachRightPort(node, ports.Add);
        return ports.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns the number of ports on the top edge.
    public int GetNbOfTopPorts(UK_EditorObject node) {
        int nbOfPorts= 0;
        ForEachTopPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the bottom edge.
    public int GetNbOfBottomPorts(UK_EditorObject node) {
        int nbOfPorts= 0;
        ForEachBottomPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the left edge.
    public int GetNbOfLeftPorts(UK_EditorObject node) {
        int nbOfPorts= 0;
        ForEachLeftPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the right edge.
    public int GetNbOfRightPorts(UK_EditorObject node) {
        int nbOfPorts= 0;
        ForEachRightPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    public void ForEachTopPort(UK_EditorObject node, System.Action<UK_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnTopEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachBottomPort(UK_EditorObject node, System.Action<UK_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnBottomEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachLeftPort(UK_EditorObject node, System.Action<UK_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnLeftEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachRightPort(UK_EditorObject node, System.Action<UK_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnRightEdge, fnc));
    }


    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolve collision on parents.
    void ResolveCollision(UK_EditorObject node, Vector2 _delta) {
        ResolveCollisionOnChildren(node, _delta);
        if(!IsValid(node.ParentId)) return;
        ResolveCollision(GetParent(node), _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren(UK_EditorObject node, Vector2 _delta) {
        bool didCollide= false;
        for(int i= 0; i < EditorObjects.Count-1; ++i) {
            UK_EditorObject child1= EditorObjects[i];
            if(child1.ParentId != node.InstanceId) continue;
            if(!IsVisible(child1)) continue;
            if(!child1.IsNode) continue;
            for(int j= i+1; j < EditorObjects.Count; ++j) {
                UK_EditorObject child2= EditorObjects[j];
                if(child2.ParentId != node.InstanceId) continue;
                if(!IsVisible(child2)) continue;
                if(!child2.IsNode) continue;
                didCollide |= ResolveCollisionBetweenTwoNodes(child1, child2, _delta);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildren(node, _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(UK_EditorObject node, UK_EditorObject otherNode, Vector2 _delta) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithGutter(node, otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(node, GetPosition(otherNode));
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
    public bool DoesCollide(UK_EditorObject node, UK_EditorObject otherNode) {
        return Math3D.DoesCollide(GetPosition(node), GetPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithGutter(UK_EditorObject node, UK_EditorObject otherNode) {
        return Math3D.DoesCollide(RectWithGutter(GetPosition(node)), GetPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    static Rect RectWithGutter(Rect _rect) {
        float gutterSize= UK_EditorConfig.GutterSize;
        float gutterSize2= 2.0f*gutterSize;
        return new Rect(_rect.x-gutterSize, _rect.y-gutterSize, _rect.width+gutterSize2, _rect.height+gutterSize2);        
    }
    
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(UK_EditorObject node, Rect _rect) {
        Rect myRect= RectWithGutter(GetPosition(node));
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
	Vector2 GetSeperationVector(UK_EditorObject node, UK_EditorObject otherNode) {
	    return GetSeperationVector(node, GetPosition(otherNode));
	}

    // ----------------------------------------------------------------------
    // Returns true if the given point is inside the node coordinates.
    bool IsInside(UK_EditorObject node, Vector2 point) {
        // Extend the node range to include the ports.
        float portSize= UK_EditorConfig.PortSize;
        Rect nodePos= GetPosition(node);
        nodePos.x-= portSize;
        nodePos.y-= portSize;
        nodePos.width+= 2f*portSize;
        nodePos.height+= 2f*portSize;
        return nodePos.Contains(point);
    }


    // ======================================================================
    // Layout from UK_Port
    // ----------------------------------------------------------------------
    public void UpdatePortEdge(UK_EditorObject port) {
        if(IsValid(port.Source)) UpdatePortEdges(port, EditorObjects[port.Source]);
        Prelude.forEach(p=> UpdatePortEdges(port, p), FindConnectedPorts(port));
    }
    // ----------------------------------------------------------------------
    public void UpdatePortEdges(UK_EditorObject p1, UK_EditorObject p2) {
        // Don't update port edges for a transition bridge.  Leave the update
        // to the corresponding data connection & transition connection.
        if(IsBridgeConnection(p1,p2)) return;
        UK_EditorObject.EdgeEnum p1Edge= p1.Edge;
        UK_EditorObject.EdgeEnum p2Edge= p2.Edge;
        UpdatePortEdgesInternal(p1, p2);
        if(p1Edge != p1.Edge) SetDirty(p1);
        if(p2Edge != p2.Edge) SetDirty(p2);
    }
    void UpdatePortEdgesInternal(UK_EditorObject p1, UK_EditorObject p2) {
        // Reset edge information.
        p1.Edge= UK_EditorObject.EdgeEnum.None;
        p2.Edge= UK_EditorObject.EdgeEnum.None;
        UpdatePortEdgeHardConstraints(p1);
        UpdatePortEdgeHardConstraints(p2);
        if(p1.Edge != UK_EditorObject.EdgeEnum.None && p2.Edge != UK_EditorObject.EdgeEnum.None) return;
        UK_EditorObject p1Parent= GetParent(p1);
        UK_EditorObject p2Parent= GetParent(p2);
        // Verify connection between nested nodes.
        Rect parent1Rect= GetPosition(p1Parent);
        Rect parent2Rect= GetPosition(p2Parent);
        // Nested
        if(IsChildOf(p1Parent, p2Parent) ||
           IsChildOf(p2Parent, p1Parent)) {               
            UK_EditorObject parent= null;
            UK_EditorObject child= null;
            UK_EditorObject pPort= null;
            UK_EditorObject cPort= null;
            if(IsChildOf(p1Parent, p2Parent)) {
                parent= p2Parent;
                child= p1Parent;
                pPort= p2;
                cPort= p1;
            } else {
                parent= p1Parent;
                child= p2Parent;
                pPort= p1;
                cPort= p2;
            }
            Rect parentPos= GetPosition(parent);
            Rect childPos = GetPosition(child);
            Rect childLocalPos= new Rect(childPos.x-parentPos.x, childPos.y-parentPos.y, childPos.width, childPos.height);
            float dx= parentPos.width-childLocalPos.xMax;
            float dy= parentPos.height-childLocalPos.yMax;
            if(childLocalPos.x < childLocalPos.y) {
                if(dx < dy) {
                    if(childLocalPos.x < dx) {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Left;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Left;
                    } else {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Right;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Right;                        
                    }
                } else {
                    if(childLocalPos.x < dy) {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Left;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Left;                        
                    } else {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Bottom;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Bottom;                        
                    }
                }
            } else {
                if(dx < dy) {
                    if(childLocalPos.y < dx) {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Top;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Top;
                    } else {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Right;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Right;                        
                    }
                } else {
                    if(childLocalPos.y < dy) {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Top;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Top;                        
                    } else {
                        pPort.Edge= UK_EditorObject.EdgeEnum.Bottom;
                        cPort.Edge= UK_EditorObject.EdgeEnum.Bottom;                        
                    }
                }                
            }
            return;
        }
        // Horizontal
        if(parent1Rect.xMin <= parent2Rect.xMin && parent1Rect.xMax > parent2Rect.xMin ||
           parent2Rect.xMin <= parent1Rect.xMin && parent2Rect.xMax > parent1Rect.xMin) {
            if(parent1Rect.yMin < parent2Rect.yMin) {
                p1.Edge= UK_EditorObject.EdgeEnum.Bottom;
                p2.Edge= UK_EditorObject.EdgeEnum.Top;
            } else {
                p1.Edge= UK_EditorObject.EdgeEnum.Top;
                p2.Edge= UK_EditorObject.EdgeEnum.Bottom;                
            }
            return;
        }
        // Vertical
        if(parent1Rect.yMin <= parent2Rect.yMin && parent1Rect.yMax > parent2Rect.yMin ||
           parent2Rect.yMin <= parent1Rect.yMin && parent2Rect.yMax > parent1Rect.yMin) {
            if(parent1Rect.xMin < parent2Rect.xMin) {
                p1.Edge= UK_EditorObject.EdgeEnum.Right;
                p2.Edge= UK_EditorObject.EdgeEnum.Left;
            } else {
                p1.Edge= UK_EditorObject.EdgeEnum.Left;
                p2.Edge= UK_EditorObject.EdgeEnum.Right;                
            }
            return;
        }
        // Diagonal
        if(parent1Rect.xMin < parent2Rect.xMin) {
            if(parent1Rect.yMin < parent2Rect.yMin) {
                if(p1.Source == p2.InstanceId) {
                    p1.Edge= UK_EditorObject.EdgeEnum.Bottom;
                    p2.Edge= UK_EditorObject.EdgeEnum.Left;
                } else {
                    p1.Edge= UK_EditorObject.EdgeEnum.Right;
                    p2.Edge= UK_EditorObject.EdgeEnum.Top;                    
                }
            } else {
                if(p1.Source == p2.InstanceId) {
                    p1.Edge= UK_EditorObject.EdgeEnum.Right;
                    p2.Edge= UK_EditorObject.EdgeEnum.Bottom;
                } else {
                    p1.Edge= UK_EditorObject.EdgeEnum.Top;
                    p2.Edge= UK_EditorObject.EdgeEnum.Left;                    
                }                
            }
            return;
        }
        if(parent1Rect.yMin < parent2Rect.yMin) {
            if(p1.Source == p2.InstanceId) {
                p1.Edge= UK_EditorObject.EdgeEnum.Left;
                p2.Edge= UK_EditorObject.EdgeEnum.Top;
            } else {
                p1.Edge= UK_EditorObject.EdgeEnum.Bottom;
                p2.Edge= UK_EditorObject.EdgeEnum.Right;                    
            }
        } else {
            if(p1.Source == p2.InstanceId) {
                p1.Edge= UK_EditorObject.EdgeEnum.Top;
                p2.Edge= UK_EditorObject.EdgeEnum.Right;
            } else {
                p1.Edge= UK_EditorObject.EdgeEnum.Left;
                p2.Edge= UK_EditorObject.EdgeEnum.Bottom;                    
            }            
        }
    }
    void UpdatePortEdgeHardConstraints(UK_EditorObject port) {
        if(port.IsEnablePort) {
            port.Edge= UK_EditorObject.EdgeEnum.Top;
            return;            
        }
        if(port.IsDataPort) {
            port.Edge= port.IsInputPort ? UK_EditorObject.EdgeEnum.Left : UK_EditorObject.EdgeEnum.Right;
            return;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(UK_EditorObject port) {
        UK_EditorObject parentNode= GetParent(port);
        Rect tmp= GetPosition(port);
        Vector2 position= new Vector2(tmp.x, tmp.y);
        if(IsInside(parentNode, position)) return 0;
        Rect parentPosition= GetPosition(parentNode);
        if(position.x > parentPosition.xMin && position.x < parentPosition.xMax) {
            return Mathf.Min(Mathf.Abs(position.y-parentPosition.yMin),
                             Mathf.Abs(position.y-parentPosition.yMax));
        }
        if(position.y > parentPosition.yMin && position.y < parentPosition.yMax) {
            return Mathf.Min(Mathf.Abs(position.x-parentPosition.xMin),
                             Mathf.Abs(position.x-parentPosition.xMax));
        }
        float distance= Vector2.Distance(position, GetTopLeftCorner(parentNode));
        distance= Mathf.Min(distance, Vector2.Distance(position, GetTopRightCorner(parentNode)));
        distance= Mathf.Min(distance, Vector2.Distance(position, GetBottomLeftCorner(parentNode)));
        distance= Mathf.Min(distance, Vector2.Distance(position, GetBottomRightCorner(parentNode)));
        return distance;
    }

    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent(UK_EditorObject port) {
        if(GetNodeAt(Math3D.ToVector2(GetPosition(port))) != GetParent(port)) return false;
        return GetDistanceFromParent(port) <= UK_EditorConfig.PortSize*2;
    }

	// ----------------------------------------------------------------------
    public UK_EditorObject GetOverlappingPort(UK_EditorObject port) {
        UK_EditorObject foundPort= null;
        Rect tmp= GetPosition(port);
        Vector2 position= new Vector2(tmp.x, tmp.y);
        FilterWith(
            p=> p.IsPort && p != port,
            p=> {
                tmp= GetPosition(p);
                Vector2 pPos= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(pPos, position);
                if(distance <= 1.5*UK_EditorConfig.PortSize) {
                    foundPort= p;
                }
            }
        );
        return foundPort;
    }	

}
