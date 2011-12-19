using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof (iCS_LicenseManager))]
public class iCS_InspectorLicense : Editor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";

    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
	}

	// ----------------------------------------------------------------------
    // Deactivate the edition of the graph.
	public void OnDisable ()
	{
	}

	// ----------------------------------------------------------------------
    // Paint to inspector for the selected object (see editor).
	public override void OnInspectorGUI ()
	{
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;

        // Draw inspector window
		DrawDefaultInspector();
	}

}
