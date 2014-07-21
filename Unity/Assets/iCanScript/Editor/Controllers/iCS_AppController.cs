using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_AppController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	static iCS_AppController() {
		iCS_SoftwareUpdateController.PeriodicUpdateVerification();
		iCS_SystemEvents.Start();
        iCS_GizmoController.Start();
		iCS_InstallationController.Start();
		iCS_CodeGenerator.Start();
        
        // Install delegate to periodically verify application state.
//        EditorApplication.update+= VerifyApplicationState;		
	}
	public static void Start() {}
    public static void Shutdown() {
//        EditorApplication.update-= VerifyApplicationState;
    }

    public static void VerifyApplicationState() {
//        if(BuildPipeline.isBuildingPlayer) {
//            iCS_TextureCache.FlushAndClear();
//        }
    }
}
