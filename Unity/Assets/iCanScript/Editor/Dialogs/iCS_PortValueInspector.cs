using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_PortValueInspector  : EditorWindow, iCS_ISubEditor {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    private iCS_EditorObject          myPort        = null;
	private object					  myInitialValue= null;
	private Dictionary<string,object> myFoldoutDB   = new Dictionary<string,object>();

    // ======================================================================
    // Initialization/Teardown
    // ----------------------------------------------------------------------
    public static iCS_PortValueInspector CreateInstance(iCS_EditorObject thePort, Vector2 pos) {
        iCS_PortValueInspector valueInspector= ScriptableObject.CreateInstance(typeof(iCS_PortValueInspector)) as iCS_PortValueInspector;
        valueInspector.Init(thePort, pos);
        return valueInspector;
    }
    public void Init(iCS_EditorObject thePort, Vector2 pos) {
        myPort= thePort;
        title= thePort.Name;
		myInitialValue= thePort.PortValue;
        position= new Rect(pos.x, pos.y, 300, 200);
//        ShowAuxWindow();
		ShowPopup();
    }
    
    public void OnEnable() {}
    public void OnDisable() {}

    // ======================================================================
    // GUI Update
    // ----------------------------------------------------------------------
    public void OnGUI() {
		Rect r= EditorGUILayout.BeginVertical();
        iCS_GuiUtilities.OnInspectorDataPortGUI(myPort, myPort.IStorage, 1, myFoldoutDB);
		EditorGUILayout.EndVertical();
		position= new Rect(position.x, position.y, position.width, r.height);
		ProcessEvents();
	}
	void ProcessEvents() {
		var ev= Event.current;
        switch(ev.keyCode) {
            // Reset to default
            case KeyCode.Escape: {
				Close();
                break;
            }
		}
	}
	public bool Update() {
		return false;
	}
}
