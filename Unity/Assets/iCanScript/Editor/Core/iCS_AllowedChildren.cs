using UnityEngine;
using System.Collections;

public static class iCS_AllowedChildren {
    public static readonly string[]    BehaviourChildNames= null;
    public static readonly string[]    StateChildNames= null;
    
    public static readonly string[]     BehaviourChildTooltips= null;
    public static readonly string[]     StateChildTooltips= null;
    
    static iCS_AllowedChildren() {
        BehaviourChildNames= new string[]{
            iCS_Strings.Start,
            iCS_Strings.Update,
            iCS_Strings.LateUpdate,
            iCS_Strings.FixedUpdate,
            iCS_Strings.OnGUI,
            iCS_Strings.OnDrawGizmos
        };
        BehaviourChildTooltips= new string[]{
            "Start is called just before any of the Update methods is called the first time.",
            "Update is called every frame, if the MonoBehaviour is enabled.",
            "LateUpdate is called every frame, if the Behaviour is enabled.",
            "This function is called every fixed framerate frame, if the MonoBehaviour is enabled.",
            "OnGUI is called for rendering and handling GUI events.",
            "Implement this OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn."
        };
        StateChildNames= new string[]{
            iCS_Strings.OnEntry,
            iCS_Strings.OnUpdate,
            iCS_Strings.OnExit
        };
        StateChildTooltips= new string[]{
            "OnEntry is called when the state is first activated before OnUpdate is called.",
            "OnUpdate is called on every frame when the state is active.",
            "OnExit is called when leaving the state before OnEntry is called on the newly active states."
        };
    }
    
    // ----------------------------------------------------------------------
    public static bool CanAddChildNode(string childName, iCS_ObjectTypeEnum objType, iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return false;
        bool isAllowed= false;
        switch(parent.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour: {
                switch(objType) {
                    case iCS_ObjectTypeEnum.Module:
                    case iCS_ObjectTypeEnum.StateChart: {
                        if(NameExistsIn(childName, BehaviourChildNames) && !IsChildNodePresent(childName, parent, storage)) {
                            isAllowed= true;
                        }
                        break;
                    }
                    default: {
                        break;
                    }
                }
                break;
            }
            case iCS_ObjectTypeEnum.StateChart: {
                switch(objType) {
                    case iCS_ObjectTypeEnum.State: {
                        isAllowed= true;
                        break;
                    }
                    default: {
                        break;
                    }
                }
                break;                
            }
            case iCS_ObjectTypeEnum.State: {
                switch(objType) {
                    case iCS_ObjectTypeEnum.State:
                    case iCS_ObjectTypeEnum.TransitionModule: {
                        isAllowed= true;
                        break;
                    }
                    case iCS_ObjectTypeEnum.Module: {
                        if(NameExistsIn(childName, StateChildNames) && !IsChildNodePresent(childName, parent, storage)) {
                            isAllowed= true;
                        }
                        break;
                    }
                    default: {
                        break;
                    }
                }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionModule: {
                switch(objType) {
                    case iCS_ObjectTypeEnum.TransitionGuard:
                    case iCS_ObjectTypeEnum.TransitionAction: {
                        isAllowed= true;
                        break;
                    }
                    default: {
                        break;
                    }
                }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionGuard:
            case iCS_ObjectTypeEnum.TransitionAction:
            case iCS_ObjectTypeEnum.Module: {
                switch(objType) {
                    case iCS_ObjectTypeEnum.Behaviour:
                    case iCS_ObjectTypeEnum.State: {
                        break;
                    }
                    default: {
                        isAllowed= true;
                        break;
                    }
                }
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod:
            case iCS_ObjectTypeEnum.ClassMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.ClassField:
            case iCS_ObjectTypeEnum.TypeCast: {
                break;
            }
            default: {
                break;
            }            
        }
        return isAllowed;
    }
    
    // ----------------------------------------------------------------------
    // Returns true if a child node exists with the same name.
    public static bool IsChildNodePresent(string childName, iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return false;
        return storage.UntilMatchingChild(parent,
            child=> {
                if(child.IsNode) {
                    return child.Name == childName;
                }
                return false;
            }
        );
    }

    // ----------------------------------------------------------------------
    // Returns true if the string exists in the string array.
    static bool NameExistsIn(string name, string[] allNames) {
        foreach(var str in allNames) {
            if(name == str) return true;
        }
        return false;
    }

    // ----------------------------------------------------------------------
	public static string TooltipForBehaviourChild(string name) {
		for(int i= 0; i < BehaviourChildNames.Length; ++i) {
			if(name == BehaviourChildNames[i]) {
				return BehaviourChildTooltips[i];
			}
		}
		return null;
	}
}
