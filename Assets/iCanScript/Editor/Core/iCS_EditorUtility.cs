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

	// ----------------------------------------------------------------------
	public static Rect HeaderRect(ref Rect r, float width, float leftMargin, float rightMargin, bool isRightJustified) {
        // Validate that we have the space asked.
        float totalSize= width+leftMargin+rightMargin;
        if(totalSize > r.width) {
            // We cannot allocate the asked size, so lets reduce the width.
            width= r.width-leftMargin-rightMargin;
            if(width <= 0) return new Rect(r.x,r.y,0,r.height);
        }
        Rect result= new Rect(0,0,0,0);
        if(isRightJustified) {
    		result= new Rect(r.xMax-width-rightMargin, r.y, width, r.height);
            
        } else {
    		result= new Rect(r.x+leftMargin, r.y, width, r.height);
    		r.x+= totalSize;
        }
		r.width-= totalSize;            
		return result;
	} 

	// ----------------------------------------------------------------------
    public static void ToolbarLabel(ref Rect toolbarRect, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
		var contentSize= EditorStyles.toolbar.CalcSize(content);
		Rect r= HeaderRect(ref toolbarRect, contentSize.x, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return;
		GUI.Label(r, content, EditorStyles.toolbar);        
    }
	// ----------------------------------------------------------------------
    public static void ToolbarLabel(ref Rect toolbarRect, float width, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= HeaderRect(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return;
		GUI.Label(r, content, EditorStyles.toolbar);        
    }
	// ----------------------------------------------------------------------
    public static float ToolbarSlider(ref Rect toolbarRect, float width, float value, float leftValue, float rightValue, float rightMargin, float leftMargin, bool isRightJustified= false) {
		Rect r= HeaderRect(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        return GUI.HorizontalSlider(r, value, leftValue, rightValue);
    }
	// ----------------------------------------------------------------------
    public static string ToolbarText(ref Rect toolbarRect, float width, string value, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= HeaderRect(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        r.y+= 1f;
        return GUI.TextField(r, value, EditorStyles.toolbarTextField);
    }
	// ----------------------------------------------------------------------
    public static int ToolbarButtons(ref Rect toolbarRect, float width, int value, string[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= HeaderRect(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        return GUI.Toolbar(r, value, options, EditorStyles.toolbarButton);
    }
}
