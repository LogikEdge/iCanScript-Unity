using UnityEngine;
using UnityEditor;
using System.Collections;

/// The AppController controls the Start and Shutdown of all sub-systems.
[InitializeOnLoad]
public static class iCS_AppController {
    // ======================================================================
    // Initialization all sub-systems
    // ----------------------------------------------------------------------
	static iCS_AppController() {
        // Start all sub-systems.
		iCS_ErrorController.Start();
        iCS_SystemEvents.Start();
        iCS_TimerService.Start();
        iCS_EditionController.Start();
        iCS_LicenseController.Start();
		iCS_SoftwareUpdateController.Start();
        iCS_GizmoController.Start();
        iCS_LibraryDatabaseController.Start();
		iCS_CodeGenerator.Start();
        iCS_VisualScriptDataController.Start();
        iCS_EditorController.Start();
        iCS_SceneController.Start();
        iCS_PublicInterfaceController.Start();
		iCS_HelpController.Start();	
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Shutdown all subsystems.
		iCS_HelpController.Shutdown();
        iCS_PublicInterfaceController.Start();
        iCS_SceneController.Shutdown();
        iCS_EditorController.Shutdown();
        iCS_VisualScriptDataController.Shutdown();
        iCS_CodeGenerator.Shutdown();
        iCS_LibraryDatabaseController.Shutdown();
        iCS_GizmoController.Shutdown();
        iCS_SystemEvents.Shutdown();
        iCS_SoftwareUpdateController.Shutdown();
        iCS_LicenseController.Shutdown();
        iCS_EditionController.Shutdown();
        iCS_TimerService.Shutdown();
        iCS_SystemEvents.Shutdown();
		iCS_ErrorController.Shutdown();
    }
}
