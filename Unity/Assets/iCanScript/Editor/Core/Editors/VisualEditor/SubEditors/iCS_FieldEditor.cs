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
        Rect boxPos= new Rect(myPosition.x-2.0f, myPosition.y, myPosition.width+4.0f, myPosition.height+1f);
        Color selectionColor= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).settings.selectionColor;
        iCS_Graphics.DrawBox(boxPos, selectionColor, Color.white, new Color(1.0f,1.0f,1.0f,1.0f));
        GUI.changed= false;
        string newValue= GUI.TextField(myPosition, myValue, myStyle);
        if(GUI.changed) {
            myValue= newValue;
        }
        return GUI.changed;
    }
}
