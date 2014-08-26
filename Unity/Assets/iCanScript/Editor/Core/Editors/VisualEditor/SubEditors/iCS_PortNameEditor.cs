using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PortNameEditor : iCS_ISubEditor {
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_EditorObject    myTarget  = null;
	iCS_FieldEditor	    myEditor  = null;
	iCS_Graphics		myGraphics= null;
	
    // ======================================================================
    // Property.
	// ----------------------------------------------------------------------
	Rect 	 Position { get { return myGraphics.GetPortNameGUIPosition(myTarget, myTarget.IStorage); }}
	GUIStyle GuiStyle { get { return myGraphics.LabelStyle; }}

    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_PortNameEditor(iCS_EditorObject target, iCS_Graphics graphics) {
        myTarget= target;
		myGraphics= graphics;
		myEditor= new iCS_FieldEditor(Position, myTarget.RawName, iCS_FieldTypeEnum.String, GuiStyle);
    }
    
    // ======================================================================
    // Update.
	// ----------------------------------------------------------------------
    public bool Update() {
		myEditor.Position= Position;
		if(myEditor.Update()) {
			myTarget.Name= myEditor.ValueAsString;
			return true;
		}
		return false;
    }
}
