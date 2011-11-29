using UnityEngine;
using System.Collections;

public partial class UK_IStorage {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string TransitionGuardModuleName= "Trigger";
    const string TransitionActionModuleName= "Action";
    const string TransitionGuardPortName= "trigger";
    const string TransitionModuleName= "transition";
    
    // ======================================================================
    // Creation methods
    // ----------------------------------------------------------------------
    public void CreateTransition(UK_EditorObject outStatePort, UK_EditorObject destState) {
        Rect portRect= GetPosition(destState);
        Vector2 portPos= Math3D.ToVector2(portRect);
        outStatePort.IsNameEditable= false;
        // Create inStatePort
        UK_EditorObject inStatePort= CreatePort("[false]", destState.InstanceId, typeof(void), UK_ObjectTypeEnum.InStatePort);
        SetInitialPosition(inStatePort, portPos);
        SetSource(inStatePort, outStatePort);
        UpdatePortEdges(inStatePort, outStatePort);        
        inStatePort.IsNameEditable= false;
        // Create transition module
        UK_EditorObject transitionModule= CreateModule(GetParent(GetParent(outStatePort)).InstanceId, portPos, TransitionModuleName);
        transitionModule.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        // Create guard module
        UK_EditorObject guard= CreateModule(transitionModule.InstanceId, portPos, TransitionGuardModuleName, UK_ObjectTypeEnum.TransitionGuard);
        guard.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        UK_EditorObject guardPort= CreatePort(TransitionGuardPortName, guard.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        guardPort.IsNameEditable= false;
        SetSource(outStatePort, guardPort);
        // Create action module
        UK_EditorObject action= CreateModule(transitionModule.InstanceId, portPos, TransitionActionModuleName, UK_ObjectTypeEnum.TransitionAction);
        action.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        UK_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        // Default layout to minimized.
        Minimize(transitionModule);
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionGuardAndAction(UK_EditorObject outStatePort, out UK_EditorObject action) {
        action= null;
        UK_EditorObject guardPort= GetSource(outStatePort);
        UK_EditorObject[] connectedPorts= FindConnectedPorts(guardPort);
        if(connectedPorts.Length >= 2) {
            action= GetParent(connectedPorts[connectedPorts[0] == guardPort ? 1 : 0]);
        }
        return GetParent(guardPort);
    }
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionModule(UK_EditorObject statePort) {
        if(statePort.IsInStatePort) statePort= GetSource(statePort);
        return GetParent(GetParent(GetSource(statePort)));
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionModule(UK_EditorObject module) {
        return ForEachChild(module, child=> child.ObjectType == UK_ObjectTypeEnum.TransitionGuard);
    }
}
