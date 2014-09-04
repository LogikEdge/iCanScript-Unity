using UnityEngine;
using UnityEditor;
using System.Collections;

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
	Rect 	 Position { get { return myGraphics.GetPortNameGUIPosition(myPort, myPort.IStorage); }}
	GUIStyle GuiStyle { get { return myGraphics.ValueStyle; }}

    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_PortValueEditor(iCS_EditorObject port, iCS_Graphics graphics, Vector2 pickPoint) {
        myPort= port;
		myGraphics= graphics;
		myEditor= new iCS_FieldEditor(Position, myPort.RawName, myPort.RuntimeType, GuiStyle, pickPoint);
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
