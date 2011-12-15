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
    object[]    myRuntimeNodes      = null;
    
    // Protocol to allow modification of behaviour while the engine is running.
    Action<UK_Behaviour,object> myCodeGenerationAction= null;
    object                      myCodeGenerationObject= null;
    
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
    public void SetCodeGenerationAction(Action<UK_Behaviour, object> codeGenerationAction, object codeGenerationObject= null) {
        myCodeGenerationAction= codeGenerationAction;
        myCodeGenerationObject= codeGenerationObject;
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
        if(myCodeGenerationAction != null) {
            myCodeGenerationAction(this, myCodeGenerationObject);
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
        if(myCodeGenerationAction != null) {
            myCodeGenerationAction(this, myCodeGenerationObject);
        }
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
        if(myCodeGenerationAction != null) return;
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
        if(myCodeGenerationAction != null) return;
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
    public void ClearGeneratedCode() {
        myRuntimeNodes= null;
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
            foreach(var edObj in EditorObjects) {
                if(edObj.IsNode) {
                    // Was already generated in a previous pass.
                    if(myRuntimeNodes[edObj.InstanceId] != null) continue;
                    // Wait until parent is generated.
                    object parent= null;
                    if(edObj.ParentId != -1) {
                        UK_EditorObject edParent= GetParent(edObj);
                        if(edParent.ObjectType == UK_ObjectTypeEnum.TransitionModule) {
                            edParent= GetParent(edParent);
                        }
                        parent= myRuntimeNodes[edParent.InstanceId];
                        if(parent == null) {
                            needAdditionalPass= true;
                            continue;
                        }                        
                    }
                    // We are ready to generate new node.
                    Vector2 layout= Math3D.Middle(GetPosition(edObj));
                    switch(edObj.ObjectType) {
                        case UK_ObjectTypeEnum.Behaviour: {
                            myRuntimeNodes[edObj.InstanceId]= this;
                            break;
                        }
                        case UK_ObjectTypeEnum.StateChart: {
                            UK_StateChart stateChart= new UK_StateChart(edObj.Name, layout);
                            myRuntimeNodes[edObj.InstanceId]= stateChart;
                            InvokeAddChildIfExists(parent, stateChart);
                            break;
                        }
                        case UK_ObjectTypeEnum.State: {
                            UK_State state= new UK_State(edObj.Name, layout);
                            myRuntimeNodes[edObj.InstanceId]= state;
                            InvokeAddChildIfExists(parent, state);
                            break;
                        }
                        case UK_ObjectTypeEnum.TransitionModule: {
                            break;
                        }
                        case UK_ObjectTypeEnum.TransitionGuard:
                        case UK_ObjectTypeEnum.TransitionAction: {
                            UK_Module module= new UK_Module(edObj.Name, layout);                                
                            myRuntimeNodes[edObj.InstanceId]= module;
                            break;
                        }
                        case UK_ObjectTypeEnum.Module: {
                            UK_Module module= new UK_Module(edObj.Name, layout);                                
                            myRuntimeNodes[edObj.InstanceId]= module;
                            InvokeAddChildIfExists(parent, module);                                
                            break;
                        }
                        case UK_ObjectTypeEnum.InstanceMethod: {
                            // Create method.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
                            if(rtDesc == null) break;
                            UK_Method method= new UK_Method(edObj.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[edObj.InstanceId]= method;
                            InvokeAddChildIfExists(parent, method);
                            break;                            
                        }
                        case UK_ObjectTypeEnum.Conversion:
                        case UK_ObjectTypeEnum.StaticMethod: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
                            if(rtDesc == null) break;
                            UK_Function func= new UK_Function(edObj.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
                            myRuntimeNodes[edObj.InstanceId]= func;
                            InvokeAddChildIfExists(parent, func);
                            break;
                        }
                        case UK_ObjectTypeEnum.InstanceField: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new UK_GetInstanceField(edObj.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
                                new UK_SetInstanceField(edObj.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
                            myRuntimeNodes[edObj.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;
                        }
                        case UK_ObjectTypeEnum.StaticField: {
                            // Create function.
                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
                            if(rtDesc == null) break;
                            FieldInfo fieldInfo= rtDesc.Field;
                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
                                new UK_GetStaticField(edObj.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
                                new UK_SetStaticField(edObj.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
                            myRuntimeNodes[edObj.InstanceId]= rtField;
                            InvokeAddChildIfExists(parent, rtField);
                            break;                            
                        }
                        default: {
                            Debug.LogWarning("Code could not be generated for "+edObj.ObjectType+" editor object type.");
                            break;
                        }
                    }
                }
            }
        } while(needAdditionalPass);
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
