using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public void CreateTransition(iCS_EditorObject fromStatePort, iCS_EditorObject toState, Vector2 toStatePortPos) {
        Vector2 fromStatePortPos= fromStatePort.LayoutPosition;
        // Create toStatePort
        iCS_EditorObject toStatePort= CreatePort("", toState.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
        toStatePort.SetAnchorAndLayoutPosition(toStatePortPos);
        SetSource(toStatePort, fromStatePort);
        toStatePort.UpdatePortEdge();        
        // Update fromStatePort position.
        fromStatePort.UpdatePortEdge();
        // Determine transition parent
        iCS_EditorObject transitionParent= GetTransitionParent(toStatePort.Parent, fromStatePort.Parent);
        // Create transition module
        var transitionModulePos= 0.5f*(fromStatePortPos+toStatePortPos);
        iCS_EditorObject transitionModule= CreateAggregate(transitionParent.InstanceId, transitionModulePos, "[false]", iCS_ObjectTypeEnum.TransitionModule);
        var transitionIconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.TransitionModuleIcon);
        if(transitionIconGUID != null) {
            transitionModule.IconGUID= transitionIconGUID;            
        } else {
            Debug.LogWarning("Missing transition module icon: "+iCS_EditorStrings.TransitionModuleIcon);
        }
        transitionModule.Tooltip= "Precondition for the transition to trigger.";
        transitionModule.IsNameEditable= false;
        iCS_EditorObject inModulePort=  CreatePort(" ", transitionModule.InstanceId, typeof(void), iCS_ObjectTypeEnum.InTransitionPort);
        iCS_EditorObject outModulePort= CreatePort(" ", transitionModule.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutTransitionPort);        
        SetSource(inModulePort, fromStatePort);
        SetSource(toStatePort, outModulePort);
        Iconize(transitionModule);
        // Create guard module
        iCS_EditorObject guard= CreateAggregate(transitionModule.InstanceId, transitionModulePos, "false", iCS_ObjectTypeEnum.TransitionGuard);
        var guardIconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.TransitionTriggerIcon);
        if(guardIconGUID != null) {
            guard.IconGUID= guardIconGUID;            
        } else {
            Debug.LogWarning("Missing transition guard module icon: "+iCS_EditorStrings.TransitionTriggerIcon);
        }
        guard.Tooltip= "The guard function must evaluate to 'true' for the transition to fire.";
        iCS_EditorObject guardPort= CreatePort("trigger", guard.InstanceId, typeof(bool), iCS_ObjectTypeEnum.OutFixPort);
        guardPort.IsNameEditable= false;
        SetSource(fromStatePort, guardPort);
        Iconize(guard);
        // Create action module
        iCS_EditorObject action= CreateAggregate(transitionModule.InstanceId, transitionModulePos, "NoAction", iCS_ObjectTypeEnum.TransitionAction);
        var actionIconGUID= iCS_TextureCache.IconPathToGUID(iCS_EditorStrings.MethodIcon);
        if(actionIconGUID != null) {
            action.IconGUID= actionIconGUID;            
        } else {
            Debug.LogWarning("Missing transition action module icon: "+iCS_EditorStrings.MethodIcon);
        }
        action.Tooltip= "Action to be execute when the transition is taken.";
        iCS_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        Iconize(action);
        // Update port names
        UpdatePortNames(fromStatePort, toStatePort);
        // Set initial transition module position.
        var transitionIcon= iCS_TextureCache.GetTextureFromGUID(transitionModule.IconGUID);
        transitionModule.LayoutSize= new Vector2(transitionIcon.width, transitionIcon.height);
        outModulePort= inModulePort;
        LayoutTransitionModule(transitionModule);
    }
    // ----------------------------------------------------------------------
    // Updates the port names of a transition.
    public void UpdatePortNames(iCS_EditorObject fromStatePort, iCS_EditorObject toStatePort) {
        // State ports
        var fromParent= fromStatePort.Parent;
        var toParent  = toStatePort.Parent;
        string statePortName= fromParent.Name+"->"+toParent.Name;
        fromStatePort.Name= statePortName;
        toStatePort.Name  = statePortName;
        fromStatePort.IsNameEditable= false;
        toStatePort.IsNameEditable= false;
        // Transition module ports.
        var transitionModule = GetTransitionModule(fromStatePort);
        var inTransitionPort = GetInTransitionPort(transitionModule);
        var outTransitionPort= GetOutTransitionPort(transitionModule);
        inTransitionPort.Name= fromParent.Name+"->"+transitionModule.Name;
        outTransitionPort.Name= transitionModule.Name+"->"+toParent.Name;
        inTransitionPort.IsNameEditable= false;
        outTransitionPort.IsNameEditable= false;
    }
    // ----------------------------------------------------------------------
    // Returns the common parent of given states.
    public iCS_EditorObject GetTransitionParent(iCS_EditorObject toState, iCS_EditorObject fromState) {
        bool parentFound= false;
        iCS_EditorObject fromParent= null;
        for(fromParent= fromState; fromParent != null; fromParent= fromParent.Parent) {
            iCS_EditorObject toParent= null;
            for(toParent= toState; toParent != null; toParent= toParent.Parent) {
                if(fromParent == toParent) {
                    parentFound= true;
                    break;
                }
            }
            if(parentFound) break;
        }
        return fromParent;        
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetFromStatePort(iCS_EditorObject transitionObject) {
        if(transitionObject.IsOutStatePort) return transitionObject;
        if(transitionObject.IsInStatePort) {
            do {
                iCS_EditorObject source= transitionObject.Source;
				if(source == null) return null;
                if(source.IsOutStatePort) return source;
                iCS_EditorObject sourceParent= source.Parent;
                transitionObject= GetInTransitionPort(sourceParent);                
            } while(transitionObject != null);
            return null;
        }
        if(transitionObject.IsTransitionModule) {
            UntilMatchingChildNode(transitionObject,
                child=> {
                    if(child.IsTransitionGuard) {
                        transitionObject= child;
                        return true;
                    }
                    return false;
                }
            );
        }
        if(transitionObject.IsTransitionGuard) {
            iCS_EditorObject outStatePort= null;
            UntilMatchingChildPort(transitionObject,
                port=> {
                    if(port.IsOutDataPort && port.RuntimeType == typeof(bool)) {
                        iCS_EditorObject[] connectedPorts= port.Destinations;
                        foreach(var p in connectedPorts) {
                            if(p.IsOutStatePort) {
                                outStatePort= p;
                                return true;
                            }
                        }
                    }
                    return false;
                }
            );
            return outStatePort;
        }
        return null;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetToStatePort(iCS_EditorObject outStatePort) {
        if(outStatePort.IsInStatePort) return outStatePort;
        outStatePort= GetFromStatePort(outStatePort);
        if(outStatePort == null) return null; 
        // Find transition module.
        iCS_EditorObject[] connectedPorts= outStatePort.Destinations;
        iCS_EditorObject inStatePort= null;
        foreach(var port in connectedPorts) {
            if(port.IsInTransitionPort) inStatePort= port;
        }
        iCS_EditorObject transitionModule= inStatePort.Parent;
        // Find transition module output port.
        UntilMatchingChildPort(transitionModule,
            p=> {
                if(p.IsOutTransitionPort) {
                    inStatePort= p;
                    return true;
                }
                return false;
            }
        );
        return FindAConnectedPort(inStatePort);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetInTransitionPort(iCS_EditorObject obj) {
        if(obj.IsInTransitionPort) return obj;
        iCS_EditorObject transitionModule= obj.IsTransitionModule ?
                obj :
                (obj.IsOutTransitionPort ? obj.Parent : GetTransitionModule(obj));
        iCS_EditorObject inTransitionPort= null;
        UntilMatchingChildPort(transitionModule,
            p=> {
                if(p.IsInTransitionPort) {
                    inTransitionPort= p;
                    return true;
                }
                return false;
            }
        );
        return inTransitionPort;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetOutTransitionPort(iCS_EditorObject obj) {
        if(obj.IsOutTransitionPort) return obj;
        iCS_EditorObject transitionModule= obj.IsTransitionModule ? 
                obj :
                (obj.IsInTransitionPort ? obj.Parent : GetTransitionModule(obj));
        iCS_EditorObject outTransitionPort= null;
        UntilMatchingChildPort(transitionModule,
            p=> {
                if(p.IsOutTransitionPort) {
                    outTransitionPort= p;
                    return true;
                }
                return false;
            }
        );
        return outTransitionPort;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetTransitionModule(iCS_EditorObject statePort) {
		if(statePort == null) return null;
        // Get the outStatePort
        statePort= GetFromStatePort(statePort);
		if(statePort == null) return null;
        var source= statePort.Source;
		if(source == null) return null;
        var parent= source.Parent;
        if(parent == null) return null;
        return parent.Parent;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetTransitionGuardAndAction(iCS_EditorObject statePort, out iCS_EditorObject actionModule) {
        actionModule= null;
        iCS_EditorObject transitionModule= GetTransitionModule(statePort);
        if(transitionModule == null) return null;
        iCS_EditorObject action= null;
        iCS_EditorObject guard= null;
        ForEachChild(transitionModule,
            child=> {
                if(child.IsTransitionGuard)  guard= child;
                if(child.IsTransitionAction) action= child;
            }
        );
        actionModule= action;
        return guard;
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionActionEmpty(iCS_EditorObject action) {
        return !UntilMatchingChildNode(action, _=> true);
    }
    // ----------------------------------------------------------------------
    public string GetTransitionName(iCS_EditorObject statePort) {
        iCS_EditorObject action= null;
        iCS_EditorObject guard= GetTransitionGuardAndAction(statePort, out action);
        string name= "";
        if(guard != null) {
            name= "["+guard.Name+"]";
            if(action != null && !IsTransitionActionEmpty(action)) name+= "/"+action.Name;
        }
        // Update transition module name.
        iCS_EditorObject transitionModule= GetTransitionModule(statePort);
        if(transitionModule == null) return "";
        transitionModule.Name= name;
        return name;
    }
    // ----------------------------------------------------------------------
    public Vector2 ProposeTransitionModulePosition(iCS_EditorObject module) {
        iCS_EditorObject fromStatePort= GetFromStatePort(module);
        iCS_EditorObject toStatePort= GetToStatePort(module);
		if(toStatePort == null || fromStatePort ==null) return module.LayoutPosition;
        iCS_EditorObject parent= module.Parent;
		var startPos= fromStatePort.LayoutPosition;
		var endPos= toStatePort.LayoutPosition;
		var delta= endPos-startPos;
		var marginSize= iCS_EditorConfig.MarginSize; if(marginSize < 5) marginSize=5;
		Vector2 step= 0.5f*marginSize*(delta).normalized;
		var minPos= startPos;
		var maxPos= endPos;
		for(; Vector2.Distance(minPos, endPos) >= marginSize; minPos+= step) {
			if(GetNodeAt(minPos) == parent) {
				break;
			}
		}
		for(maxPos= minPos+step; Vector2.Distance(maxPos, endPos) >= marginSize; maxPos+= step) {
			if(GetNodeAt(maxPos) != parent) {
				break;
			}
		}
		return 0.5f*(minPos+maxPos);
    }
    // ----------------------------------------------------------------------
    public void LayoutTransitionModule(iCS_EditorObject module) {
        GetTransitionName(module);
        module.SetAnchorAndLayoutPosition(ProposeTransitionModulePosition(module));
    }
    // ----------------------------------------------------------------------
    public Vector2 GetTransitionModuleVector(iCS_EditorObject module) {
        iCS_EditorObject inStatePort      = GetToStatePort(module);
        iCS_EditorObject outStatePort     = GetFromStatePort(module);
        iCS_EditorObject inTransitionPort = GetInTransitionPort(module);
        iCS_EditorObject outTransitionPort= GetOutTransitionPort(module);
        var inStatePos= inStatePort.LayoutPosition;
        var outStatePos= outStatePort.LayoutPosition;
        var inTransitionPos= inTransitionPort.LayoutPosition;
        var outTransitionPos= outTransitionPort.LayoutPosition;
        Vector2 dir= ((inStatePos-outTransitionPos).normalized+(inTransitionPos-outStatePos).normalized).normalized;
        return dir;
    }
}
