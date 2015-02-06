using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScriptEditor {
    
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
        iCS_TimerService.Start();
        BlinkController.Start();
        EditionController.Start();
        LicenseController.Start();
		SoftwareUpdateController.Start();
        GizmoController.Start();
        iCS_LibraryDatabaseController.Start();
		CodeEngineering.CodeEngineeringController.Start();
        iCS_VisualScriptDataController.Start();
        iCS_EditorController.Start();
        SceneController.Start();
        PublicInterfaceController.Start();
		iCS_HelpController.Start();	
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Shutdown all subsystems.
		iCS_HelpController.Shutdown();
        PublicInterfaceController.Start();
        SceneController.Shutdown();
        iCS_EditorController.Shutdown();
        iCS_VisualScriptDataController.Shutdown();
        CodeEngineering.CodeEngineeringController.Shutdown();
        iCS_LibraryDatabaseController.Shutdown();
        GizmoController.Shutdown();
        SoftwareUpdateController.Shutdown();
        LicenseController.Shutdown();
        EditionController.Shutdown();
        BlinkController.Shutdown();
        iCS_TimerService.Shutdown();
        SystemEvents.Shutdown();
		ErrorController.Shutdown();
    }
}

}