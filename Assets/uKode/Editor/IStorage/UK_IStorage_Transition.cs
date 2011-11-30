using UnityEngine;
using System.Collections;

public partial class UK_IStorage {    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public void CreateTransition(UK_EditorObject outStatePort, UK_EditorObject destState) {
        Rect portRect= GetPosition(destState);
        Vector2 portPos= Math3D.ToVector2(portRect);
        outStatePort.IsNameEditable= false;
//        UK_EditorObject parent= GetParent(GetParent(outStatePort));
        // Create inStatePort
        UK_EditorObject inStatePort= CreatePort("", destState.InstanceId, typeof(void), UK_ObjectTypeEnum.InStatePort);
        SetInitialPosition(inStatePort, portPos);
        SetSource(inStatePort, outStatePort);
        UpdatePortEdges(inStatePort, outStatePort);        
        inStatePort.IsNameEditable= false;
        // Determine transition parent
        bool parentFound= false;
        UK_EditorObject parent= null;
        for(parent= GetParent(outStatePort); parent != null; parent= GetParent(parent)) {
            UK_EditorObject inParent= null;
            for(inParent= GetParent(inStatePort); inParent != null; inParent= GetParent(inParent)) {
                if(parent == inParent) {
                    parentFound= true;
                    break;
                }
            }
            if(parentFound) break;
        }
        // Create transition module
        UK_EditorObject transitionModule= CreateModule(parent.InstanceId, portPos, "[false]", UK_ObjectTypeEnum.TransitionModule);
        transitionModule.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionModuleIcon, this);
        transitionModule.ToolTip= "Precondition for the transition to trigger.";
        transitionModule.IsNameEditable= false;
        UK_EditorObject inModulePort=  CreatePort("cond", transitionModule.InstanceId, typeof(void), UK_ObjectTypeEnum.InTransitionPort);
        UK_EditorObject outModulePort= CreatePort("fired", transitionModule.InstanceId, typeof(void), UK_ObjectTypeEnum.OutTransitionPort);        
        SetSource(inModulePort, outStatePort);
        SetSource(inStatePort, outModulePort);
        Minimize(transitionModule);
        // Create guard module
        UK_EditorObject guard= CreateModule(transitionModule.InstanceId, portPos, "false", UK_ObjectTypeEnum.TransitionGuard);
        guard.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        guard.ToolTip= "The guard function must evaluate to 'true' for the transition to fire.";
        UK_EditorObject guardPort= CreatePort("trigger", guard.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        guardPort.IsNameEditable= false;
        SetSource(outStatePort, guardPort);
        Minimize(guard);
        // Create action module
        UK_EditorObject action= CreateModule(transitionModule.InstanceId, portPos, "NoAction", UK_ObjectTypeEnum.TransitionAction);
        action.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.MethodIcon, this);
        action.ToolTip= "Action to be execute when the transition is taken.";
        UK_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        Minimize(action);
        // Set initial transition module position.
        LayoutTransitionModule(transitionModule);
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public UK_EditorObject GetOutStatePort(UK_EditorObject transitionObject) {
        if(transitionObject.IsOutStatePort) return transitionObject;
        if(transitionObject.IsInStatePort) {
            do {
                UK_EditorObject source= GetSource(transitionObject);
                if(source.IsOutStatePort) return source;
                UK_EditorObject sourceParent= GetParent(source);
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
            UK_EditorObject outStatePort= null;
            ForEachChildPort(transitionObject,
                port=> {
                    if(port.IsOutDataPort && port.RuntimeType == typeof(bool)) {
                        UK_EditorObject[] connectedPorts= FindConnectedPorts(port);
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
    public UK_EditorObject GetInStatePort(UK_EditorObject outStatePort) {
        if(outStatePort.IsInStatePort) return outStatePort;
        outStatePort= GetOutStatePort(outStatePort);
        if(outStatePort == null) return null; 
        UK_EditorObject[] connectedPorts= FindConnectedPorts(outStatePort);
        UK_EditorObject inStatePort= null;
        foreach(var port in connectedPorts) {
            if(port.IsInStatePort) inStatePort= port;
        }
        return inStatePort;
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionModule(UK_EditorObject statePort) {
        // Get the outStatePort
        statePort= GetOutStatePort(statePort);
        return GetParent(GetParent(GetSource(statePort)));
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionGuardAndAction(UK_EditorObject statePort, out UK_EditorObject actionModule) {
        actionModule= null;
        UK_EditorObject transitionModule= GetTransitionModule(statePort);
        if(transitionModule == null) return null;
        UK_EditorObject action= null;
        UK_EditorObject guard= null;
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
    public bool IsTransitionActionEmpty(UK_EditorObject action) {
        return !ForEachChild(action, child=> child.IsNode);
    }
    // ----------------------------------------------------------------------
    public string GetTransitionName(UK_EditorObject statePort) {
        UK_EditorObject action= null;
        UK_EditorObject guard= GetTransitionGuardAndAction(statePort, out action);
        string name= "";
        if(guard != null) {
            name= "["+guard.Name+"]";
            if(action != null && !IsTransitionActionEmpty(action)) name+= "/"+action.Name;
        }
        // Update transition module name.
        UK_EditorObject transitionModule= GetTransitionModule(statePort);
        transitionModule.Name= name;
        return name;
    }
    // ----------------------------------------------------------------------
    public Rect ProposeTransitionModulePosition(UK_EditorObject module) {
        Rect nodePos= GetPosition(module);
        UK_EditorObject inStatePort= GetInStatePort(module);
        UK_EditorObject outStatePort= GetOutStatePort(module);
        if(inStatePort != null) {
            UK_EditorObject parent= GetParent(module);
            UK_ConnectionParams cp= new UK_ConnectionParams(inStatePort, outStatePort, this);
            Vector2 distance= cp.End-cp.Start;
            Vector2 delta= 0.5f*UK_EditorConfig.GutterSize*(distance).normalized;
            int steps= (int)(distance.magnitude/delta.magnitude);
            Vector2 pos= cp.Start;
            bool minFound= false;
            Vector2 minPos= Vector2.zero;
            Vector2 maxPos= Vector2.zero;
            for(int i= 0; i < steps; ++i, pos+= delta) {
                Vector2 point= UK_ConnectionParams.ClosestPointBezier(pos, cp.Start, cp.End, cp.StartTangent, cp.EndTangent);
                if(GetNodeAt(point) == parent) {
                    if(!minFound) {
                        minFound= true;
                        minPos= point;
                    }
                    maxPos= point;
                }
            }
            Vector2 newCenter= 0.5f*(minPos+maxPos);
            newCenter= UK_ConnectionParams.ClosestPointBezier(newCenter, cp.Start, cp.End, cp.StartTangent, cp.EndTangent);
            return new Rect(newCenter.x-0.5f*nodePos.width, newCenter.y-0.5f*nodePos.height, nodePos.width, nodePos.height);                            
        }
        return nodePos;
    }
    // ----------------------------------------------------------------------
    public void LayoutTransitionModule(UK_EditorObject module) {
        GetTransitionName(module);
        SetPosition(module, ProposeTransitionModulePosition(module));                    
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetInTransitionPort(UK_EditorObject parent) {
        UK_EditorObject inTransitionPort= null;
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
