using UnityEngine;
using UnityEditor;

// ===================================================================
// DO NOT MODIFY THIS FILE.
// This file is used to invoke the installer at the appropriate time.
//
// The installer is used to install new iCanScript nodes in bulk.
// You can modify the installer to include your own nodes from your
// classes (see iCS_Installer).
public class iCS_InspectorBase : iCS_Inspector {
    // ---------------------------------------------------------------
    // Invokes the iCanScript .
	public override void OnInspectorGUI() {
        // Assure that the components are properly installed.
    	iCS_Installer.Install();
        base.OnInspectorGUI();
	}
}

