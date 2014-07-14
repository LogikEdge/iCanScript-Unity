using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    void InstanceWizardCompleteCreation(iCS_EditorObject module) {
        module.Fold();
        Type classType= module.RuntimeType;
        if(!iCS_Types.IsStaticClass(classType)) {
            iCS_EditorObject inThisPort= InstanceWizardCreatePortIfNonExisting(module, iCS_IStorage.GetInstancePortName(classType), classType,
                                                                               iCS_ObjectTypeEnum.InFixDataPort, (int)iCS_PortIndex.InInstance);
            inThisPort.IsNameEditable= false;
        }
        if(Prefs.InstanceAutocreateOutFields)          InstanceWizardCreateOutputInstanceFields(module);
        if(Prefs.InstanceAutocreateInFields)           InstanceWizardCreateInputInstanceFields(module);
        if(Prefs.InstanceAutocreateOutProperties)      InstanceWizardCreateOutputInstanceProperties(module);
        if(Prefs.InstanceAutocreateInProperties)       InstanceWizardCreateInputInstanceProperties(module);
        if(Prefs.InstanceAutocreateOutClassFields)     InstanceWizardCreateOutputStaticFields(module);
        if(Prefs.InstanceAutocreateInClassFields)      InstanceWizardCreateInputStaticFields(module);
        if(Prefs.InstanceAutocreateOutClassProperties) InstanceWizardCreateOutputStaticProperties(module);
        if(Prefs.InstanceAutocreateInClassProperties)  InstanceWizardCreateInputStaticProperties(module);
        
        // Use the class Icon if it exists.
        iCS_MemberInfo[] components= iCS_LibraryDatabase.GetMembers(module.RuntimeType);
        if(components.Length != 0) {
            var iconGUID= iCS_TextureCache.IconPathToGUID(components[0].IconPath);
            if(iconGUID != null) {
                module.IconGUID= iconGUID;
            }            
        }
        module.Fold();
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
    iCS_EditorObject InstanceWizardGetPort(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType, int portId= -1) {
        iCS_EditorObject result= null;
        UntilMatchingChildPort(module,
            port=> {
                if(port.Name == portName && port.ObjectType == objType) {
                    if(portId != -1 && port.PortIndex != portId) {
                        return false;
                    }
                    result= port;
                    return true;
                }
                return false;
            }
        );
        return result;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardGetObjectAssociatedWithPort(iCS_EditorObject port) {
        if(port.IsInstancePort || port.IsControlPort) return port;
        iCS_EditorObject result= port;
        var objectInstance= port.Parent;
        objectInstance.ForEachChildRecursiveDepthFirst(
            c=> {
                if(c.IsPort) {
                    if(port.IsInputPort) {
                        if(c.ProviderPort == port) result= c.Parent;
                    } else {
                        if(port.ProviderPort == c) result= c.Parent;
                    }                    
                }
            }
        );
        return result;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardGetInputThisPort(iCS_EditorObject module) {
        iCS_EditorObject thisPort= InstanceWizardGetPort(module, iCS_IStorage.GetInstancePortName(module.RuntimeType),
                                                         iCS_ObjectTypeEnum.InFixDataPort, (int)iCS_PortIndex.InInstance);
        if(thisPort == null) {
            iCS_EditorObject constructor= InstanceWizardGetConstructor(module);
            if(constructor == null) return null;
            thisPort= FindInChildren(constructor, port=> port.IsOutputPort && port.RuntimeType == module.RuntimeType);
        }
        return thisPort;
    }
    // ----------------------------------------------------------------------
    void InstanceWizardDestroyPortIfNotConnected(iCS_EditorObject module, string portName, iCS_ObjectTypeEnum objType) {
        iCS_EditorObject port= InstanceWizardGetPort(module, portName, objType);
        if(port != null && port.ProviderPort == null && FindAConnectedPort(port) == null) {
            DestroyInstance(port.InstanceId);
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject InstanceWizardCreatePortIfNonExisting(iCS_EditorObject module, string portName, Type portType,
                                                           iCS_ObjectTypeEnum objType, int portIdx= -1) {
        iCS_EditorObject port= InstanceWizardGetPort(module, portName, objType, portIdx);
        if(port == null) {
            port= CreatePort(portName, module.InstanceId, portType, objType);                
			port.IsNameEditable= false;
            if(portIdx != -1) {
                port.PortIndex= portIdx;                
            }
        }
        return port;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardFindFunction(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject[] children= BuildFilteredListOfChildren(
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
        Rect moduleRect= module.GlobalRect;
        iCS_EditorObject func= CreateFunction(module.InstanceId, desc);
        func.SetInitialPosition(new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax));
        ForEachChildDataPort(func,
            port=> {
                string modulePortName= port.Name;
                if(!port.IsInstancePort) {
                    if(desc.IsField) {
                        modulePortName= (desc as iCS_FieldInfo).FieldName;
                    } else if(desc.IsProperty) {
                        modulePortName= (desc as iCS_PropertyInfo).PropertyName;
                    } else {
                        modulePortName+= "."+desc.DisplayName;                    
                    }
                }
                if(port.IsInputPort) {
                    // Special case for "instance".
                    if(port.IsInInstancePort) {
                        iCS_EditorObject classPort= InstanceWizardGetInputThisPort(module);
                        if(classPort != null) {
                            SetSource(port, classPort);
                        } else {
                            Debug.LogWarning("iCanScript: Unable to find 'this' input port in class module: "+module.Name);
                        }
                    } else {
                        iCS_EditorObject classPort= InstanceWizardGetPort(module, modulePortName, iCS_ObjectTypeEnum.InDynamicDataPort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
							classPort.IsNameEditable= false;
                            SetSource(port, classPort);
                        } else {
                            SetSource(port, classPort);
                        }
                    }
                } else {
                    // Special case for "instance".
                    if(port.IsInstancePort) {
                    } else {
                        iCS_EditorObject classPort= InstanceWizardGetPort(module, modulePortName, iCS_ObjectTypeEnum.OutDynamicDataPort);
                        if(classPort == null) {
                            classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
							classPort.IsNameEditable= false;
                            SetSource(classPort, port);
                        } else {
                            SetSource(classPort, port);
                        }                        
                    }
                }
            }
        );
        func.Iconize();
        return func;
    }
    // -------------------------------------------------------------------------
    public void InstanceWizardDestroy(iCS_EditorObject module, iCS_MethodBaseInfo desc) {
        iCS_EditorObject func= InstanceWizardFindFunction(module, desc);
        if(func != null) DestroyInstance(func);
    }
    // -------------------------------------------------------------------------
    public void InstanceWizardDestroyAllObjectsAssociatedWithPort(iCS_EditorObject port) {
        var objectInstance= port.ParentNode;
        // Destroy instance ports used as input to field/property/function to delete.
        var toDelete= InstanceWizardGetObjectAssociatedWithPort(port);
        if(toDelete == null) return;
        var portsToDestroy= new List<iCS_EditorObject>();
        toDelete.ForEachChildPort(
            p=> {
                if(p.IsInDataPort && !p.IsInInstancePort) {
                    var source= p.ProviderPort;
                    if(source != null && source.ParentNode == objectInstance) {
                        portsToDestroy.Add(source);
                    }
                }
            }
        );
        // Destroy instance ports relaying output of field/property/function to delete.
        objectInstance.ForEachChildPort(
            p=> {
                if(p.IsOutDataPort) {
                    var source= p.ProviderPort;
                    if(source != null && source.ParentNode == toDelete) {
                        portsToDestroy.Add(p);
                    }
                }
            }
        );
        // Destroy field/property/function.
        DestroyInstance(toDelete);        
        // Destroy instance object associated ports.
        foreach(var p in portsToDestroy) {
            DestroyInstance(p);
        }
    }
    
    // ----------------------------------------------------------------------
    public iCS_EditorObject InstanceWizardCreateConstructor(iCS_EditorObject module, iCS_ConstructorInfo desc) {
        InstanceWizardDestroyConstructor(module);
        iCS_EditorObject moduleThisPort= InstanceWizardGetPort(module, iCS_IStorage.GetInstancePortName(module.RuntimeType),
                                                               iCS_ObjectTypeEnum.InFixDataPort, (int)iCS_PortIndex.InInstance);
        if(moduleThisPort == null) return null;
        Rect thisPos= moduleThisPort.GlobalRect; 
        iCS_EditorObject constructor= CreateFunction(module.ParentId, desc);
        constructor.SetInitialPosition(new Vector2(thisPos.x-75f, thisPos.y));
        iCS_EditorObject constructorThisPort= FindInChildren(constructor, port=> port.IsOutputPort && port.RuntimeType == module.RuntimeType);
		constructorThisPort.IsNameEditable= false;
        SetSource(moduleThisPort, constructorThisPort);
        constructor.Iconize();
        return constructor;
    }
    public void InstanceWizardDestroyConstructor(iCS_EditorObject module) {
        iCS_EditorObject constructor= InstanceWizardGetConstructor(module);
        if(constructor == null) return;
        DestroyInstance(constructor);
    }
    public iCS_EditorObject InstanceWizardGetConstructor(iCS_EditorObject module) {
        iCS_EditorObject moduleThisPort= InstanceWizardGetPort(module, iCS_IStorage.GetInstancePortName(module.RuntimeType),
                                                               iCS_ObjectTypeEnum.InFixDataPort, (int)iCS_PortIndex.InInstance);
        if(moduleThisPort == null) return null;
        iCS_EditorObject constructorThisPort= moduleThisPort.ProviderPort;
        if(constructorThisPort == null) return null;
        iCS_EditorObject constructor= constructorThisPort.Parent;
        return constructor.IsConstructor ? constructor : null;
    }
}