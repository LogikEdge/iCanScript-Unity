using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
/// The AppController controls the Start and Shutdown of all sub-systems.
[InitializeOnLoad]
public static class AppController {
    // ======================================================================
    // Initialization all sub-systems
    // ----------------------------------------------------------------------
	static AppController() {
//        var diff= new Diff();
//        diff= null;
        
        // Start all sub-systems.
		ErrorController.Start();
        SystemEvents.Start();
        TimerService.Start();
        BlinkController.Start();
        EditionController.Start();
        LicenseController.Start();
		SoftwareUpdateController.Start();
        GizmoController.Start();
        LibraryController.Start();
        iCS_VisualScriptDataController.Start();
        iCS_EditorController.Start();
        SceneController.Start();
		HelpController.Start();
        PackageController.Start();
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {
        // Shutdown all subsystems.
        PackageController.Shutdown();
		HelpController.Shutdown();
        SceneController.Shutdown();
        iCS_EditorController.Shutdown();
        iCS_VisualScriptDataController.Shutdown();
        LibraryController.Shutdown();
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

}