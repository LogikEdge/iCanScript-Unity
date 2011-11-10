using UnityEngine;
using System;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Runtime code generation
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
        // Remove any previous runtime object creation.
        rtBehaviour.ClearGeneratedCode();
        // Create all the runtime nodes.
        GenerateRuntimeNodes(edBehaviour, rtBehaviour);
        // Connect the runtime nodes.
        ConnectRuntimeNodes(edBehaviour, rtBehaviour);
    }
    // ----------------------------------------------------------------------
    void GenerateRuntimeNodes(WD_EditorObject edNode, object rtNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    object rtChild= null;
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.Module: {
                            rtChild= new WD_Module(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.StateChart: {
                            rtChild= new WD_StateChart(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            rtChild= new WD_State(edChild.Name);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                            GenerateRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.Function: {
                            WD_RuntimeDesc desc= new WD_RuntimeDesc(edChild.DescriptorArchive);
                            if(desc == null) {
                                Debug.LogWarning("Unable to locate reflection information for: "+edChild.DescriptorArchive);
                                break;
                            }
                            int paramLen= desc.ParamTypes.Length;
                            object[] parameters= new object[paramLen];
                            for(int i= 0; i < paramLen; ++i) {
                                if(desc.ParamIsOuts[i]) {  // outputs
                                    parameters[i]= null;
                                } else {                   // inputs
                                    parameters[i]= desc.ParamDefaultValues[i] ?? WD_Types.DefaultValue(desc.ParamTypes[i]);
                                }
                            }
                            rtChild= new WD_Function(edChild.Name, desc.Method, parameters);
                            TreeCache[edChild.InstanceId].RuntimeObject= rtChild;
                            WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
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
    void ConnectRuntimeNodes(WD_EditorObject edNode, object rtNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    object rtChild= null;
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.Module: {
                            ConnectRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.StateChart: {
                            ConnectRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            ConnectRuntimeNodes(edChild, rtChild);
                            break;
                        }
                        case WD_ObjectTypeEnum.Conversion:
                        case WD_ObjectTypeEnum.Function: {
                            WD_RuntimeDesc desc= new WD_RuntimeDesc(edChild.DescriptorArchive);
                            if(desc == null) {
                                Debug.LogWarning("Unable to locate reflection information for: "+edChild.DescriptorArchive);
                                break;
                            }
                            int paramLen= desc.ParamTypes.Length;
                            WD_Connection[] connections= new WD_Connection[paramLen];
                            for(int i= 0; i < paramLen; ++i) {
                                if(desc.ParamIsOuts[i]) {  // outputs
                                    connections[i]= new WD_Connection(null, 0);                                                                                
                                } else {  // inputs
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
                            (TreeCache[edChild.InstanceId].RuntimeObject as WD_Function).Connections= connections;
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
    void ParsingTest(string descStr) {
        WD_RuntimeDesc desc= new WD_RuntimeDesc(descStr);
        Debug.Log("Parsing: "+descStr);
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
