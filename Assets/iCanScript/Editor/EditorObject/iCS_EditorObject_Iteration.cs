using UnityEngine;
using System;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Tree Iterations
    // ----------------------------------------------------------------------
    public void ForEachChild(Action<int> fnc) {
        if(!IsValid) return;
        foreach(var childId in Children) {
            if(childId != -1) fnc(childId);
        }
    }
    // ----------------------------------------------------------------------
    public bool ForEachChild(Func<int,bool> fnc) {
        if(!IsValid) return false;
        foreach(var childId in Children) {
            if(childId != -1) {
                if(fnc(childId)) return true;
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
        return false;
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst(Action<int> fnc) {
        // Nothing to do if the id is invalid.
        if(!IsValid) return;

        // First iterate through all children ...
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= myIStorage.EditorObjects[childId];
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
        fnc(InstanceId);
    }
	// ----------------------------------------------------------------------
    public void ForEachRecursiveDepthLast(Action<int> fnc) {
        // Nothing to do if the id is invalid
        if(!IsValid) return;
        // First this node ...
        fnc(InstanceId);
        // ... then iterate through all children.
        foreach(var childId in Children) {
			if(childId != -1) {
				var child= myIStorage.EditorObjects[childId];
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
    public void ForEachChildRecursiveDepthFirst(Action<int> fnc) {
        // Nothing to do if the id is invalid.
        if(IsValid) return;
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
    public void ForEachChildRecursiveDepthLast(Action<int> fnc) {
        // Nothing to do if the id is invalid
        if(IsValid) return;
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
