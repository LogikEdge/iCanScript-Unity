using UnityEngine;
using System.Collections;

public partial class UK_IStorage {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string TransitionTriggerModuleStr= "Trigger";
    const string TransitionEntryActionModuleStr= "Action";
    const string TransitionDataCollectorModuleStr= "Data Collector";
    const string TransitionTriggerPortStr= "trigger";
    const string TransitionExitModuleDummyPortStr= "(unused)";
    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public void CreateTransition(UK_EditorObject outStatePort, UK_EditorObject destState) {
        UK_EditorObject inStatePort= CreatePort("[false]", destState.InstanceId, typeof(void), UK_ObjectTypeEnum.InStatePort);
        Rect portRect= GetPosition(destState);
        SetInitialPosition(inStatePort, new Vector2(portRect.x, portRect.y));
        SetSource(inStatePort, outStatePort);
        UpdatePortEdges(inStatePort, outStatePort);        
        UK_EditorObject entryModule= CreateTransitionEntry(outStatePort);
        Hide(entryModule);
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateTransitionEntry(UK_EditorObject outStatePort) {
        UK_EditorObject mainModule= CreateModule(outStatePort.ParentId, Math3D.ToVector2(GetPosition(outStatePort)), "Transition Entry", UK_ObjectTypeEnum.TransitionEntry);
        UK_EditorObject mainOutPort= CreatePort(TransitionTriggerPortStr, mainModule.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        UK_EditorObject trigger= CreateModule(mainModule.InstanceId, Math3D.ToVector2(GetPosition(mainModule)), TransitionTriggerModuleStr);
        UK_EditorObject triggerOutPort= CreatePort(TransitionTriggerPortStr, trigger.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        trigger.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        mainModule.IsNameEditable= false;
        mainOutPort.IsNameEditable= false;
        trigger.IsNameEditable= false;
        triggerOutPort.IsNameEditable= false;
        outStatePort.Source= mainOutPort.InstanceId;
        mainOutPort.Source= triggerOutPort.InstanceId;
        return mainModule;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateTransitionExit(UK_EditorObject port) {
        UK_EditorObject mainModule= CreateModule(port.ParentId, Math3D.ToVector2(GetPosition(port)), "Transition Exit", UK_ObjectTypeEnum.TransitionExit);
        mainModule.IsNameEditable= false;
        UK_EditorObject mainInPort= CreatePort(TransitionExitModuleDummyPortStr, mainModule.InstanceId, typeof(void), UK_ObjectTypeEnum.InStaticModulePort);
        mainInPort.IsNameEditable= false;
        mainInPort.Source= port.InstanceId;
        return mainModule;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateTransitionEntryAction(UK_EditorObject entryModule) {
        // Validate input parameters.
        if(!IsTransitionEntryModule(entryModule)) {
            Debug.LogError("Transition Entry Action can only be added to a transition entry module");
        }
        // Position the action node just below the trigger node.
        UK_EditorObject triggerModule= GetTriggerModuleFromTransitionEntryModule(entryModule);
        Rect triggerPos= GetPosition(triggerModule);
        float gutter2= 2f*UK_EditorConfig.GutterSize;
        Vector2 initialPos= Math3D.ToVector2(triggerPos)+new Vector2(triggerPos.width+gutter2, triggerPos.height+gutter2);
        // Create the action node.
        UK_EditorObject entryAction= CreateModule(entryModule.InstanceId, initialPos, TransitionEntryActionModuleStr);
        entryAction.IsNameEditable= false;
        UK_EditorObject enablePort= CreateEnablePort(entryAction.InstanceId);
        enablePort.Source= GetTriggerPortFromTransitionTriggerModule(triggerModule).InstanceId;
        return entryAction;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject CreateTransitionDataCollector(UK_EditorObject entryModule) {
        // Validate input parameters.
        if(!IsTransitionEntryModule(entryModule)) {
            Debug.LogError("Transition Entry Action can only be added to a transition entry module");
        }
        // Position the action node just below the trigger node.
        UK_EditorObject triggerModule= GetTriggerModuleFromTransitionEntryModule(entryModule);
        Rect triggerPos= GetPosition(triggerModule);
        float gutter2= 2f*UK_EditorConfig.GutterSize;
        Vector2 initialPos= Math3D.ToVector2(triggerPos)+new Vector2(triggerPos.width+gutter2, 0);        
        // Create the data collector.
        UK_EditorObject dataCollector= CreateModule(entryModule.InstanceId, initialPos, TransitionDataCollectorModuleStr);
        dataCollector.IsNameEditable= false;
        UK_EditorObject enablePort= CreateEnablePort(dataCollector.InstanceId);
        enablePort.Source= GetTriggerPortFromTransitionTriggerModule(triggerModule).InstanceId;        
        // A transition must always exist if a data collector is present.
        UK_EditorObject exitModule= GetTransitionExitModule(entryModule);
        if(exitModule == null) {
            UK_EditorObject outStatePort= GetOutStatePortFromTransitionEntryModule(entryModule);
            UK_EditorObject inStatePort= FindAConnectedPort(outStatePort);
            CreateTransitionExit(inStatePort);
        }
        return dataCollector;
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public bool IsTransitionEntryModule(UK_EditorObject obj) {
        return obj != null && obj.IsTransitionEntry;
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionTriggerModule(UK_EditorObject obj) {
        if(obj == null || !obj.IsModule || obj.Name != TransitionTriggerModuleStr) return false;
        return IsTransitionEntryModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionExitModule(UK_EditorObject obj) {
        return obj != null && obj.IsTransitionExit;
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionDataCollector(UK_EditorObject obj) {
        if(obj == null || !obj.IsModule || obj.Name != TransitionDataCollectorModuleStr) return false;
        return IsTransitionEntryModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionEntryModule(UK_EditorObject obj) {
        if(obj == null) return null;
        if(IsTransitionEntryModule(obj)) return obj;
        if(obj.IsModule) {
            if(IsTransitionExitModule(obj)) {
                UK_EditorObject inStatePort= GetInStatePortFromTransitionExitModule(obj);
                return GetTransitionEntryModule(inStatePort);
            }
            return GetTransitionEntryModule(GetParent(obj));
        }
        if(obj.IsInStatePort) return GetTransitionEntryModule(GetSource(obj));
        if(obj.IsOutStatePort) {
            UK_EditorObject triggerPort= GetSource(obj);
            return triggerPort != null ? GetParent(triggerPort) : null;
        }
        return GetTransitionEntryModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionExitModule(UK_EditorObject obj) {
        if(obj == null) return null;
        if(IsTransitionExitModule(obj)) return obj;
        if(IsTransitionEntryModule(obj)) {
            UK_EditorObject inStatePort= GetInStatePortFromTransitionEntryModule(obj);
            UK_EditorObject exitModulePort= FindAConnectedPort(inStatePort);
            return exitModulePort != null ? GetParent(exitModulePort) : null;
        }
        if(obj.IsModule) {
            return GetTransitionExitModule(GetParent(obj));
        }
        if(obj.IsOutStatePort) return GetTransitionExitModule(FindAConnectedPort(obj));
        if(obj.IsInStatePort) {
            UK_EditorObject exitModulePort= FindAConnectedPort(obj);
            return exitModulePort != null ? GetParent(exitModulePort) : null;
        }
        return GetTransitionExitModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetInStatePortFromTransitionExitModule(UK_EditorObject exitModule) {
        UK_EditorObject inStatePort= null;
        ForEachChildPort(exitModule,
            p=> {
                if(p.IsInModulePort) {
                    UK_EditorObject statePort= GetOtherBridgePort(p);
                    if(statePort != null) inStatePort= statePort;
                }
            }
        );
        return inStatePort;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetOutStatePortFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject triggerPort= GetTriggerPortFromTransitionEntryModule(entryModule);
        if(triggerPort == null) return null;
        UK_EditorObject outStatePort= GetOtherBridgePort(triggerPort);
        return (outStatePort != null && outStatePort.IsOutStatePort) ? outStatePort : null;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetInStatePortFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject outStatePort= GetOutStatePortFromTransitionEntryModule(entryModule);
        return FindAConnectedPort(outStatePort);    
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetEndStateFromTransitionEntryModule(UK_EditorObject entryModule) {
        return GetParent(GetInStatePortFromTransitionEntryModule(entryModule));
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTriggerPortFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject triggerPort= null;
        ForEachChildPort(entryModule,
            port=> {
                if(port.IsOutStaticModulePort && port.RuntimeType == typeof(bool) && port.Name == TransitionTriggerPortStr) {                
                    triggerPort= port;
                    return true;
                }
                return false;
            }
        );        
        return triggerPort;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTriggerModuleFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionTriggerModuleStr) { module= m; return true; } return false; });
        return module;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetActionModuleFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionEntryActionModuleStr) { module= m; return true; } return false; });
        return module;        
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetDataCollectorModuleFromTransitionEntryModule(UK_EditorObject entryModule) {
        UK_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionDataCollectorModuleStr) { module= m; return true; } return false; });
        return module;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTriggerPortFromTransitionTriggerModule(UK_EditorObject triggerModule) {
        UK_EditorObject triggerPort= null;
        ForEachChildPort(triggerModule,
            port=> {
                if(port.IsOutStaticModulePort && port.RuntimeType == typeof(bool) && port.Name == TransitionTriggerPortStr) {
                    triggerPort= port;
                    return true;
                }
                return false;
            }
        );
        return triggerPort;
    }
    // ----------------------------------------------------------------------
    public bool SynchronizeExitModulePorts(UK_EditorObject exitModule) {
        if(!IsTransitionExitModule(exitModule)) {
            Debug.LogWarning("Trying to synchronize a transition data collector with an object that is NOT an transition exit module !!!");
            return false;
        }
        UK_EditorObject entryModule= GetTransitionEntryModule(exitModule);
        UK_EditorObject dataCollector= GetDataCollectorModuleFromTransitionEntryModule(entryModule);
        UK_EditorObject outStatePort= GetOutStatePortFromTransitionEntryModule(entryModule);
        UK_EditorObject inStatePort= FindAConnectedPort(outStatePort);
        return SynchronizeExitModulePorts(dataCollector, exitModule, inStatePort);
    }
    // ----------------------------------------------------------------------
    bool SynchronizeExitModulePorts(UK_EditorObject dataCollector, UK_EditorObject exitModule, UK_EditorObject inStatePort) {
        bool modified= false;
        // Special case for when data collector has no ports.  We need to create a dummy port.
        if(dataCollector == null || !ForEachChildPort(dataCollector, p=> p.IsInDataPort && !p.IsEnablePort)) {
            modified= ForEachChildPort(exitModule,
                p=> {
                    if(p.IsInDataPort && p.Source == inStatePort.InstanceId) {
                        if(!(p.Name == TransitionExitModuleDummyPortStr && p.RuntimeType == typeof(void) && p.IsInStaticModulePort)) {
                            DestroyInstance(p);
                            return true;
                        }
                    }
                    return false;
                }
            );
            if(!ForEachChildPort(exitModule, p=> p.Source == inStatePort.InstanceId)) {
                UK_EditorObject dummyPort= CreatePort(TransitionExitModuleDummyPortStr, exitModule.InstanceId, typeof(void), UK_ObjectTypeEnum.InStaticModulePort);
                SetSource(dummyPort, inStatePort);
                modified= true;
            }
            return modified;
        }
        // Add ports that only exist on the data collector module.
        modified= ForEachChildPort(dataCollector,
            dcPort=> {
                if(dcPort.IsInDataPort && !dcPort.IsEnablePort) {
                    bool found= ForEachChildPort(exitModule, p=> (p.Name == dcPort.Name && p.RuntimeType == dcPort.RuntimeType && p.ObjectType == dcPort.ObjectType));
                    if(found) return false;
                    UK_EditorObject newPort= CreatePort(dcPort.Name, exitModule.InstanceId, dcPort.RuntimeType, dcPort.ObjectType);
                    newPort.IsNameEditable= false;
                    SetSource(newPort, inStatePort);
                    return true;
                }
                return false;
            }
        );
        // Remove ports that only exists on exit module.
        bool rModified= ForEachChildPort(exitModule,
            p=> {
                if(p.IsInDataPort && p.Source == inStatePort.InstanceId) {
                    bool found= ForEachChildPort(dataCollector, dcPort=> (dcPort.Name == p.Name && dcPort.RuntimeType == p.RuntimeType));
                    if(found) return false;
                    DestroyInstance(p);
                    return true;
                }
                return false;
            }
        );
        return modified || rModified;
    }
}
