using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_PortValueInspector  : EditorWindow, iCS_ISubEditor {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    private iCS_EditorObject          myPort     = null;
	private Dictionary<string,object> myFoldoutDB= new Dictionary<string,object>();

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
        position= new Rect(pos.x, pos.y, 300, 200);
        ShowAuxWindow();
    }
    
    public void OnEnable() {}
    public void OnDisable() {}

    // ======================================================================
    // GUI Update
    // ----------------------------------------------------------------------
    public void OnGUI() {
        iCS_GuiUtilities.OnInspectorDataPortGUI(myPort, myPort.IStorage, 1, myFoldoutDB);        
	}
	public bool Update() {
		return true;
	}
}
