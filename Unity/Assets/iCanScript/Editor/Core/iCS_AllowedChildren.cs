using UnityEngine;
using System.Collections;

namespace iCanScript.Editor {
public static class iCS_AllowedChildren {
    public static readonly string[]    StateChildNames= null;
    public static readonly string[]     StateChildTooltips= null;

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
            if(child.IsInstanceNode) {
                return false;
            }
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
    public static bool CanAddChildNode(string childName, iCS_ObjectTypeEnum childType, iCS_EditorObject parent, iCS_IStorage iStorage) {
        if(parent == null) return false;
        // Only allow valid child for object instances.
        if(parent.IsInstanceNode || parent.IsBehaviour) {
            // Don't allow more then one copy of a node in an instance node
            if(IsChildNodePresent(childName, parent, iStorage)) {
                return false;
            }
			var typeInfo= iCS_LibraryDatabase.GetTypeInfo(parent.IsBehaviour ? typeof(MonoBehaviour) : parent.RuntimeType);
			if(typeInfo == null) {
				Debug.LogWarning("iCanScript: Unable to find type information for: "+parent.Name);
				return false;
			}
            if(parent.IsBehaviour && childType == iCS_ObjectTypeEnum.Package) {
                return true;
            }
            if(parent.IsBehaviour && childType == iCS_ObjectTypeEnum.Constructor) {
                return true;
            }
			foreach(var m in typeInfo.Members) {
				if(m.DisplayName == childName) {
                    // Special case for Behaviour.  Only messages are allowed.
                    if(parent.IsBehaviour) {
                        return m.IsMessage || m.IsField || m.IsProperty;
                    }
					return true;
				}
			}
			return false;                        
        }
        // Messages are only allow on their object instance.
        if(childType == iCS_ObjectTypeEnum.InstanceMessage || childType == iCS_ObjectTypeEnum.ClassMessage) {
            return false;
        }
        // Only allow State node in StateChart.
        if(parent.ObjectType == iCS_ObjectTypeEnum.StateChart) {
            return childType == iCS_ObjectTypeEnum.State;
        }
        // Only allow state, transition trigger package & entry/update/exit modules in state.
        if(parent.ObjectType == iCS_ObjectTypeEnum.State) {
            if(childType == iCS_ObjectTypeEnum.State || childType == iCS_ObjectTypeEnum.TransitionPackage) {
                return true;
            }
            if(childType == iCS_ObjectTypeEnum.Package) {
                return NameExistsIn(childName, StateChildNames) && !IsChildNodePresent(childName, parent, iStorage);
            }
        }
        // Allow all but Behaviour & State in module.
        if(parent.IsKindOfPackage) {
            if(childType == iCS_ObjectTypeEnum.Behaviour || childType == iCS_ObjectTypeEnum.State) {
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
}
