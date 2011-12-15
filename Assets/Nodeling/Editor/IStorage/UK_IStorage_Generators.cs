using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
    // ======================================================================
    // Runtime code generation
    // ----------------------------------------------------------------------
    public static void GenerateRuntimeCodeCallback(UK_Behaviour behaviour, object _iStorage) {
        Debug.Log("Code generation callback invoked.");
        UK_IStorage iStorage= _iStorage == null ? new UK_IStorage(behaviour) : _iStorage as UK_IStorage;
        iStorage.GenerateRuntimeCode();
        behaviour.SetCodeGenerationAction(null, null);
    }
    // ----------------------------------------------------------------------
    public void GenerateRuntimeCode() {
        // Only generate runtime code for behaviours.
        UK_Behaviour rtBehaviour= Storage as UK_Behaviour;
        if(rtBehaviour == null || EditorObjects.Count == 0) return;
        // Remove any previous runtime data.
        UK_EditorObject edBehaviour= EditorObjects[0];
        if(!edBehaviour.IsBehaviour) {
            Debug.LogError("Could not locate Behaviour object.  Aborting code generation.");
        }
        // Remove any previous runtime object creation.
        rtBehaviour.ClearGeneratedCode();
        // Create all the runtime nodes.
        rtBehaviour.GenerateRuntimeNodes();
        // Connect the runtime nodes.
        ConnectRuntimeChildNodes(edBehaviour);
    }
//    // ----------------------------------------------------------------------
//    void GenerateRuntimeChildNodes(UK_EditorObject edNode, object rtNode) {
//        ForEachChild(edNode,
//            edChild=>{
//                if(edChild.IsNode) {
//                    Vector2 layout= Math3D.Middle(GetPosition(edChild));
//                    switch(edChild.ObjectType) {
//                        case UK_ObjectTypeEnum.StateChart: {
//                            UK_StateChart stateChart= new UK_StateChart(edChild.Name, layout);
//                            TreeCache[edChild.InstanceId].RuntimeObject= stateChart;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, stateChart);
//                            GenerateRuntimeChildNodes(edChild, stateChart);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.State: {
//                            UK_State state= new UK_State(edChild.Name, layout);
//                            TreeCache[edChild.InstanceId].RuntimeObject= state;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, state);
//                            GenerateRuntimeChildNodes(edChild, state);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.TransitionModule: {
//                            GenerateRuntimeChildNodes(edChild, null);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.TransitionGuard:
//                        case UK_ObjectTypeEnum.TransitionAction:
//                        case UK_ObjectTypeEnum.Module: {
//                            UK_Module module= new UK_Module(edChild.Name, layout);                                
//                            TreeCache[edChild.InstanceId].RuntimeObject= module;
//                            if(rtNode != null) UK_Reflection.InvokeAddChildIfExists(rtNode, module);                                
//                            GenerateRuntimeChildNodes(edChild, module);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.InstanceMethod: {
//                            // Create method.
//                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edChild.RuntimeArchive);
//                            if(rtDesc == null) break;
//                            UK_Method method= new UK_Method(edChild.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
//                            TreeCache[edChild.InstanceId].RuntimeObject= method;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, method);
//                            break;                            
//                        }
//                        case UK_ObjectTypeEnum.Conversion:
//                        case UK_ObjectTypeEnum.StaticMethod: {
//                            // Create function.
//                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edChild.RuntimeArchive);
//                            if(rtDesc == null) break;
//                            UK_Function func= new UK_Function(edChild.Name, rtDesc.Method, rtDesc.PortIsOuts, layout);                                
//                            TreeCache[edChild.InstanceId].RuntimeObject= func;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, func);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.InstanceField: {
//                            // Create function.
//                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edChild.RuntimeArchive);
//                            if(rtDesc == null) break;
//                            FieldInfo fieldInfo= rtDesc.Field;
//                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
//                                new UK_GetInstanceField(edChild.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
//                                new UK_SetInstanceField(edChild.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
//                            TreeCache[edChild.InstanceId].RuntimeObject= rtField;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, rtField);
//                            break;
//                        }
//                        case UK_ObjectTypeEnum.StaticField: {
//                            // Create function.
//                            UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edChild.RuntimeArchive);
//                            if(rtDesc == null) break;
//                            FieldInfo fieldInfo= rtDesc.Field;
//                            UK_FunctionBase rtField= rtDesc.PortIsOuts[1] ?
//                                new UK_GetStaticField(edChild.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase:
//                                new UK_SetStaticField(edChild.Name, fieldInfo, rtDesc.PortIsOuts, layout) as UK_FunctionBase;                                
//                            TreeCache[edChild.InstanceId].RuntimeObject= rtField;
//                            UK_Reflection.InvokeAddChildIfExists(rtNode, rtField);
//                            break;                            
//                        }
//                        default: {
//                            Debug.LogWarning("Code could not be generated for "+edChild.ObjectType+" editor object type.");
//                            break;
//                        }
//                    }
//                }
//            }
//        );
//    }
    // ----------------------------------------------------------------------
    void ConnectRuntimeChildNodes(UK_EditorObject edNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    switch(edChild.ObjectType) {
                        case UK_ObjectTypeEnum.StateChart: {
                            ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case UK_ObjectTypeEnum.State: {
                            ForEachChildPort(edChild,
                                p=> {
                                    if(p.IsOutStatePort) {
                                        UK_EditorObject actionModule= null;
                                        UK_EditorObject guardModule= GetTransitionGuardAndAction(p, out actionModule);
                                        UK_EditorObject triggerPort= null;
                                        UK_EditorObject inStatePort= GetInStatePort(p);
                                        UK_EditorObject endState= GetParent(inStatePort);
                                        ForEachChildPort(guardModule,
                                            port=> {
                                                if(port.IsOutStaticModulePort && port.RuntimeType == typeof(bool) && port.Name == "trigger") {
                                                    triggerPort= port;
                                                    return true;
                                                }
                                                return false;
                                            }
                                        );
                                        triggerPort= GetDataConnectionSource(triggerPort);
                                        UK_FunctionBase triggerFunc= triggerPort.IsOutModulePort ? null : GetRuntimeObject(GetParent(triggerPort)) as UK_FunctionBase;
                                        int triggerIdx= triggerPort.PortIndex;
                                        Rect outStatePortPos= GetPosition(p);
                                        Rect inStatePortPos= GetPosition(inStatePort);
                                        Vector2 layout= new Vector2(0.5f*(inStatePortPos.x+outStatePortPos.x), 0.5f*(inStatePortPos.y+outStatePortPos.y));
                                        UK_Transition transition= new UK_Transition(p.Name,
                                                                                    GetRuntimeObject(endState) as UK_State,
                                                                                    GetRuntimeObject(guardModule) as UK_Module,
                                                                                    triggerFunc, triggerIdx,
                                                                                    actionModule != null ? GetRuntimeObject(actionModule) as UK_Module : null,
                                                                                    layout);
                                        UK_State state= GetRuntimeObject(edChild) as UK_State;
                                        state.AddChild(transition);
                                    }
                                }
                            );
                            ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case UK_ObjectTypeEnum.TransitionModule:
                        case UK_ObjectTypeEnum.TransitionGuard:
                        case UK_ObjectTypeEnum.TransitionAction:
                        case UK_ObjectTypeEnum.Module: {
                            ConnectRuntimeChildNodes(edChild);                                
                            break;                            
                        }
                        case UK_ObjectTypeEnum.InstanceMethod: {
                            object[] initValues;
                            UK_Connection[] connections= BuildRuntimeConnectionArray(edChild, out initValues);
                            (TreeCache[edChild.InstanceId].RuntimeObject as UK_Method).SetConnections(connections, initValues);
                            break;
                        }
                        case UK_ObjectTypeEnum.Conversion:
                        case UK_ObjectTypeEnum.StaticMethod: {
                            object[] initValues;
                            UK_Connection[] connections= BuildRuntimeConnectionArray(edChild, out initValues);
                            (TreeCache[edChild.InstanceId].RuntimeObject as UK_Function).SetConnections(connections, initValues);
                            break;
                        }
                        case UK_ObjectTypeEnum.InstanceField: {
                            object[] initValues;
                            UK_Connection[] connections= BuildRuntimeConnectionArray(edChild, out initValues);
                            (TreeCache[edChild.InstanceId].RuntimeObject as UK_FunctionBase).SetConnections(connections, initValues);
                            break;
                        }
                        case UK_ObjectTypeEnum.StaticField: {
                            object[] initValues;
                            UK_Connection[] connections= BuildRuntimeConnectionArray(edChild, out initValues);
                            (TreeCache[edChild.InstanceId].RuntimeObject as UK_FunctionBase).SetConnections(connections, initValues);
                            break;                            
                        }
                        default: {
                            Debug.LogWarning("Code could not be generated for "+edChild.ObjectType+" editor object type.");
                            break;
                        }
                    }
                }
            }
        );        
    }
    // ----------------------------------------------------------------------
    // Builds the runtime parameter array information for the given
    // editor object.
    object[] BuildRuntimePortValueArray(UK_EditorObject edObj, out UK_RuntimeDesc rtDesc) {
        rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
        if(rtDesc == null) {
            Debug.LogWarning("Unable to locate runtime information for: "+edObj.RuntimeArchive);
            return new object[0];
        }
        // Create function parameters.
        int portLen= rtDesc.PortTypes.Length;
        object[] portValues= new object[portLen];
        for(int i= 0; i < portLen; ++i) {
            if(rtDesc.PortIsOuts[i]) {  // outputs
                portValues[i]= UK_Types.DefaultValue(rtDesc.PortTypes[i]);
            } else {                   // inputs
                portValues[i]= GetDefaultValue(rtDesc, i) ?? UK_Types.DefaultValue(rtDesc.PortTypes[i]);
            }
        }        
        return portValues;
    }
    // ----------------------------------------------------------------------
    // Builds the runtime connection array information for the given
    // editor object.
    UK_Connection[] BuildRuntimeConnectionArray(UK_EditorObject edObj, out object[] initValues) {
        UK_RuntimeDesc rtDesc= new UK_RuntimeDesc(edObj.RuntimeArchive);
        if(rtDesc == null) {
            Debug.LogWarning("Unable to locate reflection information for: "+edObj.RuntimeArchive);
            initValues= new object[0];
            return new UK_Connection[0];
        }
        int portLen= rtDesc.PortTypes.Length;
        UK_Connection[] connections= new UK_Connection[portLen];
        initValues= new object[portLen];
        for(int i= 0; i < portLen; ++i) {
            connections[i]= new UK_Connection(null, -1);
            object initValue= UK_Types.DefaultValue(rtDesc.PortTypes[i]);                            
            ForEachChildPort(edObj,
                p=> {
                    if(p.IsDataPort && p.PortIndex == i) {
                        if(p.IsInputPort) {
                            UK_EditorObject sourcePort= GetDataConnectionSource(p);
                            if(sourcePort != null) {
                                // Build connection information.
                                int sourcePortIdx= sourcePort.PortIndex;
                                UK_EditorObject sourceNode= GetParent(sourcePort);
                                UK_FunctionBase rtSourceNode= TreeCache[sourceNode.InstanceId].RuntimeObject as UK_FunctionBase;
                                if(sourcePort != p && !sourcePort.IsModulePort) {
                                    connections[i]= new UK_Connection(rtSourceNode, sourcePortIdx);                                    
                                }
                                // Build initial value for port.
                                UK_RuntimeDesc rtSourceDesc= new UK_RuntimeDesc(sourceNode.RuntimeArchive);
                                initValue= GetDefaultValue(rtSourceDesc, sourcePortIdx) ?? initValue;
                            }
                        }
                        return true;
                    }
                    return false;
                }
            );
            initValues[i]= initValue;
        }
        return connections;
    }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public UK_EditorObject GetDataConnectionSource(UK_EditorObject port) {
        if(port == null || !port.IsDataPort) return null;
        for(UK_EditorObject sourcePort= GetSource(port); sourcePort != null && sourcePort.IsDataPort; sourcePort= GetSource(port)) {
            port= sourcePort;
        }
        return port;
    }
    
    // ----------------------------------------------------------------------
    void DisplayRuntimeDesc(UK_EditorObject obj) {
        UK_RuntimeDesc desc= new UK_RuntimeDesc(obj.RuntimeArchive);
        Debug.Log("Parsing: "+obj.RuntimeArchive);
        Debug.Log("Parsing result:");
        Debug.Log("Company= "+desc.Company+" Package= "+desc.Package+" ClassType= "+desc.ClassType.ToString()+" Name= "+desc.MethodName);
        string paramStr= "";
        for(int i= 0; i < desc.PortTypes.Length; ++i) {
            if(desc.PortIsOuts[i]) paramStr+= "out ";
            paramStr+= desc.PortTypes[i].ToString();
            if(desc.PortDefaultValues[i] != null) {
                paramStr+=":= "+desc.PortDefaultValues[i].ToString();
            }
            paramStr+= ";";
        }
        Debug.Log("Parameters= "+paramStr);
    }
}
