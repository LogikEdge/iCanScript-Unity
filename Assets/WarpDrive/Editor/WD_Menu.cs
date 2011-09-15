using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_Menu {

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("WarpDrive/Create Behaviour")]
	public static void CreateBehaviour() {
		// Create State Chart component.
		WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<WD_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create Behaviour", true)]
	public static bool ValidateCreateBehaviour() {
		if(Selection.activeTransform != null) {
			WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create Module Library")]
	public static void CreateModuleLibrary() {
		// Create State Chart component.
		WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<WD_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create Module Library", true)]
	public static bool ValidateCreateModuleLibrary() {
		if(Selection.activeTransform != null) {
			WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create State Chart Library")]
	public static void CreateStateChartLibrary() {
		// Create State Chart component.
		WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<WD_Graph>();
		}
	}
	[MenuItem("WarpDrive/Create State Chart Library", true)]
	public static bool ValidateCreateStateChartLibrary() {
		if(Selection.activeTransform != null) {
			WD_Graph gapGraph = Selection.activeGameObject.GetComponent<WD_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// WarpDrive Window.
	[MenuItem("Window/WarpDrive Editor")]
	public static void ShowWarpDriveEditor() {
        WD_Editor editor= EditorWindow.GetWindow<WD_Editor>();
        editor.hideFlags= HideFlags.DontSave;
	}

}
