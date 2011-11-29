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
        UK_EditorObject parent= GetParent(GetParent(outStatePort));
        // Create inStatePort
        UK_EditorObject inStatePort= CreatePort("", destState.InstanceId, typeof(void), UK_ObjectTypeEnum.InStatePort);
        SetInitialPosition(inStatePort, portPos);
        SetSource(inStatePort, outStatePort);
        UpdatePortEdges(inStatePort, outStatePort);        
        inStatePort.IsNameEditable= false;
        // Create transition module
        UK_EditorObject transitionModule= CreateModule(parent.InstanceId, portPos, "[false]", UK_ObjectTypeEnum.TransitionModule);
        transitionModule.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionModuleIcon, this);
        transitionModule.IsNameEditable= false;
        Minimize(transitionModule);
        // Create guard module
        UK_EditorObject guard= CreateModule(transitionModule.InstanceId, portPos, "false", UK_ObjectTypeEnum.TransitionGuard);
        guard.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        UK_EditorObject guardPort= CreatePort("trigger", guard.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        guardPort.IsNameEditable= false;
        SetSource(outStatePort, guardPort);
        Minimize(guard);
        // Create action module
        UK_EditorObject action= CreateModule(transitionModule.InstanceId, portPos, "action", UK_ObjectTypeEnum.TransitionAction);
        action.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.MethodIcon, this);
        UK_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        Minimize(action);
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public UK_EditorObject GetOutStatePort(UK_EditorObject statePort) {
        return statePort.IsInStatePort ? GetSource(statePort) : statePort;
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
}
