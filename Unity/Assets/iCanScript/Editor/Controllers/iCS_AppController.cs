using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_AppController {
    // ======================================================================
    // Initialization all application sub-systems
    // ----------------------------------------------------------------------
	static iCS_AppController() {
        // Start all sub-systems.
		iCS_SoftwareUpdateController.Start();
		iCS_SystemEvents.Start();
        iCS_GizmoController.Start();
		iCS_InstallationController.Start();
		iCS_CodeGenerator.Start();
        iCS_VisualScriptDataController.Start();
        
        // Install periodically update.
        EditorApplication.update+= PeriodicUpdate;		
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Remove the periodic update.
        EditorApplication.update-= PeriodicUpdate;

        // Shutdown all subsystems.
        iCS_VisualScriptDataController.Shutdown();
        iCS_CodeGenerator.Shutdown();
        iCS_InstallationController.Shutdown();
        iCS_GizmoController.Shutdown();
        iCS_SystemEvents.Shutdown();
        iCS_SoftwareUpdateController.Shutdown();
    }

    // ======================================================================
    // Initialization all application sub-systems
    // ----------------------------------------------------------------------
    public static void PeriodicUpdate() {
    }
}
