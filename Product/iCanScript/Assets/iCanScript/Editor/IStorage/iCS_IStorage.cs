using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Editor;
using iCanScript.Engine;
using P= Prelude;
using Prefs= iCanScript.Editor.PreferencesController;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
            bool                    myForceRelayout     = true;
    public  iCS_MonoBehaviourImp    iCSMonoBehaviour    = null;
    public  iCS_VisualScriptData    EditorStorage       = null;
    List<iCS_EditorObject>          myEditorObjects     = null;
    public  int                     ModificationId      = -1;
    public  bool                    CleanupDeadPorts    = true;
    public  int                     NumberOfNodes       = 0;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public List<iCS_EditorObject>   EditorObjects    { get { return myEditorObjects; }}
    public List<iCS_EngineObject>   EngineObjects    { get { return Storage.EngineObjects; }}
    public iCS_VisualScriptData Storage {
        get { return EditorStorage; }
        set { EditorStorage= value; }
    }
    public iCS_IVisualScriptData EngineStorage {
        get { return iCSMonoBehaviour; }
    }
    public GameObject HostGameObject {
        get { return iCSMonoBehaviour.gameObject; }
    }
    public iCS_VisualScriptImp VisualScript {
        get {
            return iCSMonoBehaviour as iCS_VisualScriptImp;
        }
    }
    public iCS_MonoBehaviourImp VSMonoBehaviour {
        get {
            return iCSMonoBehaviour;
        }
    }
    public iCS_EditorObject RootObject {
        get { return EditorObjects[0]; }
    }
    public int DisplayRootId {
        get {
            return Storage.DisplayRoot;
        }
        set {
            Storage.DisplayRoot= value;
            if(!IsUserTransactionActive) {
                EngineStorage.DisplayRoot= value;
            }
        }
    }
    public iCS_EditorObject DisplayRoot {
        get {
            if(myEditorObjects == null || myEditorObjects.Count == 0) {
                return null;
            }
            int id= Storage.DisplayRoot;
            if(!IsIdValid(id)) {
                Storage.DisplayRoot= 0;
                return EditorObjects[0];
            }
            var obj= EditorObjects[id];
            if(!IsValid(obj) || !obj.IsNode) {
                Storage.DisplayRoot= 0;
                return EditorObjects[0];
            }
            return obj;
        }
        set {
            // Keep EngineStorage & Storage in sync since DisplayRoot is not comprised in the Undo.
            if(value == null || !IsIdValid(value.InstanceId)) {
                Storage.DisplayRoot= 0;
                if(!IsUserTransactionActive) {
                    EngineStorage.DisplayRoot= 0;                    
                }
                return;
            }
            if(!value.IsNode) return;
            Storage.DisplayRoot= value.InstanceId;
            if(!IsUserTransactionActive) {
                EngineStorage.DisplayRoot= value.InstanceId;
            }
        }
    } 
	public bool ShowDisplayRootNode {
		get { return Storage.ShowDisplayRootNode; }
		set {
            Storage.ShowDisplayRootNode= value;
        }
	}
    public bool ForceRelayout {
        get { return myForceRelayout; }
        set { myForceRelayout= value; }
    }
    public string TypeName {
        get { return Storage.TypeName; }
        set { Storage.TypeName= value; }
    }
    public bool BaseTypeOverride {
        get { return Storage.BaseTypeOverride; }
        set { Storage.BaseTypeOverride= value; }
    }
    public string BaseType {
        get { return Storage.BaseType; }
        set { Storage.BaseType= value; }
    }
    public bool NamespaceOverride {
        get { return Storage.NamespaceOverride; }
        set { Storage.NamespaceOverride= value; }
    }
    public string Namespace {
        get { return Storage.Namespace; }
        set { Storage.Namespace= value; }
    }
	public Vector2 ScrollPosition {
	    get { return Storage.ScrollPosition; }
	    set {
            Storage.ScrollPosition= value;
            if(!IsUserTransactionActive) {
                EngineStorage.ScrollPosition= value;
            }
        }
	}
    public float GuiScale {
        get { return Storage.GuiScale; }
        set {
            Storage.GuiScale= value;
            if(!IsUserTransactionActive) {
                EngineStorage.GuiScale= value;
            }
        }
    }
    public int UndoRedoId {
        get { return Storage.UndoRedoId; }
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
    public iCS_NavigationHistory NavigationHistory {
        get { return Storage.NavigationHistory; }
    }

    

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_IStorage(iCS_MonoBehaviourImp monoBehaviour) {
        Init(monoBehaviour);
    }
    public void Init(iCS_MonoBehaviourImp monoBehaviour) {
        // Update the MonoBehaviour variable
        var oldMonoBehaviour= iCSMonoBehaviour;
        iCSMonoBehaviour= monoBehaviour;
        // Rebuild the editor information if the MonoBehaviour has changed.
        if(oldMonoBehaviour != monoBehaviour) {
			PerformEngineDataUpgrade();
            GenerateEditorData();
            PerformEditorDataUpgrade();
            // Assure that we have the default nodes if visual script is empty.
            if(EngineObjects.Count == 0) {
                if(monoBehaviour is iCS_VisualScriptImp) {
                    CreateDefaultObjectsForVisualScript();
                    SaveStorage();
                }
            }
            // Reset display root if no navigation history present
            if(!NavigationHistory.HasBackwardHistory && EditorObjects.Count > 0) {
                DisplayRoot= EditorObjects[0];
            }
            // Force a relayout
            ForceRelayout= true;
        }

        // -- Count number of nodes to limit community version --
        if(EditionController.IsCommunityEdition) {
            NumberOfNodes= 0;
            ForEach(obj=> {
                if(obj.IsNode && obj != RootObject && !obj.ParentNode.IsInstanceNode) {
                    ++NumberOfNodes;
                }
            });            
        }
        
        // Perform an initial sanity check.
		Cleanup();
        SanityCheck();

//        TestFind_iCanScript();
    }
    
//    void TestFind_iCanScript() {
//        var go= HostGameObject;
//        var components= go.GetComponents<MonoBehaviour>();
//        foreach(var c in components) {
//            var t= c.GetType();
//            if(iCS_Types.IsGeneratedByiCanScript(t.Namespace, t.Name)) {
//                Debug.Log(t.Namespace+"."+t.Name+" was generated by iCanScript");
//                Debug.Log("Source File: "+iCS_Types.GetICanScriptFile(t));
//                Debug.Log("Source File GUID: "+iCS_Types.GetICanScriptFileGUID(t));                
//            }
//        }
//
//        var baseTypeName= Prefs.CodeGenerationBaseTypeName;
//        var baseType= iCS_Types.GetTypeFromTypeString(baseTypeName);
//        if(baseType != null) {
//            var abstractMethods= iCS_Types.GetAbstractMethods(baseType);
//            Debug.Log("# abstract= "+abstractMethods.Length);
//            foreach(var m in abstractMethods) {
//                Debug.Log(m.Name);
//            }
//        }
//    }
    
    
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
    // ----------------------------------------------------------------------
    public bool IsIdValid(int id) {
		return id >= 0 && id < EngineObjects.Count;
	}
	public bool IsValid(int id) {
		return IsIdValid(id) && IsValid(EditorObjects[id]);
	}
    public bool IsValid(iCS_EditorObject obj) {
		return obj != null && IsIdValid(obj.InstanceId);
	}
    public bool IsSourceValid(iCS_EditorObject obj)  { return IsIdValid(obj.ProducerPortId); }
    public bool IsParentValid(iCS_EditorObject obj)  { return IsIdValid(obj.ParentId); }
    // ----------------------------------------------------------------------
	public bool IsAnimationPlaying {
		get { UpdateAllAnimations(); return myAnimatedObjects.Count != 0; }
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject GetParentMuxPort(iCS_EditorObject eObj) {
		return eObj.IsParentMuxPort ? eObj : (eObj.IsChildMuxPort ? eObj.Parent : null);
	}
    // ----------------------------------------------------------------------
    public System.Object GetRuntimeObject(iCS_EditorObject obj) {
		return obj == null ? null : obj.GetRuntimeObject;
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
            ForcedRelayoutOfTree();    			
        }
		
        // Update object animations.
		if(IsAnimationPlaying) {
			UpdateAllAnimations();			
		}
    }

    // ----------------------------------------------------------------------
	// This function is invoked after a change in the visual script.  It
	// is assumed that the visual script is in a stable state when this
	// function is invoked.
    public bool Cleanup() {
        bool modified= false;
		bool needsRelayout= false;
        NumberOfNodes= 0;
        ForEach(
            obj=> {
                // -- Count number of nodes to limit trial version --
                if(obj.IsNode && obj != RootObject && !obj.ParentNode.IsInstanceNode) ++NumberOfNodes;
                
                // Keep a copy of the final position.
                obj.AnimationTargetRect= obj.GlobalRect;
				// Reassign all non-connected "target" ports to script Owner
				if(obj.IsTargetPort) {
					if(obj.ProducerPort == null) {
						if(IsLocalType(obj)) {
							obj.InitialValue= OwnerTag.instance;							
						}
					}
				}
                // Cleanup disconnected or dangling ports.
                if(CleanupDeadPorts) {
					if(obj.IsPort) {
						bool shouldRemove= false;
	                    if(obj.IsDynamicDataPort && IsPortDisconnected(obj)) {
	                        shouldRemove= true;
	                    } else if(obj.IsParentMuxPort && IsPortDisconnected(obj) && obj.HasChildPort() == false) {
	                        shouldRemove= true;
	                    } else if(obj.IsChildMuxPort && obj.ProducerPort == null) {
	                        shouldRemove= true;
	                    } else if(obj.ProducerPort == null) {
							if(obj.IsChildMuxPort || obj.IsInStatePort || obj.IsInTransitionPort) {
		                        shouldRemove= true;								
							}
	                    } else if(obj.IsStatePort && IsPortDisconnected(obj)) {
	                        shouldRemove= true;
						} else if(obj.Parent == null) {
							shouldRemove= true;
						} else if(obj.Parent.IsPort && !obj.Parent.IsParentMuxPort) {
							shouldRemove= true;
						} 
						if(shouldRemove) {
	                        DestroyInstanceInternal(obj);                            
	                        modified= true;						
						}
						// -- Convert input mux to dynamic port if no children. --
						if(obj.IsInParentMuxPort) {
	                        switch(obj.NumberOfChildPorts()) {
	                            case 0:
	    					        obj.ObjectType= iCS_ObjectTypeEnum.InDynamicDataPort;
	                                break;
	                            case 1:
	                                var childPorts= obj.BuildListOfChildPorts(_=> true);
	                                obj.ProducerPort= childPorts[0].ProducerPort;
	    					        obj.ObjectType= iCS_ObjectTypeEnum.InDynamicDataPort;
	                                DestroyInstanceInternal(childPorts[0]);
                                    modified= true;
	                                break;
	                        }
						}
                        // -- Convert any dynamic ports on public functions to proposed ports --
                        if(obj.IsDynamicDataPort && obj.ParentNode.IsPublicFunction) {
                            obj.ObjectType= obj.IsInDynamicDataPort ?
                                iCS_ObjectTypeEnum.InProposedDataPort :
                                iCS_ObjectTypeEnum.OutProposedDataPort;
                        }
					}
                    // Cleanup disconnected typecasts.
    				if(obj.IsTypeCast) {
						var inDataPort= FindInChildren(obj, c=> c.IsInDataOrControlPort);
                        if(inDataPort.ProducerPort == null ||
                           FindAConnectedPort(FindInChildren(obj, c=> c.IsOutDataOrControlPort)) == null) {
                           DestroyInstanceInternal(obj);
                           modified= true;
                        }
                    }
					// Cleanup disconnected state transitions.
					if(obj.IsTransitionPackage) {
						bool hasInTransitionPort= false;
						bool hasOutTransitionPort= false;
						bool hasTriggerPort= false;
						obj.ForEachChild(
							c=> {
								if(c.IsInTransitionPort) {
									hasInTransitionPort= true;
								} else if(c.IsOutTransitionPort) {
									hasOutTransitionPort= true;
								} else if(c.IsOutFixDataPort && c.IsTriggerPort) {
									hasTriggerPort= true;
								}
							}
						);
						if(!(hasInTransitionPort && hasOutTransitionPort && hasTriggerPort)) {
							DestroyInstanceInternal(obj);
						}
					}                    
				}
            }
        );
		if(needsRelayout) {
			ForcedRelayoutOfTree();
		}
        return modified;
    }

    // ----------------------------------------------------------------------
	/// Determines if the vsObject refers to an element of the given type.
	///
	/// @param ourType The type defined by this visual script.
	/// @param baseType The type this visual script derives from.
	/// @param vsObj The object on which the search occurs.
	///
	public bool IsLocalType(iCS_EditorObject vsObj) {
		// First determine if the type is included inside the GameObject.
        var baseType= CodeGenerationConfig.GetBaseType(this);
		if(vsObj.IsIncludedInType(baseType)) return true;
		var typeNode= vsObj.ParentTypeNode;
		if(typeNode == null) return false;
		if(vsObj.Namespace == iCS_Config.kCodeGenerationNamespace) {
			return vsObj.TypeName == typeNode.CodeName;
		}
		return false;
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
        instance.CollisionOffsetFromGlobalPosition= globalPos;
        return instance;
    }
    iCS_EditorObject Copy(iCS_EditorObject srcObj, iCS_IStorage srcStorage,
                          iCS_EditorObject destParent, iCS_IStorage destStorage, Vector2 globalPos, List<Prelude.Tuple<int,int>> xlat) {
        // Create new EditorObject
        int id= destStorage.GetNextAvailableId();
        xlat.Add(new Prelude.Tuple<int,int>(srcObj.InstanceId, id));
        var newObj= destStorage[id]= iCS_EditorObject.Clone(id, srcObj, destParent, destStorage);
        if(newObj.IsNode) {
            newObj.LocalAnchorFromGlobalPosition= globalPos;
        }
        newObj.IconGUID= srcObj.IconGUID;
        srcObj.ForEachChild(
            child=> Copy(child, srcStorage, newObj, destStorage, globalPos+child.LocalAnchorPosition, xlat)
        );
		if(newObj.IsInDataOrControlPort) {
			LoadInitialPortValueFromArchive(this[id]);
		}
        return newObj;
    }
    void ReconnectCopy(iCS_EditorObject srcObj, iCS_IStorage srcStorage, iCS_IStorage destStorage, List<Prelude.Tuple<int,int>> xlat) {
        srcStorage.ForEachRecursive(srcObj,
            child=> {
                if(child.ProducerPortId != -1) {
                    int id= -1;
                    int sourceId= -1;
                    foreach(var pair in xlat) {
                        if(pair.Item1 == child.InstanceId) {
                            id= pair.Item2;
                            if(sourceId != -1) break;
                        }
                        if(pair.Item1 == child.ProducerPortId) {
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
    public iCS_EditorObject CreateBehaviour(string name) {
        // Create the function node.
        int id= GetNextAvailableId();
        // Validate that behaviour is at the root.
        if(id != 0) {
            Debug.LogError("Behaviour MUST be the root object !!!");
        }
        // Create new EditorObject
        iCS_EditorObject.CreateInstance(0, name, typeof(iCS_VisualScriptImp), -1, iCS_ObjectTypeEnum.Behaviour, this);
        this[0].LocalAnchorFromGlobalPosition= VisualEditorCenter();
        return this[0];
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreatePackage(int parentId, string name= "", iCS_ObjectTypeEnum objectType= iCS_ObjectTypeEnum.Package, Type runtimeType= null) {
		if(runtimeType == null) runtimeType= typeof(iCS_Package);
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, runtimeType, parentId, objectType, this);
        if(instance.IsInstanceNode) {
            InstanceWizardCompleteCreation(instance);
            instance.DisplayName= iCS_ObjectNames.ToTypeName(iCS_Types.TypeName(runtimeType))+" Properties";
            CreateSelfPort(id);
        }
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateStateChart(int parentId, string name= "") {
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_StateChart), parentId, iCS_ObjectTypeEnum.StateChart, this);
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateState(int parentId, string name= "") {
        // Validate that we have a good parent.
        iCS_EditorObject parent= EditorObjects[parentId];
        if(parent == null || (!parent.IsStateChart && !parent.IsState)) {
            Debug.LogError("State must be created as a child of StateChart or State.");
        }
        // Create the function node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var instance= iCS_EditorObject.CreateInstance(id, name, typeof(iCS_State), parentId, iCS_ObjectTypeEnum.State, this);
        // Set first state as the default entry state.
        instance.IsEntryState= !UntilMatchingChild(parent,
            child=> {
                if(child.IsEntryState) {
                    return true;
                }
                return false;
            }
        );
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateFunction(int parentId, iCS_FunctionPrototype desc) {
        iCS_EditorObject instance= desc.IsInstanceMember ?
                    				CreateInstanceFunction(parentId, desc) : 
                    				CreateClassFunction(parentId, desc);

		instance.MethodName= desc.MethodName;
		instance.NbOfParams= P.length(desc.Parameters);
		return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateClassFunction(int parentId, iCS_FunctionPrototype desc) {
        // Create the conversion node.
        int id= GetNextAvailableId();
        // Create new EditorObject
        var defaultName= desc.DisplayName;
        var instance= iCS_EditorObject.CreateInstance(id, defaultName, desc.ClassType, parentId, desc.ObjectType, this);
        // Determine icon.
        instance.IconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
        // Create parameter ports.
		iCS_EditorObject port= null;
		int parameterIdx= 0;
        for(; parameterIdx < P.length(desc.Parameters); ++parameterIdx) {
            var p= desc.Parameters[parameterIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixDataPort : iCS_ObjectTypeEnum.InFixDataPort;
                port= CreatePort(p.name, id, p.type, portType, (int)iCS_PortIndex.ParametersStart+parameterIdx);
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// Create return port.
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixDataPort, (int)iCS_PortIndex.Return);
		}
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateInstanceFunction(int parentId, iCS_FunctionPrototype desc) {
        // -- Grab a free ID --
        int id= GetNextAvailableId();
        // -- Create the function node --
        var defaultName= desc.DisplayName;
        var instance= iCS_EditorObject.CreateInstance(id, defaultName, desc.ClassType, parentId, desc.ObjectType, this);
        instance.IconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
		// -- Create target & self ports. --
        CreateTargetPort(id);
        CreateSelfPort(id);
        // -- Create parameter ports. --
		iCS_EditorObject port= null;
        for(int parameterIdx= 0; parameterIdx < P.length(desc.Parameters); ++parameterIdx) {
            var p= desc.Parameters[parameterIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixDataPort : iCS_ObjectTypeEnum.InFixDataPort;
                port= CreatePort(p.name, id, p.type, portType, (int)iCS_PortIndex.ParametersStart+parameterIdx);
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// -- Create return port. --
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixDataPort, (int)iCS_PortIndex.Return);
		}
        return instance;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateMessageHandler(int parentId, iCS_MessageInfo desc) {
        if(desc == null) return null;
        // -- Grab next available ID --
        int id= GetNextAvailableId();
        // -- Create event handler node --
        var instance= iCS_EditorObject.CreateInstance(id, desc.DisplayName, desc.ClassType, parentId, desc.ObjectType, this);
        instance.IconGUID= iCS_TextureCache.IconPathToGUID(desc.IconPath);
        // -- Create target port. --
		iCS_EditorObject port= null;
        if(desc.IsInstanceMember) {
            port= CreateTargetPort(id);
            if(instance.Parent.IsBehaviour) {
                port.InitialValue= instance.Parent.iCSMonoBehaviour;
            }
        }
        // -- Create parameter ports --
        for(int parameterIdx= 0; parameterIdx < P.length(desc.Parameters); ++parameterIdx) {
            var p= desc.Parameters[parameterIdx];
            if(p.type != typeof(void)) {
                iCS_ObjectTypeEnum portType= p.direction == iCS_ParamDirection.Out ? iCS_ObjectTypeEnum.OutFixDataPort : iCS_ObjectTypeEnum.InFixDataPort;
                port= CreatePort(p.name, id, p.type, portType, (int)iCS_PortIndex.ParametersStart+parameterIdx);
				object initialPortValue= p.initialValue;
				if(initialPortValue == null) {
					initialPortValue= iCS_Types.DefaultValue(p.type);
				}
                port.InitialPortValue= initialPortValue;
            }
        }
		// -- Create return port --
		if(desc.ReturnType != null && desc.ReturnType != typeof(void)) {
            port= CreatePort(desc.ReturnName, id, desc.ReturnType, iCS_ObjectTypeEnum.OutFixDataPort, (int)iCS_PortIndex.Return);
		}
        return instance;
    }    
	// ----------------------------------------------------------------------
	public iCS_EditorObject CreateObjectInstance(int parentId, string name, Type instanceType) {
        return CreatePackage(parentId, name, iCS_ObjectTypeEnum.Package, instanceType);
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateInParameterPort(string name, int parentId, Type valueType, int index) {
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.InFixDataPort, index);
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject CreateOutParameterPort(string name, int parentId, Type valueType, int index) {
		return CreatePort(name, parentId, valueType, iCS_ObjectTypeEnum.OutFixDataPort, index);
	}
    // ----------------------------------------------------------------------
	private iCS_EditorObject CreateParameterPort(string name, int parentId, Type valueType, iCS_ObjectTypeEnum portType, int index) {
		if(index < (int)iCS_PortIndex.ParametersStart || index > (int)iCS_PortIndex.ParametersEnd) {
			Debug.LogError("iCanScript: Invalid parameter port index: "+index);
		}
		return CreatePort(name, parentId, valueType, portType, index);
	}
    // ----------------------------------------------------------------------
    Vector2 VisualEditorCenter() {
        iCS_VisualEditor editor= iCS_EditorController.FindVisualEditor();
        var center= editor == null ? Vector2.zero : editor.ViewportToGraph(editor.ViewportCenter);
		return center;
    }
}
