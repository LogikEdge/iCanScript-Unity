using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_Menu {

    // ======================================================================
	// Add State to selected game object.
	[MenuItem("AnimationPro/Add Graph")]
	public static void AddGapGraph() {
		// Add State Chart component.
		AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
		if(gapGraph == null) {
			Selection.activeGameObject.AddComponent<AP_Graph>();
		}
	}
	[MenuItem("AnimationPro/Add Graph", true)]
	public static bool ValidateAddGapGraph() {
		if(Selection.activeTransform != null) {
			AP_Graph gapGraph = Selection.activeGameObject.GetComponent<AP_Graph>();
			return gapGraph == null;
		}
		return false;
	}

    // ======================================================================
	// AnimationPro Window.
	[MenuItem("Window/AnimationPro Editor")]
	public static void ShowAnimationProEditor() {
        AP_Editor editor= EditorWindow.GetWindow<AP_Editor>();
        editor.hideFlags= HideFlags.DontSave;
	}
	[MenuItem("Window/AnimationPro Editor", true)]
	public static bool ValidateShowAnimationProEditor() {
        return true;
	}

}
