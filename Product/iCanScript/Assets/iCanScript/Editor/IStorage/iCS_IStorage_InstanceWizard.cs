using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Internal.Engine;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ----------------------------------------------------------------------
        void PropertiesWizardCompleteCreation(iCS_EditorObject module) {
            module.Fold();
            Type classType= module.RuntimeType;
            if(!iCS_Types.IsStaticClass(classType)) {
                PropertiesWizardCreatePortIfNonExisting(module, "Target", classType,
                                                      VSObjectType.InFixDataPort, (int)iCS_PortIndex.Target);
            }
            module.Fold();
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject CreateInputInstancePort(Type classType, iCS_EditorObject instanceNode) {
            return PropertiesWizardCreatePortIfNonExisting(instanceNode, "Target", classType, VSObjectType.InFixDataPort, (int)iCS_PortIndex.Target);
        }

        // ======================================================================
        // Utilities.
        // ----------------------------------------------------------------------
        iCS_EditorObject PropertiesWizardGetPort(iCS_EditorObject module, string portName, VSObjectType objType, int portId= -1) {
            iCS_EditorObject result= null;
            UntilMatchingChildPort(module,
                port=> {
                    if(port.DisplayName == portName && port.ObjectType == objType) {
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
        public iCS_EditorObject PropertiesWizardGetObjectAssociatedWithPort(iCS_EditorObject port) {
            if(port.IsTargetOrSelfPort || port.IsControlPort) return port;
            iCS_EditorObject result= port;
            var objectInstance= port.Parent;
            objectInstance.ForEachChildRecursiveDepthFirst(
                c=> {
                    if(c.IsPort) {
                        if(port.IsInputPort) {
                            if(c.ProducerPort == port) result= c.Parent;
                        } else {
                            if(port.ProducerPort == c) result= c.Parent;
                        }                    
                    }
                }
            );
            return result;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject PropertiesWizardGetInputThisPort(iCS_EditorObject module) {
            return FindInputInstancePortOn(module);
        }
        // ----------------------------------------------------------------------
        void PropertiesWizardDestroyPortIfNotConnected(iCS_EditorObject module, string portName, VSObjectType objType) {
            iCS_EditorObject port= PropertiesWizardGetPort(module, portName, objType);
            if(port != null && port.ProducerPort == null && FindAConnectedPort(port) == null) {
                DestroyInstance(port.InstanceId);
            }
        }
        // ----------------------------------------------------------------------
        iCS_EditorObject PropertiesWizardCreatePortIfNonExisting(iCS_EditorObject module, string portName, Type portType,
                                                               VSObjectType objType, int portIdx= -1) {
            iCS_EditorObject port= PropertiesWizardGetPort(module, portName, objType, portIdx);
            if(port == null) {
                port= CreatePort(portName, module.InstanceId, portType, objType);                
                if(portIdx != -1) {
                    port.PortIndex= portIdx;                
                }
            }
            return port;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject PropertiesWizardFindFunction(iCS_EditorObject module, LibraryObject libraryObject) {
            iCS_EditorObject foundNode= null;
            module.ForEachChildNode(
                n=> {
                    if(n.DisplayName == libraryObject.nodeName) {
                        foundNode= n;
                    }
                }
            );
            return foundNode;
        }

        // ======================================================================
        public iCS_EditorObject PropertiesWizardCreate(iCS_EditorObject module, LibraryObject libraryObject) {
            if(PropertiesWizardFindFunction(module, libraryObject) != null) return null;
            Rect moduleRect= module.GlobalRect;
            iCS_EditorObject func= CreateNode(module.InstanceId, libraryObject);
            func.SetInitialPosition(new Vector2(0.5f*(moduleRect.x+moduleRect.xMax), moduleRect.yMax));
            ForEachChildDataPort(func,
                port=> {
                    string modulePortName= port.DisplayName;
                    if(!port.IsTargetOrSelfPort) {
						modulePortName= libraryObject.nodeName;
                    }
                    if(port.IsInputPort) {
                        // Special case for "instance".
                        if(port.IsTargetPort) {
                            iCS_EditorObject classPort= PropertiesWizardGetInputThisPort(module);
                            if(classPort != null) {
                                SetSource(port, classPort);
                            } else {
                                Debug.LogWarning("iCanScript: Unable to find 'this' input port in class module: "+module.DisplayName);
                            }
                        } else {
                            iCS_EditorObject classPort= PropertiesWizardGetPort(module, modulePortName, VSObjectType.InDynamicDataPort);
                            if(classPort == null) {
                                classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, VSObjectType.InDynamicDataPort);
                                SetSource(port, classPort);
                            } else {
                                SetSource(port, classPort);
                            }
                        }
                    } else {
                        // Special case for "instance".
                        if(port.IsTargetOrSelfPort) {
                        } else {
                            iCS_EditorObject classPort= PropertiesWizardGetPort(module, modulePortName, VSObjectType.OutDynamicDataPort);
                            if(classPort == null) {
                                classPort= CreatePort(modulePortName, module.InstanceId, port.RuntimeType, VSObjectType.OutDynamicDataPort);
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
        public void PropertiesWizardDestroy(iCS_EditorObject module, LibraryObject libraryObject) {
            iCS_EditorObject func= PropertiesWizardFindFunction(module, libraryObject);
            if(func != null) DestroyInstance(func);
        }
        // -------------------------------------------------------------------------
        public void PropertiesWizardDestroyAllObjectsAssociatedWithPort(iCS_EditorObject port) {
            var objectInstance= port.ParentNode;
            // Destroy instance ports used as input to field/property/function to delete.
            var toDelete= PropertiesWizardGetObjectAssociatedWithPort(port);
            if(toDelete == null) return;
            var portsToDestroy= new List<iCS_EditorObject>();
            toDelete.ForEachChildPort(
                p=> {
                    if(p.IsInDataPort && !p.IsTargetPort) {
                        var source= p.ProducerPort;
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
                        var source= p.ProducerPort;
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
        public iCS_EditorObject FindInputInstancePortOn(iCS_EditorObject module) {
            var portId= (int)iCS_PortIndex.Target;
            iCS_EditorObject result= null;
            UntilMatchingChildPort(module,
                port=> {
                    if(port.PortIndex == portId) {
                        result= port;
                        return true;
                    }
                    return false;
                }
            );
            return result;
        }
        // ----------------------------------------------------------------------
        iCS_EditorObject FindOutputInstancePortOn(iCS_EditorObject module) {
            var portId= (int)iCS_PortIndex.Self;
            iCS_EditorObject result= null;
            UntilMatchingChildPort(module,
                port=> {
                    if(port.PortIndex == portId) {
                        result= port;
                        return true;
                    }
                    return false;
                }
            );
            return result;
        
        }
    }

}