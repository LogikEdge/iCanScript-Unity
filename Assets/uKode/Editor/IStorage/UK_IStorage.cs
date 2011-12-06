using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
            bool            myIsDirty    = true;
    public  UK_Storage      Storage      = null;
            UK_TreeCache    TreeCache    = null;
            int             UndoRedoId   = 0;
            bool            CleanupNeeded= true;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public UK_IStorage(UK_Storage storage) {
        Init(storage);
    }
    public void Init(UK_Storage storage) {
        if(Storage != storage) {
            myIsDirty= true;
            Storage= storage;
            GenerateInternalData();            
        }
    }
    public void Reset() {
        myIsDirty= true;
        Storage= null;
        TreeCache= null;
    }
    // ----------------------------------------------------------------------
    void GenerateInternalData() {
        GenerateEditorData();
        GenerateRuntimeCode();
    }
    // ----------------------------------------------------------------------
    void GenerateEditorData() {
        TreeCache= new UK_TreeCache();
        ForEach(obj=> TreeCache.CreateInstance(obj));
    }
    
    
    // ======================================================================
    // Basic Accessors
    // ----------------------------------------------------------------------
    public List<UK_EditorObject>    EditorObjects { get { return Storage.EditorObjects; }}
    public UK_UserPreferences       Preferences   { get { return Storage.Preferences; }}
    public List<UnityEngine.Object> UnityObjects  { get { return Storage.UnityObjects; }}
    // ----------------------------------------------------------------------
    public bool IsValid(int id)                     { return id >= 0 && id < EditorObjects.Count && this[id].InstanceId != -1; }
    public bool IsInvalid(int id)                   { return !IsValid(id); }
    public bool IsValid(UK_EditorObject obj)        { return IsValid(obj.InstanceId); }
    public bool IsSourceValid(UK_EditorObject obj)  { return IsValid(obj.Source); }
    public bool IsParentValid(UK_EditorObject obj)  { return IsValid(obj.ParentId); }
    // ----------------------------------------------------------------------
    public bool IsDirty { get { ProcessUndoRedo(); return myIsDirty; }}
    // ----------------------------------------------------------------------
    public UK_EditorObject this[int id] {
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
    public UK_EditorObject GetParent(UK_EditorObject obj)        { return obj.IsParentValid ? EditorObjects[obj.ParentId] : null; }
    public UK_EditorObject GetSource(UK_EditorObject obj)        { return obj.IsSourceValid ? EditorObjects[obj.Source] : null; }
    public object          GetRuntimeObject(UK_EditorObject obj) { return IsValid(obj) ? TreeCache[obj.InstanceId].RuntimeObject : null; }
    public float           GetAnimTime(UK_EditorObject obj)      { return IsValid(obj) ? Time.realtimeSinceStartup-TreeCache[obj.InstanceId].AnimationTime : 0; }
    public void            StartAnimTimer(UK_EditorObject obj)   { if(IsValid(obj)) TreeCache[obj.InstanceId].AnimationTime= Time.realtimeSinceStartup; }
    public Rect            GetDisplayPosition(UK_EditorObject obj)           { return IsValid(obj) ? TreeCache[obj.InstanceId].DisplayPosition : default(Rect); }
    public void            SetDisplayPosition(UK_EditorObject obj, Rect pos) { if(IsValid(obj)) TreeCache[obj.InstanceId].DisplayPosition= pos; }
    // ----------------------------------------------------------------------
    public void SetDirty(UK_EditorObject obj) {
        myIsDirty= true;
        if(obj.IsPort) { GetParent(obj).IsDirty= true; }
        obj.IsDirty= true;        
    }

    // ======================================================================
    // Storage Update
    // ----------------------------------------------------------------------
    public void Update() {
        ProcessUndoRedo();
        if(!myIsDirty) {
            if(CleanupNeeded) CleanupNeeded= Cleanup();
            return;
        }
        CleanupNeeded= true;
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
    // ----------------------------------------------------------------------
    public bool Cleanup() {
        bool modified= false;
        ForEach(
            obj=> {
                // Cleanup disconnected dynamic state or module ports.
                if((obj.IsStatePort || obj.IsDynamicModulePort) && IsPortDisconnected(obj)) {
                    DestroyInstanceInternal(obj.InstanceId);
                    modified= true;
                }
            }
        );        
        return modified;
    }
    
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    public void RegisterUndo(string message= "uCode") {
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
    public UK_EditorObject CreateBehaviour() {
        // Create the function node.
        int id= GetNextAvailableId();
        // Validate that behaviour is at the root.
        if(id != 0) {
            Debug.LogError("Behaviour MUST be the root object !!!");
        }
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, null, typeof(UK_Behaviour), -1, UK_ObjectTypeEnum.Behaviour, new Rect(0,0,0,0));
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateModuleLibrary() {
        // Validate that a library can only be create at the root.
        if(EditorObjects.Count != 0) {
            Debug.LogError("Module Library MUST be the root object !!!");
        }
        return CreateModule(-1, Vector2.zero, "Module Library");
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateModule(int parentId, Vector2 initialPos, string name= "", UK_ObjectTypeEnum objectType= UK_ObjectTypeEnum.Module) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, name, typeof(UK_Module), parentId, objectType, localPos);
        this[id].IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.ModuleIcon, this);
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc();
        rtDesc.ObjectType= UK_ObjectTypeEnum.Module;
        rtDesc.Company= UK_EditorStrings.Company;
        rtDesc.Package= UK_EditorStrings.DefaultPackage;
        rtDesc.DisplayName= name;
        rtDesc.ClassType= typeof(UK_Module);
        this[id].RuntimeArchive= rtDesc.Encode(id);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateStateChartLibrary() {
        // Validate that a library can only be create at the root.
        if(EditorObjects.Count != 0) {
            Debug.LogError("Module Library MUST be the root object !!!");
        }
        return CreateStateChart(-1, Vector2.zero, "StateChart Library");
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateStateChart(int parentId, Vector2 initialPos, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, name, typeof(UK_StateChart), parentId, UK_ObjectTypeEnum.StateChart, localPos);
        // Create runtime descriptor.
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc();
        rtDesc.ObjectType= UK_ObjectTypeEnum.StateChart;
        rtDesc.Company= UK_EditorStrings.Company;
        rtDesc.Package= UK_EditorStrings.DefaultPackage;
        rtDesc.DisplayName= name;
        rtDesc.ClassType= typeof(UK_StateChart);
        this[id].RuntimeArchive= rtDesc.Encode(id);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateState(int parentId, Vector2 initialPos, string name= "") {
        // Validate that we have a good parent.
        UK_EditorObject parent= EditorObjects[parentId];
        if(parent == null || (!WD.IsStateChart(parent) && !WD.IsState(parent))) {
            Debug.LogError("State must be created as a child of StateChart or State.");
        }
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, name, typeof(UK_State), parentId, UK_ObjectTypeEnum.State, localPos);
        // Create runtime descriptor.
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc();
        rtDesc.ObjectType= UK_ObjectTypeEnum.State;
        rtDesc.Company= UK_EditorStrings.Company;
        rtDesc.Package= UK_EditorStrings.DefaultPackage;
        rtDesc.DisplayName= name;
        rtDesc.ClassType= typeof(UK_State);
        this[id].RuntimeArchive= rtDesc.Encode(id);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateMethod(int parentId, Vector2 initialPos, UK_ReflectionDesc desc) {
        return desc.ObjectType == UK_ObjectTypeEnum.InstanceMethod ?
                    CreateInstanceMethod(parentId, initialPos, desc) : 
                    CreateStaticMethod(parentId, initialPos, desc);
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateStaticMethod(int parentId, Vector2 initialPos, UK_ReflectionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, localPos);
        this[id].RuntimeArchive= desc.Encode(id);
        this[id].IconGUID= UK_Graphics.IconPathToGUID(desc.IconPath, this);
        if(this[id].IconGUID == null && desc.ObjectType == UK_ObjectTypeEnum.StaticMethod) {
            this[id].IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.MethodIcon, this);
        }
        
        // Create input/output ports.
        UK_RuntimeDesc  rtDesc= desc.RuntimeDesc;
        for(int i= 0; i < rtDesc.PortNames.Length; ++i) {
            if(rtDesc.PortTypes[i] != typeof(void)) {
                UK_ObjectTypeEnum portType= rtDesc.PortIsOuts[i] ? UK_ObjectTypeEnum.OutFunctionPort : UK_ObjectTypeEnum.InFunctionPort;
                UK_EditorObject port= CreatePort(rtDesc.PortNames[i], id, rtDesc.PortTypes[i], portType);
                port.PortIndex= i;                
            }
        }
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateInstanceMethod(int parentId, Vector2 initialPos, UK_ReflectionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new UK_EditorObject(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, localPos);
        this[id].RuntimeArchive= desc.Encode(id);
        this[id].IconGUID= UK_Graphics.IconPathToGUID(desc.IconPath, this);
        if(this[id].IconGUID == null && desc.ObjectType == UK_ObjectTypeEnum.StaticMethod) {
            this[id].IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.MethodIcon, this);
        }
        
        // Create input/output ports.
        UK_RuntimeDesc  rtDesc= desc.RuntimeDesc;
        for(int portIdx= 0; portIdx < rtDesc.PortNames.Length; ++portIdx) {
            if(rtDesc.PortTypes[portIdx] != typeof(void)) {
                UK_ObjectTypeEnum portType= rtDesc.PortIsOuts[portIdx] ? UK_ObjectTypeEnum.OutFunctionPort : UK_ObjectTypeEnum.InFunctionPort;
                UK_EditorObject port= CreatePort(rtDesc.PortNames[portIdx], id, rtDesc.PortTypes[portIdx], portType);
                port.PortIndex= portIdx;                
            }
        }
        return this[id];
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreatePort(string name, int parentId, Type valueType, UK_ObjectTypeEnum portType) {
        int id= GetNextAvailableId();
        UK_EditorObject port= this[id]= new UK_EditorObject(id, name, valueType, parentId, portType, new Rect(0,0,0,0));
        // Reajust data port position 
        if(port.IsDataPort && !port.IsEnablePort) {
            UK_EditorObject parent= EditorObjects[port.ParentId];
            if(port.IsInputPort) {
                int nbOfPorts= GetNbOfLeftPorts(parent);
                port.LocalPosition= new Rect(0, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);
            } else {
                int nbOfPorts= GetNbOfRightPorts(parent);
                port.LocalPosition= new Rect(parent.LocalPosition.width, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);                
            }
        }
        if(GetParent(port).IsModule) { AddPortToModule(port); }
        return EditorObjects[id];        
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        ProcessUndoRedo();
        DestroyInstanceInternal(id);
        // Cleanup disconnected module and state ports.
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(UK_EditorObject eObj) {
        if(eObj == null) return;
        DestroyInstance(eObj.InstanceId);
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(int id) {
        if(IsInvalid(id)) {
            Debug.LogError("Trying the delete a non-existing EditorObject with id= "+id);
        }
        // Also destroy transition exit/entry module when removing transitions.
        UK_EditorObject toDestroy= EditorObjects[id];
        if(toDestroy.IsInStatePort && IsValid(toDestroy.Source)) {
            DestroyInstanceInternal(GetSource(toDestroy));
            return;
        }
        if(toDestroy.IsOutStatePort) {
            UK_EditorObject actionModule= null;
            UK_EditorObject guardModule= GetTransitionGuardAndAction(toDestroy, out actionModule);
            DestroyInstanceInternal(guardModule);
            if(actionModule != null) DestroyInstanceInternal(actionModule);
        }
        // Disconnect ports linking to this port.
        ExecuteIf(toDestroy, WD.IsPort, _=> DisconnectPort(toDestroy));
        // Update modules runtime data when removing a module port.
        if(toDestroy.IsModulePort && GetParent(toDestroy).IsModule) RemovePortFromModule(toDestroy);
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
    void DestroyInstanceInternal(UK_EditorObject toDestroy) {
        if(toDestroy == null || IsInvalid(toDestroy.InstanceId)) return;
        DestroyInstanceInternal(toDestroy.InstanceId);
    }
    
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(UK_EditorObject eObj) {
        if(eObj.IsHidden) return false;
        if(IsInvalid(eObj.ParentId)) return true;
        UK_EditorObject parent= GetParent(eObj);
        if(eObj.IsNode && (parent.IsFolded || parent.IsMinimized)) return false;
        return IsVisible(parent);
    }
    public bool IsVisible(int id) { return IsInvalid(id) ? false : IsVisible(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsFolded(UK_EditorObject eObj) { return eObj.IsFolded; }
    // ----------------------------------------------------------------------
    public void Fold(UK_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Fold();
        SetDirty(eObj);
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Unfold(UK_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Unfold();
        SetDirty(eObj);
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMinimized(UK_EditorObject eObj) {
        return eObj.IsMinimized;
    }
    public void Minimize(UK_EditorObject eObj) {
        if(!eObj.IsNode) return;
        if(ShouldHideOnMinimize(eObj)) { Hide(eObj); return; }
        eObj.Minimize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Minimize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            SetDirty(GetParent(eObj));
        }
    }
    public void Minimize(int id) { if(IsValid(id)) Minimize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Maximize(UK_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Maximize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Maximize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            UK_EditorObject parent= GetParent(eObj);
            SetDirty(parent);
        }
    }
    public void Maximize(int id) { if(IsValid(id)) Maximize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsHidden(UK_EditorObject eObj) {
        return eObj.IsHidden;
    }
    public void Hide(UK_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Hide();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Hide(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) SetDirty(GetParent(eObj));
    }
    public void Hide(int id) { if(IsValid(id)) Hide(EditorObjects[id]); }
    


    // ======================================================================
    // Port Connectivity
    // ----------------------------------------------------------------------
    public void SetSource(UK_EditorObject obj, UK_EditorObject src) {
        obj.Source= src == null ? -1 : src.InstanceId;
        SetDirty(obj);
    }
    // ----------------------------------------------------------------------
    public void SetSource(UK_EditorObject inPort, UK_EditorObject outPort, UK_ReflectionDesc convDesc) {
        Rect inPos= GetPosition(inPort);
        Rect outPos= GetPosition(outPort);
        Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
        int grandParentId= GetParent(inPort).ParentId;
        UK_EditorObject conv= CreateMethod(grandParentId, convPos, convDesc);
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
    public void DisconnectPort(UK_EditorObject port) {
        SetSource(port, null);
        Prelude.forEach(p=> SetSource(p, null), FindConnectedPorts(port));        
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject[] FindConnectedPorts(UK_EditorObject port) {
        return Filter(p=> p.IsPort && p.Source == port.InstanceId).ToArray();
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject FindAConnectedPort(UK_EditorObject port) {
        UK_EditorObject[] connectedPorts= FindConnectedPorts(port);
        return connectedPorts.Length != 0 ? connectedPorts[0] : null;
    }
    // ----------------------------------------------------------------------
    bool IsPortConnected(UK_EditorObject port) {
        if(port.Source != -1) return true;
        foreach(var obj in EditorObjects) {
            if(obj.IsValid && obj.IsPort && obj.Source == port.InstanceId) return true;
        }
        return false;
    }
    bool IsPortDisconnected(UK_EditorObject port) { return !IsPortConnected(port); }
    // ----------------------------------------------------------------------
    public bool IsBridgeConnection(UK_EditorObject p1, UK_EditorObject p2) {
        return (p1.IsDataPort && p2.IsStatePort) || (p1.IsStatePort && p2.IsDataPort);
    }
    // ----------------------------------------------------------------------
    public bool IsInBridgeConnection(UK_EditorObject port) {
        UK_EditorObject otherPort= GetOtherBridgePort(port);
        return otherPort != null;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetOtherBridgePort(UK_EditorObject port) {
        if(IsSourceValid(port)) {
            UK_EditorObject sourcePort= GetSource(port);
            if(IsBridgeConnection(port, sourcePort)) {
                return sourcePort;
            }
        }
        UK_EditorObject remotePort= FindAConnectedPort(port);
        if(remotePort == null) return null;
        return IsBridgeConnection(port, remotePort) ? remotePort : null;
    }
    

    // ======================================================================
    // Object Picking
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public UK_EditorObject GetNodeAt(Vector2 pick) {
        UK_EditorObject foundNode= null;
        FilterWith(
            n=> n.IsNode && IsVisible(n) && IsInside(n, pick) && (foundNode == null || n.LocalPosition.width < foundNode.LocalPosition.width), 
            n=> foundNode= n
        );
        return foundNode;
    }
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public UK_EditorObject GetPortAt(Vector2 pick) {
        UK_EditorObject bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        FilterWith(
            port=> port.IsPort && IsVisible(port),
            port=> {
                Rect tmp= GetPosition(port);
                Vector2 position= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(position, pick);
                if(distance < 1.5f * UK_EditorConfig.PortRadius && distance < bestDistance) {
                    bestDistance= distance;
                    bestPort= port;
                }                                
            } 
        );
        return bestPort;
    }
    // ----------------------------------------------------------------------
    // Returns true if pick is in the titlebar of the node.
    public bool IsInTitleBar(UK_EditorObject node, Vector2 pick) {
        if(!node.IsNode) return false;
        return true;
    }


}
