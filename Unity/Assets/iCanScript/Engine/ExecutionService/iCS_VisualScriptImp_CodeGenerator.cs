//#define DEBUG

using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using CompileError  =Prelude.Tuple<int,string>;
using CompileWarning=Prelude.Tuple<int,string>;

public partial class iCS_VisualScriptImp : iCS_MonoBehaviourImp {
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
					Debug.LogWarning("iCanScript: Sanity Check: Object: "+i+" has an invalid instance id of: "+EngineObjects[i].InstanceId);
					EngineObjects[i].Reset();
					continue;
				}
				int parentId= EngineObjects[i].ParentId;
				if(i == 0) {
					if(parentId != -1) {
						sanityNeeded= true;
						EngineObjects[0].ParentId= -1;
						Debug.LogWarning("iCanScript: Sanity: Root object has a parent of: "+parentId);
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
						Debug.LogWarning("iCanScript Sanity: Port: "+parentId+" is the parent of: "+i);
						continue;										
					}				
				}
			}
			if(sanityNeeded) {
				storageCorruption= true;
				Debug.LogWarning("iCanScript: Sanity detected an error.");
			}
		} while(sanityNeeded);
		return storageCorruption;
	}
    // ----------------------------------------------------------------------
	bool IsInBounds(int id) { return id >= 0 && id < EngineObjects.Count; }
	
    // ======================================================================
    // Code Generation
    // ----------------------------------------------------------------------
    public bool GenerateCode() {
#if UNITY_EDITOR
//        Debug.Log("iCanScript: Generating real-time code for "+gameObject.name+"...");            
#endif
		// Verify for storage sanity.
		if(SanityCheck()) {
			Debug.LogWarning("iCanScript: storage corruption has been detected.  Attempting recovery...");
		}
        // Remove any previous runtime object creation.
        ClearGeneratedCode();
        // Create all the runtime nodes.
        var compileErrors= new List<CompileError>();
        var compileWarnings= new List<CompileWarning>();
        GenerateRuntimeNodes(ref compileErrors, ref compileWarnings);
        if( compileErrors.Count != 0) {
            ClearGeneratedCode();
            ShowErrors( ref compileErrors );
            return false;
        }
        if( compileWarnings.Count != 0 ) {
            ShowWarnings( ref compileWarnings );            
        }    
        
        // Connect the runtime nodes.
        ConnectRuntimeNodes(ref compileErrors, ref compileWarnings);
        if( compileErrors.Count != 0) {
            ClearGeneratedCode();
            ShowErrors( ref compileErrors );
            return false;
        }
        if( compileWarnings.Count != 0 ) {
            ShowWarnings( ref compileWarnings );
        }    
#if UNITY_EDITOR
//		Debug.Log("iCanScript: Completed code generation for "+gameObject.name);    
#endif
        return true;
    }
    // ----------------------------------------------------------------------
    private void ShowErrors(ref List<CompileError> compileErrors) {
        foreach(var error in compileErrors ) {
            try {
                Debug.LogError("iCanScript: ERROR: "+GetFullName( EngineObjects[error.Item1] )+" => "+error.Item2 );
            }
            catch(System.Exception) {}
        }
        try {
            Debug.LogError("iCanScript: COMPILE ABORTED !!! Please correct error(s) before restarting the engine.");
        }
        catch(System.Exception) {}        
    }
    // ----------------------------------------------------------------------
    private void ShowWarnings(ref List<CompileError> compileWarnings) {
        foreach(var warning in compileWarnings ) {
            Debug.LogWarning("iCanScript: Warning: "+GetFullName( EngineObjects[warning.Item1] )+" => "+warning.Item2 );
        }
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
    public void GenerateRuntimeNodes(ref List<CompileError> compileErrors, ref List<CompileWarning> compileWarnings) {
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
                try {
                    // Uninitialized.
    				if(node == null) continue;
    				if(node.InstanceId == -1) continue;
                    // Was already generated in a previous pass.
                    if(myRuntimeNodes[node.InstanceId] != null) continue;
                    int priority= node.ExecutionPriority;
    				// Special case for active ports.
    				if(node.IsParentMuxPort) {
    				    var parent= myRuntimeNodes[node.ParentId];
                        if(parent == null) {
                            needAdditionalPass= true;
                        } else {
        					var mux= new iCS_Mux(this, priority, GetNbOfChildMuxPorts(node));
        					myRuntimeNodes[node.InstanceId]= mux;
                            InvokeAddChildIfExists(parent, mux);                        
                        }
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
                                iCS_StateChart stateChart= new iCS_StateChart(this, priority);
                                myRuntimeNodes[node.InstanceId]= stateChart;
                                InvokeAddChildIfExists(parent, stateChart);
                                break;
                            }
                            case iCS_ObjectTypeEnum.State: {
                                iCS_State state= new iCS_State(this, priority);
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
                            case iCS_ObjectTypeEnum.InstanceMessage:
                            case iCS_ObjectTypeEnum.ClassMessage: {
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
                                iCS_Message message= new iCS_Message(this, priority, nbParams);                                
                                myRuntimeNodes[node.InstanceId]= message;
                                InvokeAddChildIfExists(parent, message);                                
                                break;
                            }
							case iCS_ObjectTypeEnum.OnStateEntry:
							case iCS_ObjectTypeEnum.OnStateUpdate:
							case iCS_ObjectTypeEnum.OnStateExit:
                            case iCS_ObjectTypeEnum.TransitionPackage:
                            case iCS_ObjectTypeEnum.Package: {
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
                                var module= new iCS_Package(this, priority, nbParams, nbEnables);                                
                                myRuntimeNodes[node.InstanceId]= module;
                                InvokeAddChildIfExists(parent, module);                                
                                break;
                            }
                            case iCS_ObjectTypeEnum.InstanceFunction: {
                                // Create method.
                                var methodBase= GetMethodBase( node, ref compileErrors );
                                if( methodBase == null ) {
                                    continue;
                                }
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
                                var method= new iCS_InstanceFunction(methodBase, this, priority, nbParams, nbEnables);                                
                                myRuntimeNodes[node.InstanceId]= method;
                                InvokeAddChildIfExists(parent, method);
                                break;                            
                            }
                            case iCS_ObjectTypeEnum.Constructor: {
                                // Create function.
                                var methodBase= GetMethodBase( node, ref compileErrors );
                                if( methodBase == null ) {
                                    continue;
                                }
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
                                iCS_Constructor func= new iCS_Constructor(methodBase, this, priority, nbParams, nbEnables);                                
                                myRuntimeNodes[node.InstanceId]= func;
                                InvokeAddChildIfExists(parent, func);
                                break;
                            }
                            case iCS_ObjectTypeEnum.TypeCast:
                            case iCS_ObjectTypeEnum.ClassFunction: {
                                // Create function.
                                var methodBase= GetMethodBase( node, ref compileErrors );
                                if( methodBase == null ) {
                                    continue;
                                }
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
                                var func= new iCS_ClassFunction(methodBase, this, priority, nbParams, nbEnables);                                
                                myRuntimeNodes[node.InstanceId]= func;
                                InvokeAddChildIfExists(parent, func);
                                break;
                            }
                            case iCS_ObjectTypeEnum.InstanceField: {
                                // Create function.
                                FieldInfo fieldInfo= GetFieldInfo( node, ref compileErrors );
                                if( fieldInfo == null) {
                                    continue;
                                }
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
    							var inDataPorts= GetChildInParameters(node);
                                iCS_ActionWithSignature rtField= inDataPorts.Length == 0 ?
                                    new iCS_GetInstanceField(fieldInfo, this, priority, nbEnables) as iCS_ActionWithSignature:
                                    new iCS_SetInstanceField(fieldInfo, this, priority, nbEnables) as iCS_ActionWithSignature;                                
                                myRuntimeNodes[node.InstanceId]= rtField;
                                InvokeAddChildIfExists(parent, rtField);
                                break;
                            }
                            case iCS_ObjectTypeEnum.ClassField: {
                                // Create function.
    							FieldInfo fieldInfo= GetFieldInfo( node, ref compileErrors );
                                if( fieldInfo == null ) {
                                    continue;
                                }
                                int nbParams;
                                int nbEnables;
                                GetNbOfParameterAndEnablePorts(node, out nbParams, out nbEnables);
    							var inDataPorts= GetChildInParameters(node);
                                iCS_ActionWithSignature rtField= inDataPorts.Length == 0 ?
                                    new iCS_GetClassField(fieldInfo, this, priority, nbEnables) as iCS_ActionWithSignature:
                                    new iCS_SetClassField(fieldInfo, this, priority, nbEnables) as iCS_ActionWithSignature;                                
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
                catch(System.Exception exception) {
                    Debug.LogWarning("iCanScript: Exception in node code generation: "+exception.Message);                    
                }
            }
        } while(needAdditionalPass);
    }
    // ----------------------------------------------------------------------
    public void ConnectRuntimeNodes(ref List<CompileError> compileErrors, ref List<CompileWarning> compileWarnings) {
        foreach(var port in EngineObjects) {
			if(port == null) continue;
			if(port.InstanceId == -1) continue;
            if(port.IsPort) {
                try {
                    switch(port.ObjectType) {
                        // Ports without code generation.
                        case iCS_ObjectTypeEnum.InTransitionPort:
                        case iCS_ObjectTypeEnum.OutTransitionPort:
                        case iCS_ObjectTypeEnum.OutStatePort: {
                            break;
                        }
                        case iCS_ObjectTypeEnum.InChildMuxPort:
    					case iCS_ObjectTypeEnum.OutChildMuxPort: {
    						var rtMuxPort= myRuntimeNodes[port.ParentId] as iCS_ISignature;
    						if(rtMuxPort == null) break;
                            iCS_EngineObject sourcePort= GetSourceEndPort(port);
    						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : null;
    						rtMuxPort.GetSignatureDataSource().SetConnection(port.PortIndex, connection);
    						break;
    					}
                        case iCS_ObjectTypeEnum.InStatePort: {
                            iCS_EngineObject endState= GetParent(port);
							iCS_EngineObject outTransitionPort= GetSourcePort(port);
                            iCS_EngineObject transitionPackage= GetParentNode(outTransitionPort);
                            iCS_EngineObject triggerPort= null;
                            iCS_EngineObject outStatePort= null;
                            GetTransitionPackageParts(transitionPackage, out triggerPort, out outStatePort);
							triggerPort= GetSourceEndPort(triggerPort);
                            iCS_ActionWithSignature triggerFunc= IsOutPackagePort(triggerPort) ? null : myRuntimeNodes[triggerPort.ParentId] as iCS_ActionWithSignature;
                            int triggerIdx= triggerPort.PortIndex;
                            iCS_Transition transition= new iCS_Transition(this,
                                                                        myRuntimeNodes[endState.InstanceId] as iCS_State,
                                                                        myRuntimeNodes[transitionPackage.InstanceId] as iCS_Package,
                                                                        triggerFunc, triggerIdx,
                                                                        transitionPackage.ExecutionPriority);
                            iCS_State state= myRuntimeNodes[outStatePort.ParentId] as iCS_State;
                            state.AddChild(transition);
                            break;
                        }
                        
                        // Control flow ports.
                        case iCS_ObjectTypeEnum.TriggerPort: {
                            var package= myRuntimeNodes[port.ParentId] as iCS_Package;
                            package.Trigger= false;
                            break;
                        }
                        // Data ports.
                        case iCS_ObjectTypeEnum.OutDynamicDataPort:
						case iCS_ObjectTypeEnum.OutProposedDataPort:
                        case iCS_ObjectTypeEnum.OutFixDataPort:
						case iCS_ObjectTypeEnum.OutParentMuxPort: {
							bool isMux= port.ObjectType == iCS_ObjectTypeEnum.OutParentMuxPort;
                            object parentObj= myRuntimeNodes[isMux ? port.InstanceId : port.ParentId];
//							if(port.ObjectType == iCS_ObjectTypeEnum.OutParentMuxPort) {
//								Debug.Log("iCanScript: Setting default value for: "+(parentObj as iCS_Object).FullName+" port idx= "+port.PortIndex);
//							}
                            Prelude.choice<iCS_InstanceFunction, iCS_GetInstanceField, iCS_GetClassField, iCS_SetInstanceField, iCS_SetClassField, iCS_ClassFunction, iCS_Mux>(parentObj,
                                instanceFunction=> instanceFunction[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                                getInstanceField=> getInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                                getClassField   => getClassField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                                setInstanceField=> setInstanceField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                                setClassField   => setClassField[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
                                classFunction   => classFunction[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType),
								muxFunction		=> muxFunction[port.PortIndex]= iCS_Types.DefaultValue(port.RuntimeType)
                            );
                            break;
                        }
                        case iCS_ObjectTypeEnum.InDynamicDataPort:
                        case iCS_ObjectTypeEnum.InProposedDataPort:
                        case iCS_ObjectTypeEnum.InFixDataPort:
                        case iCS_ObjectTypeEnum.EnablePort: {
                            // Build connection.
                            iCS_EngineObject sourcePort= GetSourceEndPort(port);
    						iCS_Connection connection= sourcePort != port ? BuildConnection(sourcePort) : null;
                            // Build initial value.
    						object initValue= GetInitialValue(sourcePort);
							// Automatically build instance object if not specified.
							var parent= GetParentNode(port);
							if(connection == null && initValue == null && port.PortIndex == (int)iCS_PortIndex.This) {
								if(!parent.IsMessage) {
                                    try {
    									initValue= System.Activator.CreateInstance(port.RuntimeType);                                        
                                        compileWarnings.Add( new CompileWarning(parent.InstanceId, "No instance provided.  Generating a default builder.") );
                                    }
                                    catch(System.Exception) {
                                        compileErrors.Add( new CompileError(parent.InstanceId, "No instance provided (this) and no default builder available.") );                                        
                                    }
								} else {
									// Messages 'This' port.
									initValue= this;
								}
							}
							// Refresh the game object values for Message nodes
							if(parent.IsMessage && port.IsInProposedDataPort) {
								if(port.RuntimeType == typeof(GameObject)) {
									initValue= gameObject;
								}
								else {
									var propInfo= typeof(GameObject).GetProperty(port.Name);
									if(propInfo != null) {
										initValue= propInfo.GetValue(gameObject, null);
									}
									else {
										var fieldInfo= typeof(GameObject).GetField(port.Name);
										if(fieldInfo != null) {
											initValue= fieldInfo.GetValue(gameObject);
										}
										else {
                                            compileErrors.Add( new CompileError(parent.InstanceId, "Unable to find property "+port.Name+" of "+gameObject.name) );
										}
									}
								}
							}
                            // Set data port.
                            object parentObj= myRuntimeNodes[port.ParentId];
                            Prelude.choice<iCS_Constructor, iCS_InstanceFunction, iCS_GetInstanceField, iCS_GetClassField, iCS_SetInstanceField, iCS_SetClassField, iCS_ClassFunction, iCS_Package, iCS_Message>(parentObj,
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
                                package=> {
                                    package[port.PortIndex]= initValue;
                                    package.SetConnection(port.PortIndex, connection);
                                },
                                message=> {
                                    message[port.PortIndex]= initValue;
                                }
                            );
                            break;
                        }
                    }
                }
                catch(System.Exception exception) {
                    Debug.LogWarning("iCanScript: Exception thrown in "+GetFullName(port)+" => "+exception.Message);
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    iCS_EngineObject GetTransitionPackageParts(iCS_EngineObject transitionPackage,
											  out iCS_EngineObject triggerPort,
                                              out iCS_EngineObject outStatePort) {
        triggerPort= null;
        outStatePort= null;
        foreach(var edObj in EngineObjects) {
            if(IsTransitionTriggerPort(edObj)) {
                iCS_EngineObject portParent= GetParent(edObj);
                if(portParent == transitionPackage) {
                    triggerPort= edObj;
                }
            }
            if(edObj.IsInTransitionPort) {
                if(GetParent(edObj) == transitionPackage) {
                    outStatePort= GetSourcePort(edObj);
                }
            }
        }
        return transitionPackage;
    }
    // ----------------------------------------------------------------------
	bool IsTransitionTriggerPort(iCS_EngineObject port) {
        return port.IsOutFixDataPort && port.RuntimeType == typeof(bool) && port.Name == "trigger";		
	}

    // ======================================================================
    // Runtime information extraction
    // ----------------------------------------------------------------------
	MethodBase GetMethodBase( iCS_EngineObject node, ref List<CompileError> compileErrors ) {
        var methodBase= node.GetMethodBase(EngineObjects);
        if( methodBase == null ) {
            string typeName= null;
            string assemblyName= null;
            iCS_Types.GetAssemblyQualifiedNameComponents( node.QualifiedType, ref typeName, ref assemblyName );
            compileErrors.Add( new CompileError( node.InstanceId, "Unable to locate type: "+typeName+" from assembly: "+assemblyName));
        }
        return methodBase;
	}
	FieldInfo GetFieldInfo( iCS_EngineObject node, ref List<CompileError> compileErrors ) {
        var fieldInfo= node.GetFieldInfo();
        if( fieldInfo == null ) {
            string typeName= null;
            string assemblyName= null;
            iCS_Types.GetAssemblyQualifiedNameComponents( node.QualifiedType, ref typeName, ref assemblyName );
            compileErrors.Add( new CompileError( node.InstanceId, "Unable to locate type: "+typeName+" from assembly: "+assemblyName));
        }
        return fieldInfo;
	}

    // ======================================================================
    // Port information
	// -------------------------------------------------------------------------
    void GetNbOfParameterAndEnablePorts(iCS_EngineObject node, out int nbParams, out int nbEnables) {
        nbParams= 0;
        nbEnables= 0;
        foreach(var p in EngineObjects) {
            if(p.ParentId == node.InstanceId) {
                if(p.IsParameterPort) {
                    ++nbParams;
                } else if(p.IsEnablePort) {
                    ++nbEnables;
                }
            }
        }
#if DEBUG
		Debug.Log("iCanScript: NbParams= "+nbParams+" NbEnables= "+nbEnables+" => "+GetFullName(node));
#endif
    }
	// -------------------------------------------------------------------------
    int GetNbOfChildMuxPorts(iCS_EngineObject parentMuxPort) {
        int nbOfChildMuxPorts= 0;
		foreach(var port in EngineObjects) {
            if(port != null && port.IsChildMuxPort && port.ParentId == parentMuxPort.InstanceId) {
                ++nbOfChildMuxPorts;
            }
	    }
	    return nbOfChildMuxPorts;
	}
	// -------------------------------------------------------------------------
    Type[] GetParamTypes(iCS_EngineObject node) {
	    return node.GetParamTypes(EngineObjects);
	}
    // -------------------------------------------------------------------------
    iCS_EngineObject[] GetChildInParameters(iCS_EngineObject node) {
		List<iCS_EngineObject> ports= new List<iCS_EngineObject>();
		// Get all child data ports.
		int nodeId= node.InstanceId;
		foreach(var port in EngineObjects) {
			if(port == null) continue;
			if(port.ParentId != nodeId) continue;
			if(!port.IsParameterPort) continue;
            if(!port.IsInputPort) continue;
			ports.Add(port);
		}
		// Sort child ports according to index.
		return ports.ToArray();                
    }
    // ----------------------------------------------------------------------
	object GetInitialValue(iCS_EngineObject port) {
	    if(port.InitialValueArchive == null || port.InitialValueArchive == "") return iCS_Types.DefaultValue(port.RuntimeType);
		iCS_Coder coder= new iCS_Coder(port.InitialValueArchive);
		return coder.DecodeObjectForKey("InitialValue", this) ?? iCS_Types.DefaultValue(port.RuntimeType);
	}
    // ----------------------------------------------------------------------
	iCS_Connection BuildConnection(iCS_EngineObject port) {
		iCS_Connection connection= null;
        var rtPortGroup= myRuntimeNodes[port.InstanceId] as iCS_ISignature;
		if(rtPortGroup != null) {
			connection= new iCS_Connection(rtPortGroup, (int)iCS_PortIndex.Return);	
		} else {
            bool isAlwaysReady= port.IsInputPort;
            bool isControlPort= port.IsControlPort;
			connection= new iCS_Connection(myRuntimeNodes[port.ParentId] as iCS_ISignature, port.PortIndex, isAlwaysReady, isControlPort);
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
