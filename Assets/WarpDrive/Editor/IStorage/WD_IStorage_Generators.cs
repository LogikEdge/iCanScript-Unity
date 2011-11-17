using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class WD_IStorage {
    // ======================================================================
    // Runtime code generation
    // ----------------------------------------------------------------------
    public void GenerateRuntimeCode() {
        // Only generate runtime code for behaviours.
        WD_Behaviour rtBehaviour= Storage as WD_Behaviour;
        if(rtBehaviour == null || EditorObjects.Count == 0) return;
        // Remove any previous runtime data.
        WD_EditorObject edBehaviour= EditorObjects[0];
        if(!edBehaviour.IsBehaviour) {
            Debug.LogError("Could not locate Behaviour object.  Aborting code generation.");
        }
        // Remove any previous runtime object creation.
        rtBehaviour.ClearGeneratedCode();
        // Create all the runtime nodes.
        GenerateRuntimeChildNodes(edBehaviour, rtBehaviour);
        // Connect the runtime nodes.
        ConnectRuntimeChildNodes(edBehaviour);
    }
    // ----------------------------------------------------------------------
    void GenerateRuntimeChildNodes(WD_EditorObject edNode, object rtNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.StateChart: {
                            WD_StateChart stateChart= new WD_StateChart(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= stateChart;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, stateChart);
                            GenerateRuntimeChildNodes(edChild, stateChart);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            WD_State state= new WD_State(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= state;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, state);
                            GenerateRuntimeChildNodes(edChild, state);
                            break;
                        }
                        case WD_ObjectTypeEnum.TransitionEntry: {
                            GenerateRuntimeChildNodes(edChild, null);
                            break;
                        }
                        case WD_ObjectTypeEnum.TransitionExit: {
                            WD_Module module= new WD_Module(edChild.Name, new object[0], new bool[0]);                                
                            TreeCache[edChild.InstanceId].RuntimeObject= module;
                            if(rtNode != null) WD_Reflection.InvokeAddChildIfExists(rtNode, module);
                            GenerateRuntimeChildNodes(edChild, module);
                            break;                            
                        }
                        case WD_ObjectTypeEnum.Module: {
                            WD_RuntimeDesc desc;
                            object[] parameters= BuildRuntimeParameterArray(edChild, out desc);
                            if(desc == null) break;
                            WD_Module module= new WD_Module(edChild.Name, parameters, desc.ParamIsOuts);                                
                            TreeCache[edChild.InstanceId].RuntimeObject= module;
                            if(rtNode != null) WD_Reflection.InvokeAddChildIfExists(rtNode, module);
                            GenerateRuntimeChildNodes(edChild, module);
                            break;
                        }
                        case WD_ObjectTypeEnum.InstanceMethod: {
                            // Create method.
                            WD_RuntimeDesc desc;
                            object[] parameters= BuildRuntimeParameterArray(edChild, out desc);
                            if(desc == null) break;
                            WD_Method method= new WD_Method(edChild.Name, desc.Method, parameters, desc.ParamIsOuts);                                
                            TreeCache[edChild.InstanceId].RuntimeObject= method;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, method);
                            break;                            
                        }
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.StaticMethod: {
                            // Create function.
                            WD_RuntimeDesc desc;
                            object[] parameters= BuildRuntimeParameterArray(edChild, out desc);
                            if(desc == null) break;
                            WD_Function func= new WD_Function(edChild.Name, desc.Method, parameters, desc.ParamIsOuts);                                
                            TreeCache[edChild.InstanceId].RuntimeObject= func;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, func);
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
    void ConnectRuntimeChildNodes(WD_EditorObject edNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.StateChart: {
                            ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            ForEachChildPort(edChild,
                                p=> {
                                    if(p.IsOutStatePort) {
                                        WD_EditorObject entryModule= GetTransitionEntryModule(p);
                                        if(entryModule != null) {
                                            WD_EditorObject triggerAction= GetTriggerModuleFromTransitionEntryModule(entryModule);
                                            WD_EditorObject entryAction= GetActionModuleFromTransitionEntryModule(entryModule);
                                            WD_EditorObject exitAction= GetTransitionExitModule(entryModule);
                                            WD_Transition transition= new WD_Transition(p.Name,
                                                                                        GetRuntimeObject(triggerAction) as WD_FunctionBase,
                                                                                        GetRuntimeObject(GetEndStateFromTransitionEntryModule(entryModule)) as WD_State);
                                            if(entryAction != null) { transition.AddChild(GetRuntimeObject(entryAction) as WD_Action); }
                                            if(exitAction != null)  { transition.AddChild(GetRuntimeObject(exitAction) as WD_Action); }
                                            WD_State state= GetRuntimeObject(edChild) as WD_State;
                                            state.AddChild(transition);
                                        }
                                    }
                                }
                            );
                            ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.TransitionEntry: {
                            ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.TransitionExit: {
                            break;
                        }
                        case WD_ObjectTypeEnum.Module: {
                            WD_Connection[] connections= BuildRuntimeConnectionArray(edChild);
                            (TreeCache[edChild.InstanceId].RuntimeObject as WD_FunctionBase).SetConnections(connections);
                            ConnectRuntimeChildNodes(edChild);
                            break;                            
                        }
                        case WD_ObjectTypeEnum.InstanceMethod: {
                            // Parameter connections
                            WD_Connection[] paramConnections= BuildRuntimeConnectionArray(edChild);
                            // This connection.
                            WD_Connection thisConnection= new WD_Connection(null, -1);
                            WD_RuntimeDesc desc= new WD_RuntimeDesc(edChild.RuntimeArchive);
                            int thisId= desc.ParamTypes.Length+1;
                            ForEachChildPort(edChild,
                                p=> {
                                    if(p.PortIndex == thisId) {
                                        WD_EditorObject sourcePort= GetSource(p);
                                        if(sourcePort != null) {
                                            WD_EditorObject sourceNode= GetParent(sourcePort);
                                            WD_FunctionBase rtSourceNode= TreeCache[sourceNode.InstanceId].RuntimeObject as WD_FunctionBase;
                                            thisConnection= new WD_Connection(rtSourceNode, sourcePort.PortIndex);
                                        }
                                        return true;
                                    }
                                    return false;
                                }
                            );
                            // Set connections.
                            WD_Method method= TreeCache[edChild.InstanceId].RuntimeObject as WD_Method;
                            method.SetConnections(thisConnection, paramConnections);
                            break;
                            
                        }
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.StaticMethod: {
                            WD_Connection[] connections= BuildRuntimeConnectionArray(edChild);
                            (TreeCache[edChild.InstanceId].RuntimeObject as WD_FunctionBase).SetConnections(connections);
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
    object[] BuildRuntimeParameterArray(WD_EditorObject edObj, out WD_RuntimeDesc desc) {
        desc= new WD_RuntimeDesc(edObj.RuntimeArchive);
        if(desc == null) {
            Debug.LogWarning("Unable to locate runtime information for: "+edObj.RuntimeArchive);
            return new object[0];
        }
        // Create function parameters.
        int paramLen= desc.ParamTypes.Length;
        object[] parameters= new object[paramLen];
        for(int i= 0; i < paramLen; ++i) {
            if(desc.ParamIsOuts[i]) {  // outputs
                parameters[i]= null;
            } else {                   // inputs
                parameters[i]= GetDefaultValue(desc, i) ?? WD_Types.DefaultValue(desc.ParamTypes[i]);
            }
        }        
        return parameters;
    }
    // ----------------------------------------------------------------------
    // Builds the runtime connection array information for the given
    // editor object.
    WD_Connection[] BuildRuntimeConnectionArray(WD_EditorObject edObj) {
        WD_RuntimeDesc desc= new WD_RuntimeDesc(edObj.RuntimeArchive);
        if(desc == null) {
            Debug.LogWarning("Unable to locate reflection information for: "+edObj.RuntimeArchive);
            return new WD_Connection[0];
        }
        int paramLen= desc.ParamTypes.Length;
        WD_Connection[] connections= new WD_Connection[paramLen];
        for(int i= 0; i < paramLen; ++i) {
            WD_Connection connection= new WD_Connection(null, -1);
            ForEachChildPort(edObj,
                p=> {
                    if(p.PortIndex == i) {
                        WD_EditorObject sourcePort= GetSource(p);
                        if(sourcePort != null) {
                            WD_EditorObject sourceNode= GetParent(sourcePort);
                            WD_FunctionBase rtSourceNode= TreeCache[sourceNode.InstanceId].RuntimeObject as WD_FunctionBase;
                            connection= new WD_Connection(rtSourceNode, sourcePort.PortIndex);
                        }
                        return true;
                    }
                    return false;
                }
            );
            connections[i]= connection;
        }
        return connections;
    }
    
    // ----------------------------------------------------------------------
    void DisplayRuntimeDesc(WD_EditorObject obj) {
        WD_RuntimeDesc desc= new WD_RuntimeDesc(obj.RuntimeArchive);
        Debug.Log("Parsing: "+obj.RuntimeArchive);
        Debug.Log("Parsing result:");
        Debug.Log("Company= "+desc.Company+" Package= "+desc.Package+" ClassType= "+desc.ClassType.ToString()+" Name= "+desc.MethodName);
        string paramStr= "";
        for(int i= 0; i < desc.ParamTypes.Length; ++i) {
            if(desc.ParamIsOuts[i]) paramStr+= "out ";
            paramStr+= desc.ParamTypes[i].ToString();
            if(desc.ParamDefaultValues[i] != null) {
                paramStr+=":= "+desc.ParamDefaultValues[i].ToString();
            }
            paramStr+= ";";
        }
        Debug.Log("Parameters= "+paramStr);
    }
}
