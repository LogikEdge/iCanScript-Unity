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
	// ----------------------------------------------------------------------
    // Ask the user confirmation to destroy the object.  True is returned
    // if the user has accepted to delete the object; false is returned 
    // otherwise.
    public static bool DestroyObject(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.RegisterUndo("Delete");
            storage.DestroyInstance(selectedObject.InstanceId);                        
            return true;
        }            
        return false;
    }

    // ======================================================================
    // GUI helpers
	// ----------------------------------------------------------------------
    public static GUIStyle CloneGUIStyle(string name, GUIStyle toClone) {
        GUIStyle instance= new GUIStyle();
        instance.name         = name;
        instance.normal       = CloneGUIStyleState(toClone.normal);
        instance.hover        = CloneGUIStyleState(toClone.hover);
        instance.active       = CloneGUIStyleState(toClone.active);
        instance.focused      = CloneGUIStyleState(toClone.focused);
        instance.onNormal     = CloneGUIStyleState(toClone.onNormal);
        instance.onHover      = CloneGUIStyleState(toClone.onHover);
        instance.onActive     = CloneGUIStyleState(toClone.onActive);
        instance.onFocused    = CloneGUIStyleState(toClone.onFocused);
        instance.border       = toClone.border;
        instance.margin       = toClone.margin;
        instance.padding      = toClone.padding;
        instance.overflow     = toClone.overflow;
        instance.font         = toClone.font;
        instance.imagePosition= toClone.imagePosition;
        instance.alignment    = toClone.alignment;
        instance.wordWrap     = toClone.wordWrap;
        instance.clipping     = toClone.clipping;
        instance.contentOffset= toClone.contentOffset;
        instance.fixedWidth   = toClone.fixedWidth;
        instance.fontSize     = toClone.fontSize;
        instance.fontStyle    = toClone.fontStyle;
        instance.fixedHeight  = toClone.fixedHeight;
        instance.stretchWidth = toClone.stretchWidth;
        instance.stretchHeight= toClone.stretchHeight;
        return instance;
    }
	// ----------------------------------------------------------------------
    public static GUIStyleState CloneGUIStyleState(GUIStyleState toClone) {
        GUIStyleState instance= new GUIStyleState();
        instance.textColor= toClone.textColor;
        instance.background= toClone.background;
        return instance;
    }
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
