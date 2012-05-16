using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_StringEditor : iCS_ISubEditor {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    string      myValue;
    Rect        myGuiPosition;
    GUIStyle    myStyle;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public string   Value       { get { return myValue; }       set { myValue= value; }}
    public Rect     GuiPosition { get { return myGuiPosition; } set { myGuiPosition= value; }} 
    public GUIStyle GuiStyle    { get { return myStyle; }       set { myStyle= value; }}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public iCS_StringEditor(Rect guiPosition, string initialValue, GUIStyle style) {
        myGuiPosition= guiPosition;
        myValue= initialValue ?? "";
        myStyle= style;
    }
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public bool Update() {
		EditorGUIUtility.LookLikeControls();
        GUI.changed= false;
        string newValue= GUI.TextField(myGuiPosition, myValue, myStyle);
        if(GUI.changed) {
            myValue= newValue;
        }
        return GUI.changed;
    }
}
