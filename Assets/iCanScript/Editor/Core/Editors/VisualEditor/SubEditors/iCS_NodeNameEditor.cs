using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_NodeNameEditor : iCS_ISubEditor {
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
	Rect 	 Position { get { return myGraphics.GetNodeNameGUIPosition(myTarget, myIStorage); }}
	GUIStyle GuiStyle { get { return myTarget.IsIconized ? myGraphics.LabelStyle : myGraphics.TitleStyle; }}
	
    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_NodeNameEditor(iCS_EditorObject target, iCS_IStorage iStorage, iCS_Graphics graphics) {
        myIStorage= iStorage;
        myTarget= target;
		myGraphics= graphics;
		myEditor= new iCS_FieldEditor(Position, iCS_PreferencesEditor.RemoveProductPrefix(target.RawName), iCS_FieldTypeEnum.String, GuiStyle);
    }
    
    // ======================================================================
    // Update.
	// ----------------------------------------------------------------------
    public bool Update() {
		myEditor.Position= Position;
		myEditor.GuiStyle= GuiStyle;
		if(myEditor.Update()) {
			myTarget.Name= myEditor.ValueAsString;
			return true;
		}
		return false;
    }
}
