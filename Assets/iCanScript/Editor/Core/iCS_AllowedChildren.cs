using UnityEngine;
using System.Collections;

public static class iCS_AllowedChildren {
    public static readonly string[]    BehaviourChildNames= null;
    public static readonly string[]    StateChildNames= null;
    
    static iCS_AllowedChildren() {
        BehaviourChildNames= new string[]{
            iCS_EngineStrings.BehaviourChildStart,
            iCS_EngineStrings.BehaviourChildUpdate,
            iCS_EngineStrings.BehaviourChildLateUpdate,
            iCS_EngineStrings.BehaviourChildFixedUpdate,
            iCS_EngineStrings.BehaviourChildOnGUI,
            iCS_EngineStrings.BehaviourChildOnDrawGizmos
        };
        StateChildNames= new string[]{
            iCS_EngineStrings.StateChildOnEntry,
            iCS_EngineStrings.StateChildOnUpdate,
            iCS_EngineStrings.StateChildOnExit
        };
    }
    
    // ----------------------------------------------------------------------
    public static bool CanAddChildNode(string childName, iCS_ObjectTypeEnum objType, iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return false;
        bool isAllowed= false;
        switch(parent.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour: {
                switch(objType) {
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
            case iCS_ObjectTypeEnum.StaticMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.StaticField:
            case iCS_ObjectTypeEnum.Conversion: {
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
        return storage.ForEachChild(parent,
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
}
