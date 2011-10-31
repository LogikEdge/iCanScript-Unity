using UnityEngine;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string TriggerStr= "trigger";
    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateTransitionEntry(WD_EditorObject port) {
        WD_EditorObject mainModule= CreateModule(port.ParentId, Math3D.ToVector2(GetPosition(port)), "Transition Entry");
        WD_EditorObject mainOutPort= CreatePort(TriggerStr, mainModule.InstanceId, typeof(bool), WD_ObjectTypeEnum.OutStaticModulePort);
        WD_EditorObject trigger= CreateModule(mainModule.InstanceId, Vector2.zero, TriggerStr);
        trigger.IsNameEditable= false;
        WD_EditorObject triggerOutPort= CreatePort(TriggerStr, trigger.InstanceId, typeof(bool), WD_ObjectTypeEnum.OutStaticModulePort);
        port.Source= mainOutPort.InstanceId;
        mainOutPort.Source= triggerOutPort.InstanceId;
        return mainModule;
    }
    // ----------------------------------------------------------------------
    public WD_EditorObject CreateTransitionExit(WD_EditorObject port) {
        WD_EditorObject mainModule= CreateModule(port.ParentId, Math3D.ToVector2(GetPosition(port)), "Transition Exit");
        WD_EditorObject mainInPort= CreatePort("", mainModule.InstanceId, typeof(void), WD_ObjectTypeEnum.InStaticModulePort);
        mainInPort.Source= port.InstanceId;
        return mainModule;
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
        if(obj == null || !obj.IsModule || obj.Name != TriggerStr) return false;
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
                if(port.IsOutStaticModulePort && port.RuntimeType == typeof(bool) && port.Name == TriggerStr) {                
                    triggerPort= port;
                }
            }
        );        
        return triggerPort;
    }
}
