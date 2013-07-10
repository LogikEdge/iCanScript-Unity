using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using P=Prelude;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void InstanceWizardCompleteCreation(iCS_EditorObject module) {
        Fold(module);
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            iCS_EditorObject inThisPort= InstanceWizardCreatePortIfNonExisting(module, iCS_Strings.InstanceObjectName, classType, iCS_ObjectTypeEnum.InFixPort);
            inThisPort.IsNameEditable= false;
        }
        if(iCS_PreferencesEditor.InstanceAutocreateOutFields)           InstanceWizardCreateOutputInstanceFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInFields)            InstanceWizardCreateInputInstanceFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutProperties)       InstanceWizardCreateOutputInstanceProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInProperties)        InstanceWizardCreateInputInstanceProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutClassFields)     InstanceWizardCreateOutputStaticFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInClassFields)      InstanceWizardCreateInputStaticFields(module);
        if(iCS_PreferencesEditor.InstanceAutocreateOutClassProperties) InstanceWizardCreateOutputStaticProperties(module);
        if(iCS_PreferencesEditor.InstanceAutocreateInClassProperties)  InstanceWizardCreateInputStaticProperties(module);
        
        // Use the class Icon if it exists.
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(module.RuntimeType);
        if(components.Length != 0) {
            var iconGUID= iCS_TextureCache.IconPathToGUID(components[0].IconPath);
            if(iconGUID != null) {
                module.IconGUID= iconGUID;
            }            
        }
        Fold(module);
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardCreateOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                InstanceWizardCreate(module, component.ToFieldInfo);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyOutputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceField) {
                InstanceWizardDestroy(module, component.ToFieldInfo);
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void InstanceWizardCreateInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                InstanceWizardCreate(module, component.ToFieldInfo);
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyInputInstanceFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceField) {
                InstanceWizardDestroy(module, component.ToFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardCreateOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetClassField) {
                InstanceWizardCreate(module, component.ToFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyOutputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetClassField) {
                InstanceWizardDestroy(module, component.ToFieldInfo);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void InstanceWizardCreateInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetClassField) {
                InstanceWizardCreate(module, component.ToFieldInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyInputStaticFields(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetClassField) {
                InstanceWizardDestroy(module, component.ToFieldInfo);                
            }
        }        
    }

    // ----------------------------------------------------------------------
    public void InstanceWizardCreateOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                InstanceWizardCreate(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyOutputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetInstanceProperty) {
                InstanceWizardDestroy(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardCreateInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                InstanceWizardCreate(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyInputInstanceProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetInstanceProperty) {
                InstanceWizardDestroy(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardCreateOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetClassProperty) {
                InstanceWizardCreate(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyOutputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsGetClassProperty) {
                InstanceWizardDestroy(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardCreateInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetClassProperty) {
                InstanceWizardCreate(module, component.ToPropertyInfo);                
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void InstanceWizardDestroyInputStaticProperties(iCS_EditorObject module) {
        Type classType= module.RuntimeType;
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(classType);
        foreach(var component in components) {
            if(component.IsSetClassProperty) {
                InstanceWizardDestroy(module, component.ToPropertyInfo);                
            }
        }        
    }

    // ======================================================================
    // Utilities.
    // ----------------------------------------------------------------------
    iCS_EditorObject InstanceWizardGetPort(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
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
    public iCS_EditorObject InstanceWizardGetInputThisPort(iCS_EditorObject module) {
        iCS_EditorObject thisPort= InstanceWizardGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InFixPort);
        if(thisPort == null) {
            iCS_EditorObject constructor= InstanceWizardGetConstructor(module);
            if(constructor == null) return null;
            thisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.InstanceObjectName);
        }
        return thisPort;
    }
    // ----------------------------------------------------------------------
    void InstanceWizardDestroyPortIfNotConnected(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= InstanceWizardGetPort(module, portName, objType);
        if(port != null && port.Source == null && FindAConnectedPort(port) == null) {
            DestroyInstance(port.InstanceId);
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject InstanceWizardCreatePortIfNonExisting(iCS_EditorObject module, string portName, Type portType, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= InstanceWizardGetPort(module, portName, objType);
        if(port == null) {
            port= CreatePort(portName, module.InstanceId, portType, objType);                
        }
        return port;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardFindFunction(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject[] children= BuildListOfChildren(
            child=> {
                if(child.ObjectType != desc.ObjectType || child.NbOfParams != desc.Parameters.Length) {
                    return false;
                }
                return desc.IsMethod ? desc.ToMethodInfo.Method == child.GetMethodBase(EditorObjects) : desc.ToFieldInfo.Field == child.GetFieldInfo();
            },
            module);
        return children.Length != 0 ? children[0] : null;
    }

    // ======================================================================
    public iCS_EditorObject InstanceWizardCreate(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        if(InstanceWizardFindFunction(module, desc) != null) return null;
        Rect moduleRect= module.LayoutRect;
        iCS_EditorObject func= CreateFunction(module.InstanceId, new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax), desc);
        ForEachChildDataPort(func,
            port=> {
                string modulePortName= port.Name;
                if(port.Name != iCS_Strings.InstanceObjectName && !desc.IsField && !desc.IsProperty) {
                    modulePortName+= "."+desc.DisplayName;
                }
                if(port.IsInputPort) {
                    // Special case for "this".
                    if(port.Name == iCS_Strings.InstanceObjectName) {
                        iCS_EditorObject classPort= InstanceWizardGetInputThisPort(module);
                        if(classPort != null) {
                            SetSource(port, classPort);
                        } else {
                            Debug.LogWarning("iCanScript: Unable to find 'this' input port in class module: "+module.Name);
                        }
                    } else {
                        iCS_EditorObject classPort= InstanceWizardGetPort(module, modulePortName, iCS_ObjectTypeEnum.InDynamicPort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicPort);
                            SetSource(port, classPort);
                        } else {
                            SetSource(port, classPort);
                        }
                    }
                } else {
                    // Special case for "this".
                    if(port.Name == iCS_Strings.InstanceObjectName) {
                    } else {
                        iCS_EditorObject classPort= InstanceWizardGetPort(module, modulePortName, iCS_ObjectTypeEnum.OutDynamicPort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicPort);
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
    public void InstanceWizardDestroy(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject func= InstanceWizardFindFunction(module, desc);
        if(func != null) DestroyInstance(func);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardCreateConstructor(iCS_EditorObject module, iCS_ConstructorInfo desc) {
        InstanceWizardDestroyConstructor(module);
        iCS_EditorObject moduleThisPort= InstanceWizardGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InFixPort);
        if(moduleThisPort == null) return null;
        Rect thisPos= moduleThisPort.LayoutRect; 
        iCS_EditorObject constructor= CreateFunction(module.ParentId, new Vector2(thisPos.x-50f, thisPos.y-20), desc);
        iCS_EditorObject constructorThisPort= FindInChildren(constructor, port=> port.IsOutDataPort && port.Name == iCS_Strings.InstanceObjectName);
        SetSource(moduleThisPort, constructorThisPort);
        Iconize(constructor);
        return constructor;
    }
    public void InstanceWizardDestroyConstructor(iCS_EditorObject module) {
        iCS_EditorObject constructor= InstanceWizardGetConstructor(module);
        if(constructor == null) return;
        DestroyInstance(constructor);
    }
    public iCS_EditorObject InstanceWizardGetConstructor(iCS_EditorObject module) {
        iCS_EditorObject moduleThisPort= InstanceWizardGetPort(module, iCS_Strings.InstanceObjectName, iCS_ObjectTypeEnum.InFixPort);
        iCS_EditorObject constructorThisPort= moduleThisPort.Source;
        if(constructorThisPort == null) return null;
        iCS_EditorObject constructor= constructorThisPort.Parent;
        return constructor.IsConstructor ? constructor : null;
    }
}