using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using P=Prelude;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void ClassModuleCompleteCreation(iCS_EditorObject module) {
        Fold(module);
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            iCS_EditorObject inThisPort= ClassModuleCreatePortIfNonExisting(module, iCS_Strings.This, classType, iCS_ObjectTypeEnum.InStaticModulePort);
            inThisPort.IsNameEditable= false;
            iCS_EditorObject outThisPort= ClassModuleCreatePortIfNonExisting(module, iCS_Strings.This, classType, iCS_ObjectTypeEnum.OutStaticModulePort);
            outThisPort.IsNameEditable= false;
        }
        if(iCS_PreferencesEditor.InstanceAutocreateOutFields)           ClassModuleCreateOutputInstanceFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInFields)            ClassModuleCreateInputInstanceFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutProperties)       ClassModuleCreateOutputInstanceProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInProperties)        ClassModuleCreateInputInstanceProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutStaticFields)     ClassModuleCreateOutputStaticFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInStaticFields)      ClassModuleCreateInputStaticFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutStaticProperties) ClassModuleCreateOutputStaticProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInStaticProperties)  ClassModuleCreateInputStaticProperties(module);
        
        // Use the class Icon if it exists.
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(module.RuntimeType);
        if(components.Length != 0) {
            var iconGUID= iCS_TextureCache.IconPathToGUID(components[0].IconPath);
            if(iconGUID != null) {
                module.IconGUID= iconGUID;
            }            
        }
        Fold(module);
        SetDirty(module);
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleCreate(module, component);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                ClassModuleDestroy(module, component);
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleCreate(module, component);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticField) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticField) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsGetStaticProperty) {
                ClassModuleDestroy(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticProperty) {
                ClassModuleCreate(module, component);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_ReflectionInfo[] components= iCS_DataBase.GetClassComponents(classType);
        foreach(var component in components) {
            if(component.IsSetStaticProperty) {
                ClassModuleDestroy(module, component);                
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
    iCS_EditorObject ClassModuleGetInputThisPort(iCS_EditorObject module) {
        iCS_EditorObject thisPort= ClassModuleGetPort(module, iCS_Strings.This, iCS_ObjectTypeEnum.InStaticModulePort);
        if(thisPort == null) {
            iCS_EditorObject constructor= ClassModuleGetConstructor(module);
            if(constructor == null) return null;
            thisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.This);
        }
        return thisPort;
    }
    // ----------------------------------------------------------------------
    void ClassModuleDestroyPortIfNotConnected(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= ClassModuleGetPort(module, portName, objType);
        if(port != null && GetSource(port) == null && FindAConnectedPort(port) == null) {
            DestroyInstance(port.InstanceId);
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject ClassModuleCreatePortIfNonExisting(iCS_EditorObject module, string portName, Type portType, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= ClassModuleGetPort(module, portName, objType);
        if(port == null) {
            port= CreatePort(portName, module.InstanceId, portType, objType);                
        }
        return port;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject ClassModuleFindFunction(iCS_EditorObject module, iCS_ReflectionInfo desc) {
        iCS_EditorObject[] children= BuildListOfChildren(
            child=> {
                if(child.ObjectType != desc.ObjectType || child.NbOfParams != desc.ParamTypes.Length) return false;
                return desc.Method != null ? desc.Method == child.GetMethodBase(EditorObjects) : desc.Field == child.GetFieldInfo();
            },
            module);
        return children.Length != 0 ? children[0] : null;
    }

    // ======================================================================
    public iCS_EditorObject ClassModuleCreate(iCS_EditorObject module, iCS_ReflectionInfo desc) {
        if(ClassModuleFindFunction(module, desc) != null) return null;
        Rect moduleRect= GetPosition(module);
        iCS_EditorObject func= CreateMethod(module.InstanceId, new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax), desc);
        ForEachChildDataPort(func,
            port=> {
                string modulePortName= port.Name;
                if(port.Name != iCS_Strings.This && !desc.IsField && !desc.IsProperty) {
                    modulePortName+= "."+desc.DisplayName;
                }
                if(port.IsInputPort) {
                    // Special case for "this".
                    if(port.Name == iCS_Strings.This) {
                        iCS_EditorObject classPort= ClassModuleGetInputThisPort(module);
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
                    if(port.Name == iCS_Strings.This) {
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
        NodeLayout(func);
        return func;
    }
    public void ClassModuleDestroy(iCS_EditorObject module, iCS_ReflectionInfo desc) {
        iCS_EditorObject func= ClassModuleFindFunction(module, desc);
        if(func != null) DestroyInstance(func);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject ClassModuleCreateConstructor(iCS_EditorObject module, iCS_ReflectionInfo desc) {
        ClassModuleDestroyConstructor(module);
        iCS_EditorObject moduleThisPort= ClassModuleGetPort(module, iCS_Strings.This, iCS_ObjectTypeEnum.InStaticModulePort);
        if(moduleThisPort == null) return null;
        Rect thisPos= GetPosition(moduleThisPort); 
        iCS_EditorObject constructor= CreateMethod(module.ParentId, new Vector2(thisPos.x-50f, thisPos.y-20), desc);
        iCS_EditorObject constructorThisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.This);
        SetSource(moduleThisPort, constructorThisPort);
        Minimize(constructor);
        NodeLayout(constructor);
        return constructor;
    }
    public void ClassModuleDestroyConstructor(iCS_EditorObject module) {
        iCS_EditorObject constructor= ClassModuleGetConstructor(module);
        if(constructor == null) return;
        DestroyInstance(constructor);
    }
    public iCS_EditorObject ClassModuleGetConstructor(iCS_EditorObject module) {
        iCS_EditorObject moduleThisPort= ClassModuleGetPort(module, iCS_Strings.This, iCS_ObjectTypeEnum.InStaticModulePort);
        iCS_EditorObject constructorThisPort= GetSource(moduleThisPort);
        if(constructorThisPort == null) return null;
        iCS_EditorObject constructor= GetParent(constructorThisPort);
        return constructor.IsConstructor ? constructor : null;
    }
}