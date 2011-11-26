using UnityEngine;
using UnityEditor;
using System.Collections;

public class DebugMenu : MonoBehaviour {

	[MenuItem("Debug/GetInstanceId")]
	public static void GetInstanceId() {
        GameObject go= Selection.activeGameObject;
	    Debug.Log(go.name+"'s InstanceId: "+go.GetInstanceID().ToString());
	}
	[MenuItem("Debug/GetInstanceId", true)]
	public static bool ValidateGetInstanceId() {
        return Selection.activeGameObject != null;
    }
}
