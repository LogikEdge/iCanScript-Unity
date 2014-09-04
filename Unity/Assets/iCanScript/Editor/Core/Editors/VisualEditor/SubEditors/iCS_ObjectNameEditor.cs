using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_ObjectNameEditor : iCS_ISubEditor {
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_EditorObject    myTarget  = null;
	iCS_FieldEditor	    myEditor  = null;
	iCS_Graphics		myGraphics= null;
	
    // ======================================================================
    // Property.
	// ----------------------------------------------------------------------
	Rect 	 Position {
		get {
			if(myTarget.IsPort) {
				return myGraphics.GetPortNameGUIPosition(myTarget, myTarget.IStorage);
			}
			return myGraphics.GetNodeNameGUIPosition(myTarget);
		}
	}
	GUIStyle GuiStyle {
        get {
            return myTarget.IsPort || myTarget.IsIconizedOnDisplay ?
                        myGraphics.LabelStyle :
                        myGraphics.TitleStyle;
        }
    }
	
    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_ObjectNameEditor(iCS_EditorObject target, iCS_Graphics graphics, Vector2 pickPoint) {
        myTarget= target;
		myGraphics= graphics;
		myEditor= new iCS_FieldEditor(Position, iCS_PreferencesEditor.RemoveProductPrefix(target.RawName), typeof(string), GuiStyle, pickPoint);
    }
    
    // ======================================================================
    // Update.
	// ----------------------------------------------------------------------
    public bool Update() {
        // Abort if target is invalid.
        if(myTarget == null) {
            return false;
        }
		myEditor.Position= Position;
		myEditor.GuiStyle= GuiStyle;
		if(myEditor.Update()) {
			iCS_UserCommands.ChangeName(myTarget, myEditor.Value as string);
			return true;
		}
		return false;
    }
}
