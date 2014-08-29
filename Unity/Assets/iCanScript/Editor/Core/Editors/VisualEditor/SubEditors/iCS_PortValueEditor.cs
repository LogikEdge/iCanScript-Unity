using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PortValueEditor : iCS_ISubEditor {
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_IStorage        myIStorage= null;
    iCS_EditorObject    myTarget  = null;
	iCS_FieldEditor	    myEditor  = null;
	iCS_Graphics		myGraphics= null;
	
    // ======================================================================
    // Property.
	// ----------------------------------------------------------------------
	Rect 	 Position { get { return myGraphics.GetPortNameGUIPosition(myTarget, myIStorage); }}
	GUIStyle GuiStyle { get { return myGraphics.LabelStyle; }}

    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_PortValueEditor(iCS_EditorObject target, iCS_IStorage iStorage, iCS_Graphics graphics, Vector2 pickPoint) {
        myIStorage= iStorage;
        myTarget= target;
		myGraphics= graphics;
		myEditor= new iCS_FieldEditor(Position, myTarget.RawName, iCS_FieldTypeEnum.String, GuiStyle, pickPoint);
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
