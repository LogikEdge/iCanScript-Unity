using UnityEngine;
using System;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Interpretation
    // ----------------------------------------------------------------------
    public void GenerateDynamicCode() {
        // Only generate runtime code for behaviours.
        WD_Behaviour rtBehaviour= Storage as WD_Behaviour;
        if(rtBehaviour == null || EditorObjects.Count == 0) return;
        // Remove any previous runtime data.
        WD_EditorObject edBehaviour= EditorObjects[0];
        if(!edBehaviour.IsBehaviour) {
            Debug.LogError("Could not locate Behaviour object.  Aborting code generation.");
        }
        rtBehaviour.ClearGeneratedCode();
        GenerateDynamicNodeCode(edBehaviour, rtBehaviour);
    }
    void GenerateDynamicNodeCode(WD_EditorObject edNode, object rtNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    object rtChild= null;
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.Module: {
                            rtChild= new WD_Module(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateDynamicNodeCode(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.StateChart: {
                            rtChild= new WD_StateChart(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateDynamicNodeCode(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            rtChild= new WD_State(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateDynamicNodeCode(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.Function: {
                            WD_FunctionDesc desc= WD_DataBase.GetDescriptor(edChild.DescCompany, edChild.DescPackage, edChild.DescName) as WD_FunctionDesc;
                            if(desc != null) {
                                object[] parameters= new object[desc.ParameterTypes.Length];
                                WD_Connection[] connections= new WD_Connection[desc.ParameterTypes.Length];
                                for(int i= 0; i < desc.ParameterTypes.Length; ++i) {
                                    if(desc.ParameterInOuts[i]) {  // outputs
                                        parameters[i]= null;
                                        connections[i]= new WD_Connection(null, 0);                                                                                
                                    } else {  // inputs
//                                      parameters[i]= Convert.ChangeType("1", typeof(float));
                                        parameters[i]= WD_Reflection.GetDefault(desc.ParameterTypes[i]);                                            
                                        WD_Connection connection= new WD_Connection(null, -1);
                                        ForEachChildPort(edChild,
                                            p=> {
                                                if(p.PortIndex == i) {
                                                    WD_EditorObject sourcePort= GetSource(p);
                                                    if(sourcePort != null) {
                                                        WD_EditorObject sourceNode= GetParent(sourcePort);
                                                        WD_Function rtSourceNode= TreeCache[sourceNode.InstanceId].RuntimeObject as WD_Function;
                                                        connection= new WD_Connection(rtSourceNode, sourcePort.PortIndex);
                                                    }
                                                    return true;
                                                }
                                                return false;
                                            }
                                        );
                                        connections[i]= connection;
                                    }
                                }
                                rtChild= new WD_Function(edChild.Name, desc.Method, parameters, connections, null);
                                TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                                WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            } else {
                                Debug.LogWarning("Unable to locate reflection information for: "+edChild.DescCompany+":"+edChild.DescPackage+":"+edChild.DescName);
                            }
                            break;
                        }
                        case WD_ObjectTypeEnum.Conversion: {
                            WD_ConversionDesc desc= WD_DataBase.GetDescriptor(edChild.DescCompany, edChild.DescPackage, edChild.DescName) as WD_ConversionDesc;
                            if(desc != null) {
                                object[] parameters= new object[1];
                                parameters[0]= WD_Reflection.GetDefault(desc.FromType);                                            
                                WD_Connection[] connections= new WD_Connection[1];
                                connections[0]= new WD_Connection(null, -1);
                                ForEachChildPort(edChild,
                                    p=> {
                                        if(p.PortIndex == 0) {
                                            WD_EditorObject sourcePort= GetSource(p);
                                            if(sourcePort != null) {
                                                WD_EditorObject sourceNode= GetParent(sourcePort);
                                                WD_Function rtSourceNode= TreeCache[sourceNode.InstanceId].RuntimeObject as WD_Function;
                                                connections[0]= new WD_Connection(rtSourceNode, sourcePort.PortIndex);
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                                rtChild= new WD_Function(edChild.Name, desc.Method, parameters, connections, null);
                                TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                                WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            } else {
                                Debug.LogWarning("Unable to locate reflection information for: "+edChild.DescCompany+":"+edChild.DescPackage+":"+edChild.DescName);
                            }
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
}
