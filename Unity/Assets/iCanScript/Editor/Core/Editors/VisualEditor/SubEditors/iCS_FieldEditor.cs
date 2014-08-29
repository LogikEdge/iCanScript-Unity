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
	public object ValueAs(Type type) {
		return Convert.ChangeType(myValue, type);
	}
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    public iCS_FieldEditor(Rect position, object initialValue, iCS_FieldTypeEnum fieldType, GUIStyle guiStyle, Vector2 pickPoint) {
        myValue    = (string)Convert.ChangeType(initialValue, typeof(string));
        myPosition = position;
        myFieldType= fieldType;
        myStyle    = guiStyle;
        myCursor   = GetCursorIndexFromPosition(myPosition, pickPoint, myValue, myStyle);
    }

    // =================================================================================
    // Update
    // ---------------------------------------------------------------------------------
    public bool Update() {
        Rect boxPos= new Rect(myPosition.x-2.0f, myPosition.y-1f, myPosition.width+4.0f, myPosition.height+2f);
        Color selectionColor= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).settings.selectionColor;
        iCS_Graphics.DrawBox(boxPos, new Color(0f,0f,0f,0.25f), selectionColor, Color.white);
		boxPos.x+= 1f;
		boxPos.width-= 2f;
		boxPos.y+= 1f;
		boxPos.height-= 2f;
        iCS_Graphics.DrawBox(boxPos, Color.clear, selectionColor, Color.white);
		GUI.Label(myPosition, myValue, myStyle);
		ShowCursor(myPosition, myValue, myCursor, Color.red, 0.5f, myStyle);
        var oldValue= myValue;
        if(ProcessKeys(ref myValue, ref myCursor, (ch,_,__)=> !char.IsControl(ch))) {
            RestartCursorBlink();
        }
        return oldValue != myValue;
    }

    // =================================================================================
    // Cursor Management
    // ---------------------------------------------------------------------------------
    static float ourCursorBlinkStartTime= 0f;
    static void RestartCursorBlink() {
        ourCursorBlinkStartTime= Time.realtimeSinceStartup;
    }
    // ---------------------------------------------------------------------------------
	static void ShowCursor(Rect r, string value, int cursor, Color cursorColor, float blinkSpeed, GUIStyle style) {
		if(Math3D.IsNotZero(blinkSpeed)) {
			int step= (int)((Time.realtimeSinceStartup-ourCursorBlinkStartTime)/blinkSpeed);
			if((step & 1) == 1) {
				return;
			}
		}
        var x= r.x+GetCursorGUIOffset(value, cursor, style);
		var y= r.y-2f;
		var yMax= r.yMax+2f;
		Handles.color= cursorColor;
		Handles.DrawLine(new Vector3(x,y,0), new Vector3(x,yMax,0));
		Handles.DrawLine(new Vector3(x+1,y,0), new Vector3(x+1,yMax,0));
	}
    // ---------------------------------------------------------------------------------
    static int GetCursorIndexFromPosition(Rect r, Vector2 pickPoint, string value, GUIStyle style) {
        if(value == null) return 0;
        var len= value.Length;
        if(len == 0) return 0;
        var x= pickPoint.x - r.x;
        var size= style.CalcSize(new GUIContent(value));
        // Determine rough estimate of cursor.
        var step= size.x/len;
        int cursor= (int)((x/step)+0.5f);
        if(cursor < 0) cursor= 0;
        if(cursor > len) cursor= len;
        // Fine tune cursor.
        var offset= GetCursorGUIOffset(value, cursor, style);
        var diff= Mathf.Abs(offset-x);
        while(cursor > 0 && offset > x) {
            --cursor;
            offset= GetCursorGUIOffset(value, cursor, style);
            var newDiff= Mathf.Abs(offset-x);
            if(newDiff > diff) {
                ++cursor;
                break;
            }
            diff= newDiff;
        }
        while(cursor < len && offset < x) {
            ++cursor;
            offset= GetCursorGUIOffset(value, cursor, style);
            var newDiff= Mathf.Abs(offset-x);
            if(newDiff > diff) {
                --cursor;
                break;
            }
            diff= newDiff;
        }
        return cursor;
    }
    // ---------------------------------------------------------------------------------
    static float GetCursorGUIOffset(string value, int cursor, GUIStyle style) {
        float x= 0f;
		if(cursor != 0) {
			// Limit the cursor movement within the value string
			if(cursor > value.Length) {
				cursor= value.Length;
			}
            // Avoid end-of-string space removal by appending "A".
			var beforeCursor= value.Substring(0, cursor)+"A";
			var size= style.CalcSize(new GUIContent(beforeCursor));
            var toTrim= style.CalcSize(new GUIContent("A"));
			x= size.x-toTrim.x;
		}
        return x;        
    }

    // =================================================================================
    // Input Validation
    // ---------------------------------------------------------------------------------
    static bool ValidateStringInput(char newChar, string value, int cursor) {
        return !char.IsControl(newChar);
    }
    static bool ValidateIntInput(char newChar, string value, int cursor) {
        return char.IsDigit(newChar);
    }
    static bool ValidateFloatInput(char newChar, string value, int cursor) {
        return char.IsDigit(newChar);        
    }
    
    // =================================================================================
    // Keyboard processing
    // ---------------------------------------------------------------------------------
    static bool ProcessKeys(ref string value, ref int cursor, Func<char,string,int,bool> filter) {
        // Nothing to do if not a keyboard event
        var ev= Event.current;
        if(ev.type == EventType.KeyDown) {
            var len= value.Length;
            switch(ev.keyCode) {
                case KeyCode.RightArrow: {
                    cursor= cursor+1;
                    if(cursor > len) {
                        cursor= len;
                    }
                    Event.current.Use();
                    return true;
                }
                case KeyCode.LeftArrow: {
                    cursor= cursor-1;
                    if(cursor < 0) {
                        cursor= 0;
                    }
                    Event.current.Use();
                    return true;;
                }
                case KeyCode.Delete:
                case KeyCode.Backspace: {
                    if(cursor > 0) {
                        value= value.Substring(0, cursor-1)+value.Substring(cursor, len-cursor);
                        --cursor;
                    }
                    Event.current.Use();
                    return true;
                }
                default: {
                    if(ev.isKey) {
                        char c= ev.character;
                        if(filter(c, value, cursor)) {
                            value= value.Substring(0, cursor)+c+value.Substring(cursor, len-cursor);
                            ++cursor;                            
                            Event.current.Use();
                            return true;
                        }
                    }
                    break;                    
                }
            }            
        }
        return false;
    }
}
