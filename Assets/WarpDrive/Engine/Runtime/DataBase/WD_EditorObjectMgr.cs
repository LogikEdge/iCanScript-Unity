using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class WD_EditorObjectMgr {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool                     IsDirty      = true;
    public List<WD_EditorObject>    EditorObjects= new List<WD_EditorObject>();
    public WD_TreeCache             TreeCache    = new WD_TreeCache();

    // ======================================================================
    // Object Creation
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateInstance<T>(string name, int parentId, Vector2 initialPos) where T : WD_Object {
        // Find the next available id.
        int id= 0;
        while(id < EditorObjects.Count && EditorObjects[id].IsValid) { ++id; }
        if(id >= EditorObjects.Count) {
            id= EditorObjects.Count;
            EditorObjects.Add(null);
        }
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);

        // Create new EditorObject
        WD_EditorObject obj= new WD_EditorObject(id, name, typeof(T), parentId, new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0));
        EditorObjects[id]= obj;
        T rtObj= obj.CreateRuntimeObject() as T;
        rtObj.Init(name, IsValid(parentId) ? TreeCache[parentId].RuntimeObject as WD_Aggregate: null);            
        TreeCache.CreateInstance(obj.InstanceId, parentId, rtObj);
        
        // Create ports for each field tagged with InPort or OutPort.
        foreach(var field in WD_Reflection.GetInPortFields(typeof(T))) {
            CreateInstance<WD_InFieldPort>(field.Name, obj.InstanceId, initialPos);
        }
        foreach(var field in WD_Reflection.GetOutPortFields(typeof(T))) {
            CreateInstance<WD_OutFieldPort>(field.Name, obj.InstanceId, initialPos);
        }

        return obj;
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        if(IsInvalid(id)) {
            Debug.LogError("Trying the delete a non-existing EditorObject with id= "+id);
        }
        // Remove all children first
        while(TreeCache[id].Children.Count != 0) {
            DestroyInstance(TreeCache[id].Children[0]);
        }
        // Disconnect ports linking to this port.
        ExecuteIf<WD_FieldPort>(EditorObjects[id],
            (instance) => {
                ForEach<WD_FieldPort>(
                    (obj) => {
                        if(obj.Source == id) {
                            (GetRuntimeObject(obj) as WD_FieldPort).Source= null;
                            obj.Source= -1;
                        }
                    }
                );                
            }
        );
        // Remove all related objects.
        TreeCache.DestroyInstance(id);
        EditorObjects[id].Reset();
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(WD_EditorObject eObj) {
        DestroyInstance(eObj.InstanceId);
    }

    // ======================================================================
    // Editor Object Container Management
    // ----------------------------------------------------------------------
    public WD_EditorObject this[int i] {
        get { return EditorObjects[i]; }
    }
    // ----------------------------------------------------------------------
    public bool IsValid(int id)   { return id >= 0 && id < EditorObjects.Count && EditorObjects[id].IsValid; }
    public bool IsInvalid(int id) { return !IsValid(id); }
    // ----------------------------------------------------------------------
    public bool IsChildOf(WD_EditorObject obj, WD_EditorObject parent) {
        if(IsInvalid(obj.ParentId)) return false;
        if(obj.ParentId == parent.InstanceId) return true;
        return IsChildOf(EditorObjects[obj.ParentId], parent);
    }
    // ----------------------------------------------------------------------
    public WD_Object GetRuntimeObject(int id) {
        return IsValid(id) ? TreeCache[id].RuntimeObject : null;
    }
    public WD_Object GetRuntimeObject(WD_EditorObject eObj) {
        return GetRuntimeObject(eObj.InstanceId);
    }
    // ----------------------------------------------------------------------
    public void SetSource(WD_EditorObject obj, WD_EditorObject src) {
        obj.Source= src.InstanceId;
        obj.ExecuteIf<WD_FieldPort>(
            (port) => {
                (GetRuntimeObject(port.InstanceId) as WD_FieldPort).Source= GetRuntimeObject(src.InstanceId) as WD_FieldPort;
            }
        );
    }
    
    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    // Executes the given action if the given object matches the T type.
    public void ExecuteIf<T>(WD_EditorObject obj, Action<WD_EditorObject> fnc) where T : WD_Object {
        if(obj.IsValid && obj.IsRuntimeA<T>()) fnc(obj);
    }
    public void ExecuteIf<T>(int id, Action<WD_EditorObject> fnc) where T : WD_Object {
        if(!IsValid(id)) return;
        ExecuteIf<T>(EditorObjects[id], fnc);
    }
    public void Case<T1,T2>(WD_EditorObject obj, Action<WD_EditorObject> fnc1,
                                                 Action<WD_EditorObject> fnc2,
                                                 Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                           where T2 : WD_Object {
        obj.Case<T1,T2>(fnc1,fnc2,defaultFnc);
    }
    public void Case<T1,T2>(int id, Action<WD_EditorObject> fnc1,
                                    Action<WD_EditorObject> fnc2,
                                    Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                              where T2 : WD_Object {
        if(IsInvalid(id)) return;
        Case<T1,T2>(EditorObjects[id], fnc1, fnc2, defaultFnc);
    }
    public void Case<T1,T2,T3>(WD_EditorObject obj, Action<WD_EditorObject> fnc1,
                                                    Action<WD_EditorObject> fnc2,
                                                    Action<WD_EditorObject> fnc3,
                                                    Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                              where T2 : WD_Object
                                                                                              where T3 : WD_Object {
        obj.Case<T1,T2,T3>(fnc1, fnc2, fnc3, defaultFnc);
    }
    public void Case<T1,T2,T3>(int id, Action<WD_EditorObject> fnc1,
                                       Action<WD_EditorObject> fnc2,
                                       Action<WD_EditorObject> fnc3,
                                       Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                 where T2 : WD_Object
                                                                                 where T3 : WD_Object {
        if(IsInvalid(id)) return;
        Case<T1,T2,T3>(EditorObjects[id], fnc1, fnc2, fnc3, defaultFnc);
    }
    public void ForEachChild(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachChild((id) => { fnc(EditorObjects[id]); } );            
        }
        else {
            TreeCache.ForEachChild(parent.InstanceId, (id) => { fnc(EditorObjects[id]); } );            
        }
    }
    public void ForEachChild<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachChild(parent, (child) => { ExecuteIf<T>(child, fnc); });
    }
    public void ForEach(Action<WD_EditorObject> fnc) {
        foreach(var obj in EditorObjects) {
            if(obj.IsValid) fnc(obj);
        }
    }
    public void ForEach<T>(Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEach((obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ForEachRecursiveDepthLast(parent, fnc);
    }
    public void ForEachRecursive<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachRecursive(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast((id) => { fnc(EditorObjects[id]); });                                
        } else {
            TreeCache.ForEachRecursiveDepthLast(parent.InstanceId, (id) => { fnc(EditorObjects[id]); });                    
        }
    }
    public void ForEachRecursiveDepthLast<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachRecursiveDepthLast(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst((id) => { fnc(EditorObjects[id]); });        
        } else {
            TreeCache.ForEachRecursiveDepthFirst(parent.InstanceId, (id) => { fnc(EditorObjects[id]); });                    
        }
    }
    public void ForEachRecursiveDepthFirst<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachRecursiveDepthFirst(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachChildRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ForEachChildRecursiveDepthLast(parent, fnc);
    }
    public void ForEachChildRecursive<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachChildRecursive(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachChildRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast((id) => { fnc(EditorObjects[id]); });        
        } else {
            TreeCache.ForEachChildRecursiveDepthLast(parent.InstanceId, (id) => { fnc(EditorObjects[id]); });                    
        }
    }
    public void ForEachChildRecursiveDepthLast<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachChildRecursiveDepthLast(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }
    public void ForEachChildRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst((id) => { fnc(EditorObjects[id]); });                    
        } else {
            TreeCache.ForEachChildRecursiveDepthFirst(parent.InstanceId, (id) => { fnc(EditorObjects[id]); });        
        }
    }
    public void ForEachChildRecursiveDepthFirst<T>(WD_EditorObject parent, Action<WD_EditorObject> fnc) where T : WD_Object {
        ForEachChildRecursiveDepthFirst(parent, (obj) => { ExecuteIf<T>(obj, fnc); });
    }

    // ======================================================================
    // OBJECT PICKING
    // ----------------------------------------------------------------------
    public WD_EditorObject GetRootNode() {
        foreach(var obj in EditorObjects) {
            if(obj.ParentId == -1) return obj;
        }
        Debug.LogError("No RootNode found!!!");
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public WD_EditorObject GetNodeAt(Vector2 pick) {
        WD_EditorObject foundNode= null;
        ForEach<WD_Node>(
            (node) => {
                if(node.IsVisible && IsInside(node, pick)) {
                    if(foundNode == null || node.LocalPosition.width < foundNode.LocalPosition.width) {
                        foundNode= node;
                    }
                }                
            }
        );
        return foundNode ?? GetRootNode();
    }
    
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public WD_EditorObject GetPortAt(Vector2 pick) {
        WD_EditorObject bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        ForEach<WD_Port>(
            (port) => {
                if(port.IsVisible) {
                    Rect tmp= GetPosition(port);
                    Vector2 position= new Vector2(tmp.x, tmp.y);
                    float distance= Vector2.Distance(position, pick);
                    if(distance < 1.5f * WD_EditorConfig.PortRadius && distance < bestDistance) {
                        bestDistance= distance;
                        bestPort= port;
                    }                
                }                
            }
        );
        return bestPort;
    }

    // ======================================================================
    // Editor Graph Layout Functions
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void SetInitialPosition(WD_EditorObject obj, Vector2 initialPosition) {
        if(IsValid(obj.ParentId)) {
            Rect position= GetPosition(EditorObjects[obj.ParentId]);
            obj.LocalPosition.x= initialPosition.x - position.x;
            obj.LocalPosition.y= initialPosition.y - position.y;            
        }
        else {
            obj.LocalPosition.x= initialPosition.x;
            obj.LocalPosition.y= initialPosition.y;                        
        }
        IsDirty= true;
    }

    // ----------------------------------------------------------------------
    public void Layout(WD_EditorObject obj) {
        obj.IsDirty= false;
        Case<WD_Node, WD_Port>(obj,
            (node) => { NodeLayout(node); },
            (port) => { PortLayout(port); }
        );
    }
    public void Layout(int id) {
        if(IsInvalid(id)) return;
        Layout(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    // Recompute the layout of a parent node.
    // Returns "true" if the new layout is within the window area.
    public void NodeLayout(WD_EditorObject node) {
        // Don't layout node if it is not visible.
        if(!node.IsVisible) return;
        
        // Resolve collision on children.
        ResolveCollisionOnChildren(node, Vector2.zero);
        
        // Determine needed child rect.
        Rect  childRect   = ComputeChildRect(node);

        // Compute needed width.
        float titleWidth  = WD_EditorConfig.GetNodeWidth(node.NameOrTypeName);
        float leftMargin  = ComputeLeftMargin(node);
        float rightMargin = ComputeRightMargin(node);
        float width       = 2.0f*WD_EditorConfig.GutterSize + Mathf.Max(titleWidth, leftMargin + rightMargin + childRect.width);

        // Process case without child nodes
        Rect position= GetPosition(node);
        if(MathfExt.IsZero(childRect.width) || MathfExt.IsZero(childRect.height)) {
            // Compute needed height.
            List<WD_EditorObject> leftPorts= GetLeftPorts(node);
            List<WD_EditorObject> rightPorts= GetRightPorts(node);
            int nbOfPorts= leftPorts.Count > rightPorts.Count ? leftPorts.Count : rightPorts.Count;
            float height= Mathf.Max(WD_EditorConfig.NodeTitleHeight+nbOfPorts*WD_EditorConfig.MinimumPortSeparation, WD_EditorConfig.MinimumNodeHeight);                                

            // Apply new width and height.
            if(MathfExt.IsNotEqual(height, position.height) || MathfExt.IsNotEqual(width, position.width)) {
                float deltaWidth = width - position.width;
                float deltaHeight= height - position.height;
                Rect newPos= new Rect(position.xMin-0.5f*deltaWidth, position.yMin-0.5f*deltaHeight, width, height);
                SetPosition(node, newPos);
            }
        }
        // Process case with child nodes.
        else {
            // Adjust children local offset.
            float neededChildXOffset= WD_EditorConfig.GutterSize+leftMargin;
            float neededChildYOffset= WD_EditorConfig.GutterSize+WD_EditorConfig.NodeTitleHeight;
            if(MathfExt.IsNotEqual(neededChildXOffset, childRect.x) ||
               MathfExt.IsNotEqual(neededChildYOffset, childRect.y)) {
                   AdjustChildLocalPosition(node, new Vector2(neededChildXOffset-childRect.x, neededChildYOffset-childRect.y));
            }
            
            // Compute needed height.
            int nbOfLeftPorts = GetNbOfLeftPorts(node);
            int nbOfRightPorts= GetNbOfRightPorts(node);
            int nbOfPorts= nbOfLeftPorts > nbOfRightPorts ? nbOfLeftPorts : nbOfRightPorts;
            float portHeight= nbOfPorts*WD_EditorConfig.MinimumPortSeparation;                                
            float height= WD_EditorConfig.NodeTitleHeight+Mathf.Max(portHeight, childRect.height+2.0f*WD_EditorConfig.GutterSize);

            float deltaWidth = width - node.LocalPosition.width;
            float deltaHeight= height - node.LocalPosition.height;
            float xMin= position.xMin-0.5f*deltaWidth;
            float yMin= position.yMin-0.5f*deltaHeight;
            if(MathfExt.IsNotEqual(xMin, position.xMin) || MathfExt.IsNotEqual(yMin, position.yMin) ||
               MathfExt.IsNotEqual(width, node.LocalPosition.width) || MathfExt.IsNotEqual(height, node.LocalPosition.height)) {
                Rect newPos= new Rect(xMin, yMin, width, height);
                SetPosition(node, newPos);
            }
        }

        // Layout child ports
        LayoutPorts(node);
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void MoveTo(WD_EditorObject node, Vector2 _newPos) {
        Rect position = GetPosition(node);
        DeltaMove(node, new Vector2(_newPos.x - position.x, _newPos.y - position.y));
    }
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    public void DeltaMove(WD_EditorObject node, Vector2 _delta) {
        // Move the node
        DeltaMoveInternal(node, _delta);
        // Resolve collision between siblings.
        LayoutParent(node, _delta);
	}
    // ----------------------------------------------------------------------
    // Moves the node without changing its size.
    void DeltaMoveInternal(WD_EditorObject node, Vector2 _delta) {
        if(MathfExt.IsNotZero(_delta)) {
            node.LocalPosition.x+= _delta.x;
            node.LocalPosition.y+= _delta.y;
            node.IsDirty= true;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the absolute position of the node.
    public Rect GetPosition(WD_EditorObject node) {
        if(!IsValid(node.ParentId)) return node.LocalPosition;
        Rect position= GetPosition(EditorObjects[node.ParentId]);
        return new Rect(position.x+node.LocalPosition.x,
                        position.y+node.LocalPosition.y,
                        node.LocalPosition.width,
                        node.LocalPosition.height);
    }
    public Rect GetPosition(int id) {
        return GetPosition(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    void SetPosition(WD_EditorObject node, Rect _newPos) {
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
    Vector2 GetTopLeftCorner(WD_EditorObject node)     {
        Rect position= GetPosition(node);
        return new Vector2(position.xMin, position.yMin);
    }
    Vector2 GetTopRightCorner(WD_EditorObject node)    {
        Rect position= GetPosition(node);
        return new Vector2(position.xMax, position.yMin);
    }
    Vector2 GetBottomLeftCorner(WD_EditorObject node)  {
        Rect position= GetPosition(node);
        return new Vector2(position.xMin, position.yMax);
    }
    Vector2 GetBottomRightCorner(WD_EditorObject node) {
        Rect position= GetPosition(node);
        return new Vector2(position.xMax, position.yMax);
    }
    // ----------------------------------------------------------------------
    void LayoutParent(WD_EditorObject node, Vector2 _deltaMove) {
        if(!IsValid(node.ParentId)) return;
        WD_EditorObject parentNode= EditorObjects[node.ParentId];
        ResolveCollision(parentNode, _deltaMove);
        Layout(parentNode);
    }
    // ----------------------------------------------------------------------
    void AdjustChildLocalPosition(WD_EditorObject node, Vector2 _delta) {
        ForEachChild<WD_Node>(node, (child)=> { DeltaMoveInternal(child, _delta); } );
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildRect(WD_EditorObject node) {
        // Compute child space.
        Rect childRect= new Rect(0.5f*node.LocalPosition.width,0.5f*node.LocalPosition.height,0,0);
        ForEachChild<WD_Node>(node,
            (child)=> {
                if(child.IsVisible) {
                    childRect= Physics2D.Merge(childRect, child.LocalPosition);
                }
            }
        );
        return childRect;
    }
    // ----------------------------------------------------------------------
    // Returns the inner left margin.
    float ComputeLeftMargin(WD_EditorObject node) {
        float LeftMargin= 0;
        ForEachLeftPort(node,
            (port)=> {
                Vector2 labelSize= WD_EditorConfig.GetPortLabelSize(port.Name);
                float nameSize= labelSize.x+WD_EditorConfig.PortSize;
                if(LeftMargin < nameSize) LeftMargin= nameSize;
            }
        );
        return LeftMargin;
    }
    // ----------------------------------------------------------------------
    // Returns the inner right margin.
    float ComputeRightMargin(WD_EditorObject node) {
        float RightMargin= 0;
        ForEachRightPort(node,
            (port) => {
                Vector2 labelSize= WD_EditorConfig.GetPortLabelSize(port.Name);
                float nameSize= labelSize.x+WD_EditorConfig.PortSize;
                if(RightMargin < nameSize) RightMargin= nameSize;
            }
        );
        return RightMargin;
    }
    // ----------------------------------------------------------------------
    // Returns the inner top margin.
    static float ComputeTopMargin(WD_EditorObject node) {
        return WD_EditorConfig.GetNodeHeight(node.NameOrTypeName);
    }
    // ----------------------------------------------------------------------
    // Returns the inner bottom margin.
    static float ComputeBottomMargin(WD_EditorObject node) {
        return 0;
    }


    // ======================================================================
    // Port Layout
    // ----------------------------------------------------------------------
    // Recomputes the port position.
    public void LayoutPorts(WD_EditorObject node) {
		// Gather all ports.
        List<WD_EditorObject> topPorts   = new List<WD_EditorObject>();
        List<WD_EditorObject> bottomPorts= new List<WD_EditorObject>();
        List<WD_EditorObject> leftPorts  = new List<WD_EditorObject>();
        List<WD_EditorObject> rightPorts = new List<WD_EditorObject>();
        ForEachChild<WD_Port>(node,
            (port)=> {
                if(port.IsOnTopEdge)         { topPorts.Add(port); }
                else if(port.IsOnBottomEdge) { bottomPorts.Add(port); }
                else if(port.IsOnLeftEdge)   { leftPorts.Add(port); }
                else if(port.IsOnRightEdge)  { rightPorts.Add(port); }
            }
        );
        
        // Relayout top ports.
        Rect position= GetPosition(node);
        if(topPorts.Count != 0) {
            SortPorts(topPorts);
            float xStep= position.width / topPorts.Count;
            for(int i= 0; i < topPorts.Count; ++i) {
                if(topPorts[i].IsBeingDragged == false) {
                    topPorts[i].LocalPosition.x= (i+0.5f) * xStep;
                    topPorts[i].LocalPosition.y= 0;                
                }
            }            
        }

        // Relayout bottom ports.
        if(bottomPorts.Count != 0) {
            SortPorts(bottomPorts);
            float xStep= position.width / bottomPorts.Count;
            for(int i= 0; i < bottomPorts.Count; ++i) {
                if(bottomPorts[i].IsBeingDragged == false) {
                    bottomPorts[i].LocalPosition.x= (i+0.5f) * xStep;
                    bottomPorts[i].LocalPosition.y= position.height;                
                }
            }            
        }

        // Relayout left ports.
        if(leftPorts.Count != 0) {
            SortPorts(leftPorts);
            float topOffset= WD_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / leftPorts.Count;
            for(int i= 0; i < leftPorts.Count; ++i) {
                if(leftPorts[i].IsBeingDragged == false) {
                    leftPorts[i].LocalPosition.x= 0;
                    leftPorts[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;                
                }
            }            
        }

        // Relayout right ports.
        if(rightPorts.Count != 0) {
            SortPorts(rightPorts);
            float topOffset= WD_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / rightPorts.Count;
            for(int i= 0; i < rightPorts.Count; ++i) {
                if(rightPorts[i].IsBeingDragged == false) {
                    rightPorts[i].LocalPosition.x= position.width;
                    rightPorts[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;                
    
                }
            }
        }        
    }

    // ----------------------------------------------------------------------
    // Sorts the given port according to their relative positions.
    void SortPorts(List<WD_EditorObject> _ports) {
        for(int i= 0; i < _ports.Count-1; ++i) {
            Vector2 localPos= new Vector2(_ports[i].LocalPosition.x, _ports[i].LocalPosition.y);
            float sqrMag= localPos.sqrMagnitude;
            for(int j= i+1; j < _ports.Count; ++j) {
                localPos= new Vector2(_ports[j].LocalPosition.x, _ports[j].LocalPosition.y);
				float sqrMag2= localPos.sqrMagnitude;
				if(sqrMag > sqrMag2) {
                    WD_EditorObject p= _ports[i];
                    _ports[i]= _ports[j];
                    _ports[j]= p;
					sqrMag= sqrMag2;
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    // Returns all ports position on the top edge.
    public List<WD_EditorObject> GetTopPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachTopPort(node, (port)=> { ports.Add(port); } );
        return ports;
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the bottom edge.
    public List<WD_EditorObject> GetBottomPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachBottomPort(node, (port)=> { ports.Add(port); } );
        return ports;
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the left edge.
    public List<WD_EditorObject> GetLeftPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachLeftPort(node, (port)=> { ports.Add(port); } );
        return ports;        
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the right edge.
    public List<WD_EditorObject> GetRightPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachRightPort(node, (port)=> { ports.Add(port); } );
        return ports;
    }
    // ----------------------------------------------------------------------
    // Returns the number of ports on the top edge.
    public int GetNbOfTopPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachTopPort(node, (port)=> { ++nbOfPorts; } );
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the bottom edge.
    public int GetNbOfBottomPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachBottomPort(node, (port)=> { ++nbOfPorts; } );
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the left edge.
    public int GetNbOfLeftPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachLeftPort(node, (port)=> { ++nbOfPorts; } );
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the right edge.
    public int GetNbOfRightPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachRightPort(node, (port)=> { ++nbOfPorts; } );
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    public void ForEachTopPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChild<WD_Port>(node,
            (port)=> {
                if(port.IsOnTopEdge) {
                    fnc(port);
                }
            }
        );        
    }
    
    // ----------------------------------------------------------------------
    public void ForEachBottomPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChild<WD_Port>(node,
            (port)=> {
                if(port.IsOnBottomEdge) {
                    fnc(port);
                }
            }
        );        
    }
    
    // ----------------------------------------------------------------------
    public void ForEachLeftPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChild<WD_Port>(node,
            (port)=> {
                if(port.IsOnLeftEdge) {
                    fnc(port);
                }
            }
        );        
    }
    
    // ----------------------------------------------------------------------
    public void ForEachRightPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChild<WD_Port>(node,
            (port)=> {
                if(port.IsOnRightEdge) {
                    fnc(port);
                }
            }
        );        
    }


    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolve collision on parents.
    void ResolveCollision(WD_EditorObject node, Vector2 _delta) {
        ResolveCollisionOnChildren(node, _delta);
        if(!IsValid(node.ParentId)) return;
        ResolveCollision(EditorObjects[node.ParentId], _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren(WD_EditorObject node, Vector2 _delta) {
        bool didCollide= false;
        for(int i= 0; i < EditorObjects.Count-1; ++i) {
            WD_EditorObject child1= EditorObjects[i];
            if(child1.ParentId != node.InstanceId) continue;
            if(!child1.IsVisible) continue;
            if(!child1.IsRuntimeA<WD_Node>()) continue;
            for(int j= i+1; j < EditorObjects.Count; ++j) {
                WD_EditorObject child2= EditorObjects[j];
                if(child2.ParentId != node.InstanceId) continue;
                if(!child2.IsVisible) continue;
                if(!child2.IsRuntimeA<WD_Node>()) continue;
                didCollide |= ResolveCollisionBetweenTwoNodes(child1, child2, _delta);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildren(node, _delta);
    }

    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(WD_EditorObject node, WD_EditorObject otherNode, Vector2 _delta) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithGutter(node, otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(node, GetPosition(otherNode));
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

		// Seperate using the known movement.
        if( !MathfExt.IsZero(_delta) ) {
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
    public bool DoesCollide(WD_EditorObject node, WD_EditorObject otherNode) {
        return Physics2D.DoesCollide(GetPosition(node), GetPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithGutter(WD_EditorObject node, WD_EditorObject otherNode) {
        return Physics2D.DoesCollide(RectWithGutter(GetPosition(node)), GetPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    static Rect RectWithGutter(Rect _rect) {
        float gutterSize= WD_EditorConfig.GutterSize;
        float gutterSize2= 2.0f*gutterSize;
        return new Rect(_rect.x-gutterSize, _rect.y-gutterSize, _rect.width+gutterSize2, _rect.height+gutterSize2);        
    }
    
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(WD_EditorObject node, Rect _rect) {
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
	Vector2 GetSeperationVector(WD_EditorObject node, WD_EditorObject otherNode) {
	    return GetSeperationVector(node, GetPosition(otherNode));
	}

    // ----------------------------------------------------------------------
    // Returns true if the given point is inside the node coordinates.
    bool IsInside(WD_EditorObject node, Vector2 _point) {
        return Physics2D.IsInside(_point, GetPosition(node));
    }

    // ======================================================================
    // Layout from WD_Port
    // ----------------------------------------------------------------------
    public void PortLayout(WD_EditorObject port) {
        // Don't interfear with dragging.
        if(port.IsBeingDragged) return;

        // Retreive parent layout information.
        if(!IsValid(port.ParentId)) {
            Debug.LogWarning("Trying to layout a port who does not have a parent!!!");
            return;
        }
        WD_EditorObject parentNode= EditorObjects[port.ParentId];
        Rect parentPosition= GetPosition(parentNode);

        // Make certain that the port is on an edge.
        switch(port.Edge) {
            case WD_EditorObject.EdgeEnum.Top:
                if(!MathfExt.IsZero(port.LocalPosition.y)) {
                    port.LocalPosition.y= 0;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;                    
                }
                if(port.LocalPosition.x > parentPosition.width) {
                    port.LocalPosition.x= parentPosition.width-WD_EditorConfig.PortSize;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;
                }
                break;
            case WD_EditorObject.EdgeEnum.Bottom:
                if(MathfExt.IsNotEqual(port.LocalPosition.y, parentPosition.height)) {
                    port.LocalPosition.y= parentPosition.height;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;                    
                }
                if(port.LocalPosition.x > parentPosition.width) {
                    port.LocalPosition.x= parentPosition.width-WD_EditorConfig.PortSize;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;
                }
                break;
            case WD_EditorObject.EdgeEnum.Left:
                if(!MathfExt.IsZero(port.LocalPosition.x)) {
                    port.LocalPosition.x= 0;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;                    
                }
                if(port.LocalPosition.y > parentPosition.height) {
                    port.LocalPosition.y= parentPosition.height-WD_EditorConfig.PortSize;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;
                }
                break;
            case WD_EditorObject.EdgeEnum.Right:
                if(MathfExt.IsNotEqual(port.LocalPosition.x, parentPosition.width)) {
                    port.LocalPosition.x= parentPosition.width;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;                    
                }
                if(port.LocalPosition.y > parentPosition.height) {
                    port.LocalPosition.y= parentPosition.height-WD_EditorConfig.PortSize;
                    port.IsDirty= true;
                    parentNode.IsDirty= true;
                }
                break;            
        }
    }
    // ----------------------------------------------------------------------
    public void SnapToParent(WD_EditorObject port) {
        WD_EditorObject parentNode= EditorObjects[port.ParentId];
        Rect parentPosition= GetPosition(parentNode);
        float parentHeight= parentPosition.height;
        float parentWidth= parentPosition.width;
        float portRadius= WD_EditorConfig.PortRadius;
        if(MathfExt.IsWithin(port.LocalPosition.y, -portRadius, portRadius)) {
            port.Edge= WD_EditorObject.EdgeEnum.Top;
        }        
        if(MathfExt.IsWithin(port.LocalPosition.y, parentHeight-portRadius, parentHeight+portRadius)) {
            port.Edge= WD_EditorObject.EdgeEnum.Bottom;
        }
        if(MathfExt.IsWithin(port.LocalPosition.x, -portRadius, portRadius)) {
            port.Edge= WD_EditorObject.EdgeEnum.Left;
        }
        if(MathfExt.IsWithin(port.LocalPosition.x, parentWidth-portRadius, parentWidth+portRadius)) {
            port.Edge= WD_EditorObject.EdgeEnum.Right;
        }
        port.IsDirty= true;
        PortLayout(port);
    }

    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent(WD_EditorObject port) {
        WD_EditorObject parentNode= EditorObjects[port.ParentId];
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
    public bool IsNearParent(WD_EditorObject port) {
        return GetDistanceFromParent(port) <= WD_EditorConfig.PortSize*2;
    }

	// ----------------------------------------------------------------------
    public WD_EditorObject GetOverlappingPort(WD_EditorObject port) {
        WD_EditorObject foundPort= null;
        Rect tmp= GetPosition(port);
        Vector2 position= new Vector2(tmp.x, tmp.y);
        ForEach<WD_Port>(
            (p) => {
                if(p != port) {
                    tmp= GetPosition(p);
                    Vector2 pPos= new Vector2(tmp.x, tmp.y);
                    float distance= Vector2.Distance(pPos, position);
                    if(distance <= 1.5*WD_EditorConfig.PortSize) {
                        foundPort= p;
                    }
                }                
            }
        );
        return foundPort;
    }	

}
