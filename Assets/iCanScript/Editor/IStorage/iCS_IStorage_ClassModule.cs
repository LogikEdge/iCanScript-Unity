using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void ClassModuleCompleteCreation(iCS_EditorObject module) {
        ClassModuleShowAsFieldOutputs(module);
    }
    // ----------------------------------------------------------------------
    public void ClassModuleShowAsFieldOutputs(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.InStaticModulePort);
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
            if(component.IsSetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleShowAsFieldInputs(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.OutStaticModulePort);
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
            if(component.IsGetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject ClassModuleGetPort(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject result= null;
        ForEachChildPort(module,
            port=> {
                if(port.Name == portName && port.ObjectType == objType) {
                    result= port;
                    return true;
                }
                return false;
            }
        );
        return result;
    }
    // ----------------------------------------------------------------------
    void ClassModuleDestroyPortIfNotConnected(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= ClassModuleGetPort(module, portName, objType);
        if(port != null && GetSource(port) == null && FindAConnectedPort(port) == null) {
            DestroyInstance(port.InstanceId);
        }
    }
    // ----------------------------------------------------------------------
    void ClassModuleCreatePortIfNonExisting(iCS_EditorObject module, string portName, Type portType, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= ClassModuleGetPort(module, portName, objType);
        if(port == null) {
            CreatePort(portName, module.InstanceId, portType, objType);                
        }
    }
}