using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_EditorUtility {
    // ======================================================================
    // Tree Navigation
	// ----------------------------------------------------------------------
    public static iCS_EditorObject GetFirstChild(iCS_EditorObject parent, iCS_IStorage storage) {
        if(parent == null) return null;
        iCS_EditorObject firstChild= parent;
        storage.ForEachChild(parent,
            child=> {
                if(child.IsNode) {
                    firstChild= child;
                    return true;
                }
                return false;
            }
        );
        return firstChild;
    }
	// ----------------------------------------------------------------------
    public static iCS_EditorObject GetNextSibling(iCS_EditorObject node, iCS_IStorage storage) {
        if(node == null || storage.GetParent(node) == null) return null;
        iCS_EditorObject nextSibling= null;
        iCS_EditorObject firstChild= null;
        bool nodeFound= false;
        storage.ForEachChild(storage.GetParent(node),
            child=> {
                if(child.IsNode) {
                    if(firstChild == null) {
                        firstChild= child;
                    }
                    if(child == node) {
                        nodeFound= true;
                        return false;
                    }
                    if(nodeFound) {
                        nextSibling= child;
                        return true;
                    }
                }
                return false;
            }
        );
        if(!nodeFound) return node;
        return nextSibling ?? firstChild;
    }
	// ----------------------------------------------------------------------
    public static iCS_EditorObject GetPreviousSibling(iCS_EditorObject node, iCS_IStorage storage) {
        if(node == null || storage.GetParent(node) == null) return null;
        iCS_EditorObject prevSibling= null;
        storage.ForEachChild(storage.GetParent(node),
            child=> {
                if(child.IsNode) {
                    if(child == node && prevSibling != null) {
                        return true;
                    }
                    prevSibling= child;
                }
                return false;
            }
        );
        return prevSibling ?? node;
    }

    // ======================================================================
    // Object high-order manipulation.
	// ----------------------------------------------------------------------
    // Ask the user confirmation to destroy the object.  True is returned
    // if the user has accepted to delete the object; false is returned 
    // otherwise.
    public static void DestroyObject(iCS_EditorObject selectedObject, iCS_IStorage iStorage) {
        iStorage.RegisterUndo("Removing: "+selectedObject.Name);
        iStorage.DestroyInstance(selectedObject.InstanceId);                        
    }

    // ======================================================================
    // GUI helpers
	// ----------------------------------------------------------------------
	public static float GetGUIStyleHeight(GUIStyle style) {
		float height= style.lineHeight+style.border.vertical;
		Texture backgroundTexture= style.normal.background;
		if(backgroundTexture != null) {
			height= backgroundTexture.height;
		}
		return height;		
	}
}
