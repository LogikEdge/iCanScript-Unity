using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript { namespace Editor {
    
/// The AppController controls the Start and Shutdown of all sub-systems.
[InitializeOnLoad]
public static class AppController {
    // ======================================================================
    // Initialization all sub-systems
    // ----------------------------------------------------------------------
	static AppController() {
        // Start all sub-systems.
		ErrorController.Start();
        SystemEvents.Start();
        TimerService.Start();
        BlinkController.Start();
        EditionController.Start();
        LicenseController.Start();
		SoftwareUpdateController.Start();
        GizmoController.Start();
        LibraryDatabaseController.Start();
        iCS_VisualScriptDataController.Start();
        iCS_EditorController.Start();
        SceneController.Start();
		HelpController.Start();	
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Shutdown all subsystems.
		HelpController.Shutdown();
        SceneController.Shutdown();
        iCS_EditorController.Shutdown();
        iCS_VisualScriptDataController.Shutdown();
        LibraryDatabaseController.Shutdown();
        GizmoController.Shutdown();
        SoftwareUpdateController.Shutdown();
        LicenseController.Shutdown();
        EditionController.Shutdown();
        BlinkController.Shutdown();
        TimerService.Shutdown();
        SystemEvents.Shutdown();
		ErrorController.Shutdown();
    }
}

}}