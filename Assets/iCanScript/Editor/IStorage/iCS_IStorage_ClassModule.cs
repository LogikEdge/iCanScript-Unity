using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void ClassModuleCompleteCreation(iCS_EditorObject module) {
        iCS_UserPreferences.UserOnCreateClassModule control= Preferences.OnCreateClassModule;
        if(control.OutputInstanceVariables)  ClassModuleCreateOutputInstanceFields(module);
        if(control.InputInstanceVariables)   ClassModuleCreateInputInstanceFields(module);
        if(control.OutputInstanceProperties) ClassModuleCreateOutputInstanceProperties(module);
        if(control.InputInstanceProperties)  ClassModuleCreateInputInstanceProperties(module);
        if(control.OutputClassVariables)     ClassModuleCreateOutputStaticFields(module);
        if(control.InputClassVariables)      ClassModuleCreateInputStaticFields(module);
        if(control.OutputClassProperties)    ClassModuleCreateOutputStaticProperties(module);
        if(control.InputClassProperties)     ClassModuleCreateInputStaticProperties(module);
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.InStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleDestroyPortIfNotConnected(module, "this", iCS_ObjectTypeEnum.OutStaticModulePort);
        }

        // Automatically create class fields.
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        List<iCS_ReflectionDesc> components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }

    // ======================================================================
    // Utilities.
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