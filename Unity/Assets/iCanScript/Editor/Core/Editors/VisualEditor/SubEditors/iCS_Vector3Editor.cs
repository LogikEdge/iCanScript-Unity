using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {
public class iCS_Vector3Editor : iCS_ISubEditor {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    Vector3     myValue;
    Rect        myGuiPosition;
    GUIStyle    myStyle;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public Vector3  Value       { get { return myValue; }       set { myValue= value; }}
    public Rect     GuiPosition { get { return myGuiPosition; } set { myGuiPosition= value; }} 
    public GUIStyle GuiStyle    { get { return myStyle; }       set { myStyle= value; }}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public iCS_Vector3Editor(Rect guiPosition, Vector3 initialValue, GUIStyle style) {
        myGuiPosition= guiPosition;
        myValue= initialValue;
        myStyle= style;
    }
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public bool Update() {
        Rect boxPos= new Rect(myGuiPosition.x-2.0f, myGuiPosition.y, myGuiPosition.width+4.0f, myGuiPosition.height+1f);
        iCS_Graphics.DrawBox(boxPos, Color.black, myStyle.normal.textColor, new Color(1.0f,1.0f,1.0f,0.15f));
        GUI.changed= false;
        Vector3 newValue= EditorGUI.Vector3Field(myGuiPosition, "", myValue);
        if(GUI.changed) {
            myValue= newValue;
        }
        return GUI.changed;
    }
}
}
