using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

/*
    TODO: Cleanup validation once child management as been cleaned up.
*/
public partial class iCS_EditorObject {
    // Children Iterations =================================================
    public void ForEachChild(Action<iCS_EditorObject> fnc) {
        foreach(var childId in Children) {
            fnc(EditorObjects[childId]);
        }
    }
    // ----------------------------------------------------------------------
    public void ForEachChildPort(Action<iCS_EditorObject> action) {
        ForEachChild(o=> { if(o.IsPort) action(o); });
    }
    // ----------------------------------------------------------------------
    public void ForEachChildNode(Action<iCS_EditorObject> action) {
        ForEachChild(o=> { if(o.IsNode) action(o); });
    }
    // ----------------------------------------------------------------------
    public void ForEachLeftChildPort(Action<iCS_EditorObject> action) {
        ForEachChildPort(o=> { if(o.IsOnLeftEdge) action(o); });
    }
    // ----------------------------------------------------------------------
    public void ForEachRightChildPort(Action<iCS_EditorObject> action) {
        ForEachChildPort(o=> { if(o.IsOnRightEdge) action(o); });
    }
    // ----------------------------------------------------------------------
    public void ForEachTopChildPort(Action<iCS_EditorObject> action) {
        ForEachChildPort(o=> { if(o.IsOnTopEdge) action(o); });
    }
    // ----------------------------------------------------------------------
    public void ForEachBottomChildPort(Action<iCS_EditorObject> action) {
        ForEachChildPort(o=> { if(o.IsOnBottomEdge) action(o); });
    }
    
    // ----------------------------------------------------------------------
    public bool ForEachChild(Func<iCS_EditorObject,bool> fnc) {
        foreach(var childId in Children) {
            if(fnc(EditorObjects[childId])) return true;
        }
        return false;
    }

    // Recursive Iterations =================================================
    public void ForEachRecursiveDepthFirst(Action<iCS_EditorObject> fnc) {
        // First iterate through all children ...
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= EditorObjects[childId];
				if(child != null) {
		            child.ForEachRecursiveDepthFirst(fnc);									
				} else {
					Debug.LogWarning("Mismatch between children list and EditorObject container !!!");
				}
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
        
        // ... then this node.
        fnc(EditorObject);
    }
	// ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast(Action<iCS_EditorObject> fnc) {
        // First this node ...
        fnc(EditorObject);
        // ... then iterate through all children.
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= EditorObjects[childId];
				if(child != null) {
		            child.ForEachRecursiveDepthLast(fnc);									
				} else {
					Debug.LogWarning("Mismatch between children list and EditorObject container !!!");
				}
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
    }
    // ----------------------------------------------------------------------
    public void ForEachChildRecursiveDepthFirst(Action<iCS_EditorObject> fnc) {
        // Iterate through all children ...
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= myIStorage[childId];
				if(child != null) {
		            child.ForEachRecursiveDepthFirst(fnc);
				} else {
					Debug.LogWarning("Mismatch between children list and EditorObject container !!!");
				}
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
    }
    // ----------------------------------------------------------------------
    public void ForEachChildRecursiveDepthLast(Action<iCS_EditorObject> fnc) {
        // Iterate through all children.
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= myIStorage[childId];
				if(child != null) {
            		child.ForEachRecursiveDepthLast(fnc);
				} else {
					Debug.LogWarning("Mismatch between children list and EditorObject container !!!");
				}
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
    }

}
