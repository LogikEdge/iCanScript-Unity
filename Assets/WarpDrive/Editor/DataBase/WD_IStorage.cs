using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WD_IStorage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    bool            IsDirty  = true;
    WD_Storage      Storage  = null;
    WD_TreeCache    TreeCache= null;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_IStorage(WD_Storage storage) {
        Init(storage);
    }
    public void Init(WD_Storage storage) {
        if(Storage != storage) {
            IsDirty= true;
            Storage= storage;
            GenerateEditorData();            
        }
    }
    public void Reset() {
        IsDirty= true;
        Storage= null;
        TreeCache= null;
    }
    // ----------------------------------------------------------------------
    void GenerateEditorData() {
        TreeCache= new WD_TreeCache();
        ForEach(obj=> TreeCache.CreateInstance(obj));
    }
    
    
    // ======================================================================
    // Basic Accessors
    // ----------------------------------------------------------------------
    public List<WD_EditorObject> EditorObjects { get { return Storage.EditorObjects; }}
    public WD_UserPreferences    Preferences   { get { return Storage.Preferences; }}
    // ----------------------------------------------------------------------
    public bool IsValid(int id)     { return id >= 0 && id < EditorObjects.Count && this[id].InstanceId != -1; }
    public bool IsInvalid(int id)   { return !IsValid(id); }
    // ----------------------------------------------------------------------
    public WD_EditorObject this[int id] {
        get { return EditorObjects[id]; }
        set {
            if(value.InstanceId != id) Debug.LogError("Trying to add EditorObject at wrong index.");
            EditorObjects[id]= value;
            if(TreeCache.IsValid(id)) TreeCache.UpdateInstance(value);
            else                      TreeCache.CreateInstance(value);            
        }
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetParent(WD_EditorObject obj) { return IsValid(obj.ParentId) ? EditorObjects[obj.ParentId] : null; }
    public WD_EditorObject GetSource(WD_EditorObject obj) { return IsValid(obj.Source) ? EditorObjects[obj.Source] : null; }
    // ----------------------------------------------------------------------
    public void SetDirty(WD_EditorObject obj) {
        if(obj.IsPort) { GetParent(obj).IsDirty= true; }
        obj.IsDirty= true;        
    }

    // ======================================================================
    // Storage Update
    // ----------------------------------------------------------------------
    public void Update() {
        if(IsDirty) {
            IsDirty= false;
            Undo.RegisterUndo(Storage, "WarpDrive");
            EditorUtility.SetDirty(Storage);
        }
    }

    // ======================================================================
    // Editor Object Creation/Destruction
    // ----------------------------------------------------------------------
    public int GetNextAvailableId() {
        // Find the next available id.
        int id= 0;
        int len= EditorObjects.Count;
        while(id < len && IsValid(id)) { ++id; }
        if(id >= len) {
            id= len;
            EditorObjects.Add(null);
        }
        return id;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateInstance(string name, int parentId, WD_ObjectTypeEnum objType, Vector2 initialPos, Type rtType) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, name, rtType, parentId, objType, localPos);
        TreeCache.CreateInstance(EditorObjects[id]);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateBehaviour() {
        // Create the function node.
        int id= GetNextAvailableId();
        // Validate that behaviour is at the root.
        if(id != 0) {
            Debug.LogError("Behaviour MUST be the root object !!!");
        }
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, "Behaviour", typeof(WD_Behaviour), -1, WD_ObjectTypeEnum.Behaviour, new Rect(0,0,0,0));
        TreeCache.CreateInstance(EditorObjects[id]);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateModuleLibrary() {
        // Validate that a library can only be create at the root.
        if(EditorObjects.Count != 0) {
            Debug.LogError("Module Library MUST be the root object !!!");
        }
        return CreateModule(-1, Vector2.zero, "Module Library");
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateModule(int parentId, Vector2 initialPos, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, name, typeof(WD_Module), parentId, WD_ObjectTypeEnum.Module, localPos);
        TreeCache.CreateInstance(EditorObjects[id]);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateStateChartLibrary() {
        // Validate that a library can only be create at the root.
        if(EditorObjects.Count != 0) {
            Debug.LogError("Module Library MUST be the root object !!!");
        }
        return CreateStateChart(-1, Vector2.zero, "StateChart Library");
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateStateChart(int parentId, Vector2 initialPos, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, name, typeof(WD_StateChart), parentId, WD_ObjectTypeEnum.StateChart, localPos);
        TreeCache.CreateInstance(EditorObjects[id]);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateState(int parentId, Vector2 initialPos, string name= "") {
        // Validate that we have a good parent.
        WD_EditorObject parent= EditorObjects[parentId];
        if(parent == null || (!WD.IsStateChart(parent) && !WD.IsState(parent))) {
            Debug.LogError("State must be created as a child of StateChart or State.");
        }
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, name, typeof(WD_State), parentId, WD_ObjectTypeEnum.State, localPos);
        TreeCache.CreateInstance(EditorObjects[id]);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_BaseDesc desc) {
        if(desc is WD_ClassDesc) {
            return CreateFunction(parentId, initialPos, desc as WD_ClassDesc);
        }
        else if(desc is WD_FunctionDesc) {
            return CreateFunction(parentId, initialPos, desc as WD_FunctionDesc);
        }
        else if(desc is WD_ConversionDesc) {
            return CreateFunction(parentId, initialPos, desc as WD_ConversionDesc);
        }
        return null;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_ClassDesc desc) {
        // Create the class node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        EditorObjects[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Class, localPos, desc.Icon);
        TreeCache.CreateInstance(EditorObjects[id]);
        // Create field ports
        for(int i= 0; i < desc.FieldNames.Length; ++i) {
            WD_ObjectTypeEnum portType= desc.FieldInOuts[i] ? WD_ObjectTypeEnum.OutFieldPort : WD_ObjectTypeEnum.InFieldPort;
            CreatePort(desc.FieldNames[i], id, desc.FieldTypes[i], portType);
        }
        // Create property ports
        for(int i= 0; i < desc.PropertyNames.Length; ++i) {
            WD_ObjectTypeEnum portType= desc.PropertyInOuts[i] ? WD_ObjectTypeEnum.OutPropertyPort : WD_ObjectTypeEnum.InPropertyPort;
            CreatePort(desc.PropertyNames[i], id, desc.PropertyTypes[i], portType);
        }
        // Create methods.
        int nbOfMethodsToShow= 0;
        for(int i= 0; i < desc.MethodNames.Length; ++i) {
            if(desc.ParameterNames[i].Length != 0) ++nbOfMethodsToShow;
        }
        for(int i= 0; i < desc.MethodNames.Length; ++i) {
            int methodId= -1;
            if(nbOfMethodsToShow > 1) {
                methodId= GetNextAvailableId();
                EditorObjects[methodId]= new WD_EditorObject(methodId, desc.MethodNames[i], desc.ClassType, id, WD_ObjectTypeEnum.Function, new Rect(0,0,0,0), desc.MethodIcons[i]);
                TreeCache.CreateInstance(EditorObjects[methodId]);                
            }
            for(int p= 0; p < desc.ParameterNames[i].Length; ++p) {
                WD_ObjectTypeEnum portType= desc.ParameterInOuts[i][p] ? WD_ObjectTypeEnum.OutModulePort : WD_ObjectTypeEnum.InModulePort;
                WD_EditorObject classPort= CreatePort(desc.ParameterNames[i][p], id, desc.ParameterTypes[i][p], portType);
                if(nbOfMethodsToShow > 1) {
                    portType= desc.ParameterInOuts[i][p] ? WD_ObjectTypeEnum.OutFunctionPort : WD_ObjectTypeEnum.InFunctionPort;
                    WD_EditorObject funcPort= CreatePort(desc.ParameterNames[i][p], methodId, desc.ParameterTypes[i][p], portType);
                    if(portType == WD_ObjectTypeEnum.OutFunctionPort) {
                        SetSource(classPort, funcPort);
                    }
                    else {
                        SetSource(funcPort, classPort);
                    }                    
                }
            }
            if(desc.ReturnTypes[i] != null) {
                WD_EditorObject classPort= CreatePort(desc.ReturnNames[i], id, desc.ReturnTypes[i], WD_ObjectTypeEnum.OutModulePort);
                if(nbOfMethodsToShow > 1) {
                    WD_EditorObject funcPort= CreatePort(desc.ReturnNames[i], methodId, desc.ReturnTypes[i], WD_ObjectTypeEnum.OutFunctionPort);
                    SetSource(classPort, funcPort);                    
                }
            }
        }
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_FunctionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Function, localPos, desc.Icon);
        TreeCache.CreateInstance(EditorObjects[id]);
        // Create input/output ports.
        for(int i= 0; i < desc.ParameterNames.Length; ++i) {
            WD_ObjectTypeEnum portType= desc.ParameterInOuts[i] ? WD_ObjectTypeEnum.OutFunctionPort : WD_ObjectTypeEnum.InFunctionPort;
            CreatePort(desc.ParameterNames[i], id, desc.ParameterTypes[i], portType);
        }
        if(desc.ReturnType != null) {
            CreatePort(desc.ReturnName, id, desc.ReturnType, WD_ObjectTypeEnum.OutFunctionPort);
        }
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_ConversionDesc desc) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        EditorObjects[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Conversion, localPos, desc.Icon);
        TreeCache.CreateInstance(EditorObjects[id]);
        // Create input/output ports.
        CreatePort(desc.FromType.Name, id, desc.FromType, WD_ObjectTypeEnum.InFunctionPort);
        CreatePort(desc.ToType.Name,   id, desc.ToType,   WD_ObjectTypeEnum.OutFunctionPort);
        return EditorObjects[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreatePort(string name, int parentId, Type valueType, WD_ObjectTypeEnum portType) {
        int id= GetNextAvailableId();
        WD_EditorObject port= EditorObjects[id]= new WD_EditorObject(id, name, valueType, parentId, portType, new Rect(0,0,0,0));
        TreeCache.CreateInstance(port);
        // Reajust data port position 
        if(port.IsDataPort && !port.IsEnablePort) {
            WD_EditorObject parent= EditorObjects[port.ParentId];
            if(port.IsInputPort) {
                int nbOfPorts= GetNbOfLeftPorts(parent);
                port.LocalPosition= new Rect(0, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);
            } else {
                int nbOfPorts= GetNbOfRightPorts(parent);
                port.LocalPosition= new Rect(parent.LocalPosition.width, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);                
            }
        }
        return EditorObjects[id];        
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        DestroyInstanceInternal(id);
        ForEach(
            (port) => {
                if(port.IsModulePort && EditorObjects[port.ParentId].IsModule && IsPortDisconnected(port)) {
                    DestroyInstanceInternal(port.InstanceId);
                }
            }
        );
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(WD_EditorObject eObj) {
        DestroyInstance(eObj.InstanceId);
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(int id) {
        if(IsInvalid(id)) {
            Debug.LogError("Trying the delete a non-existing EditorObject with id= "+id);
        }
        // Remove all children first
        while(TreeCache[id].Children.Count != 0) {
            DestroyInstanceInternal(TreeCache[id].Children[0]);
        }
        // Disconnect ports linking to this port.
        ExecuteIf(EditorObjects[id], WD.IsPort, (instance) => { DisconnectPort(EditorObjects[id]); });
        // Remove all related objects.
        if(IsValid(EditorObjects[id].ParentId)) SetDirty(GetParent(EditorObjects[id]));
        TreeCache.DestroyInstance(id);
        EditorObjects[id].Reset();
    }


    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(WD_EditorObject eObj) {
        if(eObj.IsHidden) return false;
        if(IsInvalid(eObj.ParentId)) return true;
        WD_EditorObject parent= EditorObjects[eObj.ParentId];
        if(eObj.IsNode && (parent.IsFolded || parent.IsMinimized)) return false;
        return IsVisible(parent);
    }
    public bool IsVisible(int id) { return IsInvalid(id) ? false : IsVisible(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsFolded(WD_EditorObject eObj) { return eObj.IsFolded; }
    // ----------------------------------------------------------------------
    public void Fold(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Fold();
        eObj.IsDirty= true;
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Unfold(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Unfold();
        eObj.IsDirty= true;
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMinimized(WD_EditorObject eObj) {
        return eObj.IsMinimized;
    }
    public void Minimize(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Minimize();
        ForEachChild(eObj, (child) => { if(child.IsPort) child.Minimize(); });
        eObj.IsDirty= true;
        if(IsValid(eObj.ParentId)) EditorObjects[eObj.ParentId].IsDirty= true;
    }
    public void Minimize(int id) { if(IsValid(id)) Minimize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Maximize(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Maximize();
        ForEachChild(eObj, (child) => { if(child.IsPort) child.Maximize(); });
        eObj.IsDirty= true;
        if(IsValid(eObj.ParentId)) EditorObjects[eObj.ParentId].IsDirty= true;
    }
    public void Maximize(int id) { if(IsValid(id)) Maximize(EditorObjects[id]); }
    


    // ======================================================================
    // Port Connectivity
    // ----------------------------------------------------------------------
    public void SetSource(WD_EditorObject obj, WD_EditorObject src) {
        obj.Source= src == null ? -1 : src.InstanceId;
    }
    // ----------------------------------------------------------------------
    public void SetSource(WD_EditorObject inPort, WD_EditorObject outPort, WD_ConversionDesc convDesc) {
        Rect inPos= GetPosition(inPort);
        Rect outPos= GetPosition(outPort);
        Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
        int grandParentId= EditorObjects[inPort.ParentId].ParentId;
        WD_EditorObject conv= CreateFunction(grandParentId, convPos, convDesc);
        ForEachChild(conv,
            (child) => {
                if(child.IsInputPort) {
                    SetSource(child, outPort);
                } else if(child.IsOutputPort) {
                    SetSource(inPort, child);
                }
            }
        );
        Minimize(conv);
    }
    // ----------------------------------------------------------------------
    public void DisconnectPort(WD_EditorObject port) {
        SetSource(port, null);
        Prelude.forEach(p=> SetSource(p, null), FindConnectedPorts(port));
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject[] FindConnectedPorts(WD_EditorObject port) {
        return Filter(p=> p.IsPort && p.Source == port.InstanceId).ToArray();
    }
    // ----------------------------------------------------------------------
    bool IsPortConnected(WD_EditorObject port) {
        if(port.Source != -1) return true;
        foreach(var obj in EditorObjects) {
            if(obj.IsValid && obj.IsPort && obj.Source == port.InstanceId) return true;
        }
        return false;
    }
    bool IsPortDisconnected(WD_EditorObject port) { return !IsPortConnected(port); }


    // ======================================================================
    // Object Picking
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public WD_EditorObject GetNodeAt(Vector2 pick) {
        WD_EditorObject foundNode= null;
        FilterWith(
            n=> n.IsNode && IsVisible(n) && IsInside(n, pick) && (foundNode == null || n.LocalPosition.width < foundNode.LocalPosition.width), 
            n=> foundNode= n
        );
        return foundNode;
    }
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public WD_EditorObject GetPortAt(Vector2 pick) {
        WD_EditorObject bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        FilterWith(
            port=> port.IsPort && IsVisible(port),
            port=> {
                Rect tmp= GetPosition(port);
                Vector2 position= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(position, pick);
                if(distance < 1.5f * WD_EditorConfig.PortRadius && distance < bestDistance) {
                    bestDistance= distance;
                    bestPort= port;
                }                                
            } 
        );
        return bestPort;
    }
    // ----------------------------------------------------------------------
    // Returns true if pick is in the titlebar of the node.
    public bool IsInTitleBar(WD_EditorObject node, Vector2 pick) {
        if(!node.IsNode) return false;
        return true;
    }

    // ======================================================================
    // Node Layout
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
        ExecuteIf(obj, WD.IsNode, NodeLayout);
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
        if(!IsVisible(node)) return;

        // Minimized nodes are fully collapsed.
        if(node.IsMinimized) {
            Texture2D icon= WD_Graphics.GetMaximizeIcon(node, null, this);
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
        float titleWidth  = WD_EditorConfig.GetNodeWidth(node.NameOrTypeName)+WD_EditorConfig.ExtraIconWidth;
        float leftMargin  = ComputeLeftMargin(node);
        float rightMargin = ComputeRightMargin(node);
        float width       = 2.0f*WD_EditorConfig.GutterSize + Mathf.Max(titleWidth, leftMargin + rightMargin + childRect.width);

        // Process case without child nodes
        Rect position= GetPosition(node);
        if(MathfExt.IsZero(childRect.width) || MathfExt.IsZero(childRect.height)) {
            // Compute needed height.
            WD_EditorObject[] leftPorts= GetLeftPorts(node);
            WD_EditorObject[] rightPorts= GetRightPorts(node);
            int nbOfPorts= leftPorts.Length > rightPorts.Length ? leftPorts.Length : rightPorts.Length;
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
            SetDirty(node);
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
        ForEachChild(node, (child)=> { if(child.IsNode) DeltaMoveInternal(child, _delta); } );
    }
    // ----------------------------------------------------------------------
    // Returns the space used by all children.
    Rect ComputeChildRect(WD_EditorObject node) {
        // Compute child space.
        Rect childRect= new Rect(0.5f*node.LocalPosition.width,0.5f*node.LocalPosition.height,0,0);
        ForEachChild(node,
            (child)=> {
                if(child.IsNode && IsVisible(child)) {
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
        WD_EditorObject[] topPorts   = SortTopPorts(node);
        WD_EditorObject[] bottomPorts= SortBottomPorts(node);
        WD_EditorObject[] leftPorts  = SortLeftPorts(node);
        WD_EditorObject[] rightPorts = SortRightPorts(node);
        
        // Relayout top ports.
        Rect position= GetPosition(node);
        if(topPorts.Length != 0) {
            float xStep= position.width / topPorts.Length;
            for(int i= 0; i < topPorts.Length; ++i) {
                if(topPorts[i].IsBeingDragged == false) {
                    topPorts[i].LocalPosition.x= (i+0.5f) * xStep;
                    topPorts[i].LocalPosition.y= 0;                
                }
            }            
        }

        // Relayout bottom ports.
        if(bottomPorts.Length != 0) {
            float xStep= position.width / bottomPorts.Length;
            for(int i= 0; i < bottomPorts.Length; ++i) {
                if(bottomPorts[i].IsBeingDragged == false) {
                    bottomPorts[i].LocalPosition.x= (i+0.5f) * xStep;
                    bottomPorts[i].LocalPosition.y= position.height;                
                }
            }            
        }

        // Relayout left ports.
        if(leftPorts.Length != 0) {
            float topOffset= WD_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / leftPorts.Length;
            for(int i= 0; i < leftPorts.Length; ++i) {
                if(leftPorts[i].IsBeingDragged == false) {
                    leftPorts[i].LocalPosition.x= 0;
                    leftPorts[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;                
                }
            }            
        }

        // Relayout right ports.
        if(rightPorts.Length != 0) {
            float topOffset= WD_EditorConfig.NodeTitleHeight;
            float yStep= (position.height-topOffset) / rightPorts.Length;
            for(int i= 0; i < rightPorts.Length; ++i) {
                if(rightPorts[i].IsBeingDragged == false) {
                    rightPorts[i].LocalPosition.x= position.width;
                    rightPorts[i].LocalPosition.y= topOffset + (i+0.5f) * yStep;
                }
            }
        }        
    }

    // ----------------------------------------------------------------------
    WD_EditorObject[] SortTopPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetTopPorts(node);
        float[] angles= Prelude.map(port=> GetAverageConnectionAngle(port, 270.0f), ports);
        foreach(var v in angles) Debug.Log("Top Angles: "+v);
        return Prelude.reverse(SortPorts(ports, angles));
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortBottomPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetBottomPorts(node);
        float[] angles= Prelude.map(port=> GetAverageConnectionAngle(port, 90.0f), ports);
        foreach(var v in angles) Debug.Log("Bottom Angles: "+v);
        return SortPorts(ports, angles);
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortRightPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetRightPorts(node);
        float[] angles= Prelude.map(port=> GetAverageConnectionAngle(port, 180.0f), ports);
        foreach(var v in angles) Debug.Log("Right Angles: "+v);
        return Prelude.reverse(SortPorts(ports, angles));
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortLeftPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetLeftPorts(node);
        float[] angles= Prelude.map(port=> GetAverageConnectionAngle(port, 0.0f), ports);
        foreach(var v in angles) Debug.Log("Left Angles: "+v);
        return SortPorts(ports, angles);
    }
    // ----------------------------------------------------------------------
    float GetAverageConnectionAngle(WD_EditorObject port, float angleOffset) {
        Rect portRect= GetPosition(port);
        Vector2 portPos= new Vector2(portRect.x, portRect.y);
        int nbOfAngles= 0;
        float angleSum= 0.0f;
        if(IsValid(port.Source)) {
            Rect sourceRect= GetPosition(EditorObjects[port.Source]);
            angleSum= GetAngleWithOffset(portPos, new Vector2(sourceRect.x, sourceRect.y), angleOffset);
            nbOfAngles= 1;
        }
        angleSum= Prelude.fold<WD_EditorObject,float>(
            (remotePort,sum)=> {
                ++nbOfAngles;
                Rect remoteRect= GetPosition(remotePort);
                return sum+GetAngleWithOffset(portPos, new Vector2(remoteRect.x, remoteRect.y), angleOffset);
            },
            angleSum,
            FindConnectedPorts(port) 
        );
        return nbOfAngles != 0 ? angleSum/nbOfAngles : 180.0f;        
    }
    // ----------------------------------------------------------------------
    static float GetAngleWithOffset(Vector2 v1, Vector2 v2, float angleOffset) {
        float angle= MathfExt.GetAngle(v1,v2) - angleOffset;
        if(angle < 0.0f) angle+= 360.0f;
        if(angle > 360.0) angle-= 360.0f;
        return angle;
    } 
    // ----------------------------------------------------------------------
    // Sorts the given port according to their relative positions.
    WD_EditorObject[] SortPorts(WD_EditorObject[] ports, float[] angles) {
        for(int i= 0; i < angles.Length-1; ++i) {
            for(int j= i+1; j < angles.Length; ++j) {
                if(angles[i] > angles[j]) {
                    Prelude.exchange(ref angles[i], ref angles[j]);
                    Prelude.exchange(ref ports[i], ref ports[j]);
                }
            }
        }
        return ports;
    }
    // ----------------------------------------------------------------------
    // Returns all ports position on the top edge.
    public WD_EditorObject[] GetTopPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachTopPort(node, port=> ports.Add(port));
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the bottom edge.
    public WD_EditorObject[] GetBottomPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachBottomPort(node, port=> ports.Add(port));
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the left edge.
    public WD_EditorObject[] GetLeftPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachLeftPort(node, port=> ports.Add(port));
        return ports.ToArray();        
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the right edge.
    public WD_EditorObject[] GetRightPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachRightPort(node, port=> ports.Add(port));
        return ports.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns the number of ports on the top edge.
    public int GetNbOfTopPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachTopPort(node, port=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the bottom edge.
    public int GetNbOfBottomPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachBottomPort(node, port=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the left edge.
    public int GetNbOfLeftPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachLeftPort(node, port=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the right edge.
    public int GetNbOfRightPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachRightPort(node, port=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    public void ForEachChildPort(WD_EditorObject node, Action<WD_EditorObject> action) {
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, action));
    }
    // ----------------------------------------------------------------------
    public void ForEachTopPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnTopEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachBottomPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnBottomEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachLeftPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnLeftEdge, fnc));
    }
    // ----------------------------------------------------------------------
    public void ForEachRightPort(WD_EditorObject node, System.Action<WD_EditorObject> fnc) {
        ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsOnRightEdge, fnc));
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
            if(!IsVisible(child1)) continue;
            if(!child1.IsNode) continue;
            for(int j= i+1; j < EditorObjects.Count; ++j) {
                WD_EditorObject child2= EditorObjects[j];
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
    bool IsInside(WD_EditorObject node, Vector2 point) {
        return GetPosition(node).Contains(point);
    }


    // ======================================================================
    // Layout from WD_Port
    // ----------------------------------------------------------------------
    public void UpdatePortEdge(WD_EditorObject port) {
        if(IsValid(port.Source)) UpdatePortEdges(port, EditorObjects[port.Source]);
        Prelude.forEach(p=> UpdatePortEdges(port, p), FindConnectedPorts(port));
    }
    // ----------------------------------------------------------------------
    public void UpdatePortEdges(WD_EditorObject p1, WD_EditorObject p2) {
        WD_EditorObject.EdgeEnum p1Edge= p1.Edge;
        WD_EditorObject.EdgeEnum p2Edge= p2.Edge;
        UpdatePortEdgesInternal(p1, p2);
        if(p1Edge != p1.Edge) SetDirty(p1);
        if(p2Edge != p2.Edge) SetDirty(p2);
    }
    void UpdatePortEdgesInternal(WD_EditorObject p1, WD_EditorObject p2) {
        // Reset edge information.
        p1.Edge= WD_EditorObject.EdgeEnum.None;
        p2.Edge= WD_EditorObject.EdgeEnum.None;
        UpdatePortEdgeHardConstraints(p1);
        UpdatePortEdgeHardConstraints(p2);
        if(p1.Edge != WD_EditorObject.EdgeEnum.None && p2.Edge != WD_EditorObject.EdgeEnum.None) return;
        WD_EditorObject p1Parent= GetParent(p1);
        WD_EditorObject p2Parent= GetParent(p2);
        // Verify connection between nested nodes.
        Rect parent1Rect= GetPosition(p1Parent);
        Rect parent2Rect= GetPosition(p2Parent);
        // Nested
        if(IsChildOf(p1Parent, p2Parent) ||
           IsChildOf(p2Parent, p1Parent)) {
            Debug.LogError("Update of nested ports not implemented...");
            return;
        }
        // Horizontal
        if(parent1Rect.xMin <= parent2Rect.xMin && parent1Rect.xMax > parent2Rect.xMin ||
           parent2Rect.xMin <= parent1Rect.xMin && parent2Rect.xMax > parent1Rect.xMin) {
            if(parent1Rect.yMin < parent2Rect.yMin) {
                p1.Edge= WD_EditorObject.EdgeEnum.Bottom;
                p2.Edge= WD_EditorObject.EdgeEnum.Top;
            } else {
                p1.Edge= WD_EditorObject.EdgeEnum.Top;
                p2.Edge= WD_EditorObject.EdgeEnum.Bottom;                
            }
            return;
        }
        // Vertical
        if(parent1Rect.yMin <= parent2Rect.yMin && parent1Rect.yMax > parent2Rect.yMin ||
           parent2Rect.yMin <= parent1Rect.yMin && parent2Rect.yMax > parent1Rect.yMin) {
            if(parent1Rect.xMin < parent2Rect.xMin) {
                p1.Edge= WD_EditorObject.EdgeEnum.Right;
                p2.Edge= WD_EditorObject.EdgeEnum.Left;
            } else {
                p1.Edge= WD_EditorObject.EdgeEnum.Left;
                p2.Edge= WD_EditorObject.EdgeEnum.Right;                
            }
            return;
        }
        // Diagonal
        if(parent1Rect.xMin < parent2Rect.xMin) {
            if(parent1Rect.yMin < parent2Rect.yMin) {
                if(p1.Source == p2.InstanceId) {
                    p1.Edge= WD_EditorObject.EdgeEnum.Bottom;
                    p2.Edge= WD_EditorObject.EdgeEnum.Left;
                } else {
                    p1.Edge= WD_EditorObject.EdgeEnum.Right;
                    p2.Edge= WD_EditorObject.EdgeEnum.Top;                    
                }
            } else {
                if(p1.Source == p2.InstanceId) {
                    p1.Edge= WD_EditorObject.EdgeEnum.Right;
                    p2.Edge= WD_EditorObject.EdgeEnum.Bottom;
                } else {
                    p1.Edge= WD_EditorObject.EdgeEnum.Top;
                    p2.Edge= WD_EditorObject.EdgeEnum.Left;                    
                }                
            }
            return;
        }
        if(parent1Rect.yMin < parent2Rect.yMin) {
            if(p1.Source == p2.InstanceId) {
                p1.Edge= WD_EditorObject.EdgeEnum.Left;
                p2.Edge= WD_EditorObject.EdgeEnum.Top;
            } else {
                p1.Edge= WD_EditorObject.EdgeEnum.Bottom;
                p2.Edge= WD_EditorObject.EdgeEnum.Right;                    
            }
        } else {
            if(p1.Source == p2.InstanceId) {
                p1.Edge= WD_EditorObject.EdgeEnum.Top;
                p2.Edge= WD_EditorObject.EdgeEnum.Right;
            } else {
                p1.Edge= WD_EditorObject.EdgeEnum.Left;
                p2.Edge= WD_EditorObject.EdgeEnum.Bottom;                    
            }            
        }
    }
    void UpdatePortEdgeHardConstraints(WD_EditorObject port) {
        if(port.IsEnablePort) {
            port.Edge= WD_EditorObject.EdgeEnum.Top;
            return;            
        }
        if(port.IsDataPort) {
            port.Edge= port.IsInputPort ? WD_EditorObject.EdgeEnum.Left : WD_EditorObject.EdgeEnum.Right;
            return;
        }
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
        FilterWith(
            p=> p.IsPort && p != port,
            p=> {
                tmp= GetPosition(p);
                Vector2 pPos= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(pPos, position);
                if(distance <= 1.5*WD_EditorConfig.PortSize) {
                    foundPort= p;
                }
            }
        );
        return foundPort;
    }	


    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    // Executes the given action if the given object matches the T type.
    public static void ExecuteIf(WD_EditorObject obj, Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> f) {
        Prelude.executeIf<WD_EditorObject>(obj,cond,f);
    }
    public void ExecuteIf(int id, Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> f) {
        if(!IsValid(id)) return;
        ExecuteIf(EditorObjects[id], cond, f);
    }
    public void FilterWith(Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> action) {
        Prelude.filterWith(cond, action, EditorObjects);
    }
    public List<WD_EditorObject> Filter(Func<WD_EditorObject,bool> cond) {
        return Prelude.filter(cond, EditorObjects);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, defaultFnc);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, defaultFnc);
    }
    public static void Case(WD_EditorObject obj,
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                     Func<WD_EditorObject,bool> c8, Action<WD_EditorObject> f8,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, defaultFnc);
    }
    public static void Case(WD_EditorObject obj, 
                     Func<WD_EditorObject,bool> c1, Action<WD_EditorObject> f1,
                     Func<WD_EditorObject,bool> c2, Action<WD_EditorObject> f2,
                     Func<WD_EditorObject,bool> c3, Action<WD_EditorObject> f3,
                     Func<WD_EditorObject,bool> c4, Action<WD_EditorObject> f4,
                     Func<WD_EditorObject,bool> c5, Action<WD_EditorObject> f5,
                     Func<WD_EditorObject,bool> c6, Action<WD_EditorObject> f6,
                     Func<WD_EditorObject,bool> c7, Action<WD_EditorObject> f7,
                     Func<WD_EditorObject,bool> c8, Action<WD_EditorObject> f8,
                     Func<WD_EditorObject,bool> c9, Action<WD_EditorObject> f9,
                                                    Action<WD_EditorObject> defaultFnc= null) {
        Prelude.choice(obj, c1, f1, c2, f2, c3, f3, c4, f4, c5, f5, c6, f6, c7, f7, c8, f8, c9, f9, defaultFnc);
    }
    public void ForEachChild(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public void ForEach(Action<WD_EditorObject> fnc) {
        Prelude.filterWith(WD.IsValid, fnc, EditorObjects);
    }
    public void ForEachRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ForEachRecursiveDepthLast(parent, fnc);
    }
    public void ForEachRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));                                
        } else {
            TreeCache.ForEachRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id => fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ForEachChildRecursiveDepthLast(parent, fnc);
    }
    public void ForEachChildRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachChildRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id=> fnc(EditorObjects[id]));                    
        } else {
            TreeCache.ForEachChildRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));        
        }
    }
    // ----------------------------------------------------------------------
    public bool IsChildOf(WD_EditorObject child, WD_EditorObject parent) {
        if(IsInvalid(child.ParentId)) return false;
        if(child.ParentId == parent.InstanceId) return true;
        return IsChildOf(EditorObjects[child.ParentId], parent);
    }

}
