using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public void CreateTransition(iCS_EditorObject outStatePort, iCS_EditorObject destState) {
        Rect portRect= GetPosition(destState);
        Vector2 portPos= Math3D.ToVector2(portRect);
        outStatePort.IsNameEditable= false;
//        iCS_EditorObject parent= GetParent(GetParent(outStatePort));
        // Create inStatePort
        iCS_EditorObject inStatePort= CreatePort("", destState.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
        SetInitialPosition(inStatePort, portPos);
        SetSource(inStatePort, outStatePort);
        UpdatePortEdges(inStatePort, outStatePort);        
        inStatePort.IsNameEditable= false;
        // Determine transition parent
        iCS_EditorObject parent= GetTransitionParent(GetParent(inStatePort), GetParent(outStatePort));
        // Create transition module
        iCS_EditorObject transitionModule= CreateModule(parent.InstanceId, portPos, "[false]", iCS_ObjectTypeEnum.TransitionModule);
        transitionModule.IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.TransitionModuleIcon, this);
        transitionModule.ToolTip= "Precondition for the transition to trigger.";
        transitionModule.IsNameEditable= false;
        iCS_EditorObject inModulePort=  CreatePort("cond", transitionModule.InstanceId, typeof(void), iCS_ObjectTypeEnum.InTransitionPort);
        iCS_EditorObject outModulePort= CreatePort("fired", transitionModule.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutTransitionPort);        
        SetSource(inModulePort, outStatePort);
        SetSource(inStatePort, outModulePort);
        Minimize(transitionModule);
        // Create guard module
        iCS_EditorObject guard= CreateModule(transitionModule.InstanceId, portPos, "false", iCS_ObjectTypeEnum.TransitionGuard);
        guard.IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.TransitionTriggerIcon, this);
        guard.ToolTip= "The guard function must evaluate to 'true' for the transition to fire.";
        iCS_EditorObject guardPort= CreatePort("trigger", guard.InstanceId, typeof(bool), iCS_ObjectTypeEnum.OutStaticModulePort);
        guardPort.IsNameEditable= false;
        SetSource(outStatePort, guardPort);
        Minimize(guard);
        // Create action module
        iCS_EditorObject action= CreateModule(transitionModule.InstanceId, portPos, "NoAction", iCS_ObjectTypeEnum.TransitionAction);
        action.IconGUID= iCS_Graphics.IconPathToGUID(iCS_EditorStrings.MethodIcon, this);
        action.ToolTip= "Action to be execute when the transition is taken.";
        iCS_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        Minimize(action);
        // Set initial transition module position.
        LayoutTransitionModule(transitionModule);
    }
    // ----------------------------------------------------------------------
    // Returns the common parent of given states.
    public iCS_EditorObject GetTransitionParent(iCS_EditorObject inState, iCS_EditorObject outState) {
        bool parentFound= false;
        iCS_EditorObject parent= null;
        for(parent= outState; parent != null; parent= GetParent(parent)) {
            iCS_EditorObject inParent= null;
            for(inParent= inState; inParent != null; inParent= GetParent(inParent)) {
                if(parent == inParent) {
                    parentFound= true;
                    break;
                }
            }
            if(parentFound) break;
        }
        return parent;        
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetOutStatePort(iCS_EditorObject transitionObject) {
        if(transitionObject.IsOutStatePort) return transitionObject;
        if(transitionObject.IsInStatePort) {
            do {
                iCS_EditorObject source= GetSource(transitionObject);
				if(source == null) return null;
                if(source.IsOutStatePort) return source;
                iCS_EditorObject sourceParent= GetParent(source);
                transitionObject= GetInTransitionPort(sourceParent);                
            } while(transitionObject != null);
            return null;
        }
        if(transitionObject.IsTransitionModule) {
            ForEachChild(transitionObject,
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
            ForEachChildPort(transitionObject,
                port=> {
                    if(port.IsOutDataPort && port.RuntimeType == typeof(bool)) {
                        iCS_EditorObject[] connectedPorts= FindConnectedPorts(port);
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
    public iCS_EditorObject GetInStatePort(iCS_EditorObject outStatePort) {
        if(outStatePort.IsInStatePort) return outStatePort;
        outStatePort= GetOutStatePort(outStatePort);
        if(outStatePort == null) return null; 
        // Find transition module.
        iCS_EditorObject[] connectedPorts= FindConnectedPorts(outStatePort);
        iCS_EditorObject inStatePort= null;
        foreach(var port in connectedPorts) {
            if(port.IsInTransitionPort) inStatePort= port;
        }
        iCS_EditorObject transitionModule= GetParent(inStatePort);
        // Find transition module output port.
        ForEachChildPort(transitionModule,
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
    public iCS_EditorObject GetTransitionModule(iCS_EditorObject statePort) {
		if(statePort == null) return null;
        // Get the outStatePort
        statePort= GetOutStatePort(statePort);
		if(statePort == null) return null;
        return GetParent(GetParent(GetSource(statePort)));
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
        return !ForEachChild(action, child=> child.IsNode);
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
        transitionModule.Name= name;
        return name;
    }
    // ----------------------------------------------------------------------
    public Rect ProposeTransitionModulePosition(iCS_EditorObject module) {
        Rect nodePos= GetPosition(module);
        iCS_EditorObject inStatePort= GetInStatePort(module);
        iCS_EditorObject outStatePort= GetOutStatePort(module);
        if(inStatePort != null) {
            iCS_EditorObject parent= GetParent(module);
            iCS_ConnectionParams cp= new iCS_ConnectionParams(inStatePort, outStatePort, this);
            Vector2 distance= cp.End-cp.Start;
            Vector2 delta= 0.5f*iCS_EditorConfig.GutterSize*(distance).normalized;
            int steps= (int)(distance.magnitude/delta.magnitude);
            Vector2 pos= cp.Start;
            bool minFound= false;
            Vector2 minPos= Vector2.zero;
            Vector2 maxPos= Vector2.zero;
            for(int i= 0; i < steps; ++i, pos+= delta) {
                Vector2 point= iCS_ConnectionParams.ClosestPointBezier(pos, cp.Start, cp.End, cp.StartTangent, cp.EndTangent);
                if(GetNodeAt(point) == parent) {
                    if(!minFound) {
                        minFound= true;
                        minPos= point;
                    }
                    maxPos= point;
                }
            }
            Vector2 newCenter= 0.5f*(minPos+maxPos);
            newCenter= iCS_ConnectionParams.ClosestPointBezier(newCenter, cp.Start, cp.End, cp.StartTangent, cp.EndTangent);
            return new Rect(newCenter.x-0.5f*nodePos.width, newCenter.y-0.5f*nodePos.height, nodePos.width, nodePos.height);                            
        }
        return nodePos;
    }
    // ----------------------------------------------------------------------
    public void LayoutTransitionModule(iCS_EditorObject module) {
        GetTransitionName(module);
        SetPosition(module, ProposeTransitionModulePosition(module));                    
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetInTransitionPort(iCS_EditorObject parent) {
        iCS_EditorObject inTransitionPort= null;
        ForEachChildPort(parent,
            port=> {
                if(port.IsInTransitionPort) {
                    inTransitionPort= port;
                    return true;
                }
                return false;
            }
        );
        return inTransitionPort;
    }
}
