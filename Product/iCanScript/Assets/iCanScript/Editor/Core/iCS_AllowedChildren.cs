using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
public static class iCS_AllowedChildren {
    public static readonly string[] StateChildNames= null;
    public static readonly string[] StateChildTooltips= null;

    static iCS_AllowedChildren() {
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
    public static bool CanAddChildNode(string childName, iCS_EngineObject child, iCS_EditorObject parent, iCS_IStorage iStorage) {
        if(parent == null || child == null) return false;
        if(parent.IsBehaviour) {
            if(child.IsPackage) {
                if(IsChildNodePresent(childName, parent, iStorage)) {
                    iCS_EditorController.ShowNotificationOnVisualEditor("Node with name=> "+childName+" already exist.\nPlease rename existing "+childName+" and retry.");
                    return false;
                }
                return true;
            }
        }
        return CanAddChildNode(childName, child.ObjectType, parent, iStorage);
    }
    // ----------------------------------------------------------------------
    public static bool CanAddChildNode(string childName, VSObjectType childType, iCS_EditorObject parent, iCS_IStorage iStorage) {
        if(parent == null) return false;
        // Only allow valid child for object instances.
        if(parent.IsBehaviour) {
            // Don't allow more then one copy of a node in an instance node
            if(IsChildNodePresent(childName, parent, iStorage)) {
                return false;
            }
            if(childType == VSObjectType.Package || childType == VSObjectType.InstanceMessage) {
                return true;
            }
            if(childType == VSObjectType.Constructor) {
                return true;
            }
			return false;                        
        }
        // Messages are only allow on their object instance.
        if(childType == VSObjectType.InstanceMessage || childType == VSObjectType.StaticMessage) {
            return false;
        }
        // Only allow State node in StateChart.
        if(parent.ObjectType == VSObjectType.StateChart) {
            return childType == VSObjectType.State;
        }
        // Only allow state, transition trigger package & entry/update/exit modules in state.
        if(parent.ObjectType == VSObjectType.State) {
            if(childType == VSObjectType.State || childType == VSObjectType.TransitionPackage) {
                return true;
            }
            if(childType == VSObjectType.Package) {
                return NameExistsIn(childName, StateChildNames) && !IsChildNodePresent(childName, parent, iStorage);
            }
        }
        // Allow all but Behaviour & State in module.
        if(parent.IsKindOfPackage) {
            if(childType == VSObjectType.Behaviour || childType == VSObjectType.State) {
                return false;
            }
            return true;
        }
        // Reject all other type of node nesting.
        return false;
    }
    
    // ----------------------------------------------------------------------
    // Returns true if a child node exists with the same name.
    public static bool IsChildNodePresent(string childName, iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return false;
        return storage.UntilMatchingChild(parent,
            child=> {
                if(child.IsNode) {
                    return child.DisplayName == childName;
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
}
