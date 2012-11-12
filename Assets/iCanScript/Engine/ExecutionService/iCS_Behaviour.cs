using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public sealed class iCS_Behaviour : iCS_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_Action   myStartAction       = null;
    iCS_Action   myUpdateAction      = null;
    iCS_Action   myLateUpdateAction  = null;
    iCS_Action   myFixedUpdateAction = null;
    int          myUpdateFrameId     = 0;
    int          myFixedUpdateFrameId= 0;
    object[]     myRuntimeNodes      = new object[0];
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int UpdateFrameId        { get { return myUpdateFrameId; }}
    public int FixedUpdateFrameId   { get { return myFixedUpdateFrameId; }}
    
    // ----------------------------------------------------------------------
    void Reset() {
        myStartAction       = null;
        myUpdateAction      = null;
        myLateUpdateAction  = null;
        myFixedUpdateAction = null;
        myUpdateFrameId     = 0;
        myFixedUpdateFrameId= 0;        
    }
    
    // ----------------------------------------------------------------------
    // This function should be used to find references to other objects.
    // Awake is invoked after all the objects are initialized.  Awake replaces
    // the constructor.
    void Awake() {
    }

    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        GenerateCode();
        if(myStartAction != null) {
            do {
                myStartAction.Execute(-2);
                if(myStartAction.IsStalled) {
                    Debug.LogError("The Start() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!myStartAction.IsCurrent(-2));
        }
    }
    
    // ----------------------------------------------------------------------
    void OnEnable() {}
    // ----------------------------------------------------------------------
    void OnDisable() {}
    // ----------------------------------------------------------------------
    void OnDestroy() {
    }
    void OnDrawGizmos() {
        var p= transform.position;
        Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
        if(renderer != null) {
            Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
            Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
            Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
            Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
            Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
        }
     }
    
    // ======================================================================
    // Graph Updates
    // ----------------------------------------------------------------------
    // Called on every frame.
    void Update() {
        ++myUpdateFrameId;
        if(myUpdateAction != null) {
            RunEvent(myUpdateAction, myUpdateFrameId);
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            RunEvent(myLateUpdateAction, myUpdateFrameId);
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            RunEvent(myFixedUpdateAction, myFixedUpdateFrameId);
        }
    }
    // ----------------------------------------------------------------------
    void RunEvent(iCS_Action action, int frameId) {
        do {
            action.Execute(frameId);                                
            if(action.IsStalled) {
                action.ForceExecute(frameId);
            }
        } while(!action.IsCurrent(frameId));        
    }
    
    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        iCS_Action action= obj as iCS_Action;
        if(action == null) return;
        switch(action.Name) {
            case iCS_Strings.Start: {
                myStartAction= action;
                break;
            }
            case iCS_Strings.Update: {
                myUpdateAction= action;
                break;
            }
            case iCS_Strings.LateUpdate: {
                myLateUpdateAction= action;
                break;
            }
            case iCS_Strings.FixedUpdate: {
                myFixedUpdateAction= action;
                break;
            }
            default: {
                break;
            }
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(object obj) {
        iCS_Action action= obj as iCS_Action;
        if(action == null) return;
        switch(action.Name) {
            case iCS_Strings.Start: {
                myStartAction= null;
                break;
            }
            case iCS_Strings.Update: {
                myUpdateAction= null;
                break;
            }
            case iCS_Strings.LateUpdate: {
                myLateUpdateAction= null;
                break;
            }
            case iCS_Strings.FixedUpdate: {
                myFixedUpdateAction= null;
                break;
            }
            default: {
                break;
            }
        }        
    }


    // ======================================================================
    // Sanity Check
    // ----------------------------------------------------------------------
	public bool SanityCheck() {
		bool storageCorruption= false;
		bool sanityNeeded= false;
		do {
			sanityNeeded= false;
			for(int i= 0; i < EngineObjects.Count; ++i) {
				if(EngineObjects[i].InstanceId == -1) continue;
				if(EngineObjects[i].InstanceId != i) {
					sanityNeeded= true;
					Debug.LogWarning("iCanScript Sanity: Object: "+i+" has an invalid instance id of: "+EngineObjects[i].InstanceId);
					EngineObjects[i].Reset();
					continue;
				}
				int parentId= EngineObjects[i].ParentId;
				if(i == 0) {
					if(parentId != -1) {
						sanityNeeded= true;
						EngineObjects[0].ParentId= -1;
						Debug.LogWarning("iCanScript Sanity: Root object has a parent of: "+parentId);
						continue;
					}
				} else {
					// The parent id must be valid.
					if(!IsInBounds(parentId)) {
						sanityNeeded= true;
						EngineObjects[i].Reset();
						Debug.LogWarning("iCanScript Sanity: Object: "+i+" has an invalid parent: "+parentId);
						continue;				
					}
					// A port cannot be a parent.
					if(EngineObjects[parentId].IsPort && !EngineObjects[parentId].IsOutMuxPort) {
						sanityNeeded= true;
						EngineObjects[i].Reset();
						Debug.Log("iCanScript Sanity: Port: "+parentId+" is the parent of: "+i);
						continue;										
					}				
				}
			}
			if(sanityNeeded) {
				storageCorruption= true;
				Debug.Log("Sanity detected an error.");
			}
		} while(sanityNeeded);
		return storageCorruption;
	}
    // ----------------------------------------------------------------------
	bool IsInBounds(int id) { return id >= 0 && id < EngineObjects.Count; }
	
    // ======================================================================
    // Code Generation
    // ----------------------------------------------------------------------
    public void GenerateCode() {
        Debug.Log("iCanScript: Generating real-time code for "+gameObject.name+"...");
		// Verify for storage sanity.
		if(SanityCheck()) {
			Debug.LogWarning("iCanScript: storage corruption has been detected.  Attempting recovery...");
		}
        // Remove any previous runtime object creation.
        ClearGeneratedCode();
        // Create all the runtime nodes.
        GenerateRuntimeNodes();
        // Connect the runtime nodes.
        ConnectRuntimeNodes();        
    }
    // ----------------------------------------------------------------------
    public object GetRuntimeObject(int id) {
        if(id < 0 || id >= myRuntimeNodes.Length) return null;
        return id == 0 ? this : myRuntimeNodes[id];
    }
    // ----------------------------------------------------------------------
    public void ClearGeneratedCode() {
        myRuntimeNodes= new object[0];
        Reset();
    }
    // ----------------------------------------------------------------------
    public void GenerateRuntimeNodes() {
        // Allocate runtime node array (if not already done).
        if(EngineObjects.Count != myRuntimeNodes.Length) {
            myRuntimeNodes= new iCS_Object[EngineObjects.Count];
			for(int i= 0; i < myRuntimeNodes.Length; ++i) myRuntimeNodes[i]= null;
        }
        bool needAdditionalPass= false;
        do {
            // Assume that this is the last pass.
            needAdditionalPass= false;
            // Generate all runtime nodes.
            foreach(var node in EngineObjects) {
                // Uninitialized.
				if(node == null) continue;
				if(node.InstanceId == -1) continue;
                // Was already generated in a previous pass.
                if(myRuntimeNodes[node.InstanceId] != null) continue;
                Vector2 layout= Math3D.Middle(GetGlobalRect(node));
				// Special case for active ports.
				if(node.IsOutMuxPort) {
					myRuntimeNodes[node.InstanceId]= new iCS_MuxPort(node.Name, layout);
					continue;
				}
                if(node.IsNode) {
                    // Wait until parent is generated.
                    object parent= null;
					switch(node.ParentId) {
                        case -1: {
                            break;
                        }
                        case 0: {
                            parent= this;
                            break;
                        }
                        default: {
							iCS_EngineObject edParent= GetParent(node);
                            if(edParent.ObjectType == iCS_ObjectTypeEnum.TransitionModule) {
                                edParent= GetParent(edParent);
                            }
                            parent= myRuntimeNodes[edParent.InstanceId];
                            if(parent == null) {
                                needAdditionalPass= true;
                                continue;
                            }
                            break;
                        }
                    }
                    // We are ready to generate new node.
                    switch(node.ObjectType) {
                        case iCS_ObjectTypeEnum.Behaviour: {
                            break;
                        }
                        case iCS_ObjectTypeEnum.StateChart: {
                            iCS_StateChart stateChart= new iCS_StateChart(node.Name, layout);
                            myRuntimeNodes[node.InstanceId]= stateChart;
                            InvokeAddChildIfExists(parent, stateChart);
                            break;
                        }
                        case iCS_ObjectTypeEnum.State: {
                            iCS_State state= new iCS_State(node.Name, layout);
                            myRuntimeNodes[node.InstanceId]= state;
                            InvokeAddChildIfExists(parent, state);
                            if(node.IsEntryState) {
                                if(parent is iCS_StateChart) {
                                    iCS_StateChart parentStateChart= parent as iCS_StateChart;
                                    parentStateChart.EntryState= state;
                                } else {
                                    iCS_State parentState= parent as iCS_State;
                                    parentState.EntryState= state;
                                }
                            }
                            break;
                        }
                        case iCS_ObjectTypeEnum.TransitionModule: {
                            break;
                        }
                        case iCS_ObjectTypeEnum.TransitionGuard:
                        case iCS_ObjectTypeEnum.TransitionAction: {
                            iCS_Module module= new iCS_Module(node.Name, layout);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            break;
                        }
                        case iCS_ObjectTypeEnum.Module: {
                            iCS_Module module= new iCS_Module(node.Name, layout);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            InvokeAddChildIfExists(parent, module);                                
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceMethod: {
                            // Create method.
                            iCS_Method method= new iCS_Method(node.Name, GetMethodBase(node), GetPortIsOuts(node), layout);                                
                            myRuntimeNodes[node.InstanceId]= method;
                            InvokeAddChildIfExists(parent, method);
                            break;                            
                        }
                        case iCS_ObjectTypeEnum.Constructor: {
                            // Create function.
                            iCS_Constructor func= new iCS_Constructor(node.Name, GetMethodBase(node), GetPortIsOuts(node), layout);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case iCS_ObjectTypeEnum.TypeCast:
                        case iCS_ObjectTypeEnum.StaticMethod: {
                            // Create function.
                            iCS_Function func= new iCS_Function(node.Name, GetMethodBase(node), GetPortIsOuts(node), layout);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceField: {
                            // Create function.
                            FieldInfo fieldInfo= GetFieldInfo(node);
							bool[] portIsOuts= GetPortIsOuts(node);
                            iCS_FunctionBase rtField= portIsOuts.Length == 0 ?
                                new iCS_GetInstanceField(node.Name, fieldInfo, portIsOuts, layout) as iCS_FunctionBase:
                                new iCS_SetInstanceField(node.Name, fieldInfo, portIsOuts, layout) as iCS_FunctionBase;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;
                        }
                        case iCS_ObjectTypeEnum.StaticField: {
                            // Create function.
							FieldInfo fieldInfo= GetFieldInfo(node);
							bool[] portIsOuts= GetPortIsOuts(node);
                            iCS_FunctionBase rtField= portIsOuts.Length == 0 ?
                                new iCS_GetStaticField(node.Name, fieldInfo, portIsOuts, layout) as iCS_FunctionBase:
                                new iCS_SetStaticField(node.Name, fieldInfo, portIsOuts, layout) as iCS_FunctionBase;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;                            
                        }
                        default: {
                            Debug.LogWarning("Code could not be generated for "+node.ObjectType+" editor object type.");
                            break;
                        }
                    }
                }
            }
        } while(needAdditionalPass);
    }
    // ----------------------------------------------------------------------
    public void ConnectRuntimeNodes() {
        foreach(var port in EngineObjects) {
			if(port == null) continue;
			if(port.InstanceId == -1) continue;
            if(port.IsPort) {
                switch(port.ObjectType) {
                    // Ports without code generation.
                    case iCS_ObjectTypeEnum.InTransitionPort:
                    case iCS_ObjectTypeEnum.OutTransitionPort:
                    case iCS_ObjectTypeEnum.InDynamicModulePort:
                    case iCS_ObjectTypeEnum.OutDynamicModulePort:
                    case iCS_ObjectTypeEnum.InStaticModulePort:
                    case iCS_ObjectTypeEnum.OutStaticModulePort:
                    case iCS_ObjectTypeEnum.OutStatePort: {
                        break;
                    }
					case iCS_ObjectTypeEnum.InMuxPort: {
						iCS_IParams rtMuxPort= myRuntimeNodes[port.ParentId] as iCS_IParams;
						if(rtMuxPort == null) break;
                        iCS_EngineObject sourcePort= GetDataConnectionSource(port);
						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : iCS_Connection.NoConnection;
						rtMuxPort.SetParameterConnection(port.PortIndex, connection);
						break;
					}
                    case iCS_ObjectTypeEnum.InStatePort: {
                        iCS_EngineObject endState= GetParent(port);
                        iCS_EngineObject transitionModule= GetParent(GetSource(port));
                        iCS_EngineObject actionModule= null;
                        iCS_EngineObject triggerPort= null;
                        iCS_EngineObject outStatePort= null;
                        iCS_EngineObject guardModule= GetTransitionModuleParts(transitionModule, out actionModule, out triggerPort, out outStatePort);
                        triggerPort= GetDataConnectionSource(triggerPort);
                        iCS_FunctionBase triggerFunc= triggerPort.IsOutModulePort ? null : myRuntimeNodes[triggerPort.ParentId] as iCS_FunctionBase;
                        int triggerIdx= triggerPort.PortIndex;
                        Rect outStatePortPos= GetGlobalRect(outStatePort);
                        Rect inStatePortPos= GetGlobalRect(port);
                        Vector2 layout= new Vector2(0.5f*(inStatePortPos.x+outStatePortPos.x), 0.5f*(inStatePortPos.y+outStatePortPos.y));
                        iCS_Transition transition= new iCS_Transition(transitionModule.Name,
                                                                    myRuntimeNodes[endState.InstanceId] as iCS_State,
                                                                    myRuntimeNodes[guardModule.InstanceId] as iCS_Module,
                                                                    triggerFunc, triggerIdx,
                                                                    actionModule != null ? myRuntimeNodes[actionModule.InstanceId] as iCS_Module : null,
                                                                    layout);
                        iCS_State state= myRuntimeNodes[outStatePort.ParentId] as iCS_State;
                        state.AddChild(transition);
                        break;
                    }
                    
                    // Data ports.
                    case iCS_ObjectTypeEnum.OutFunctionPort: {
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_Method, iCS_GetInstanceField, iCS_GetStaticField, iCS_SetInstanceField, iCS_SetStaticField, iCS_Function>(parentObj,
                            method          => method[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            getInstanceField=> getInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            getStaticField  => getStaticField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            setInstanceField=> setInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            setStaticField  => setStaticField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            function        => function[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType)
                        );
                        break;
                    }
                    case iCS_ObjectTypeEnum.InFunctionPort:
                    case iCS_ObjectTypeEnum.EnablePort: {
                        // Build connection.
                        iCS_EngineObject sourcePort= GetDataConnectionSource(port);
						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : iCS_Connection.NoConnection;
                        // Build initial value.
						object initValue= GetInitialValue(sourcePort);
                        // Set data port.
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_Constructor, iCS_Method, iCS_GetInstanceField, iCS_GetStaticField, iCS_SetInstanceField, iCS_SetStaticField, iCS_Function>(parentObj,
                            constructor=> {
                                constructor[port.PortIndex]= initValue;
                                constructor.SetParameterConnection(port.PortIndex, connection);
                            },
                            method=> {
                                method[port.PortIndex]= initValue;
                                method.SetParameterConnection(port.PortIndex, connection);
                            },
                            getInstanceField=> {
                                getInstanceField[port.PortIndex]= initValue;
                                getInstanceField.SetParameterConnection(port.PortIndex, connection);
                            },
                            getStaticField=> {
                                getStaticField[port.PortIndex]= initValue;
                                getStaticField.SetParameterConnection(port.PortIndex, connection);
                            },
                            setInstanceField=> {
                                setInstanceField[port.PortIndex]= initValue;
                                setInstanceField.SetParameterConnection(port.PortIndex, connection);
                            },
                            setStaticField=> {
                                setStaticField[port.PortIndex]= initValue;
                                setStaticField.SetParameterConnection(port.PortIndex, connection);
                            },
                            function=> {
                                function[port.PortIndex]= initValue;
                                function.SetParameterConnection(port.PortIndex, connection);
                            }
                        );
                        break;
                    }
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    iCS_EngineObject GetTransitionModuleParts(iCS_EngineObject transitionModule, out iCS_EngineObject actionModule,
                                                                               out iCS_EngineObject triggerPort,
                                                                               out iCS_EngineObject outStatePort) {
        iCS_EngineObject guardModule= null;
        actionModule= null;
        triggerPort= null;
        outStatePort= null;
        foreach(var edObj in EngineObjects) {
            if(edObj.IsTransitionAction) {
                if(GetParent(edObj) == transitionModule) {
                    actionModule= edObj;
                }
            }
            if(edObj.IsTransitionGuard) {
                if(GetParent(edObj) == transitionModule) {
                    guardModule= edObj;
                }
            }
            if(edObj.IsOutStaticModulePort && edObj.RuntimeType == typeof(bool) && edObj.Name == "trigger") {
                iCS_EngineObject gModule= GetParent(edObj);
                if(gModule.IsTransitionGuard && GetParent(gModule) == transitionModule) {
                    triggerPort= edObj;
                }
            }
            if(edObj.IsInTransitionPort) {
                if(GetParent(edObj) == transitionModule) {
                    outStatePort= GetSource(edObj);
                }
            }
        }
        return guardModule;
    }
    

    // ======================================================================
    // Runtime information extraction
    // ----------------------------------------------------------------------
	MethodBase GetMethodBase(iCS_EngineObject node) {
        return node.GetMethodBase(EngineObjects);
	}
	FieldInfo GetFieldInfo(iCS_EngineObject node) {
        return node.GetFieldInfo();
	}
	bool[] GetPortIsOuts(iCS_EngineObject node) {
		iCS_EngineObject[] ports= GetChildPorts(node);
		bool[] isOuts= new bool[node.NbOfParams];
		for(int i= 0; i < isOuts.Length; ++i) {
			isOuts[i]= ports[i].IsOutputPort;
		}
		return isOuts;
	}
	Type[] GetParamTypes(iCS_EngineObject node) {
	    return node.GetParamTypes(EngineObjects);
	}
	iCS_EngineObject[] GetChildPorts(iCS_EngineObject node) {
		List<iCS_EngineObject> ports= new List<iCS_EngineObject>();
		// Get all child data ports.
		int nodeId= node.InstanceId;
		foreach(var port in EngineObjects) {
			if(port == null) continue;
			if(port.ParentId != nodeId) continue;
			if(!port.IsDataPort) continue;
			if(port.IsEnablePort) continue;
			ports.Add(port);
		}
		// Sort child ports according to index.
		iCS_EngineObject[] result= ports.ToArray();
		Array.Sort(result, (x,y)=> x.PortIndex - y.PortIndex);
		return result;
	}
    // ----------------------------------------------------------------------
	object GetInitialValue(iCS_EngineObject port) {
	    if(port.InitialValueArchive == null || port.InitialValueArchive == "") return null;
		iCS_Coder coder= new iCS_Coder(port.InitialValueArchive);
		return coder.DecodeObjectForKey("InitialValue", this) ?? iCS_Types.DefaultValue(port.RuntimeType);
	}
    // ----------------------------------------------------------------------
	iCS_Connection BuildConnection(iCS_EngineObject port) {
		iCS_Connection connection= iCS_Connection.NoConnection;
		if(myRuntimeNodes[port.InstanceId] != null) {
			connection= new iCS_Connection(myRuntimeNodes[port.InstanceId] as iCS_IParams, 0);							
		} else {
			connection= new iCS_Connection(myRuntimeNodes[port.ParentId] as iCS_IParams, port.PortIndex);
		}
		return connection;
	}
	
    // ======================================================================
    // Child Management Utilities
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the AddChild method.
    public static MethodInfo GetAddChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(iCS_Strings.AddChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) return null;
        ParameterInfo[] parameters= methodInfo.GetParameters();
        if(parameters.Length != 1) return null;
        return methodInfo;
    }
    public static void InvokeAddChildIfExists(object parent, object child) {
        MethodInfo method= GetAddChildMethodInfo(parent);
        if(method == null) return;
        method.Invoke(parent, new object[1]{child});
    }
    
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the RemoveChild method.
    public static MethodInfo GetRemoveChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(iCS_Strings.RemoveChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) return null;
        ParameterInfo[] parameters= methodInfo.GetParameters();
        if(parameters.Length != 1) return null;
        return methodInfo;
    }
    public static void InvokeRemoveChildIfExists(object parent, object child) {
        MethodInfo method= GetRemoveChildMethodInfo(parent);
        if(method == null) return;
        method.Invoke(parent, new object[1]{child});
    }
}
