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
            iCS_EditorObject inThisPort= ClassModuleCreatePortIfNonExisting(module, iCS_Strings.InstanceObjectName, classType, iCS_ObjectTypeEnum.InStaticModulePort);
            inThisPort.IsNameEditable= false;
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
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(module.RuntimeType);
        if(components.Length != 0) {
            var iconGUID= iCS_TextureCache.IconPathToGUID(components[0].iconPath);
            if(iconGUID != null) {
                module.IconGUID= iconGUID;
            }            
        }
        Fold(module);
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetInstanceField) {
                ClassModuleCreate(module, component.toFieldInfo);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetInstanceField) {
                ClassModuleDestroy(module, component.toFieldInfo);
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetInstanceField) {
                ClassModuleCreate(module, component.toFieldInfo);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetInstanceField) {
                ClassModuleDestroy(module, component.toFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetClassField) {
                ClassModuleCreate(module, component.toFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetClassField) {
                ClassModuleDestroy(module, component.toFieldInfo);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetClassField) {
                ClassModuleCreate(module, component.toFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetClassField) {
                ClassModuleDestroy(module, component.toFieldInfo);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetInstanceProperty) {
                ClassModuleCreate(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetInstanceProperty) {
                ClassModuleDestroy(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetInstanceProperty) {
                ClassModuleCreate(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetInstanceProperty) {
                ClassModuleDestroy(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetClassProperty) {
                ClassModuleCreate(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isGetClassProperty) {
                ClassModuleDestroy(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleCreateInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetClassProperty) {
                ClassModuleCreate(module, component.toPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClassModuleDestroyInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.isSetClassProperty) {
                ClassModuleDestroy(module, component.toPropertyInfo);                
            }
        }        
    }

    // ======================================================================
    // Utilities.
    // ----------------------------------------------------------------------
    iCS_EditorObject ClassModuleGetPort(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject result= null;
        UntilMatchingChildPort(module,
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
    public iCS_EditorObject ClassModuleGetInputThisPort(iCS_EditorObject module) {
        iCS_EditorObject thisPort= ClassModuleGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InStaticModulePort);
        if(thisPort == null) {
            iCS_EditorObject constructor= ClassModuleGetConstructor(module);
            if(constructor == null) return null;
            thisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.InstanceObjectName);
        }
        return thisPort;
    }
    // ----------------------------------------------------------------------
    void ClassModuleDestroyPortIfNotConnected(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= ClassModuleGetPort(module, portName, objType);
        if(port != null && port.Source == null && FindAConnectedPort(port) == null) {
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
    public iCS_EditorObject ClassModuleFindFunction(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject[] children= BuildListOfChildren(
            child=> {
                if(child.ObjectType != desc.objectType || child.NbOfParams != desc.parameters.Length) {
                    return false;
                }
                return desc.isMethod ? desc.toMethodInfo.method == child.GetMethodBase(EditorObjects) : desc.toFieldInfo.field == child.GetFieldInfo();
            },
            module);
        return children.Length != 0 ? children[0] : null;
    }

    // ======================================================================
    public iCS_EditorObject ClassModuleCreate(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        if(ClassModuleFindFunction(module, desc) != null) return null;
        Rect moduleRect= module.LayoutRect;
        iCS_EditorObject func= CreateMethod(module.InstanceId, new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax), desc);
        ForEachChildDataPort(func,
            port=> {
                string modulePortName= port.Name;
                if(port.Name != iCS_Strings.InstanceObjectName && !desc.isField && !desc.isProperty) {
                    modulePortName+= "."+desc.displayName;
                }
                if(port.IsInputPort) {
                    // Special case for "this".
                    if(port.Name == iCS_Strings.InstanceObjectName) {
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
                    if(port.Name == iCS_Strings.InstanceObjectName) {
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
        Iconize(func);
        return func;
    }
    public void ClassModuleDestroy(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject func= ClassModuleFindFunction(module, desc);
        if(func != null) DestroyInstance(func);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject ClassModuleCreateConstructor(iCS_EditorObject module, iCS_ConstructorInfo desc) {
        ClassModuleDestroyConstructor(module);
        iCS_EditorObject moduleThisPort= ClassModuleGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InStaticModulePort);
        if(moduleThisPort == null) return null;
        Rect thisPos= moduleThisPort.LayoutRect; 
        iCS_EditorObject constructor= CreateMethod(module.ParentId, new Vector2(thisPos.x-50f, thisPos.y-20), desc);
        iCS_EditorObject constructorThisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.InstanceObjectName);
        SetSource(moduleThisPort, constructorThisPort);
        Iconize(constructor);
        return constructor;
    }
    public void ClassModuleDestroyConstructor(iCS_EditorObject module) {
        iCS_EditorObject constructor= ClassModuleGetConstructor(module);
        if(constructor == null) return;
        DestroyInstance(constructor);
    }
    public iCS_EditorObject ClassModuleGetConstructor(iCS_EditorObject module) {
        iCS_EditorObject moduleThisPort= ClassModuleGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InStaticModulePort);
        iCS_EditorObject constructorThisPort= moduleThisPort.Source;
        if(constructorThisPort == null) return null;
        iCS_EditorObject constructor= constructorThisPort.Parent;
        return constructor.IsConstructor ? constructor : null;
    }
}