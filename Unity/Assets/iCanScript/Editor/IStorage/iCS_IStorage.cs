using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
            bool                myForceRelayout     = true;
            bool                myIsDirty           = true;
            bool                CleanupNeeded       = true;
    public  iCS_Storage         Storage             = null;
    List<iCS_EditorObject>      myEditorObjects     = null;
    public  int                 UndoRedoId          = 0;
    public  int                 ModificationId      = -1;
    public  bool                CleanupDeadPorts    = true;
    private bool                myIsAnimationPlaying= true;
    
    // ======================================================================
    // Public Accessors
    // ----------------------------------------------------------------------
    public List<iCS_EditorObject>    EditorObjects    { get { return myEditorObjects; }}
    public List<iCS_EngineObject>    EngineObjects    { get { return Storage.EngineObjects; }}
    public bool ForceRelayout {
        get { return myForceRelayout; }
        set { myForceRelayout= value; }
    }
	public Vector2 ScrollPosition {
	    get { return Storage.ScrollPosition; }
	    set { Storage.ScrollPosition= value; }
	}
    public float GuiScale {
        get { return Storage.GuiScale; }
        set { Storage.GuiScale= value; }
    }
    public int SelectedObjectId {
        get { return Storage.SelectedObject; }
        set { Storage.SelectedObject= value; }
    }
    public iCS_EditorObject SelectedObject {
        get { return this[SelectedObjectId]; }
        set { SelectedObjectId= value != null ? value.InstanceId : -1; }
    }
    public bool IsDirty {
        get { return myIsDirty; }
        set {
            myIsDirty= value;
            if(value) ++ModificationId;
        }
    }
    public iCS_EditorObject this[int id] {
        get {
            if(!IsIdValid(id)) return null;
            return EditorObjects[id];
        }
        set {
            DetectUndoRedo();
            EditorObjects[id]= value;
        }
    }

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_IStorage(iCS_Storage storage) {
        Init(storage);
    }
    public void Init(iCS_Storage storage) {
        if(Storage != storage) {
            IsDirty= true;
            Storage= storage;
            UndoRedoId= Storage.UndoRedoId;          
			PerformEngineDataUpgrade();
            GenerateEditorData();
            ForceRelayout= true;
        }
    }
    // ----------------------------------------------------------------------
    void GenerateEditorData() {
		// Rebuild Editor Objects from the Engine Objects.
		if(myEditorObjects == null) {
		    myEditorObjects= new List<iCS_EditorObject>();
	    }
		iCS_EditorObject.RebuildFromEngineObjects(this);
		
        // Re-initialize internal values.
        if(EditorObjects.Count > 0 && IsValid(EditorObjects[0])) {
            ForEach(obj=> {
				// Initialize initial port values.
				if(obj.IsInDataPort) {
					LoadInitialPortValueFromArchive(obj);
				}
            });            
        }
        CleanupUnityObjects();
    }
    
    // ----------------------------------------------------------------------
    public bool IsBehaviour {
		get {
			return IsValid(EditorObjects[0]) && EditorObjects[0].IsBehaviour;
		}
	}
    public bool IsEmptyBehaviour    {
        get {
            if(!IsBehaviour) return false;
            for(int i= 1; i < EditorObjects.Count; ++i) {
                if(IsValid(EditorObjects[i])) return false;
            }
            return true;
        }
    }
    public bool IsLibrary {
		get {
			return IsValid(EditorObjects[0]) && !EditorObjects[0].IsBehaviour;
		}
	}
    // ----------------------------------------------------------------------
    public bool IsIdValid(int id) {
		return id >= 0 && id < EngineObjects.Count;
	}
	public bool IsValid(int id) {
		return IsIdValid(id) && EditorObjects[id] != null;
	}
    public bool IsValid(iCS_EditorObject obj) {
		return obj != null && IsIdValid(obj.InstanceId);
	}
    public bool IsSourceValid(iCS_EditorObject obj)  { return IsIdValid(obj.SourceId); }
    public bool IsParentValid(iCS_EditorObject obj)  { return IsIdValid(obj.ParentId); }
    // ----------------------------------------------------------------------
	public bool IsAnimationPlaying {
		get { return myIsAnimationPlaying; }
		set { myIsAnimationPlaying= value; }
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject GetParentMuxPort(iCS_EditorObject eObj) {
		return eObj.IsParentMuxPort ? eObj : (eObj.IsChildMuxPort ? eObj.Parent : null);
	}
    // ----------------------------------------------------------------------
    public object GetRuntimeObject(iCS_EditorObject obj) {
        iCS_BehaviourImp bh= Storage as iCS_BehaviourImp;
        return obj == null || bh == null ? null : bh.GetRuntimeObject(obj.InstanceId);
    }
    
    // ======================================================================
    // Storage Update
    // ----------------------------------------------------------------------
    public void Update() {
        // Processing any changed caused by Undo/Redo
        DetectUndoRedo();
        
        // Force a relayout if it is requested
        if(myForceRelayout) {
            myForceRelayout= false;
            ForcedRelayoutOfTree(EditorObjects[0]);    			
        }
		
	    // Perform layout if one or more objects has changed.
	    if(myIsDirty) {
	        // Tell Unity that our storage has changed.
	        EditorUtility.SetDirty(Storage);
	        // Prepare for cleanup after storage change.
	        CleanupNeeded= true;
	        myIsDirty= false;
	    }
        // Update object animations.
		if(IsAnimationPlaying) {
			UpdateAnimations();			
		}

        // Perform graph cleanup once objects & layout are stable.
        if(CleanupNeeded) {
            UpdateExecutionPriority();
            CleanupNeeded= Cleanup();
        }
    }

    // ----------------------------------------------------------------------
	public void UpdateAnimations() {
        IsAnimationPlaying= false;
        if(iCS_PreferencesEditor.AnimationEnabled) {
            ForEach(
                obj=> {
        			if(obj.IsAnimated) {
    					obj.UpdateAnimation();
        				IsAnimationPlaying= true;
        			}
                }
            );
			// Force full relayout after animation has completed.
            if(IsAnimationPlaying == false) {
				ForcedRelayoutOfTree(EditorObjects[0]);
			}
        }		
	}
	
    // ----------------------------------------------------------------------
    /*
        FEATURE: Should use the layout rule the determine execution priority.
    */
    public void UpdateExecutionPriority() {
        var len= EditorObjects.Count;
        for(int i= 0; i < len; ++i) {
            if(IsValid(i)) {
                EditorObjects[i].ExecutionPriority= i;
            }
        }
    }
    // ----------------------------------------------------------------------
    public bool Cleanup() {
        bool modified= false;
        ForEach(
            obj=> {
                // Cleanup disconnected dynamic state or module ports.
				var parent= obj.Parent;
                if(CleanupDeadPorts) {
					bool shouldRemove= false;
					if(obj.IsParentMuxPort) {
						int nbOfChildren= NbOfChildren(obj, c=> c.IsInDataPort);
						if(nbOfChildren == 1) {
							iCS_EditorObject child= GetChildInputDataPorts(obj)[0];
							obj.SourceId= child.SourceId;
							obj.ObjectType= iCS_ObjectTypeEnum.OutDynamicPort;
							DestroyInstanceInternal(child);
						} else {
							shouldRemove= nbOfChildren == 0 && IsPortDisconnected(obj);							
						}
					} else if(obj.IsChildMuxPort) {
						shouldRemove= obj.Source == null;
					} else {
						shouldRemove= ((obj.IsStatePort || obj.IsDynamicModulePort) && IsPortDisconnected(obj)) ||
						              (obj.IsDynamicModulePort && obj.Source == null && (parent.IsStateChart || parent.IsState));
						
					}
					if(shouldRemove) {
                        DestroyInstanceInternal(obj);                            
                        modified= true;						
					}
                    // Cleanup disconnected typecasts.
    				if(obj.IsTypeCast) {
						var inDataPort= FindInChildren(obj, c=> c.IsInDataPort);
                        if(inDataPort.Source == null &&
                           FindAConnectedPort(FindInChildren(obj, c=> c.IsOutDataPort)) == null) {
                           DestroyInstanceInternal(obj);
                           modified= true;
                        }
                    }                    
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
                if(obj.IsInDataPort && obj.SourceId == -1 && obj.InitialValue != null) {
                    StoreInitialPortValueInArchive(obj);
                }
                else {
                    obj.InitialValueArchive= null; 
                }
            }
        );
    }
    
    // ======================================================================
    // Editor Object Creation/Destruction
    // ----------------------------------------------------------------------
    int GetNextAvailableId() {
        // Covers Undo?redo for all creation operation
        DetectUndoRedo();
        // Find the next available id.
        int id= 0;
        int len= EditorObjects.Count;
        while(id < len && IsValid(EditorObjects[id])) { ++id; }
        return id;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject Copy(iCS_EditorObject srcObj, iCS_IStorage srcStorage,
                                 iCS_EditorObject destParent, Vector2 globalPos, iCS_IStorage destStorage) {
        // Create new EditorObject
        List<Prelude.Tuple<int, int>> xlat= new List<Prelude.Tuple<int, int>>();
        iCS_EditorObject instance= Copy(srcObj, srcStorage, destParent, destStorage, globalPos, xlat);
        ReconnectCopy(srcObj, srcStorage, destStorage, xlat);
        instance.LayoutRect= iCS_EditorObject.BuildRect(globalPos, Vector2.zero);
        return instance;
    }
    iCS_EditorObject Copy(iCS_EditorObject srcObj, iCS_IStorage srcStorage,
                          iCS_EditorObject destParent, iCS_IStorage destStorage, Vector2 globalPos, List<Prelude.Tuple<int,int>> xlat) {
        // Create new EditorObject
        int id= destStorage.GetNextAvailableId();
        xlat.Add(new Prelude.Tuple<int,int>(srcObj.InstanceId, id));
        var newObj= destStorage[id]= iCS_EditorObject.Clone(id, srcObj, destParent, destStorage);
        newObj.SetAnchorAndLayoutPosition(globalPos);
        newObj.IconGUID= srcObj.IconGUID;
        srcObj.ForEachChild(
            child=> Copy(child, srcStorage, newObj, destStorage, globalPos+child.LocalAnchorPosition, xlat)
        );
		if(newObj.IsInDataPort) {
			LoadInitialPortValueFromArchive(this[id]);
		}
        return newObj;
    }
    void ReconnectCopy(iCS_EditorObject srcObj, iCS_IStorage srcStorage, iCS_IStorage destStorage, List<Prelude.Tuple<int,int>> xlat) {
        srcStorage.ForEachRecursive(srcObj,
            child=> {
                if(child.SourceId != -1) {
                    int id= -1;
                    int sourceId= -1;
                    foreach(var pair in xlat) {
                        if(pair.Item1 == child.InstanceId) {
                            id= pair.Item2;
                            if(sourceId != -1) break;
                        }
                        if(pair.Item1 == child.SourceId) {
                            sourceId= pair.Item2;
                            if(id != -1) break;
                        }
                    }
                    if(sourceId != -1) {
                        destStorage.SetSource(destStorage.EditorObjects[id], destStorage.EditorObjects[sourceId]);                        
                    }
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
        iCS_EditorObject.CreateInstance(0, "Behaviour", typeof(iCS_BehaviourImp), -1, iCS_ObjectTypeEnum.Behaviour, this);
        this[0].SetAnchorAndLayoutPosition(VisualEditorCenter());
		this[0].IsNameEditable= false;
        return this[0];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateModule(int parentId, Vector2 globalPos, string name= "", iCS_ObjectTypeEnum objectType= iCS_ObjectTypeEnum.Module, Type runtimeType= null) {
		if(runtimeType == null) runtimeType= typeof(iCS_Module);
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, runtimeType, parentId, objectType, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
	    instance.IconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.ModuleIcon);			
        if(instance.IsObjectInstance) InstanceWizardCompleteCreation(instance);
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateStateChart(int parentId, Vector2 globalPos, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_StateChart), parentId, iCS_ObjectTypeEnum.StateChart, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
        // Automatically create entry state.
        CreateState(id, globalPos, "EntryState");
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateState(int parentId, Vector2 globalPos, string name= "") {
        // Validate that we have a good parent.
        iCS_EditorObject parent= EditorObjects[parentId];
        if(parent == null || (!parent.IsStateChart && !parent.IsState)) {
            Debug.LogError("State must be created as a child of StateChart or State.");
        }
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_State), parentId, iCS_ObjectTypeEnum.State, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
        // Set first state as the default entry state.
        instance.IsEntryState= !UntilMatchingChild(parent,
            child=> {
                if(child.IsEntryState) {
                    return true;
                }
                return false;
            }
        );
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateMethod(int parentId, Vector2 globalPos, iCS_MethodBaseInfo desc) {
        iCS_EditorObject instance= desc.IsInstanceMember ?
                    				CreateInstanceMethod(parentId, globalPos, desc) : 
                    				CreateStaticMethod(parentId, globalPos, desc);

		instance.MethodName= desc.MethodName;
		instance.NbOfParams= desc.Parameters != null ? desc.Parameters.Length : 0;
		return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateStaticMethod(int parentId, Vector2 globalPos, iCS_MethodBaseInfo desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Determine icon.
        var iconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
        if(iconGUID == null && desc.ObjectType == iCS_ObjectTypeEnum.ClassMethod) {
            iconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.MethodIcon);
        }        
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
        instance.IconGUID= iconGUID;
        // Create parameter ports.
		int portIdx= 0;
		iCS_EditorObject port= null;
        for(; portIdx < desc.Parameters.Length; ++portIdx) {
            var p= desc.Parameters[portIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixPort : iCS_ObjectTypeEnum.InFixPort;
                port= CreatePort(p.name, id, p.type, portType);
                port.PortIndex= portIdx;
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixPort);
            port.PortIndex= portIdx;			
		}
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateInstanceMethod(int parentId, Vector2 globalPos, iCS_MethodBaseInfo desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Determine minimized icon.
        var iconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
        if(iconGUID == null && desc.ObjectType == iCS_ObjectTypeEnum.InstanceMethod) {
            iconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.MethodIcon);
        }        
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
        instance.IconGUID= iconGUID;
        // Create parameter ports.
		int portIdx= 0;
		iCS_EditorObject port= null;
        for(; portIdx < desc.Parameters.Length; ++portIdx) {
            var p= desc.Parameters[portIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixPort : iCS_ObjectTypeEnum.InFixPort;
                port= CreatePort(p.name, id, p.type, portType);
                port.PortIndex= portIdx;                
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixPort);
            port.PortIndex= portIdx++;			
		} else {
		    ++portIdx;
		}
		// Create 'this' ports.
        port= CreatePort(iCS_Strings.InstanceObjectName, id, desc.ClassType, iCS_ObjectTypeEnum.InFixPort);
        port.PortIndex= portIdx++;			
        port= CreatePort(iCS_Strings.InstanceObjectName, id, desc.ClassType, iCS_ObjectTypeEnum.OutFixPort);
        port.PortIndex= portIdx;			
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateMessage(int parentId, Vector2 globalPos, iCS_MethodBaseInfo desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Determine icon.
        var iconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
        if(iconGUID == null && desc.ObjectType == iCS_ObjectTypeEnum.ClassMethod) {
            iconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.MethodIcon);
        }        
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, this);
        instance.SetAnchorAndLayoutPosition(globalPos);
        instance.IconGUID= iconGUID;
        // Create parameter ports.
		int portIdx= 0;
		iCS_EditorObject port= null;
        for(; portIdx < desc.Parameters.Length; ++portIdx) {
            var p= desc.Parameters[portIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixPort : iCS_ObjectTypeEnum.InFixPort;
                port= CreatePort(p.name, id, p.type, portType);
                port.PortIndex= portIdx;
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixPort);
            port.PortIndex= portIdx++;			
		}
		// Special case for behaviour messages.
		if(instance.Parent.IsBehaviour) {
            portIdx= UpdateBehaviourMessagePorts(instance, portIdx);
		}
        // Perform initial node layout.
        instance.Unhide();
        return instance;
    }    
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreatePort(string name, int parentId, Type valueType, iCS_ObjectTypeEnum portType) {
        int id= GetNextAvailableId();
        var parent= EditorObjects[parentId];
        var globalPos= parent.LayoutPosition;
        iCS_EditorObject port= iCS_EditorObject.CreateInstance(id, name, valueType, parentId, portType, this);
        if(port.IsModulePort || port.IsChildMuxPort) 	{ AddDynamicPort(port); }
		port.UpdatePortEdge();
		port.LayoutPosition= globalPos;
        return EditorObjects[id];        
    }
    // ----------------------------------------------------------------------
    Vector2 VisualEditorCenter() {
        iCS_VisualEditor editor= iCS_EditorMgr.FindVisualEditor();
        var center= editor == null ? Vector2.zero : editor.ViewportToGraph(editor.ViewportCenter);
//		Debug.Log("Visual Editor center: "+center);
		return center;
    }
    
    // ======================================================================
    // Port Connectivity
    // ----------------------------------------------------------------------
    public void SetSource(iCS_EditorObject obj, iCS_EditorObject src) {
        int id= src == null ? -1 : src.InstanceId;
        if(id != obj.SourceId) {
            obj.SourceId= id; 
        }
    }
    // ----------------------------------------------------------------------
    public void SetSource(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_TypeCastInfo convDesc) {
        if(convDesc == null) {
            SetSource(inPort, outPort);
            return;
        }
        var inPos= inPort.LayoutPosition;
        var outPos= outPort.LayoutPosition;
        Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
        int grandParentId= inPort.ParentNode.ParentId;
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
        Iconize(conv);
    }
    // ----------------------------------------------------------------------
    public void DisconnectPort(iCS_EditorObject port) {
        SetSource(port, null);
        Prelude.forEach(p=> SetSource(p, null), port.Destinations);        
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject FindAConnectedPort(iCS_EditorObject port) {
        iCS_EditorObject[] connectedPorts= port.Destinations;
        return connectedPorts.Length != 0 ? connectedPorts[0] : null;
    }
    // ----------------------------------------------------------------------
    public bool IsPortSourced(iCS_EditorObject port) {
        return port.Destinations.Length != 0;
    }
    // ----------------------------------------------------------------------
    bool IsPortConnected(iCS_EditorObject port) {
        if(port.IsSourceValid) return true;
        if(FindFirst(o=> o.IsPort && o.SourceId == port.InstanceId) != null) return true;
        return false;
    }
    bool IsPortDisconnected(iCS_EditorObject port) { return !IsPortConnected(port); }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public iCS_EditorObject GetDataConnectionSource(iCS_EditorObject port) {
        iCS_EngineObject engineObject= Storage.GetDataConnectionSource(port.EngineObject);
        return engineObject != null ? EditorObjects[engineObject.InstanceId] : null;
    }
    
}
