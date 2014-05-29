using UnityEngine;
using System.Collections;

public static class iCS_AppController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	static iCS_AppController() {
		iCS_SoftwareUpdateController.PeriodicUpdateVerification();
		iCS_SystemEvents.Start();
		iCS_InstallerMgr.Start();
		iCS_CodeGenerator.Start();		
	}
	public static void Start() {}
    public static void Shutdown() {}
}
