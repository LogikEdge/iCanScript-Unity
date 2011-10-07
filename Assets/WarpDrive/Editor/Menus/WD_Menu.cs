using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_Menu {

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("WarpDrive/Create Behaviour")]
	public static void CreateBehaviour() {
		// Create State Chart component.
		WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
		if(gapGraph == null) {
			gapGraph= Selection.activeGameObject.AddComponent<WD_Behaviour>();
            int id= gapGraph.EditorObjects.GetNextAvailableId();
            gapGraph.EditorObjects[id]= new WD_EditorObject(id, "Behaviour", typeof(WD_Behaviour), -1, WD_ObjectTypeEnum.Behaviour, new Rect(0,0,0,0));
		}
	}
	[MenuItem("WarpDrive/Create Behaviour", true)]
	public static bool ValidateCreateBehaviour() {
		if(Selection.activeTransform != null) {
			WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create Module Library")]
	public static void CreateModuleLibrary() {
		// Create State Chart component.
		WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
		if(gapGraph == null) {
			gapGraph= Selection.activeGameObject.AddComponent<WD_Behaviour>();
            gapGraph.enabled= false;
            int id= gapGraph.EditorObjects.GetNextAvailableId();
            gapGraph.EditorObjects[id]= new WD_EditorObject(id, "Module Library", typeof(WD_Behaviour), -1, WD_ObjectTypeEnum.Module, new Rect(0,0,0,0));
		}
	}
	[MenuItem("WarpDrive/Create Module Library", true)]
	public static bool ValidateCreateModuleLibrary() {
		if(Selection.activeTransform != null) {
			WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create State Chart Library")]
	public static void CreateStateChartLibrary() {
		// Create State Chart component.
		WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
		if(gapGraph == null) {
			gapGraph= Selection.activeGameObject.AddComponent<WD_Behaviour>();
            gapGraph.enabled= false;
            int id= gapGraph.EditorObjects.GetNextAvailableId();
            gapGraph.EditorObjects[id]= new WD_EditorObject(id, "StateChart Library", typeof(WD_Behaviour), -1, WD_ObjectTypeEnum.StateChart, new Rect(0,0,0,0));
		}
	}
	[MenuItem("WarpDrive/Create State Chart Library", true)]
	public static bool ValidateCreateStateChartLibrary() {
		if(Selection.activeTransform != null) {
			WD_Behaviour gapGraph = Selection.activeGameObject.GetComponent<WD_Behaviour>();
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
