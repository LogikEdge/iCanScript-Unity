using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_InputMgr {
    // Selected GUI input verification.
    public static bool IsGUI(string guiName) {
        return GUI.GetNameOfFocusedControl() == guiName;
    }
    public static bool HasKeyboardFocus(string guiName) {
        return GUI.GetNameOfFocusedControl() == guiName;        
    }
    public static bool IsEscapeOn(string guiName) {
        return IsGUI(guiName) && IsEscape;
    }
    public static bool IsReturnOn(string guiName) {
        return IsGUI(guiName) && IsReturn;
    }    


    // Raw input verification.
    public static bool IsEscape {
        get {
            var ev= Event.current;
            return ev.isKey && ev.keyCode == KeyCode.Escape;            
        }
    }
    public static bool IsReturn {
        get {
            var ev= Event.current;
            return ev.isKey && ev.keyCode == KeyCode.Return;            
        }
    }
    
}

// ----------------------------------------------------------------------
public class iCS_BufferedTextField {
    private int                    myControlId     = -1;
    private string                 myBuffer        = null;
    
    public iCS_BufferedTextField() {}
    public void Update(string label, string originalValue, System.Action<string> onChangeFnc) {
        if(myControlId == -1) {
            myBuffer= originalValue;
        }
        GUI.changed= false;
        myBuffer= EditorGUILayout.TextField(label, myBuffer);
        if(GUI.changed) {
            myControlId= GUIUtility.keyboardControl;
        }
        if(myControlId != GUIUtility.keyboardControl || !EditorGUIUtility.editingTextField) {
            // Save any change to the text field.
            if(myBuffer != originalValue) {
                onChangeFnc(myBuffer);                        
            }
            myControlId= -1;                
        }
    }
    public void Update(Rect rect, string originalValue, System.Action<string> onChangeFnc) {
        if(myControlId == -1) {
            myBuffer= originalValue;
        }
        GUI.changed= false;
        myBuffer= EditorGUI.TextField(rect, myBuffer);
        if(GUI.changed) {
            myControlId= GUIUtility.keyboardControl;
        }
        if(myControlId != GUIUtility.keyboardControl || !EditorGUIUtility.editingTextField) {
            // Save any change to the text field.
            if(myBuffer != originalValue) {
                onChangeFnc(myBuffer);                        
            }
            myControlId= -1;                
        }
    }
}
