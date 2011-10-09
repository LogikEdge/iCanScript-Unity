using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_Menu {

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("WarpDrive/Create Behaviour")]
	public static void CreateBehaviour() {
		// Create State Chart component.
		WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<WD_Storage>();
            storage.EditorObjects.CreateInstance("Behaviour", -1, WD_ObjectTypeEnum.Behaviour, Vector2.zero, typeof(WD_Storage));
		}
	}
	[MenuItem("WarpDrive/Create Behaviour", true)]
	public static bool ValidateCreateBehaviour() {
		if(Selection.activeTransform != null) {
			WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
			return storage == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create Module Library")]
	public static void CreateModuleLibrary() {
		// Create State Chart component.
		WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<WD_Storage>();
            storage.enabled= false;
            storage.EditorObjects.CreateInstance("Module Library", -1, WD_ObjectTypeEnum.Module, Vector2.zero, typeof(WD_Storage));
		}
	}
	[MenuItem("WarpDrive/Create Module Library", true)]
	public static bool ValidateCreateModuleLibrary() {
		if(Selection.activeTransform != null) {
			WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
			return storage == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("WarpDrive/Create State Chart Library")]
	public static void CreateStateChartLibrary() {
		// Create State Chart component.
		WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<WD_Storage>();
            storage.enabled= false;
            storage.EditorObjects.CreateInstance("StateChart Library", -1, WD_ObjectTypeEnum.StateChart, Vector2.zero, typeof(WD_Storage));
		}
	}
	[MenuItem("WarpDrive/Create State Chart Library", true)]
	public static bool ValidateCreateStateChartLibrary() {
		if(Selection.activeTransform != null) {
			WD_Storage storage = Selection.activeGameObject.GetComponent<WD_Storage>();
			return storage == null;
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
