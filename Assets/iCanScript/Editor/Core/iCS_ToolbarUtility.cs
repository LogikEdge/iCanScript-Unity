using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_ToolbarUtility {
    // ======================================================================
    // Toolbar utilities.
	// ----------------------------------------------------------------------
    public static Rect BuildToolbar(float width, float yOffset= 0) {
        Rect toolbarRect= new Rect(0,yOffset,width,GetHeight());
        Rect r= toolbarRect;
        r.height+= 20f;
		GUI.Box(r, "", EditorStyles.toolbar);
        return toolbarRect;
    }
	// ----------------------------------------------------------------------
    public static float GetHeight() {
		return iCS_EditorUtility.GetGUIStyleHeight(EditorStyles.toolbar);        
    }	// ----------------------------------------------------------------------
	public static Rect ReserveArea(ref Rect r, float width, float leftMargin, float rightMargin, bool isRightJustified) {
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
    public static void Label(ref Rect toolbarRect, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
		var contentSize= EditorStyles.toolbar.CalcSize(content);
		Rect r= ReserveArea(ref toolbarRect, contentSize.x, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return;
		GUI.Label(r, content, EditorStyles.toolbar);        
    }
	// ----------------------------------------------------------------------
    public static void Label(ref Rect toolbarRect, float width, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return;
		GUI.Label(r, content, EditorStyles.toolbar);        
    }
	// ----------------------------------------------------------------------
    public static float Slider(ref Rect toolbarRect, float width, float value, float leftValue, float rightValue, float rightMargin, float leftMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        return GUI.HorizontalSlider(r, value, leftValue, rightValue);
    }
	// ----------------------------------------------------------------------
    public static string Text(ref Rect toolbarRect, float width, string value, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        r.y+= 2f;
        return GUI.TextField(r, value, EditorStyles.toolbarTextField);
    }
	// ----------------------------------------------------------------------
    public static int Buttons(ref Rect toolbarRect, float width, int value, string[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return value;
        int newValue= GUI.Toolbar(r, value, options, EditorStyles.toolbarButton);
		return newValue;
    }
	// ----------------------------------------------------------------------
    public static string Search(ref Rect toolbarRect, float width, string value, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        float iconSize= GetHeight();
        if(r.width < iconSize+1f) return value;
        r.y+= 2f;
        r.width-= iconSize;
        Texture2D cancelIcon= null;
        if(iCS_TextureCache.GetTexture(iCS_EditorStrings.CancelIcon, out cancelIcon)) {
            GUI.DrawTexture(new Rect(r.xMax, r.y, iconSize, iconSize), cancelIcon);
        } else {
            Debug.LogWarning("iCanScript: Cannot find cancel Icon in resource folder !!!");
        }
        return GUI.TextField(r, value, EditorStyles.toolbarTextField);        
    }
	// ----------------------------------------------------------------------
    public static int Popup(ref Rect toolbarRect, float width, string label, int index, string[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return index;
        return EditorGUI.Popup(r, label, index, options, EditorStyles.toolbarDropDown);
    }
	// ----------------------------------------------------------------------
    public static int Popup(ref Rect toolbarRect, float width, GUIContent content, int index, GUIContent[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
        if(r.width < 1f) return index;
        return EditorGUI.Popup(r, content, index, options, EditorStyles.toolbarDropDown);
    }
}
