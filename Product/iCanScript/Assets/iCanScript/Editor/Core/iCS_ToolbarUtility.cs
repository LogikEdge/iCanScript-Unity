using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_ToolbarUtility {
        // ======================================================================
        // Fields
    	// ----------------------------------------------------------------------
        static GUIStyle    myButtonStyle    = null;
        static GUIContent  myEmptyGUIContent= null;
    
        // ======================================================================
        // Initialization
    	// ----------------------------------------------------------------------
        static iCS_ToolbarUtility() {
            myButtonStyle        = new GUIStyle(EditorStyles.toolbarButton);
            myButtonStyle.padding= new RectOffset(0,0,0,0);
            myEmptyGUIContent    = new GUIContent();
        }
    
        // ======================================================================
        // Toolbar utilities.
    	// ----------------------------------------------------------------------
        public static Rect BuildToolbar(float width, float yOffset= 0) {
            Rect toolbarRect= new Rect(0,yOffset,width,GetHeight());
            return BuildToolbar(toolbarRect);
        }
    	// ----------------------------------------------------------------------
        public static Rect BuildToolbar(Rect toolbarRect) {
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
        public static void MiniLabel(ref Rect toolbarRect, string label, float leftMargin, float rightMargin, bool isRightJustified= false) {
            MiniLabel(ref toolbarRect, new GUIContent(label), leftMargin, rightMargin, isRightJustified);
    	}
    	// ----------------------------------------------------------------------
        public static void MiniLabel(ref Rect toolbarRect, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		var contentSize= EditorStyles.miniLabel.CalcSize(content);
    		Rect r= ReserveArea(ref toolbarRect, contentSize.x, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return;
            float offset= 0.5f*(r.height-contentSize.y);
            r.y+= offset;
            r.height-= offset;
    		GUI.Label(r, content, EditorStyles.miniLabel);        
        }
    	// ----------------------------------------------------------------------
        public static void MiniLabel(ref Rect toolbarRect, float width, string label, float leftMargin, float rightMargin, bool isRightJustified= false) {
            MiniLabel(ref toolbarRect, width, new GUIContent(label), leftMargin, rightMargin, isRightJustified);
        }
    	// ----------------------------------------------------------------------
        public static void MiniLabel(ref Rect toolbarRect, float width, GUIContent content, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		var contentSize= EditorStyles.miniLabel.CalcSize(content);
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return;
            float offset= 0.5f*(r.height-contentSize.y);
            r.y+= offset;
            r.height-= offset;
    		GUI.Label(r, content, EditorStyles.miniLabel);        
        }
    	// ----------------------------------------------------------------------
        public static float Slider(ref Rect toolbarRect, float width, float value, float leftValue, float rightValue, float rightMargin, float leftMargin, bool isRightJustified= false) {
    		var contentSize= GUI.skin.horizontalSlider.CalcSize(new GUIContent());
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return value;
            float offset= 0.5f*(r.height-contentSize.y);
            r.y+= offset;
            r.height-= offset;
            return GUI.HorizontalSlider(r, value, leftValue, rightValue);
        }
    	// ----------------------------------------------------------------------
        public static string Text(ref Rect toolbarRect, float width, string value, float leftMargin, float rightMargin, bool isRightJustified= false) {
            GUIContent content= new GUIContent(value);
    		var contentSize= EditorStyles.toolbarTextField.CalcSize(content);
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return value;
            float offset= 0.5f*(r.height-contentSize.y);
            r.y+= offset;
            r.height-= offset;
            return GUI.TextField(r, value, EditorStyles.toolbarTextField);
        }
    	// ----------------------------------------------------------------------
        public static void Texture(ref Rect toolbarRect, Texture texture, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		var textureSize= new Vector2(texture.width, texture.height);
            if(texture.height > toolbarRect.height) {
                textureSize*= 0.8f*toolbarRect.height/texture.height;
            }
    		Rect r= ReserveArea(ref toolbarRect, textureSize.x, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return;
            float offset= 0.5f*(r.height-textureSize.y);
            r.y+= offset;
            r.height-= offset;
            GUI.Label(r, texture);
        }
    	// ----------------------------------------------------------------------
        public static void Texture(ref Rect toolbarRect, float width, float height, Texture texture, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		var textureSize= new Vector2(width, height);
    		Rect r= ReserveArea(ref toolbarRect, textureSize.x, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return;
            float offset= 0.5f*(r.height-textureSize.y);
            r.y+= offset;
            r.height-= offset;
            GUI.Label(r, texture);
        }
    	// ----------------------------------------------------------------------
    	// FIXME: CenterLabel: left margin not functional
        public static void CenteredTitle(ref Rect toolbarRect, string title) {
            GUIContent content= new GUIContent(title);
            var style= EditorStyles.boldLabel;
    		var contentSize= style.CalcSize(content);
    		float w= contentSize.x;
    		float dx= 0.5f*(toolbarRect.width-w);
    		if(dx < 0) {
    			dx= 0;
    			w= toolbarRect.width;
    		}
            float offset= 0.5f*(toolbarRect.height-contentSize.y);
            Rect r= toolbarRect;
            r.y+= offset;
            r.height-= offset;
    		r.x+= dx;
    		r.width= w;
            GUI.Label(r, content, style);
        }
    	// ----------------------------------------------------------------------
        public static bool Button(ref Rect toolbarRect, bool value, Texture texture, float leftMargin, float rightMargin, bool isRightJustified= false) {
            float width= texture.width;
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return value;
            if(!value) {
                GUI.color= new Color(1f,1f,1f,0.35f);            
            }
            var newValue= GUI.Button(r, texture, myButtonStyle);
            GUI.color= Color.white;            
            return newValue;
        }
    	// ----------------------------------------------------------------------
        public static bool Button(ref Rect toolbarRect, float width, bool value, Texture texture, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return value;
            if(r.width < 1f) return value;
            if(!value) {
                GUI.color= new Color(1f,1f,1f,0.35f);            
            }
            var newValue= GUI.Button(r, texture, myButtonStyle);
            GUI.color= Color.white;            
            return newValue;
        }
    	// ----------------------------------------------------------------------
        public static bool Button(ref Rect toolbarRect, float width, bool value, string text, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return value;
            if(r.width < 1f) return value;
            if(!value) {
                GUI.color= new Color(1f,1f,1f,0.35f);            
            }
            var newValue= GUI.Button(r, text, myButtonStyle);
            GUI.color= Color.white;            
            return newValue;
        }
    	// ----------------------------------------------------------------------
        public static int Toolbar(ref Rect toolbarRect, float width, int value, string[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
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
            Texture2D searchIcon= null;
            if(TextureCache.GetTexture(iCS_EditorStrings.SearchIcon, out searchIcon)) {
                GUI.DrawTexture(new Rect(r.xMin-searchIcon.width, r.y-1f, searchIcon.width, searchIcon.height), searchIcon);
            } else {
                Debug.LogWarning("iCanScript: Cannot find search Icon in resource folder !!!");
            }
            Texture2D cancelIcon= null;
            if(TextureCache.GetTexture(iCS_EditorStrings.CancelIcon, out cancelIcon)) {
    			Rect cancelRect= new Rect(r.xMax, r.y-1f, cancelIcon.width, cancelIcon.height);
                GUI.DrawTexture(cancelRect, cancelIcon);
    			// Clear the search if the cancel icon is depressed.
    			var ev= Event.current;
    			if(ev != null && ev.isMouse) {
    				if(cancelRect.Contains(ev.mousePosition)) {
    					return "";
    				}
    			}
            } else {
                Debug.LogWarning("iCanScript: Cannot find cancel Icon in resource folder !!!");
            }
            return GUI.TextField(r, value, EditorStyles.toolbarTextField);        
        }
    	// ----------------------------------------------------------------------
        public static int Popup(ref Rect toolbarRect, float width, string label, int index, string[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return index;
            var idx= EditorGUI.Popup(r, label, index, options, EditorStyles.toolbarDropDown);
			return idx;
        }
    	// ----------------------------------------------------------------------
        public static int Popup(ref Rect toolbarRect, float width, GUIContent content, int index, GUIContent[] options, float leftMargin, float rightMargin, bool isRightJustified= false) {
    		Rect r= ReserveArea(ref toolbarRect, width, leftMargin, rightMargin, isRightJustified);		
            if(r.width < 1f) return index;
            return EditorGUI.Popup(r, content, index, options, EditorStyles.toolbarDropDown);
        }
    	// ----------------------------------------------------------------------
        public static bool Toggle(ref Rect toolbarRect, bool value, float leftMargin, float rightMargin, bool isRightJustified= false) {
            var contentSize= GUI.skin.toggle.CalcSize(myEmptyGUIContent);
    		Rect r= ReserveArea(ref toolbarRect, contentSize.x, leftMargin, rightMargin, isRightJustified);
            if(r.width < 1f) return value;
            float offset= 0.5f*(r.height-contentSize.y);
            r.y+= offset;
            r.height-= offset;
    		return GUI.Toggle(r, value, "");
        }
    	// ----------------------------------------------------------------------
        public static void Separator(ref Rect toolbarRect, bool isRightJustified= false) {
            Rect r= ReserveArea(ref toolbarRect, 2, 0, 0, isRightJustified);
            GUI.Box(r, "", EditorStyles.toolbarButton);
        }
    }
}

