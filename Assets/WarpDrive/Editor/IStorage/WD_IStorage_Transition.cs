using UnityEngine;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string TransitionTriggerModuleStr= "Trigger";
    const string TransitionEntryActionModuleStr= "Action";
    const string TransitionDataCollectorModuleStr= "Data Collector";
    const string TransitionTriggerPortStr= "trigger";
    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateTransitionEntry(WD_EditorObject port) {
        WD_EditorObject mainModule= CreateModule(port.ParentId, Math3D.ToVector2(GetPosition(port)), "Transition Entry");
        WD_EditorObject mainOutPort= CreatePort(TransitionTriggerPortStr, mainModule.InstanceId, typeof(bool), WD_ObjectTypeEnum.OutStaticModulePort);
        WD_EditorObject trigger= CreateModule(mainModule.InstanceId, Vector2.zero, TransitionTriggerModuleStr);
        WD_EditorObject triggerOutPort= CreatePort(TransitionTriggerPortStr, trigger.InstanceId, typeof(bool), WD_ObjectTypeEnum.OutStaticModulePort);
        mainModule.IsNameEditable= false;
        mainOutPort.IsNameEditable= false;
        trigger.IsNameEditable= false;
        triggerOutPort.IsNameEditable= false;
        port.Source= mainOutPort.InstanceId;
        mainOutPort.Source= triggerOutPort.InstanceId;
        return mainModule;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateTransitionExit(WD_EditorObject port) {
        WD_EditorObject mainModule= CreateModule(port.ParentId, Math3D.ToVector2(GetPosition(port)), "Transition Exit");
        mainModule.IsNameEditable= false;
        WD_EditorObject mainInPort= CreatePort("(unused)", mainModule.InstanceId, typeof(void), WD_ObjectTypeEnum.InStaticModulePort);
        mainInPort.IsNameEditable= false;
        mainInPort.Source= port.InstanceId;
        return mainModule;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateTransitionEntryAction(WD_EditorObject entryModule) {
        if(!IsTransitionEntryModule(entryModule)) {
            Debug.LogError("Transition Entry Action can only be added to a transition entry module");
        }
        WD_EditorObject entryAction= CreateModule(entryModule.InstanceId, Vector2.zero, TransitionEntryActionModuleStr);
        entryAction.IsNameEditable= false;
        WD_EditorObject enablePort= CreateEnablePort(entryAction.InstanceId);
        WD_EditorObject triggerPort= GetTriggerPortFromTransitionEntryModule(entryAction);
        enablePort.Source= triggerPort.Source;
        return entryAction;
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public bool IsTransitionEntryModule(WD_EditorObject obj) {
        if(obj == null || !obj.IsModule) return false;
        return GetOutStatePortFromTransitionEntryModule(obj) != null;
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionTriggerModule(WD_EditorObject obj) {
        if(obj == null || !obj.IsModule || obj.Name != TransitionTriggerModuleStr) return false;
        return IsTransitionEntryModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionExitModule(WD_EditorObject obj) {
        if(obj == null || !obj.IsModule) return false;
        return GetInStatePortFromTransitionExitModule(obj) != null;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetTransitionEntryModule(WD_EditorObject obj) {
        if(obj == null) return null;
        if(obj.IsModule) {
            if(IsTransitionEntryModule(obj)) return obj;
            if(IsTransitionExitModule(obj)) {
                WD_EditorObject inStatePort= GetInStatePortFromTransitionExitModule(obj);
                return GetTransitionEntryModule(inStatePort);
            }
            return GetTransitionEntryModule(GetParent(obj));
        }
        if(obj.IsInStatePort) return GetTransitionEntryModule(GetSource(obj));
        if(obj.IsOutStatePort) {
            WD_EditorObject triggerPort= GetSource(obj);
            return triggerPort != null ? GetParent(triggerPort) : null;
        }
        return GetTransitionEntryModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetTransitionExitModule(WD_EditorObject obj) {
        if(obj == null) return null;
        if(obj.IsModule) {
            if(IsTransitionExitModule(obj)) return obj;
            if(IsTransitionEntryModule(obj)) {
                WD_EditorObject outStatePort= GetOutStatePortFromTransitionEntryModule(obj);
                WD_EditorObject inStatePort= FindAConnectedPort(outStatePort);
                WD_EditorObject exitModulePort= FindAConnectedPort(inStatePort);
                return exitModulePort != null ? GetParent(exitModulePort) : null;
            }
            return GetTransitionExitModule(GetParent(obj));
        }
        if(obj.IsOutStatePort) return GetTransitionExitModule(FindAConnectedPort(obj));
        if(obj.IsInStatePort) {
            WD_EditorObject exitModulePort= FindAConnectedPort(obj);
            return exitModulePort != null ? GetParent(exitModulePort) : null;
        }
        return GetTransitionExitModule(GetParent(obj));
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetInStatePortFromTransitionExitModule(WD_EditorObject exitModule) {
        WD_EditorObject inStatePort= null;
        ForEachChildPort(exitModule,
            p=> {
                if(p.IsInModulePort) {
                    WD_EditorObject statePort= GetOtherBridgePort(p);
                    if(statePort != null) inStatePort= statePort;
                }
            }
        );
        return inStatePort;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetOutStatePortFromTransitionEntryModule(WD_EditorObject entryModule) {
        WD_EditorObject outStatePort= GetOtherBridgePort(GetTriggerPortFromTransitionEntryModule(entryModule));
        return (outStatePort != null && outStatePort.IsOutStatePort) ? outStatePort : null;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetTriggerPortFromTransitionEntryModule(WD_EditorObject entryModule) {
        WD_EditorObject triggerPort= null;
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
    public WD_EditorObject GetTriggerModuleFromTransitionEntryModule(WD_EditorObject entryModule) {
        WD_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionTriggerModuleStr) { module= m; return true; } return false; });
        return module;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetActionModuleFromTransitionEntryModule(WD_EditorObject entryModule) {
        WD_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionEntryActionModuleStr) { module= m; return true; } return false; });
        return module;        
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject GetDataCollectorModuleFromTransitionEntryModule(WD_EditorObject entryModule) {
        WD_EditorObject module= null;
        ForEachChild(entryModule, m=> { if(m.IsModule && m.Name == TransitionDataCollectorModuleStr) { module= m; return true; } return false; });
        return module;
    }
}
