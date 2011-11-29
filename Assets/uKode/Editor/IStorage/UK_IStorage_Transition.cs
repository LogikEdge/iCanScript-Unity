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
        // Create guard module
        UK_EditorObject guard= CreateModule(parent.InstanceId, portPos, "false", UK_ObjectTypeEnum.TransitionGuard);
        guard.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.TransitionTriggerIcon, this);
        UK_EditorObject guardPort= CreatePort("trigger", guard.InstanceId, typeof(bool), UK_ObjectTypeEnum.OutStaticModulePort);
        guardPort.IsNameEditable= false;
        SetSource(outStatePort, guardPort);
        Minimize(guard);
        // Create action module
        UK_EditorObject action= CreateModule(parent.InstanceId, portPos, "action", UK_ObjectTypeEnum.TransitionAction);
        action.IconGUID= UK_Graphics.IconPathToGUID(UK_EditorStrings.MethodIcon, this);
        UK_EditorObject enablePort= CreateEnablePort(action.InstanceId);
        enablePort.IsNameEditable= false;
        SetSource(enablePort, guardPort);
        Minimize(action);
    }
    
    // ======================================================================
    // Transition helpers.
    // ----------------------------------------------------------------------
    public UK_EditorObject GetTransitionGuardAndAction(UK_EditorObject outStatePort, out UK_EditorObject action) {
        action= null;
        UK_EditorObject guardPort= GetSource(outStatePort);
        if(guardPort == null) return null;
        UK_EditorObject[] connectedPorts= FindConnectedPorts(guardPort);
        foreach(var port in connectedPorts) {
            if(port.IsEnablePort && GetParent(port).IsTransitionAction) {
                action= GetParent(port);
            }
        }
        return GetParent(guardPort);
    }
    // ----------------------------------------------------------------------
    public bool IsTransitionActionEmpty(UK_EditorObject action) {
        return !ForEachChild(action, child=> child.IsNode);
    }
    // ----------------------------------------------------------------------
    public string GetTransitionName(UK_EditorObject statePort) {
        if(statePort.IsInStatePort) statePort= GetSource(statePort);
        UK_EditorObject action= null;
        UK_EditorObject guard= GetTransitionGuardAndAction(statePort, out action);
        if(guard == null) return "";
        string name= "["+guard.Name+"]";
        if(action != null && !IsTransitionActionEmpty(action)) name+= "/"+action.Name;
        return name;
    }
}
