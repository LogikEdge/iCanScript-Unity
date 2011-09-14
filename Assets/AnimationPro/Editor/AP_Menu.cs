using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_Menu {

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("WarpDrive/Create Behaviour")]
	public static void CreateBehaviour() {
		// Create State Chart component.
		AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<AP_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create Behaviour", true)]
	public static bool ValidateCreateBehaviour() {
		if(Selection.activeTransform != null) {
			AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create Module Library")]
	public static void CreateModuleLibrary() {
		// Create State Chart component.
		AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<AP_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create Module Library", true)]
	public static bool ValidateCreateModuleLibrary() {
		if(Selection.activeTransform != null) {
			AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create State Chart Library")]
	public static void CreateStateChartLibrary() {
		// Create State Chart component.
		AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<AP_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create State Chart Library", true)]
	public static bool ValidateCreateStateChartLibrary() {
		if(Selection.activeTransform != null) {
			AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// AnimationPro Window.
	[MenuItem("Window/WarpDrive Editor")]
	public static void ShowWarpDriveEditor() {
        AP_Editor editor= EditorWindow.GetWindow<AP_Editor>();
        editor.hideFlags= HideFlags.DontSave;
	}

}
