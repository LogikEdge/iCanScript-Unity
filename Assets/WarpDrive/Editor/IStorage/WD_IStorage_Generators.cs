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
                    object rtChild= null;
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.StateChart: {
                            rtChild= new WD_StateChart(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateRuntimeChildNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            rtChild= new WD_State(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateRuntimeChildNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.Module: {
                            if(IsTransitionEntryModule(edChild)) {
                                WD_EditorObject triggerModule= GetTriggerModuleFromTransitionEntryModule(edChild);
                                if(triggerModule != null) {
                                    rtChild= new WD_Module(triggerModule.Name, new object[0], new bool[0]);
                                    TreeCache[triggerModule.InstanceId].RuntimeObject= rtChild;
                                    GenerateRuntimeChildNodes(triggerModule, rtChild);
                                    WD_EditorObject entryAction= GetActionModuleFromTransitionEntryModule(edChild);
                                    if(entryAction != null) {
                                        rtChild= new WD_Module(entryAction.Name, new object[0], new bool[0]);
                                        TreeCache[entryAction.InstanceId].RuntimeObject= rtChild;
                                        GenerateRuntimeChildNodes(entryAction, null);
                                    }
                                }
                                break;
                            } else if(IsTransitionExitModule(edChild)) {
                                rtChild= new WD_Module(edChild.Name, new object[0], new bool[0]);
                                TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                                GenerateRuntimeChildNodes(edChild, rtChild);
                                break;
                            }
                            goto WD_ObjectTypeEnum.Function;
                        }
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.Function: {
                            WD_RuntimeDesc desc= new WD_RuntimeDesc(edChild.RuntimeArchive);
                            if(desc == null) {
                                Debug.LogWarning("Unable to locate runtime information for: "+edChild.RuntimeArchive);
                                break;
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
                            // Create function.
                            if(edChild.ObjectType == WD_ObjectTypeEnum.Module) {
                                rtChild= new WD_Module(edChild.Name, parameters, desc.ParamIsOuts);                                
                            } else {
                                rtChild= new WD_Function(edChild.Name, desc.Method, parameters, desc.ParamIsOuts);                                
                            }
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            // Ask the children to do the same.
                            if(edChild.ObjectType == WD_ObjectTypeEnum.Module) { GenerateRuntimeChildNodes(edChild, rtChild); }
                            break;
                        }
                        case WD_ObjectTypeEnum.Class: {
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
                                        WD_EditorObject transitionEntry= GetTransitionEntryModule(p);
                                        if(transitionEntry != null) {
                                            WD_EditorObject triggerAction= GetTriggerModuleFromTransitionEntryModule(transitionEntry);
                                            WD_EditorObject entryAction= GetActionModuleFromTransitionEntryModule(transitionEntry);
                                            WD_EditorObject exitAction= GetTransitionExitModule(transitionEntry);
                                            WD_Transition transition= new WD_Transition(p.Name, GetRuntimeObject(triggerAction) as WD_FunctionBase);
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
                        case WD_ObjectTypeEnum.Module:
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.Function: {
                            WD_RuntimeDesc desc= new WD_RuntimeDesc(edChild.RuntimeArchive);
                            if(desc == null) {
                                Debug.LogWarning("Unable to locate reflection information for: "+edChild.RuntimeArchive);
                                break;
                            }
                            int paramLen= desc.ParamTypes.Length;
                            WD_Connection[] connections= new WD_Connection[paramLen];
                            for(int i= 0; i < paramLen; ++i) {
                                WD_Connection connection= new WD_Connection(null, -1);
                                ForEachChildPort(edChild,
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
                            (TreeCache[edChild.InstanceId].RuntimeObject as WD_FunctionBase).SetConnections(connections);
                            // Ask the children to generate their connections.
                            if(edChild.ObjectType == WD_ObjectTypeEnum.Module) ConnectRuntimeChildNodes(edChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.Class: {
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
