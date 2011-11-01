using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class WD_IStorage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    bool            myIsDirty = true;
    WD_Storage      Storage   = null;
    WD_TreeCache    TreeCache = null;
    int             UndoRedoId= 0;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_IStorage(WD_Storage storage) {
        Init(storage);
    }
    public void Init(WD_Storage storage) {
        if(Storage != storage) {
            myIsDirty= true;
            Storage= storage;
            GenerateEditorData();            
        }
    }
    public void Reset() {
        myIsDirty= true;
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
    public bool IsValid(int id)                     { return id >= 0 && id < EditorObjects.Count && this[id].InstanceId != -1; }
    public bool IsInvalid(int id)                   { return !IsValid(id); }
    public bool IsValid(WD_EditorObject obj)        { return IsValid(obj.InstanceId); }
    public bool IsSourceValid(WD_EditorObject obj)  { return IsValid(obj.Source); }
    public bool IsParentValid(WD_EditorObject obj)  { return IsValid(obj.ParentId); }
    // ----------------------------------------------------------------------
    public bool IsDirty { get { ProcessUndoRedo(); return myIsDirty; }}
    // ----------------------------------------------------------------------
    public WD_EditorObject this[int id] {
        get { return EditorObjects[id]; }
        set {
            ProcessUndoRedo();
            if(value.InstanceId != id) Debug.LogError("Trying to add EditorObject at wrong index.");
            EditorObjects[id]= value;
            if(TreeCache.IsValid(id)) TreeCache.UpdateInstance(value);
            else                      TreeCache.CreateInstance(value);            
            SetDirty(EditorObjects[id]);
        }
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetParent(WD_EditorObject obj) { return obj.IsParentValid ? EditorObjects[obj.ParentId] : null; }
    public WD_EditorObject GetSource(WD_EditorObject obj) { return obj.IsSourceValid ? EditorObjects[obj.Source] : null; }
    // ----------------------------------------------------------------------
    public void SetDirty(WD_EditorObject obj) {
        myIsDirty= true;
        if(obj.IsPort) { GetParent(obj).IsDirty= true; }
        obj.IsDirty= true;        
    }

    // ======================================================================
    // Storage Update
    // ----------------------------------------------------------------------
    public void Update() {
        ProcessUndoRedo();
        if(!myIsDirty) return;
        myIsDirty= false;
//        Debug.Log("Storage has something to update");

        // Perform layout of modified nodes.
        ForEachRecursiveDepthLast(EditorObjects[0],
            obj=> {
                if(obj.IsDirty) {
                    Layout(obj);
                }
            }
        );            
    }

    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    public void RegisterUndo(string message= "WarpDrive") {
        Undo.RegisterUndo(Storage, message);
        Storage.UndoRedoId= ++UndoRedoId;        
        EditorUtility.SetDirty(Storage);
    }
    // ----------------------------------------------------------------------
    void ProcessUndoRedo() {
        // Regenerate internal structures if undo/redo was performed.
        if(Storage.UndoRedoId != UndoRedoId) {
            SynchronizeAfterUndoRedo();
        }        
    }
    // ----------------------------------------------------------------------
    void SynchronizeAfterUndoRedo() {
//        Debug.Log("Undo/Redo was performed");
        GenerateEditorData();
        foreach(var obj in EditorObjects) {
            if(IsValid(obj.InstanceId)) {
                SetDirty(obj);
            }
            else {
                obj.IsDirty= false;
            }
        }
        Storage.UndoRedoId= ++UndoRedoId;        
    }
    
    // ======================================================================
    // Editor Object Creation/Destruction
    // ----------------------------------------------------------------------
    int GetNextAvailableId() {
        // Covers Undo?redo for all creation operation
        ProcessUndoRedo();
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
    public WD_EditorObject CreateBehaviour() {
        // Create the function node.
        int id= GetNextAvailableId();
        // Validate that behaviour is at the root.
        if(id != 0) {
            Debug.LogError("Behaviour MUST be the root object !!!");
        }
        // Create new EditorObject
        this[id]= new WD_EditorObject(id, null, typeof(WD_Behaviour), -1, WD_ObjectTypeEnum.Behaviour, new Rect(0,0,0,0));
        return this[id];
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
        this[id]= new WD_EditorObject(id, name, typeof(WD_Module), parentId, WD_ObjectTypeEnum.Module, localPos);
        return this[id];
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
        this[id]= new WD_EditorObject(id, name, typeof(WD_StateChart), parentId, WD_ObjectTypeEnum.StateChart, localPos);
        return this[id];
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
        this[id]= new WD_EditorObject(id, name, typeof(WD_State), parentId, WD_ObjectTypeEnum.State, localPos);
        return this[id];
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
        this[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Class, localPos);
        this[id].IconGUID= WD_Graphics.IconPathToGUID(desc.IconPath, this);
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
                this[methodId]= new WD_EditorObject(methodId, desc.MethodNames[i], desc.ClassType, id, WD_ObjectTypeEnum.Function, new Rect(0,0,0,0));
                this[methodId].IconGUID= WD_Graphics.IconPathToGUID(desc.MethodIcons[i], this);
            }
            for(int p= 0; p < desc.ParameterNames[i].Length; ++p) {
                WD_ObjectTypeEnum portType= desc.ParameterInOuts[i][p] ? WD_ObjectTypeEnum.OutStaticModulePort : WD_ObjectTypeEnum.InStaticModulePort;
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
                WD_EditorObject classPort= CreatePort(desc.ReturnNames[i], id, desc.ReturnTypes[i], WD_ObjectTypeEnum.OutStaticModulePort);
                if(nbOfMethodsToShow > 1) {
                    WD_EditorObject funcPort= CreatePort(desc.ReturnNames[i], methodId, desc.ReturnTypes[i], WD_ObjectTypeEnum.OutFunctionPort);
                    SetSource(classPort, funcPort);                    
                }
            }
        }
        return this[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_FunctionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Function, localPos);
        this[id].IconGUID= WD_Graphics.IconPathToGUID(desc.IconPath, this);
        // Create input/output ports.
        for(int i= 0; i < desc.ParameterNames.Length; ++i) {
            WD_ObjectTypeEnum portType= desc.ParameterInOuts[i] ? WD_ObjectTypeEnum.OutFunctionPort : WD_ObjectTypeEnum.InFunctionPort;
            CreatePort(desc.ParameterNames[i], id, desc.ParameterTypes[i], portType);
        }
        if(desc.ReturnType != null) {
            CreatePort(desc.ReturnName, id, desc.ReturnType, WD_ObjectTypeEnum.OutFunctionPort);
        }
        return this[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateFunction(int parentId, Vector2 initialPos, WD_ConversionDesc desc) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new WD_EditorObject(id, desc.Name, desc.ClassType, parentId, WD_ObjectTypeEnum.Conversion, localPos);
        this[id].IconGUID= WD_Graphics.IconPathToGUID(desc.IconPath, this);
        // Create input/output ports.
        CreatePort(desc.FromType.Name, id, desc.FromType, WD_ObjectTypeEnum.InFunctionPort);
        CreatePort(desc.ToType.Name,   id, desc.ToType,   WD_ObjectTypeEnum.OutFunctionPort);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreatePort(string name, int parentId, Type valueType, WD_ObjectTypeEnum portType) {
        int id= GetNextAvailableId();
        WD_EditorObject port= this[id]= new WD_EditorObject(id, name, valueType, parentId, portType, new Rect(0,0,0,0));
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
        ProcessUndoRedo();
        DestroyInstanceInternal(id);
        // Cleanup disconnected module and state ports.
        CleanupAfterDestroyOrDisconnect();
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(WD_EditorObject eObj) {
        if(eObj == null) return;
        DestroyInstance(eObj.InstanceId);
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(int id) {
        if(IsInvalid(id)) {
            Debug.LogError("Trying the delete a non-existing EditorObject with id= "+id);
        }
        // Also destroy transition exit/entry module when removing transitions.
        WD_EditorObject toDestroy= EditorObjects[id];
        if(toDestroy.IsStatePort) {
            WD_EditorObject entryModule= GetTransitionEntryModule(toDestroy);
            WD_EditorObject exitModule= GetTransitionExitModule(toDestroy);
            DestroyInstanceInternal(entryModule);            
            DestroyInstanceInternal(exitModule);
        }
        // Disconnect ports linking to this port.
        ExecuteIf(toDestroy, WD.IsPort, _=> DisconnectPortInternal(toDestroy));
        // Remove all children first.
        while(TreeCache[id].Children.Count != 0) {
            DestroyInstanceInternal(TreeCache[id].Children[0]);
        }
        TreeCache.DestroyInstance(id);
        // Set the parent dirty to force a relayout.
        if(IsValid(toDestroy.ParentId)) SetDirty(GetParent(toDestroy));
        toDestroy.Reset();
        myIsDirty= true;
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(WD_EditorObject toDestroy) {
        if(toDestroy == null || IsInvalid(toDestroy.InstanceId)) return;
        DestroyInstanceInternal(toDestroy.InstanceId);
    }
    // ----------------------------------------------------------------------
    void CleanupAfterDestroyOrDisconnect() {
        ForEach(
            port=> {
                if((port.IsStatePort || port.IsDynamicModulePort) && IsPortDisconnected(port)) {
                    DestroyInstanceInternal(port.InstanceId);
                }
            }
        );        
    }
    
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(WD_EditorObject eObj) {
        if(eObj.IsHidden) return false;
        if(IsInvalid(eObj.ParentId)) return true;
        WD_EditorObject parent= GetParent(eObj);
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
        SetDirty(eObj);
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Unfold(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Unfold();
        SetDirty(eObj);
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMinimized(WD_EditorObject eObj) {
        return eObj.IsMinimized;
    }
    public void Minimize(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Minimize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Minimize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) SetDirty(GetParent(eObj));
    }
    public void Minimize(int id) { if(IsValid(id)) Minimize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Maximize(WD_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Maximize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Maximize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) SetDirty(GetParent(eObj));
    }
    public void Maximize(int id) { if(IsValid(id)) Maximize(EditorObjects[id]); }
    


    // ======================================================================
    // Port Connectivity
    // ----------------------------------------------------------------------
    public void SetSource(WD_EditorObject obj, WD_EditorObject src) {
        int prevSource= obj.Source;
        obj.Source= src == null ? -1 : src.InstanceId;
        if(prevSource != obj.Source) CleanupAfterDestroyOrDisconnect();
    }
    // ----------------------------------------------------------------------
    public void SetSource(WD_EditorObject inPort, WD_EditorObject outPort, WD_ConversionDesc convDesc) {
        Rect inPos= GetPosition(inPort);
        Rect outPos= GetPosition(outPort);
        Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
        int grandParentId= GetParent(inPort).ParentId;
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
        DisconnectPortInternal(port);
        CleanupAfterDestroyOrDisconnect();
    }
    // ----------------------------------------------------------------------
    void DisconnectPortInternal(WD_EditorObject port) {
        SetSource(port, null);
        Prelude.forEach(p=> SetSource(p, null), FindConnectedPorts(port));        
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject[] FindConnectedPorts(WD_EditorObject port) {
        return Filter(p=> p.IsPort && p.Source == port.InstanceId).ToArray();
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject FindAConnectedPort(WD_EditorObject port) {
        WD_EditorObject[] connectedPorts= FindConnectedPorts(port);
        return connectedPorts.Length != 0 ? connectedPorts[0] : null;
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
    // ----------------------------------------------------------------------
    public bool IsBridgeConnection(WD_EditorObject p1, WD_EditorObject p2) {
        return (p1.IsDataPort && p2.IsStatePort) || (p1.IsStatePort && p2.IsDataPort);
    }
    // ----------------------------------------------------------------------
    public bool IsInBridgeConnection(WD_EditorObject port) {
        WD_EditorObject otherPort= GetOtherBridgePort(port);
        return otherPort != null;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetOtherBridgePort(WD_EditorObject port) {
        if(IsSourceValid(port)) {
            WD_EditorObject sourcePort= GetSource(port);
            if(IsBridgeConnection(port, sourcePort)) {
                return sourcePort;
            }
        }
        WD_EditorObject remotePort= FindAConnectedPort(port);
        if(remotePort == null) return null;
        return IsBridgeConnection(port, remotePort) ? remotePort : null;
    }
    

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
        SetDirty(obj);
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
        float titleWidth  = WD_EditorConfig.GetNodeWidth(node.Name)+WD_EditorConfig.ExtraIconWidth;
        float leftMargin  = ComputeLeftMargin(node);
        float rightMargin = ComputeRightMargin(node);
        float width       = 2.0f*WD_EditorConfig.GutterSize + Mathf.Max(titleWidth, leftMargin + rightMargin + childRect.width);

        // Process case without child nodes
        Rect position= GetPosition(node);
        if(Math3D.IsZero(childRect.width) || Math3D.IsZero(childRect.height)) {
            // Compute needed height.
            WD_EditorObject[] leftPorts= GetLeftPorts(node);
            WD_EditorObject[] rightPorts= GetRightPorts(node);
            int nbOfPorts= leftPorts.Length > rightPorts.Length ? leftPorts.Length : rightPorts.Length;
            float height= Mathf.Max(WD_EditorConfig.NodeTitleHeight+nbOfPorts*WD_EditorConfig.MinimumPortSeparation, WD_EditorConfig.MinimumNodeHeight);                                
            
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
            float neededChildXOffset= WD_EditorConfig.GutterSize+leftMargin;
            float neededChildYOffset= WD_EditorConfig.GutterSize+WD_EditorConfig.NodeTitleHeight;
            if(Math3D.IsNotEqual(neededChildXOffset, childRect.x) ||
               Math3D.IsNotEqual(neededChildYOffset, childRect.y)) {
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
        if(Math3D.IsNotZero(_delta)) {
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
        WD_EditorObject parentNode= GetParent(node);
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
                    childRect= Math3D.Merge(childRect, child.LocalPosition);
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
            port=> {
                if(!port.IsStatePort) {
                    Vector2 labelSize= WD_EditorConfig.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+WD_EditorConfig.PortSize;
                    if(LeftMargin < nameSize) LeftMargin= nameSize;
                }
            }
        );
        return LeftMargin;
    }
    // ----------------------------------------------------------------------
    // Returns the inner right margin.
    float ComputeRightMargin(WD_EditorObject node) {
        float RightMargin= 0;
        ForEachRightPort(node,
            port => {
                if(!port.IsStatePort) {
                    Vector2 labelSize= WD_EditorConfig.GetPortLabelSize(port.Name);
                    float nameSize= labelSize.x+WD_EditorConfig.PortSize;
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
        Rect position= GetPosition(node);
        
        // Relayout top ports.
        WD_EditorObject[] ports= SortTopPorts(node);
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
            float topOffset= WD_EditorConfig.NodeTitleHeight;
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
            float topOffset= WD_EditorConfig.NodeTitleHeight;
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
    bool IsChildrenInSameOrder(WD_EditorObject node, WD_EditorObject[] orderedChildren) {
        return TreeCache[node.InstanceId].IsChildrenInSameOrder(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    void ReorderChildren(WD_EditorObject node, WD_EditorObject[] orderedChildren) {
        TreeCache[node.InstanceId].ReorderChildren(Prelude.map(c=> c.InstanceId, orderedChildren));
    }
    // ----------------------------------------------------------------------
    void SetAllConnectedPortsDirty(WD_EditorObject[] ports) {
        foreach(var p in ports) {
            if(IsValid(p.Source)) SetDirty(p);
            WD_EditorObject[] connectedPorts= FindConnectedPorts(p);
            foreach(var cp in connectedPorts) SetDirty(cp);
        }        
    }
    // ----------------------------------------------------------------------
    Vector2 GetAverageConnectionPosition(WD_EditorObject port) {
        WD_EditorObject[] connectedPorts= FindConnectedPorts(port);
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
    WD_EditorObject[] SortTopPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetTopPorts(node);
        Vector2[] portsPos= Prelude.map(p=> Math3D.ToVector2(GetPosition(p)), ports);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(p=> p.x, connectedPos); 
        float[] secondKeys= Prelude.zipWith((p,cp)=> p.x < cp.x ? cp.y : -cp.y, portsPos, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortBottomPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetBottomPorts(node);
        Vector2[] portsPos= Prelude.map(p=> Math3D.ToVector2(GetPosition(p)), ports);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(p=> p.x, connectedPos); 
        float[] secondKeys= Prelude.zipWith((p,cp)=> p.x < cp.x ? -cp.y : cp.y, portsPos, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortLeftPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetLeftPorts(node);                             
        Vector2[] portsPos= Prelude.map(p=> Math3D.ToVector2(GetPosition(p)), ports);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(p=> p.y, connectedPos); 
        float[] secondKeys= Prelude.zipWith((p,cp)=> p.y < cp.y ? cp.x : -cp.x, portsPos, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    WD_EditorObject[] SortRightPorts(WD_EditorObject node) {
        WD_EditorObject[] ports= GetRightPorts(node);
        Vector2[] portsPos= Prelude.map(p=> Math3D.ToVector2(GetPosition(p)), ports);
        Vector2[] connectedPos= Prelude.map(p=> GetAverageConnectionPosition(p), ports);
        float[] firstKeys = Prelude.map(p=> p.y, connectedPos); 
        float[] secondKeys= Prelude.zipWith((p,cp)=> p.y < cp.y ? -cp.x : cp.x, portsPos, connectedPos);
        return SortPorts(ports, firstKeys, secondKeys);
    }
    // ----------------------------------------------------------------------
    // Sorts the given port according to their relative positions.
    WD_EditorObject[] SortPorts(WD_EditorObject[] ports, float[] keys1, float[] keys2) {
        for(int i= 0; i < ports.Length-1; ++i) {
            for(int j= i+1; j < ports.Length; ++j) {
                if(Math3D.IsGreater(keys1[i], keys1[j])) {
                    Prelude.exchange(ref ports[i], ref ports[j]);
                    Prelude.exchange(ref keys1[i], ref keys1[j]);
                    Prelude.exchange(ref keys2[i], ref keys2[j]);
                } else if(Math3D.IsEqual(keys1[i], keys1[j])) {                
                    if(keys2[i] > keys2[j]) {
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
    public WD_EditorObject[] GetTopPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachTopPort(node, ports.Add);
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the bottom edge.
    public WD_EditorObject[] GetBottomPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachBottomPort(node, ports.Add);
        return ports.ToArray();
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the left edge.
    public WD_EditorObject[] GetLeftPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachLeftPort(node, ports.Add);
        return ports.ToArray();        
    }

    // ----------------------------------------------------------------------
    // Returns all ports position on the right edge.
    public WD_EditorObject[] GetRightPorts(WD_EditorObject node) {
        List<WD_EditorObject> ports= new List<WD_EditorObject>();
        ForEachRightPort(node, ports.Add);
        return ports.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns the number of ports on the top edge.
    public int GetNbOfTopPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachTopPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the bottom edge.
    public int GetNbOfBottomPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachBottomPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the left edge.
    public int GetNbOfLeftPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachLeftPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    // Returns the number of ports on the right edge.
    public int GetNbOfRightPorts(WD_EditorObject node) {
        int nbOfPorts= 0;
        ForEachRightPort(node, _=> ++nbOfPorts);
        return nbOfPorts;
    }

    // ----------------------------------------------------------------------
    public void ForEachChildPort(WD_EditorObject node, Action<WD_EditorObject> action) {
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, action));
    }
    // ----------------------------------------------------------------------
    public bool ForEachChildPort(WD_EditorObject node, Func<WD_EditorObject,bool> fnc) {
        return ForEachChild(node, child=> child.IsPort ? fnc(child) : false);
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
        ResolveCollision(GetParent(node), _delta);
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
    public bool DoesCollide(WD_EditorObject node, WD_EditorObject otherNode) {
        return Math3D.DoesCollide(GetPosition(node), GetPosition(otherNode));
    }

    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithGutter(WD_EditorObject node, WD_EditorObject otherNode) {
        return Math3D.DoesCollide(RectWithGutter(GetPosition(node)), GetPosition(otherNode));
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
        // Extend the node range to include the ports.
        float portSize= WD_EditorConfig.PortSize;
        Rect nodePos= GetPosition(node);
        nodePos.x-= portSize;
        nodePos.y-= portSize;
        nodePos.width+= 2f*portSize;
        nodePos.height+= 2f*portSize;
        return nodePos.Contains(point);
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
        // Don't update port edges for a transition bridge.  Leave the update
        // to the corresponding data connection & transition connection.
        if(IsBridgeConnection(p1,p2)) return;
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
            WD_EditorObject parent= null;
            WD_EditorObject child= null;
            WD_EditorObject pPort= null;
            WD_EditorObject cPort= null;
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
                        pPort.Edge= WD_EditorObject.EdgeEnum.Left;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Left;
                    } else {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Right;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Right;                        
                    }
                } else {
                    if(childLocalPos.x < dy) {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Left;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Left;                        
                    } else {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Bottom;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Bottom;                        
                    }
                }
            } else {
                if(dx < dy) {
                    if(childLocalPos.y < dx) {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Top;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Top;
                    } else {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Right;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Right;                        
                    }
                } else {
                    if(childLocalPos.y < dy) {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Top;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Top;                        
                    } else {
                        pPort.Edge= WD_EditorObject.EdgeEnum.Bottom;
                        cPort.Edge= WD_EditorObject.EdgeEnum.Bottom;                        
                    }
                }                
            }
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
        WD_EditorObject parentNode= GetParent(port);
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


}
