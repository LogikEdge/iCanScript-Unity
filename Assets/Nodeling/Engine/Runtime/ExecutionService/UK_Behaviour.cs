using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_Behaviour : UK_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    UK_Action   myUpdateAction      = null;
    UK_Action   myLateUpdateAction  = null;
    UK_Action   myFixedUpdateAction = null;
    int         myUpdateFrameId     = 0;
    int         myFixedUpdateFrameId= 0;
    object[]    myRuntimeNodes      = new object[0];
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int UpdateFrameId        { get { return myUpdateFrameId; }}
    public int FixedUpdateFrameId   { get { return myFixedUpdateFrameId; }}
    
    // ----------------------------------------------------------------------
    void Reset() {
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
    void Awake() {}

    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        GenerateCode();
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
        UK_Action action= obj as UK_Action;
        if(action == null) return;
        switch(action.Name) {
            case UK_EngineStrings.UpdateAction: {
                myUpdateAction= action;
                break;
            }
            case UK_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= action;
                break;
            }
            case UK_EngineStrings.FixedUpdateAction: {
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
        UK_Action action= obj as UK_Action;
        if(action == null) return;
        switch(action.Name) {
            case UK_EngineStrings.UpdateAction: {
                myUpdateAction= null;
                break;
            }
            case UK_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= null;
                break;
            }
            case UK_EngineStrings.FixedUpdateAction: {
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
            myRuntimeNodes= new UK_Object[EditorObjects.Count];
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
                            UK_EditorObject edParent= GetParent(node);
                            if(edParent.ObjectType == UK_ObjectTypeEnum.TransitionModule) {
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
                        case UK_ObjectTypeEnum.Behaviour: {
                            break;
                        }
                        case UK_ObjectTypeEnum.StateChart: {
                            UK_StateChart stateChart= new UK_StateChart(node.Name, layout);
                            myRuntimeNodes[node.InstanceId]= stateChart;
                            InvokeAddChildIfExists(parent, stateChart);
                            break;
                        }
                        case UK_ObjectTypeEnum.State: {
                            UK_State state= new UK_State(node.Name, layout);
                            myRuntimeNodes[node.InstanceId]= state;
                            InvokeAddChildIfExists(parent, state);
                            break;
                        }
                        case UK_ObjectTypeEnum.TransitionModule: {
                            break;
                        }
                        case UK_ObjectTypeEnum.TransitionGuard:
                        case UK_ObjectTypeEnum.TransitionAction: {
                            UK_Module module= new UK_Module(node.Name, layout);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            break;
                        }
                        case UK_ObjectTypeEnum.Module: {
                            UK_Module module= new UK_Module(node.Name, layout);                                
                            myRuntimeNodes[node.InstanceId]= module;
                            InvokeAddChildIfExists(parent, module);                                
                            break;
                        }
                        case UK_ObjectTypeEnum.InstanceMethod: {
                            // Create method.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            UK_Method method= new UK_Method(node.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[node.InstanceId]= method;
                            InvokeAddChildIfExists(parent, method);
                            break;                            
                        }
                        case UK_ObjectTypeEnum.Conversion:
                        case UK_ObjectTypeEnum.StaticMethod: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            UK_Function func= new UK_Function(node.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[node.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case UK_ObjectTypeEnum.InstanceField: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new UK_GetInstanceField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
                                new UK_SetInstanceField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
                            myRuntimeNodes[node.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;
                        }
                        case UK_ObjectTypeEnum.StaticField: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(node.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new UK_GetStaticField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
                                new UK_SetStaticField(node.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
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
                    case UK_ObjectTypeEnum.InTransitionPort:
                    case UK_ObjectTypeEnum.OutTransitionPort:
                    case UK_ObjectTypeEnum.InDynamicModulePort:
                    case UK_ObjectTypeEnum.OutDynamicModulePort:
                    case UK_ObjectTypeEnum.InStaticModulePort:
                    case UK_ObjectTypeEnum.OutStaticModulePort: {
                        break;
                    }

                    // State transition ports.
                    case UK_ObjectTypeEnum.OutStatePort: {
                        break;
                    }
                    case UK_ObjectTypeEnum.InStatePort: {
                        UK_EditorObject endState= GetParent(port);
                        UK_EditorObject transitionModule= GetParent(GetSource(port));
                        UK_EditorObject actionModule= null;
                        UK_EditorObject triggerPort= null;
                        UK_EditorObject outStatePort= null;
                        UK_EditorObject guardModule= GetTransitionModuleParts(transitionModule, out actionModule, out triggerPort, out outStatePort);
                        triggerPort= GetDataConnectionSource(triggerPort);
                        UK_FunctionBase triggerFunc= triggerPort.IsOutModulePort ? null : myRuntimeNodes[triggerPort.ParentId] as UK_FunctionBase;
                        int triggerIdx= triggerPort.PortIndex;
                        Rect outStatePortPos= GetPosition(outStatePort);
                        Rect inStatePortPos= GetPosition(port);
                        Vector2 layout= new Vector2(0.5f*(inStatePortPos.x+outStatePortPos.x), 0.5f*(inStatePortPos.y+outStatePortPos.y));
                        UK_Transition transition= new UK_Transition(transitionModule.Name,
                                                                    myRuntimeNodes[endState.InstanceId] as UK_State,
                                                                    myRuntimeNodes[guardModule.InstanceId] as UK_Module,
                                                                    triggerFunc, triggerIdx,
                                                                    actionModule != null ? myRuntimeNodes[actionModule.InstanceId] as UK_Module : null,
                                                                    layout);
                        UK_State state= myRuntimeNodes[outStatePort.ParentId] as UK_State;
                        state.AddChild(transition);
                        break;
                    }
                    
                    // Data ports.
                    case UK_ObjectTypeEnum.OutFieldPort:
                    case UK_ObjectTypeEnum.OutPropertyPort:
                    case UK_ObjectTypeEnum.OutFunctionPort: {
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<UK_Method, UK_GetInstanceField, UK_GetStaticField, UK_SetInstanceField, UK_SetStaticField, UK_Function>(parentObj,
                            method          => method[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType),
                            getInstanceField=> getInstanceField[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType),
                            getStaticField  => getStaticField[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType),
                            setInstanceField=> setInstanceField[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType),
                            setStaticField  => setStaticField[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType),
                            function        => function[port.PortIndex]= UK_Types.DefaultValue(port.RuntimeType)
                        );
                        break;
                    }
                    case UK_ObjectTypeEnum.InFieldPort:
                    case UK_ObjectTypeEnum.InPropertyPort:
                    case UK_ObjectTypeEnum.InFunctionPort:
                    case UK_ObjectTypeEnum.EnablePort: {
                        // Build connection.
                        UK_EditorObject sourcePort= GetDataConnectionSource(port);
                        UK_Connection connection= sourcePort == port ?
                                UK_Connection.NoConnection :
                                new UK_Connection(myRuntimeNodes[sourcePort.ParentId] as UK_FunctionBase, sourcePort.PortIndex);
                        // Build initial value.
                        UK_EditorObject sourceNode= GetParent(sourcePort);
                        UK_RuntimeDesc rtSourceDesc= new UK_RuntimeDesc(sourceNode.RuntimeArchive);
                        object initValue= rtSourceDesc.GetDefaultValue(sourcePort.PortIndex, this) ?? UK_Types.DefaultValue(sourcePort.RuntimeType);
                        // Set data port.
                        object parentObj= myRuntimeNodes[port.ParentId];
                        Prelude.choice<UK_Method, UK_GetInstanceField, UK_GetStaticField, UK_SetInstanceField, UK_SetStaticField, UK_Function>(parentObj,
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
    UK_EditorObject GetTransitionModuleParts(UK_EditorObject transitionModule, out UK_EditorObject actionModule,
                                                                               out UK_EditorObject triggerPort,
                                                                               out UK_EditorObject outStatePort) {
        UK_EditorObject guardModule= null;
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
                UK_EditorObject gModule= GetParent(edObj);
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
        MethodInfo methodInfo= objType.GetMethod(UK_EngineStrings.AddChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
        MethodInfo methodInfo= objType.GetMethod(UK_EngineStrings.RemoveChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
