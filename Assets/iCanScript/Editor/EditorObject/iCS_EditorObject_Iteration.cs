using UnityEngine;
using System;
using System.Collections;

/*
    TODO: Cleanup validation once child management as been cleaned up.
*/
public partial class iCS_EditorObject {
    // ======================================================================
    // Tree Iterations
    // ----------------------------------------------------------------------
    public void ForEachChild(Action<iCS_EditorObject> fnc) {
        if(!IsValid) return;
        foreach(var childId in Children) {
            if(childId != -1) fnc(EditorObjects[childId]);
        }
    }
    // ----------------------------------------------------------------------
    public bool ForEachChild(Func<iCS_EditorObject,bool> fnc) {
        if(!IsValid) return false;
        foreach(var childId in Children) {
            if(childId != -1) {
                if(fnc(EditorObjects[childId])) return true;
			} else {
				Debug.LogWarning("Children list includes an invalid id");
			}
        }
        return false;
    }
    // ----------------------------------------------------------------------
    public void ForEachRecursiveDepthFirst(Action<iCS_EditorObject> fnc) {
        // Nothing to do if the id is invalid.
        if(!IsValid) return;

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
        // Nothing to do if the id is invalid
        if(!IsValid) return;
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
    public void ForEachChildRecursiveDepthLast(Action<iCS_EditorObject> fnc) {
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
