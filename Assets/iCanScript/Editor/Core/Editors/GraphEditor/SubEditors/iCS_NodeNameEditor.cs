using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_NodeNameEditor : iCS_ISubEditor {
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_IStorage        myIStorage= null;
    iCS_EditorObject    myTarget  = null;
	iCS_StringEditor	myEditor  = null;
	iCS_Graphics		myGraphics= null;
	
    // ======================================================================
    // Property.
	// ----------------------------------------------------------------------
	Rect 	 Position { get { return myGraphics.GetNodeNameGUIPosition(myTarget, myIStorage); }}
	GUIStyle GuiStyle { get { return myTarget.IsMinimized ? myGraphics.LabelStyle : myGraphics.TitleStyle; }}
	
    // ======================================================================
    // Initialization.
	// ----------------------------------------------------------------------
    public iCS_NodeNameEditor(iCS_EditorObject target, iCS_IStorage iStorage, iCS_Graphics graphics) {
        myIStorage= iStorage;
        myTarget= target;
		myGraphics= graphics;
		myEditor= new iCS_StringEditor(Position, graphics.GetNodeName(target, iStorage), GuiStyle);
    }
    
    public bool Update() {
		myEditor.GuiPosition= Position;
		myEditor.GuiStyle= GuiStyle;
		if(myEditor.Update()) {
			myTarget.Name= myEditor.Value;
			return true;
		}
		return false;
    }
}
