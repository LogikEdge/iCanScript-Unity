using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualScriptImp : iCS_Storage {
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
					if(EngineObjects[parentId].IsPort && !EngineObjects[parentId].IsParentMuxPort) {
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
        //Debug.Log("iCanScript: Generating real-time code for "+gameObject.name+"...");            
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
                int priority= node.ExecutionPriority;
				// Special case for active ports.
				if(node.IsParentMuxPort) {
					myRuntimeNodes[node.InstanceId]= new iCS_MuxPort(node.Name, priority, GetNbOfChildMuxPorts(node));
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
                            iCS_StateChart stateChart= new iCS_StateChart(node.Name, priority);
                            myRuntimeNodes[node.InstanceId]= stateChart;
                            InvokeAddChildIfExists(parent, stateChart);
                            break;
                        }
                        case iCS_ObjectTypeEnum.State: {
                            iCS_State state= new iCS_State(node.Name, priority);
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
                            var module= new iCS_Aggregate(node.Name, priority);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceMessage:
                        case iCS_ObjectTypeEnum.ClassMessage: {
                            var nbParams= GetInputEndPortsLastIndex(node)+1;
                            iCS_Message message= new iCS_Message(node.Name, priority, nbParams);                                
                            myRuntimeNodes[node.InstanceId]= message;
                            InvokeAddChildIfExists(parent, message);                                
                            break;
                        }
                        case iCS_ObjectTypeEnum.Aggregate: {
                            var nbParams= GetInputEndPortsLastIndex(node)+1;
                            var module= new iCS_Aggregate(node.Name, priority, nbParams);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            InvokeAddChildIfExists(parent, module);                                
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceFunction: {
                            // Create method.
                            var method= new iCS_InstanceFunction(GetMethodBase(node), node.Name, priority, node.NbOfParams);                                
                            myRuntimeNodes[node.InstanceId]= method;
                            InvokeAddChildIfExists(parent, method);
                            break;                            
                        }
                        case iCS_ObjectTypeEnum.Constructor: {
                            // Create function.
                            iCS_Constructor func= new iCS_Constructor(GetMethodBase(node), node.Name, priority, node.NbOfParams);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case iCS_ObjectTypeEnum.TypeCast:
                        case iCS_ObjectTypeEnum.ClassFunction: {
                            // Create function.
                            var func= new iCS_ClassFunction(GetMethodBase(node), node.Name, priority, node.NbOfParams);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceField: {
                            // Create function.
                            FieldInfo fieldInfo= GetFieldInfo(node);
							bool[] portIsOuts= GetPortIsOuts(node);
                            iCS_ActionWithSignature rtField= portIsOuts.Length == 0 ?
                                new iCS_GetInstanceField(fieldInfo, node.Name, priority) as iCS_ActionWithSignature:
                                new iCS_SetInstanceField(fieldInfo, node.Name, priority) as iCS_ActionWithSignature;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;
                        }
                        case iCS_ObjectTypeEnum.ClassField: {
                            // Create function.
							FieldInfo fieldInfo= GetFieldInfo(node);
							bool[] portIsOuts= GetPortIsOuts(node);
                            iCS_ActionWithSignature rtField= portIsOuts.Length == 0 ?
                                new iCS_GetClassField(fieldInfo, node.Name, priority) as iCS_ActionWithSignature:
                                new iCS_SetClassField(fieldInfo, node.Name, priority) as iCS_ActionWithSignature;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;                            
                        }
                        default: {
                            Debug.LogWarning(iCS_Config.ProductName+": Code could not be generated for "+node.ObjectType+" object type.");
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
                    case iCS_ObjectTypeEnum.OutStatePort: {
                        break;
                    }
					case iCS_ObjectTypeEnum.ChildMuxPort: {
						var rtMuxPort= myRuntimeNodes[port.ParentId] as iCS_ISignature;
						if(rtMuxPort == null) break;
                        iCS_EngineObject sourcePort= GetSourceEndPort(port);
						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : null;
						rtMuxPort.GetSignatureDataSource().SetConnection(port.PortIndex, connection);
						break;
					}
                    case iCS_ObjectTypeEnum.InStatePort: {
                        iCS_EngineObject endState= GetParent(port);
                        iCS_EngineObject transitionModule= GetParentNode(GetSourcePort(port));
                        iCS_EngineObject actionModule= null;
                        iCS_EngineObject triggerPort= null;
                        iCS_EngineObject outStatePort= null;
                        iCS_EngineObject guardModule= GetTransitionModuleParts(transitionModule, out actionModule, out triggerPort, out outStatePort);
                        triggerPort= GetSourceEndPort(triggerPort);
                        iCS_ActionWithSignature triggerFunc= IsOutAggregatePort(triggerPort) ? null : myRuntimeNodes[triggerPort.ParentId] as iCS_ActionWithSignature;
                        int triggerIdx= triggerPort.PortIndex;
                        iCS_Transition transition= new iCS_Transition(transitionModule.Name,
                                                                    myRuntimeNodes[endState.InstanceId] as iCS_State,
                                                                    myRuntimeNodes[guardModule.InstanceId] as iCS_Aggregate,
                                                                    triggerFunc, triggerIdx,
                                                                    actionModule != null ? myRuntimeNodes[actionModule.InstanceId] as iCS_Aggregate : null,
                                                                    transitionModule.ExecutionPriority);
                        iCS_State state= myRuntimeNodes[outStatePort.ParentId] as iCS_State;
                        state.AddChild(transition);
                        break;
                    }
                    
                    // Data ports.
                    case iCS_ObjectTypeEnum.OutDynamicPort:
                    case iCS_ObjectTypeEnum.OutFixPort: {
						if(GetParentNode(port).IsKindOfAggregate) break;
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_InstanceFunction, iCS_GetInstanceField, iCS_GetClassField, iCS_SetInstanceField, iCS_SetClassField, iCS_ClassFunction>(parentObj,
                            instanceFunction=> instanceFunction[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            getInstanceField=> getInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            getClassField   => getClassField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            setInstanceField=> setInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            setClassField   => setClassField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                            classFunction   => classFunction[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType)
                        );
                        break;
                    }
                    case iCS_ObjectTypeEnum.InDynamicPort:
                    case iCS_ObjectTypeEnum.InFixPort:
                    case iCS_ObjectTypeEnum.EnablePort: {
						if(GetParentNode(port).IsKindOfAggregate && !(port.IsInputPort && IsEndPort(port))) {
						    break;
					    }
                        // Build connection.
                        iCS_EngineObject sourcePort= GetSourceEndPort(port);
						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : null;
                        // Build initial value.
						object initValue= GetInitialValue(sourcePort);
                        // Set data port.
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_Constructor, iCS_InstanceFunction, iCS_GetInstanceField, iCS_GetClassField, iCS_SetInstanceField, iCS_SetClassField, iCS_ClassFunction, iCS_Aggregate, iCS_Message>(parentObj,
                            constructor=> {
                                constructor[port.PortIndex]= initValue;
                                constructor.SetConnection(port.PortIndex, connection);
                            },
                            instanceFunction=> {
                                instanceFunction[port.PortIndex]= initValue;
                                instanceFunction.SetConnection(port.PortIndex, connection);
                            },
                            getInstanceField=> {
                                getInstanceField[port.PortIndex]= initValue;
                                getInstanceField.SetConnection(port.PortIndex, connection);
                            },
                            getClassField=> {
                                getClassField[port.PortIndex]= initValue;
                                getClassField.SetConnection(port.PortIndex, connection);
                            },
                            setInstanceField=> {
                                setInstanceField[port.PortIndex]= initValue;
                                setInstanceField.SetConnection(port.PortIndex, connection);
                            },
                            setClassField=> {
                                setClassField[port.PortIndex]= initValue;
                                setClassField.SetConnection(port.PortIndex, connection);
                            },
                            classFunction=> {
                                classFunction[port.PortIndex]= initValue;
                                classFunction.SetConnection(port.PortIndex, connection);
                            },
                            aggregate=> {
                                aggregate[port.PortIndex]= initValue;                                
                            },
                            message=> {
                                message[port.PortIndex]= initValue;
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
            if(edObj.IsOutFixPort && edObj.RuntimeType == typeof(bool) && edObj.Name == "trigger") {
                iCS_EngineObject gModule= GetParent(edObj);
                if(gModule.IsTransitionGuard && GetParent(gModule) == transitionModule) {
                    triggerPort= edObj;
                }
            }
            if(edObj.IsInTransitionPort) {
                if(GetParent(edObj) == transitionModule) {
                    outStatePort= GetSourcePort(edObj);
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
	int GetNbOfChildMuxPorts(iCS_EngineObject parentMuxPort) {
        int nbOfChildMuxPorts= 0;
		foreach(var port in EngineObjects) {
            if(port != null && port.IsChildMuxPort && port.ParentId == parentMuxPort.InstanceId) {
                ++nbOfChildMuxPorts;
            }
	    }
	    return nbOfChildMuxPorts;
	}
	int GetInputEndPortsLastIndex(iCS_EngineObject node) {
        int lastIndex= -1;
		iCS_EngineObject[] ports= GetChildPorts(node);
	    for(int i= 0; i < ports.Length; ++i) {
	        var p= ports[i];
	        if(p.IsInputPort && IsEndPort(p) && p.PortIndex > lastIndex) {
	            lastIndex= p.PortIndex;
	        }
	    }
	    return lastIndex;
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
		iCS_Connection connection= null;
        var rtPortGroup= myRuntimeNodes[port.InstanceId] as iCS_ISignature;
		if(rtPortGroup != null) {
			connection= new iCS_Connection(rtPortGroup, rtPortGroup.GetSignatureDataSource().ReturnIndex);							
		} else {
			connection= new iCS_Connection(myRuntimeNodes[port.ParentId] as iCS_ISignature, port.PortIndex);
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
