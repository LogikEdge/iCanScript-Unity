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
    
    
    // ======================================================================
    // Graph Updates
    // ----------------------------------------------------------------------
    // Called on every frame.
    void Update() {
        ++myUpdateFrameId;
        if(myUpdateAction != null) {
            do {        
                myUpdateAction.Execute(myUpdateFrameId);
                if(myUpdateAction.IsStalled) {
//                    Debug.LogWarning("Upadte is STALLED. Attempting to unblock.");
                    myUpdateAction.ForceExecute(myUpdateFrameId);
                }                
            } while(!myUpdateAction.IsCurrent(myUpdateFrameId));
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            do {
                myLateUpdateAction.Execute(myUpdateFrameId);                                            
                if(myLateUpdateAction.IsStalled) {
//                    Debug.LogWarning("LateUpadte is STALLED. Attempting to unblock.");
                    myLateUpdateAction.ForceExecute(myUpdateFrameId);
                }
            } while(!myLateUpdateAction.IsCurrent(myUpdateFrameId));
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            do {
                myFixedUpdateAction.Execute(myFixedUpdateFrameId);                                
                if(myFixedUpdateAction.IsStalled) {
//                    Debug.LogWarning("FixedUpadte is STALLED. Attempting to unblock.");
                    myFixedUpdateAction.ForceExecute(myFixedUpdateFrameId);
                }
            } while(!myFixedUpdateAction.IsCurrent(myFixedUpdateFrameId));
        }
    }

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        iCS_Action action= obj as iCS_Action;
        if(action == null) return;
        switch(action.Name) {
            case iCS_EngineStrings.StartAction: {
                myStartAction= action;
                break;
            }
            case iCS_EngineStrings.UpdateAction: {
                myUpdateAction= action;
                break;
            }
            case iCS_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= action;
                break;
            }
            case iCS_EngineStrings.FixedUpdateAction: {
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
            case iCS_EngineStrings.StartAction: {
                myStartAction= null;
                break;
            }
            case iCS_EngineStrings.UpdateAction: {
                myUpdateAction= null;
                break;
            }
            case iCS_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= null;
                break;
            }
            case iCS_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= null;
                break;
            }
            default: {
                break;
            }
        }        
    }

    // ======================================================================
    // Code Generation
    // ----------------------------------------------------------------------
    public void GenerateCode() {
        Debug.Log(gameObject.name+": Generating real-time code...");
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
                            iCS_RuntimeDesc rtDesc= new iCS_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            iCS_Method method= new iCS_Method(node.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[node.InstanceId]= method;
                            InvokeAddChildIfExists(parent, method);
                            break;                            
                        }
                        case iCS_ObjectTypeEnum.Conversion:
                        case iCS_ObjectTypeEnum.StaticMethod: {
                            // Create function.
                            iCS_RuntimeDesc rtDesc= new iCS_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            iCS_Function func= new iCS_Function(node.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case iCS_ObjectTypeEnum.InstanceField: {
                            // Create function.
                            iCS_RuntimeDesc rtDesc= new iCS_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            iCS_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new iCS_GetInstanceField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as iCS_FunctionBase:
                                new iCS_SetInstanceField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as iCS_FunctionBase;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;
                        }
                        case iCS_ObjectTypeEnum.StaticField: {
                            // Create function.
                            iCS_RuntimeDesc rtDesc= new iCS_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            iCS_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new iCS_GetStaticField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as iCS_FunctionBase:
                                new iCS_SetStaticField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as iCS_FunctionBase;                                
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
                        iCS_EditorObject sourceNode= GetParent(sourcePort);
                        iCS_RuntimeDesc rtSourceDesc= new iCS_RuntimeDesc(sourceNode.RuntimeArchive);
                        object initValue= rtSourceDesc.GetDefaultValue(sourcePort.PortIndex, this) ?? iCS_Types.DefaultValue(sourcePort.RuntimeType);
                        // Set data port.
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<iCS_Method, iCS_GetInstanceField, iCS_GetStaticField, iCS_SetInstanceField, iCS_SetStaticField, iCS_Function>(parentObj,
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
