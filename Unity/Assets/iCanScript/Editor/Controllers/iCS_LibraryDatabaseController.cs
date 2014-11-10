using UnityEngine;
using System.Collections;

public class iCS_LibraryDatabaseController {
    // ======================================================================
    // Common Controller activation/deactivation
    // ----------------------------------------------------------------------
	static iCS_LibraryDatabaseController() {
        // Extract nodes from the active assemblies.
        iCS_Reflection.ParseAppDomain();
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {}

}
