using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void CompleteClassModuleCreation(iCS_EditorObject module) {
        ShowAsFieldOutputs(module);
    }
    // ----------------------------------------------------------------------
    void ShowAsFieldOutputs(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            CreatePort("this", module.InstanceId, classType, iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                CreatePort(component.FieldName, module.InstanceId, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    void ShowAsFieldInputs(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            CreatePort("this", module.InstanceId, classType, iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetField && component.ObjectType == iCS_ObjectTypeEnum.InstanceField) {
                CreatePort(component.FieldName, module.InstanceId, component.ReturnType, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
}