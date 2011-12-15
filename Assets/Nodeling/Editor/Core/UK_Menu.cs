using UnityEngine;
using UnityEditor;
using System.Collections;

public class UK_Menu {

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("iCanScript/Create Behaviour")]
	public static void CreateBehaviour() {
		// Create State Chart component.
		UK_Behaviour storage = Selection.activeGameObject.GetComponent<UK_Behaviour>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<UK_Behaviour>();
            UK_IStorage iStorage= new UK_IStorage(storage);
            iStorage.CreateBehaviour();
            iStorage= null;
		}
	}
	[MenuItem("iCanScript/Create Behaviour", true)]
	public static bool ValidateCreateBehaviour() {
		if(Selection.activeTransform != null) {
			UK_Storage storage = Selection.activeGameObject.GetComponent<UK_Storage>();
			return storage == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("iCanScript/Create Module Library")]
	public static void CreateModuleLibrary() {
		// Create State Chart component.
		UK_Storage storage = Selection.activeGameObject.GetComponent<UK_Library>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<UK_Library>();
            storage.enabled= false;
            UK_IStorage iStorage= new UK_IStorage(storage);
            iStorage.CreateModule(-1, Vector2.zero, "Module Library");
            iStorage= null;
		}
	}
	[MenuItem("iCanScript/Create Module Library", true)]
	public static bool ValidateCreateModuleLibrary() {
		if(Selection.activeTransform != null) {
			UK_Storage storage = Selection.activeGameObject.GetComponent<UK_Storage>();
			return storage == null;
		}
		return false;
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("iCanScript/Create State Chart Library")]
	public static void CreateStateChartLibrary() {
		// Create State Chart component.
		UK_Storage storage = Selection.activeGameObject.GetComponent<UK_Library>();
		if(storage == null) {
			storage= Selection.activeGameObject.AddComponent<UK_Library>();
            storage.enabled= false;
            UK_IStorage iStorage= new UK_IStorage(storage);
            iStorage.CreateStateChart(-1, Vector2.zero, "State Chart Library");
            iStorage= null;
		}
	}
	[MenuItem("iCanScript/Create State Chart Library", true)]
	public static bool ValidateCreateStateChartLibrary() {
		if(Selection.activeTransform != null) {
			UK_Storage storage = Selection.activeGameObject.GetComponent<UK_Storage>();
			return storage == null;
		}
		return false;
	}
    
    // ----------------------------------------------------------------------
    bool IsStorageAlreadyPresent() {
        return false;
    }
    
    // ======================================================================
	// iCanScript Window.
	[MenuItem("Window/iCanScript Editor")]
	public static void ShowiCanScriptEditor() {
        UK_Editor editor= EditorWindow.GetWindow(typeof(UK_Editor), false, "iCanScript Editor") as UK_Editor;
        editor.hideFlags= HideFlags.DontSave;
	}

}
