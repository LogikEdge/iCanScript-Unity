using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

    public class PropertyEditor : NodeEditor {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        DSCellView              myMainView         = null;
        iCS_PropertyController  myController       = null;
    	
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        const int   kSpacer= 8;
        
        // =================================================================================
        // Activation/Deactivation.
        // ---------------------------------------------------------------------------------
    	public static new EditorWindow Create(iCS_EditorObject node, Vector2 screenPosition) {
    		// Get existing open window or if none, make a new one:
    		var self = PropertyEditor.CreateInstance<PropertyEditor>();
            self.vsObject= node;
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            self.titleContent= new GUIContent("Property Editor", iCanScriptLogo);
            self.ShowUtility();
            return self;
    	}
    	
        // ---------------------------------------------------------------------------------
        bool IsInitialized() {
    		// Update main view if selection has changed.
            if(myMainView == null || myController == null) {
                   myController= new iCS_PropertyController(vsObject, vsObject.IStorage);            
                   myMainView  = new DSCellView(new RectOffset(0,0,kSpacer,0), true, myController.View);
    			   ResizeToFit();
    			   ResizeToFit();
            }		
            return true;
        }
    	
        // =================================================================================
        // Display.
        // ---------------------------------------------------------------------------------
        public new void OnGUI() {
            // Wait until window is configured.
            if(!IsInitialized()) return;
    
            myMainView.Display(new Rect(0,0,position.width, position.height));
        }
        // ---------------------------------------------------------------------------------
    	/// Resizes the editor panel to fit the content.
    	void ResizeToFit() {
    		var displaySize= myMainView.GetSizeToDisplay(position);
    		position= new Rect(position.x, position.y, displaySize.x, displaySize.y);		
    	}
    }
    
}