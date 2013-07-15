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
    public static bool CanAddChildNode(string childName, iCS_ObjectTypeEnum childType, iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return false;
        // Only allow valid child for object instances.
        if(parent.IsObjectInstance || parent.IsBehaviour) {
            // Don't allow more then one copy of a node in an instance node
            if(IsChildNodePresent(childName, parent, storage)) {
                return false;
            }
			var typeInfo= iCS_LibraryDatabase.GetTypeInfo(parent.IsBehaviour ? typeof(MonoBehaviour) : parent.RuntimeType);
			if(typeInfo == null) {
				Debug.LogWarning("iCanScript: Unable to find type information for: "+parent.Name);
				return false;
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
        // Only allow state, module transition & entry/do/exit modules in state.
        if(parent.ObjectType == iCS_ObjectTypeEnum.State) {
            if(childType == iCS_ObjectTypeEnum.State || childType == iCS_ObjectTypeEnum.TransitionModule) {
                return true;
            }
            if(childType == iCS_ObjectTypeEnum.Package) {
                return NameExistsIn(childName, StateChildNames) && !IsChildNodePresent(childName, parent, storage);
            }
        }
        // Only allow TransitionGuard & TransitionAction in TransitionModule
        if(parent.ObjectType == iCS_ObjectTypeEnum.TransitionModule) {
            return childType == iCS_ObjectTypeEnum.TransitionGuard || childType == iCS_ObjectTypeEnum.TransitionAction;
        }
        // Allow all but Behaviour & State in module.
        if(parent.IsKindOfAggregate) {
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
