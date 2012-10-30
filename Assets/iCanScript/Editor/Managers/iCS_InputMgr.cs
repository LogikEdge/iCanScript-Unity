using UnityEngine;
using System.Collections;

public static class iCS_InputMgr {
    // Selected GUI input verification.
    public static bool IsGUI(string guiName) {
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
            return ev.type == EventType.KeyDown && ev.character == '\x1b';            
        }
    }
    public static bool IsReturn {
        get {
            var ev= Event.current;
            return ev.type == EventType.KeyDown && ev.character == '\x0a';            
        }
    }
}
