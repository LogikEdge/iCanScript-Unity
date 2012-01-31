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
    // Indicates if the installation  has already been executed.
    static bool IsInstalltionDone= false;

    // ---------------------------------------------------------------
    // Invokes the iCanScript installer if not already done.
	new void OnEnable() {
	    base.OnEnable();
        if(!IsInstalltionDone) {
            IsInstalltionDone= true;
    	    iCS_Installer.Install();
        }
	}
}
