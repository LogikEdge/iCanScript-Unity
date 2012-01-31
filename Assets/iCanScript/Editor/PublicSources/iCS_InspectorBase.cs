using UnityEngine;
using UnityEditor;

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
