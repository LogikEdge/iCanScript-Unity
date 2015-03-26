using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScript.Editor {
    public class iCS_PortValueEditor : iCS_ISubEditor {
        // ======================================================================
        // Field.
    	// ----------------------------------------------------------------------
        iCS_EditorObject    myPort    = null;
    	iCS_FieldEditor	    myEditor  = null;
    	iCS_Graphics		myGraphics= null;
    	
        // ======================================================================
        // Property.
    	// ----------------------------------------------------------------------
    	Rect 	 Position { get { return myGraphics.GetPortValueGUIPosition(myPort); }}
    	GUIStyle GuiStyle { get { return iCS_Layout.ValueStyle; }}
    
        // ======================================================================
    	// Use to determine which value type is supported.
    	// ----------------------------------------------------------------------
    	public static bool IsValueEditionSupported(Type type) {
    		if(iCS_FieldEditor.GetInputValidator(type) != null) return true;
    		return false;
    	}
    	
        // ======================================================================
        // Initialization.
    	// ----------------------------------------------------------------------
        public iCS_PortValueEditor(iCS_EditorObject port, iCS_Graphics graphics, Vector2 pickPoint) {
            myPort= port;
    		myGraphics= graphics;
    		myEditor= new iCS_FieldEditor(Position, myPort.PortValue, myPort.RuntimeType, GuiStyle, pickPoint);
    		myEditor.SetBackgroundAlpha(1f);
        }
        
        // ======================================================================
        // Update.
    	// ----------------------------------------------------------------------
        public bool Update() {
    		myEditor.Position= Position;
    		if(myEditor.Update()) {
    			iCS_UserCommands.ChangePortValue(myPort, myEditor.Value);
    			return true;
    		}
    		return false;
        }
    }
}