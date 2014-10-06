using UnityEngine;
using UnityEditor;
using System.Collections;

/// The AppController controls the Start and Shutdown of all sub-systems.
public static class iCS_AppController {
    // ======================================================================
    // Initialization all sub-systems
    // ----------------------------------------------------------------------
	static iCS_AppController() {
        // Start all sub-systems.
        iCS_SystemEvents.Start();
        iCS_TimerService.Start();
        iCS_EditionController.Start();
        iCS_LicenseController.Start();
		iCS_SoftwareUpdateController.Start();
		iCS_SystemEvents.Start();
        iCS_GizmoController.Start();
		iCS_InstallationController.Start();
		iCS_CodeGenerator.Start();
        iCS_VisualScriptDataController.Start();
        iCS_SceneController.Start();
		// TODO: Does not start untill a graph is on screen!  Needs to be available to Library window at all times.
		iCS_HelpSearch.Start();	
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Shutdown all subsystems.
        iCS_SceneController.Shutdown();
        iCS_VisualScriptDataController.Shutdown();
        iCS_CodeGenerator.Shutdown();
        iCS_InstallationController.Shutdown();
        iCS_GizmoController.Shutdown();
        iCS_SystemEvents.Shutdown();
        iCS_SoftwareUpdateController.Shutdown();
        iCS_LicenseController.Shutdown();
        iCS_EditionController.Shutdown();
        iCS_TimerService.Shutdown();
        iCS_SystemEvents.Shutdown();
		iCS_HelpSearch.Shutdown();
    }
}
