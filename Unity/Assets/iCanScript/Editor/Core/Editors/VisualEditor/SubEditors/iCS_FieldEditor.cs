using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public enum iCS_FieldTypeEnum { String, Integer, Float };
public class iCS_FieldEditor : iCS_ISubEditor {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    string              myValue;
    Rect                myPosition;
    iCS_FieldTypeEnum   myFieldType;
    GUIStyle            myStyle;
	int					myCursor= 0;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public Rect     Position        { get { return myPosition; } set { myPosition= value; }}
    public GUIStyle GuiStyle        { get { return myStyle; }    set { myStyle= value; }}
    public string   ValueAsString   { get { return myValue; }}
    public long     ValueAsInteger  { get { return (long)Convert.ChangeType(myValue, typeof(long)); }}
    public float    ValueAsFloat    { get { return (float)Convert.ChangeType(myValue, typeof(float)); }}
    public object   Value       {
        get {
            switch(myFieldType) {
                case iCS_FieldTypeEnum.String: {
                    return ValueAsString;
                }
                case iCS_FieldTypeEnum.Integer: {
                    return ValueAsInteger;
                }
                case iCS_FieldTypeEnum.Float: {
                    return ValueAsFloat;
                }
            }
            return myValue;
        }
    }
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public iCS_FieldEditor(Rect position, object initialValue, iCS_FieldTypeEnum fieldType, GUIStyle guiStyle) {
        myValue    = (string)Convert.ChangeType(initialValue, typeof(string));
        myPosition = position;
        myFieldType= fieldType;
        myStyle    = guiStyle;
    }

    // =================================================================================
    // Update
    // ---------------------------------------------------------------------------------
    public bool Update() {
        Rect boxPos= new Rect(myPosition.x-2.0f, myPosition.y-1f, myPosition.width+4.0f, myPosition.height+2f);
        Color selectionColor= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).settings.selectionColor;
        iCS_Graphics.DrawBox(boxPos, new Color(0.5f,0.5f,0.5f,0.5f), selectionColor, Color.white);
		boxPos.x+= 1f;
		boxPos.width-= 2f;
		boxPos.y+= 1f;
		boxPos.height-= 2f;
        iCS_Graphics.DrawBox(boxPos, Color.clear, selectionColor, Color.white);
		GUI.Label(myPosition, myValue, myStyle);
        GUI.changed= false;
        GUI.SetNextControlName("SubEditor");
//        string newValue= GUI.TextField(myPosition, myValue, myStyle);
//		var guiStyle= new GUIStyle(EditorStyles.textField);
//		guiStyle.fontSize= myStyle.fontSize;
//		guiStyle.fontStyle= myStyle.fontStyle;
//		GUI.skin.settings.cursorColor= Color.red;
//		GUI.skin.settings.cursorFlashSpeed= 1;
//		GUI.backgroundColor= Color.clear;
//        string newValue= GUI.TextField(boxPos/*myPosition*/, myValue, guiStyle);
		ShowCursor(4, Color.red, 0.5f);
		var newValue= myValue;
        if(GUI.changed) {
            myValue= newValue;
        }
        return GUI.changed;
    }
    // ---------------------------------------------------------------------------------
	void ShowCursor(int cursor, Color cursorColor, float blinkSpeed) {
		if(Math3D.IsNotZero(blinkSpeed)) {
			int step= (int)(Time.realtimeSinceStartup/blinkSpeed);
			if((step & 1) == 0) {
				return;
			}
		}
		Handles.color= cursorColor;
		var x= myPosition.x;
		var y= myPosition.y;
		var yMax= myPosition.yMax;
		if(cursor != 0) {
			// Limit the cursor movement within the value string
			if(cursor > myValue.Length) {
				cursor= myValue.Length;
			}
			var beforeCursor= myValue.Substring(0, cursor);
			var size= myStyle.CalcSize(new GUIContent(beforeCursor));
			x+= size.x;
		}
		Handles.DrawLine(new Vector3(x,y,0), new Vector3(x,yMax,0));
		Handles.DrawLine(new Vector3(x+1,y,0), new Vector3(x+1,yMax,0));
	}
}
