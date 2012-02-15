using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using P=Prelude;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void ClassModuleCompleteCreation(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.InStaticModulePort);
            ClassModuleCreatePortIfNonExisting(module, "this", classType, iCS_ObjectTypeEnum.OutStaticModulePort);            
        }
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
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
                System.Object constantValue= component.Field.GetRawConstantValue();
                if(constantValue != null) Debug.Log(constantValue.GetType().Name);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
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
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ReturnType, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleDestroyPortIfNotConnected(module, component.FieldName, iCS_ObjectTypeEnum.OutStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticProperty) {
                ClassModuleCreatePortIfNonExisting(module, component.FieldName, component.ParamTypes[0], iCS_ObjectTypeEnum.InStaticModulePort);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(classType);
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
    // ----------------------------------------------------------------------
    public iCS_EditorObject ClassModuleFindFunction(iCS_EditorObject module, iCS_ReflectionDesc desc) {
        iCS_EditorObject[] children= BuildListOfChildren(
            child=> {
                if(child.ObjectType != desc.ObjectType || child.NbOfParams != desc.ParamTypes.Length) return false;
                return desc.Method != null ? desc.Method == child.GetMethodBase(EditorObjects) : desc.Field == child.GetFieldInfo();
            },
            module);
        return children.Length != 0 ? children[0] : null;
    }

    // ======================================================================
    public void ClassModuleCreate(iCS_EditorObject module, iCS_ReflectionDesc desc) {
        if(ClassModuleFindFunction(module, desc) != null) return;
        Rect moduleRect= GetPosition(module);
        iCS_EditorObject func= CreateMethod(module.InstanceId, new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax), desc);
        ForEachChildDataPort(func,
            port=> {
                string modulePortName= port.Name;
                if(port.Name != "this" && !desc.IsField && !desc.IsProperty) {
                    modulePortName+= "."+desc.DisplayName;
                }
                if(port.IsInputPort) {
                    // Special case for "this".
                    if(port.Name == "this") {
                        iCS_EditorObject classPort= ClassModuleGetPort(module, modulePortName, iCS_ObjectTypeEnum.InStaticModulePort);
                        if(classPort != null) {
                            SetSource(port, classPort);
                        } else {
                            Debug.LogWarning("iCanScript: Unable to find 'this' input port in class module: "+module.Name);
                        }
                    } else {
                        iCS_EditorObject classPort= ClassModuleGetPort(module, modulePortName, iCS_ObjectTypeEnum.InDynamicModulePort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                            SetSource(port, classPort);
                        } else {
                            SetSource(port, classPort);
                        }
                    }
                } else {
                    // Special case for "this".
                    if(port.Name == "this") {
                    } else {
                        iCS_EditorObject classPort= ClassModuleGetPort(module, modulePortName, iCS_ObjectTypeEnum.OutDynamicModulePort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                            SetSource(classPort, port);
                        } else {
                            SetSource(classPort, port);
                        }                        
                    }
                }
            }
        );
        Minimize(func);
    }
    public void ClassModuleDestroy(iCS_EditorObject module, iCS_ReflectionDesc desc) {
        iCS_EditorObject func= ClassModuleFindFunction(module, desc);
        if(func != null) DestroyInstance(func);
    }
}