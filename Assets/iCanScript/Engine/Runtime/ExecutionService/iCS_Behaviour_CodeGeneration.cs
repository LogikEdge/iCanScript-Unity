using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public sealed partial class iCS_Behaviour : iCS_Storage {
    // ======================================================================
    // Code Generation
    // ----------------------------------------------------------------------
    public void GenerateCode() {
        Debug.Log("iCanScript: Generating real-time code for "+gameObject.name+"...");
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
        if(EditorObjects.Count != myRuntimeNodes.Length) {
            myRuntimeNodes= new iCS_Object[EditorObjects.Count];
        }
        bool needAdditionalPass= false;
        do {
            // Assume that this is the last pass.
            needAdditionalPass= false;
            // Generate all runtime nodes.
            foreach(var node in EditorObjects) {
                if(node.IsNode) {
                    // Was already generated in a previous pass.
                    if(myRuntimeNodes[node.InstanceId] != null) continue;
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
                            iCS_EditorObject edParent= GetParent(node);
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
                    Vector2 layout= Math3D.Middle(GetPosition(node));
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
                        case iCS_ObjectTypeEnum.Conversion:
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
                            iCS_FunctionBase rtField= portIsOuts[1] ?
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
                            iCS_FunctionBase rtField= portIsOuts[1] ?
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
        foreach(var port in EditorObjects) {
            if(port.IsPort) {
                switch(port.ObjectType) {
                    // Ports without code generation.
                    case iCS_ObjectTypeEnum.InTransitionPort:
                    case iCS_ObjectTypeEnum.OutTransitionPort:
                    case iCS_ObjectTypeEnum.InDynamicModulePort:
                    case iCS_ObjectTypeEnum.OutDynamicModulePort:
                    case iCS_ObjectTypeEnum.InStaticModulePort:
                    case iCS_ObjectTypeEnum.OutStaticModulePort: {
                        break;
                    }

                    // State transition ports.
                    case iCS_ObjectTypeEnum.OutStatePort: {
                        break;
                    }
                    case iCS_ObjectTypeEnum.InStatePort: {
                        iCS_EditorObject endState= GetParent(port);
                        iCS_EditorObject transitionModule= GetParent(GetSource(port));
                        iCS_EditorObject actionModule= null;
                        iCS_EditorObject triggerPort= null;
                        iCS_EditorObject outStatePort= null;
                        iCS_EditorObject guardModule= GetTransitionModuleParts(transitionModule, out actionModule, out triggerPort, out outStatePort);
                        triggerPort= GetDataConnectionSource(triggerPort);
                        iCS_FunctionBase triggerFunc= triggerPort.IsOutModulePort ? null : myRuntimeNodes[triggerPort.ParentId] as iCS_FunctionBase;
                        int triggerIdx= triggerPort.PortIndex;
                        Rect outStatePortPos= GetPosition(outStatePort);
                        Rect inStatePortPos= GetPosition(port);
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
                    case iCS_ObjectTypeEnum.OutFieldPort:
                    case iCS_ObjectTypeEnum.OutPropertyPort:
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
                    case iCS_ObjectTypeEnum.InFieldPort:
                    case iCS_ObjectTypeEnum.InPropertyPort:
                    case iCS_ObjectTypeEnum.InFunctionPort:
                    case iCS_ObjectTypeEnum.EnablePort: {
                        // Build connection.
                        iCS_EditorObject sourcePort= GetDataConnectionSource(port);
                        iCS_Connection connection= sourcePort == port ?
                                iCS_Connection.NoConnection :
                                new iCS_Connection(myRuntimeNodes[sourcePort.ParentId] as iCS_FunctionBase, sourcePort.PortIndex);
                        // Build initial value.
						iCS_Coder coder= new iCS_Coder(sourcePort.InitialValueArchive);
						object initValue= coder.DecodeObjectForKey("InitialValue") ?? iCS_Types.DefaultValue(sourcePort.RuntimeType);
                        // Set data port.
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_Constructor, iCS_Method, iCS_GetInstanceField, iCS_GetStaticField, iCS_SetInstanceField, iCS_SetStaticField, iCS_Function>(parentObj,
                            constructor=> {
                                constructor[port.PortIndex]= initValue;
                                constructor.SetConnection(port.PortIndex, connection);
                            },
                            method=> {
                                method[port.PortIndex]= initValue;
                                method.SetConnection(port.PortIndex, connection);
                            },
                            getInstanceField=> {
                                getInstanceField[port.PortIndex]= initValue;
                                getInstanceField.SetConnection(port.PortIndex, connection);
                            },
                            getStaticField=> {
                                getStaticField[port.PortIndex]= initValue;
                                getStaticField.SetConnection(port.PortIndex, connection);
                            },
                            setInstanceField=> {
                                setInstanceField[port.PortIndex]= initValue;
                                setInstanceField.SetConnection(port.PortIndex, connection);
                            },
                            setStaticField=> {
                                setStaticField[port.PortIndex]= initValue;
                                setStaticField.SetConnection(port.PortIndex, connection);
                            },
                            function=> {
                                function[port.PortIndex]= initValue;
                                function.SetConnection(port.PortIndex, connection);
                            }
                        );
                        break;
                    }
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject GetTransitionModuleParts(iCS_EditorObject transitionModule, out iCS_EditorObject actionModule,
                                                                               out iCS_EditorObject triggerPort,
                                                                               out iCS_EditorObject outStatePort) {
        iCS_EditorObject guardModule= null;
        actionModule= null;
        triggerPort= null;
        outStatePort= null;
        foreach(var edObj in EditorObjects) {
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
                iCS_EditorObject gModule= GetParent(edObj);
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
	MethodBase GetMethodBase(iCS_EditorObject node) {
        // Extract MethodBase for constructor.
        MethodBase method= null;
		Type classType= node.RuntimeType;
        if(node.ObjectType == iCS_ObjectTypeEnum.Constructor) {
            method= classType.GetConstructor(GetParamTypes(node));
            if(method == null) {
                string signature="(";
                bool first= true;
                foreach(var param in GetParamTypes(node)) {
                    if(first) { first= false; } else { signature+=", "; }
                    signature+= param.Name;
                }
                signature+=")";
                Debug.LogWarning("Unable to extract constructor: "+classType.Name+signature);
            }
            return method;
        }
        // Extract MethodBase for class methods.
        if(node.MethodName == null) return null;
		Type[] paramTypes= GetParamTypes(node);
        method= classType.GetMethod(node.MethodName, paramTypes);            
        if(method == null) {
            string signature="(";
            bool first= true;
            foreach(var param in paramTypes) {
                if(first) { first= false; } else { signature+=", "; }
                signature+= param.Name;
            }
            signature+=")";
            Debug.LogWarning("iCanScript: Unable to extract MethodInfo from RuntimeDesc: "+node.MethodName+signature);
        }
        return method;		
	}
	FieldInfo GetFieldInfo(iCS_EditorObject node) {
        if(node.MethodName == null) return null;
		Type classType= node.RuntimeType;
        FieldInfo field= classType.GetField(node.MethodName);
        if(field == null) {
            Debug.LogWarning("iCanScript: Unable to extract FieldInfo from RuntimeDesc: "+node.MethodName);                
        }
        return field;		
	}
	bool[] GetPortIsOuts(iCS_EditorObject node) {
		iCS_EditorObject[] ports= GetChildPorts(node);
		bool[] isOuts= new bool[ports.Length];
		for(int i= 0; i < isOuts.Length; ++i) {
			isOuts[i]= ports[i].IsOutputPort;
		}
        switch(node.ObjectType) {
            case iCS_ObjectTypeEnum.InstanceMethod: {
				if(node.HasVoidReturn) {
					int len= isOuts.Length+1;
					bool[] tmp= new bool[len];
	                Array.Copy(isOuts, tmp, len-1);
					isOuts= tmp;
					isOuts[len-1]= isOuts[len-2];
					isOuts[len-2]= isOuts[len-3];
					isOuts[len-3]= true;
				}
                break;
            }
            case iCS_ObjectTypeEnum.StaticMethod: {
				if(node.HasVoidReturn) {
					bool[] tmp= new bool[isOuts.Length+1];
	                Array.Copy(isOuts, tmp, isOuts.Length);
					isOuts= tmp;
					isOuts[isOuts.Length-1]= true;
				}
                break;
            }
            case iCS_ObjectTypeEnum.Module:
            case iCS_ObjectTypeEnum.Conversion:
            case iCS_ObjectTypeEnum.Constructor:
            default: {
                break;
            }
		}
		return isOuts;
	}
	Type[] GetParamTypes(iCS_EditorObject node) {
        Type[] result= null;
		Type[] portTypes= GetPortTypes(node);
        switch(node.ObjectType) {
            case iCS_ObjectTypeEnum.Module: {
                result= portTypes;
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod: {
				int paramLen= node.HasVoidReturn ? portTypes.Length-2 : portTypes.Length-3;
                result= new Type[paramLen];
                Array.Copy(portTypes, result, result.Length);
                break;
            }
            case iCS_ObjectTypeEnum.Conversion:
            case iCS_ObjectTypeEnum.Constructor: {
                result= new Type[portTypes.Length-1];
                Array.Copy(portTypes, result, result.Length);
				break;
			}
            case iCS_ObjectTypeEnum.StaticMethod: {
				int paramLen= node.HasVoidReturn ? portTypes.Length : portTypes.Length-1;
                result= new Type[paramLen];
                Array.Copy(portTypes, result, result.Length);
                break;
            }
            default: {
                result= new Type[0]; 
                break;
            }
        }
        return result;		
	}
	Type[] GetPortTypes(iCS_EditorObject node) {
		iCS_EditorObject[] ports= GetChildPorts(node);
		Type[] portTypes= new Type[ports.Length];
		for(int i= 0; i < portTypes.Length; ++i) {
			portTypes[i]= ports[i].RuntimeType;
		}
		return portTypes;		
	}
	iCS_EditorObject[] GetChildPorts(iCS_EditorObject node) {
		List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
		// Get all child data ports.
		int nodeId= node.InstanceId;
		foreach(var port in EditorObjects) {
			if(port.ParentId != nodeId) continue;
			if(!port.IsDataPort) continue;
			ports.Add(port);
		}
		// Sort child ports according to index.
		iCS_EditorObject[] result= ports.ToArray();
		int i= 0;
		for(int retry= 0; i < result.Length && retry < result.Length;) {
			int portIndex= result[i].PortIndex;
			if(portIndex == i) { ++i; continue; }
			if(++retry > result.Length) break;
			iCS_EditorObject tmp= result[portIndex];
			result[portIndex]= result[i];
			result[i]= tmp;
		}
		if(i < result.Length) {
			Debug.LogError("iCanScript: Cannot generate port information for node: "+node.Name);
		}
		return result;
	}

    // ======================================================================
    // Child Management Utilities
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the AddChild method.
    public static MethodInfo GetAddChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(iCS_EngineStrings.AddChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
        MethodInfo methodInfo= objType.GetMethod(iCS_EngineStrings.RemoveChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
