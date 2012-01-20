using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
            bool            myIsDirty    = true;
    public  iCS_Storage     Storage      = null;
            iCS_TreeCache   TreeCache    = null;
            int             UndoRedoId   = 0;
            bool            CleanupNeeded= true;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_IStorage(iCS_Storage storage) {
        Init(storage);
    }
    public void Init(iCS_Storage storage) {
        if(Storage != storage) {
            myIsDirty= true;
            Storage= storage;
            GenerateEditorData();
            UndoRedoId= Storage.UndoRedoId;          
        }
    }
    public void Reset() {
        myIsDirty= true;
        Storage= null;
        TreeCache= null;
    }
    // ----------------------------------------------------------------------
    void GenerateEditorData() {
        TreeCache= new iCS_TreeCache();
        ForEach(obj=> TreeCache.CreateInstance(obj));
        if(IsValid(0)) {
            Vector2 graphCenter= Math3D.Middle(GetPosition(EditorObjects[0]));
            ForEach(obj=> {
		        // Initialize display position.
                TreeCache[obj.InstanceId].DisplayPosition= new Rect(graphCenter.x,graphCenter.y,0,0);
				// Initialize initial port values.
				if(obj.IsInDataPort) {
					LoadInitialPortValueFromArchive(obj);
				}
            });            
        }
        CleanupUnityObjects();
    }
    
    
    // ======================================================================
    // Basic Accessors
    // ----------------------------------------------------------------------
    public List<iCS_EditorObject>    EditorObjects { get { return Storage.EditorObjects; }}
    public iCS_UserPreferences       Preferences   { get { return Storage.Preferences; }}
    // ----------------------------------------------------------------------
    public bool IsValid(int id)                      { return id >= 0 && id < EditorObjects.Count && this[id].InstanceId != -1; }
    public bool IsInvalid(int id)                    { return !IsValid(id); }
    public bool IsValid(iCS_EditorObject obj)        { return obj != null && IsValid(obj.InstanceId); }
    public bool IsSourceValid(iCS_EditorObject obj)  { return IsValid(obj.Source); }
    public bool IsParentValid(iCS_EditorObject obj)  { return IsValid(obj.ParentId); }
    // ----------------------------------------------------------------------
    public bool IsDirty { get { ProcessUndoRedo(); return myIsDirty; }}
    // ----------------------------------------------------------------------
    public iCS_EditorObject this[int id] {
        get { return EditorObjects[id]; }
        set {
            ProcessUndoRedo();
            if(value.InstanceId != id) Debug.LogError("Trying to add EditorObject at wrong index.");
            EditorObjects[id]= value;
            if(TreeCache.IsValid(id)) TreeCache.UpdateInstance(value);
            else                      TreeCache.CreateInstance(value);            
            SetDirty(EditorObjects[id]);
            iCS_EditorObject parent= GetParent(EditorObjects[id]);
            if(parent != null) SetDirty(parent);
        }
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetParent(iCS_EditorObject obj)        { return Storage.GetParent(obj); }
    public iCS_EditorObject GetSource(iCS_EditorObject obj)        { return Storage.GetSource(obj); }
    public float           GetAnimTime(iCS_EditorObject obj)      { return IsValid(obj) ? Time.realtimeSinceStartup-TreeCache[obj.InstanceId].AnimationTime : 0; }
    public void            StartAnimTimer(iCS_EditorObject obj)   { if(IsValid(obj)) TreeCache[obj.InstanceId].AnimationTime= Time.realtimeSinceStartup; }
    public Rect            GetDisplayPosition(iCS_EditorObject obj)           { return IsValid(obj) ? TreeCache[obj.InstanceId].DisplayPosition : default(Rect); }
    public void            SetDisplayPosition(iCS_EditorObject obj, Rect pos) { if(IsValid(obj)) TreeCache[obj.InstanceId].DisplayPosition= pos; }
    // ----------------------------------------------------------------------
    public object          GetRuntimeObject(iCS_EditorObject obj) {
        iCS_Behaviour bh= Storage as iCS_Behaviour;
        return obj == null || bh == null ? null : bh.GetRuntimeObject(obj.InstanceId);
    }
    // ----------------------------------------------------------------------
    public void SetDirty(iCS_EditorObject obj) {
        myIsDirty= true;
        if(obj.IsPort) { GetParent(obj).IsDirty= true; }
        obj.IsDirty= true;        
    }
    // ----------------------------------------------------------------------
    public void SetParent(iCS_EditorObject edObj, iCS_EditorObject newParent) {
        Rect pos= GetPosition(edObj);
        iCS_EditorObject oldParent= GetParent(edObj);
        edObj.ParentId= newParent.InstanceId;
        TreeCache.UpdateInstance(oldParent);
        TreeCache.UpdateInstance(newParent);
        TreeCache.UpdateInstance(edObj);
        SetPosition(edObj, pos);
        SetDirty(edObj);
        SetDirty(oldParent);
        SetDirty(newParent);
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
    // ----------------------------------------------------------------------
    public void CleanupUnityObjects() {
        Storage.ClearUnityObjects();
        ForEach(
            obj=> {
                if(obj.IsInDataPort && obj.Source == -1 && TreeCache[obj.InstanceId].InitialValue != null) {
                    ArchiveInitialPortValue(obj);
                }
                else {
                    obj.InitialValueArchive= null; 
                }
            }
        );
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
        // Keep a copy of the previous display position.
        List<Rect> displayPositions= new List<Rect>();
        for(int i= 0; i < EditorObjects.Count; ++i) {
            displayPositions.Add(IsValid(i) ? GetDisplayPosition(EditorObjects[i]) : new Rect(0,0,0,0));
        }
        // Rebuild editor data.
        GenerateEditorData();
        // Put back the previous display position
        for(int i= 0; i < displayPositions.Count; ++i) {
            SetDisplayPosition(EditorObjects[i], displayPositions[i]);
        }
        // Set all object dirty.
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
    public iCS_EditorObject CopyFrom(iCS_EditorObject srcObj, iCS_IStorage srcStorage, iCS_EditorObject destParent, Vector2 initialPos) {
        // Create new EditorObject
        Rect parentPos= IsValid(destParent) ? GetPosition(destParent) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        List<Prelude.Tuple<int, int>> xlat= new List<Prelude.Tuple<int, int>>();
        iCS_EditorObject instance= CopyFrom(srcObj, srcStorage, destParent, localPos, xlat);
        ReconnectCopy(srcObj, srcStorage, xlat);
        return instance;
    }
    iCS_EditorObject CopyFrom(iCS_EditorObject srcObj, iCS_IStorage srcStorage, iCS_EditorObject destParent, Rect localPos, List<Prelude.Tuple<int,int>> xlat) {
        // Create new EditorObject
        int id= GetNextAvailableId();
        xlat.Add(new Prelude.Tuple<int,int>(srcObj.InstanceId, id));
        this[id]= iCS_EditorObject.Clone(id, srcObj, destParent, localPos);
        this[id].IconGUID= srcObj.IconGUID;
		RemoveReferenceInitialValues(this[id]);
        srcStorage.ForEachChild(srcObj,
            child=> CopyFrom(child, srcStorage, this[id], child.LocalPosition, xlat)
        );
        return this[id];
    }
    void ReconnectCopy(iCS_EditorObject srcObj, iCS_IStorage srcStorage, List<Prelude.Tuple<int,int>> xlat) {
        srcStorage.ForEachRecursive(srcObj,
            child=> {
                if(child.Source != -1) {
                    int id= -1;
                    int source= -1;
                    foreach(var pair in xlat) {
                        if(pair.Item1 == child.InstanceId) {
                            id= pair.Item2;
                            if(source != -1) break;
                        }
                        if(pair.Item1 == child.Source) {
                            source= pair.Item2;
                            if(id != -1) break;
                        }
                    }
                    if(source != -1) {
                        SetSource(EditorObjects[id], EditorObjects[source]);                        
                    }
                }
            }
        );
    }
    void RemoveReferenceInitialValues(iCS_EditorObject obj) {
		if(!obj.IsNode) return;
		ForEachChildPort(obj,
			child=> {
				if(child.IsDataPort && iCS_Types.IsA<UnityEngine.Object>(child.RuntimeType)) {
					SetInitialPortValue(child, null);
				}
			}
		);
	}
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateBehaviour() {
        // Create the function node.
        int id= GetNextAvailableId();
        // Validate that behaviour is at the root.
        if(id != 0) {
            Debug.LogError("Behaviour MUST be the root object !!!");
        }
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, null, typeof(iCS_Behaviour), -1, iCS_ObjectTypeEnum.Behaviour, new Rect(0,0,0,0));
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateModule(int parentId, Vector2 initialPos, string name= "", iCS_ObjectTypeEnum objectType= iCS_ObjectTypeEnum.Module) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, name, typeof(iCS_Module), parentId, objectType, localPos);
        this[id].IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.ModuleIcon, this);
        TreeCache[id].DisplayPosition= new Rect(initialPos.x,initialPos.y,0,0);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateStateChart(int parentId, Vector2 initialPos, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= IsValid(parentId) ? GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, name, typeof(iCS_StateChart), parentId, iCS_ObjectTypeEnum.StateChart, localPos);
        TreeCache[id].DisplayPosition= new Rect(initialPos.x,initialPos.y,0,0);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateState(int parentId, Vector2 initialPos, string name= "") {
        // Validate that we have a good parent.
        iCS_EditorObject parent= EditorObjects[parentId];
        if(parent == null || (!WD.IsStateChart(parent) && !WD.IsState(parent))) {
            Debug.LogError("State must be created as a child of StateChart or State.");
        }
        // Create the function node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, name, typeof(iCS_State), parentId, iCS_ObjectTypeEnum.State, localPos);
        TreeCache[id].DisplayPosition= new Rect(initialPos.x,initialPos.y,0,0);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateMethod(int parentId, Vector2 initialPos, iCS_ReflectionDesc desc) {
        iCS_EditorObject instance= desc.ObjectType == iCS_ObjectTypeEnum.InstanceMethod ?
                    				CreateInstanceMethod(parentId, initialPos, desc) : 
                    				CreateStaticMethod(parentId, initialPos, desc);

		instance.MethodName= desc.MethodName;
		instance.NbOfParams= desc.ParamTypes != null ? desc.ParamTypes.Length : 0;
		return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateStaticMethod(int parentId, Vector2 initialPos, iCS_ReflectionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, localPos);
        this[id].IconGUID= iCS_Graphics.IconPathToGUID(desc.IconPath, this);
        if(this[id].IconGUID == null && desc.ObjectType == iCS_ObjectTypeEnum.StaticMethod) {
            this[id].IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.MethodIcon, this);
        }
        // Create parameter ports.
		int portIdx= 0;
		iCS_EditorObject port= null;
        for(; portIdx < desc.ParamNames.Length; ++portIdx) {
            if(desc.ParamTypes[portIdx] != typeof(void)) {
                iCS_ObjectTypeEnum portType= desc.ParamIsOuts[portIdx] ? iCS_ObjectTypeEnum.OutFunctionPort : iCS_ObjectTypeEnum.InFunctionPort;
                port= CreatePort(desc.ParamNames[portIdx], id, desc.ParamTypes[portIdx], portType);
                port.PortIndex= portIdx;
                SetInitialPortValue(port, desc.ParamInitialValues[portIdx]);
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFunctionPort);
            port.PortIndex= portIdx++;			
		}
		// Create 'this' ports for constructors.
		if(desc.ObjectType == iCS_ObjectTypeEnum.Constructor) {
			port= CreatePort("this", id, desc.ClassType, iCS_ObjectTypeEnum.OutFunctionPort);
	        port.PortIndex= portIdx;						
		}
        
        TreeCache[id].DisplayPosition= new Rect(initialPos.x,initialPos.y,0,0);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateInstanceMethod(int parentId, Vector2 initialPos, iCS_ReflectionDesc desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= GetPosition(parentId);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        this[id]= new iCS_EditorObject(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, localPos);
        this[id].IconGUID= iCS_Graphics.IconPathToGUID(desc.IconPath, this);
        if(this[id].IconGUID == null && desc.ObjectType == iCS_ObjectTypeEnum.StaticMethod) {
            this[id].IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.MethodIcon, this);
        }        
        // Create parameter ports.
		int portIdx= 0;
		iCS_EditorObject port= null;
        for(; portIdx < desc.ParamNames.Length; ++portIdx) {
            if(desc.ParamTypes[portIdx] != typeof(void)) {
                iCS_ObjectTypeEnum portType= desc.ParamIsOuts[portIdx] ? iCS_ObjectTypeEnum.OutFunctionPort : iCS_ObjectTypeEnum.InFunctionPort;
                port= CreatePort(desc.ParamNames[portIdx], id, desc.ParamTypes[portIdx], portType);
                port.PortIndex= portIdx;                
                SetInitialPortValue(port, desc.ParamInitialValues[portIdx]);
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFunctionPort);
            port.PortIndex= portIdx++;			
		}
		// Create 'this' ports.
        port= CreatePort("this", id, desc.ClassType, iCS_ObjectTypeEnum.InFunctionPort);
        port.PortIndex= portIdx++;			
        port= CreatePort("this", id, desc.ClassType, iCS_ObjectTypeEnum.OutFunctionPort);
        port.PortIndex= portIdx;			

        TreeCache[id].DisplayPosition= new Rect(initialPos.x,initialPos.y,0,0);
        return this[id];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreatePort(string name, int parentId, Type valueType, iCS_ObjectTypeEnum portType) {
        int id= GetNextAvailableId();
        iCS_EditorObject port= this[id]= new iCS_EditorObject(id, name, valueType, parentId, portType, new Rect(0,0,0,0));
        // Reajust data port position 
        if(port.IsDataPort && !port.IsEnablePort) {
            iCS_EditorObject parent= GetParent(port);
            if(port.IsInputPort) {
                int nbOfPorts= GetNbOfLeftPorts(parent);
                port.LocalPosition= new Rect(0, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);
            } else {
                int nbOfPorts= GetNbOfRightPorts(parent);
                port.LocalPosition= new Rect(parent.LocalPosition.width, parent.LocalPosition.height/(nbOfPorts+1), 0, 0);                
            }
        }
        if(GetParent(port).IsModule) { AddPortToModule(port); }
        Rect parentPos= GetPosition(GetParent(port));
        TreeCache[id].DisplayPosition= new Rect(0.5f*(parentPos.x+parentPos.xMax), 0.5f*(parentPos.y+parentPos.yMax),0,0);
        return EditorObjects[id];        
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        ProcessUndoRedo();
        DestroyInstanceInternal(id);
        // Cleanup disconnected module and state ports.
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(iCS_EditorObject eObj) {
        if(eObj == null) return;
        DestroyInstance(eObj.InstanceId);
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(int id) {
        if(IsInvalid(id)) {
            Debug.LogError("Trying the delete a non-existing EditorObject with id= "+id);
        }
        // Also destroy transition exit/entry module when removing transitions.
        iCS_EditorObject toDestroy= EditorObjects[id];
        if(toDestroy.IsInStatePort && IsValid(toDestroy.Source)) {
            DestroyInstanceInternal(GetSource(toDestroy));
            return;
        }
        if(toDestroy.IsOutStatePort) {
            iCS_EditorObject actionModule= null;
            iCS_EditorObject guardModule= GetTransitionGuardAndAction(toDestroy, out actionModule);
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
    void DestroyInstanceInternal(iCS_EditorObject toDestroy) {
        if(toDestroy == null || IsInvalid(toDestroy.InstanceId)) return;
        DestroyInstanceInternal(toDestroy.InstanceId);
    }
    
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(iCS_EditorObject eObj) {
        if(IsInvalid(eObj.ParentId)) return true;
        iCS_EditorObject parent= GetParent(eObj);
        if(eObj.IsNode && (parent.IsFolded || parent.IsMinimized)) return false;
        return IsVisible(parent);
    }
    public bool IsVisible(int id) { return IsInvalid(id) ? false : IsVisible(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsFolded(iCS_EditorObject eObj) { return eObj.IsFolded; }
    // ----------------------------------------------------------------------
    public void Fold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Fold();
        SetDirty(eObj);
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Unfold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        eObj.Unfold();
        SetDirty(eObj);
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMinimized(iCS_EditorObject eObj) {
        return eObj.IsMinimized;
    }
    public void Minimize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Minimize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Minimize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            SetDirty(GetParent(eObj));
        }
    }
    public void Minimize(int id) { if(IsValid(id)) Minimize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Maximize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Maximize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Maximize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            iCS_EditorObject parent= GetParent(eObj);
            SetDirty(parent);
        }
    }
    public void Maximize(int id) { if(IsValid(id)) Maximize(EditorObjects[id]); }

    // ======================================================================
    // Port Connectivity
    // ----------------------------------------------------------------------
    public void SetSource(iCS_EditorObject obj, iCS_EditorObject src) {
        obj.Source= src == null ? -1 : src.InstanceId;
        SetDirty(obj);
    }
    // ----------------------------------------------------------------------
    public void SetSource(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionDesc convDesc) {
        if(convDesc == null) { SetSource(inPort, outPort); return; }
        Rect inPos= GetPosition(inPort);
        Rect outPos= GetPosition(outPort);
        Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
        int grandParentId= GetParent(inPort).ParentId;
        iCS_EditorObject conv= CreateMethod(grandParentId, convPos, convDesc);
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
    public void DisconnectPort(iCS_EditorObject port) {
        SetSource(port, null);
        Prelude.forEach(p=> SetSource(p, null), FindConnectedPorts(port));        
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject[] FindConnectedPorts(iCS_EditorObject port) {
        return Filter(p=> p.IsPort && p.Source == port.InstanceId).ToArray();
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject FindAConnectedPort(iCS_EditorObject port) {
        iCS_EditorObject[] connectedPorts= FindConnectedPorts(port);
        return connectedPorts.Length != 0 ? connectedPorts[0] : null;
    }
    // ----------------------------------------------------------------------
    bool IsPortConnected(iCS_EditorObject port) {
        if(port.Source != -1) return true;
        foreach(var obj in EditorObjects) {
            if(obj.IsValid && obj.IsPort && obj.Source == port.InstanceId) return true;
        }
        return false;
    }
    bool IsPortDisconnected(iCS_EditorObject port) { return !IsPortConnected(port); }
    // ----------------------------------------------------------------------
    public bool IsBridgeConnection(iCS_EditorObject p1, iCS_EditorObject p2) {
        return (p1.IsDataPort && p2.IsStatePort) || (p1.IsStatePort && p2.IsDataPort);
    }
    // ----------------------------------------------------------------------
    public bool IsInBridgeConnection(iCS_EditorObject port) {
        iCS_EditorObject otherPort= GetOtherBridgePort(port);
        return otherPort != null;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetOtherBridgePort(iCS_EditorObject port) {
        if(IsSourceValid(port)) {
            iCS_EditorObject sourcePort= GetSource(port);
            if(IsBridgeConnection(port, sourcePort)) {
                return sourcePort;
            }
        }
        iCS_EditorObject remotePort= FindAConnectedPort(port);
        if(remotePort == null) return null;
        return IsBridgeConnection(port, remotePort) ? remotePort : null;
    }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public iCS_EditorObject GetDataConnectionSource(iCS_EditorObject port) {
        return Storage.GetDataConnectionSource(port);
    }
    

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
                return !excludeFlag && n.IsNode && IsVisible(n) && IsInside(n, pick) && (foundNode == null || n.LocalPosition.width < foundNode.LocalPosition.width);
            },
            n=> foundNode= n
        );
        return foundNode;
    }
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public iCS_EditorObject GetPortAt(Vector2 pick) {
        iCS_EditorObject bestPort= null;
        float bestDistance= 100000;     // Simply a big value
        FilterWith(
            port=> port.IsPort && IsVisible(port),
            port=> {
                Rect tmp= GetPosition(port);
                Vector2 position= new Vector2(tmp.x, tmp.y);
                float distance= Vector2.Distance(position, pick);
                if(distance < 3f * iCS_EditorConfig.PortRadius && distance < bestDistance) {
                    bestDistance= distance;
                    bestPort= port;
                }                                
            } 
        );
        return bestPort;
    }
    // ----------------------------------------------------------------------
    // Returns true if pick is in the titlebar of the node.
    public bool IsInTitleBar(iCS_EditorObject node, Vector2 pick) {
        if(!node.IsNode) return false;
        return true;
    }


}
