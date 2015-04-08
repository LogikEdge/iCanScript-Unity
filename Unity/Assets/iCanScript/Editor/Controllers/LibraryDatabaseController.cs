using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace iCanScript.Editor {
    
    /// @date 2015-04-08    Code Review
    public class LibraryDatabaseController {
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        /// Scans the application library and extracts the needed nodes.
    	static LibraryDatabaseController() {
            iCS_Reflection.ParseAppDomain();
    	}        
        // ----------------------------------------------------------------------
        /// Start the application controller.
    	public static void Start() {}
        // ----------------------------------------------------------------------
        /// Shutdowns the application controller.
        public static void Shutdown() {}
    }
    
}