using UnityEngine;
using System.Collections;

public static class iCS_EditorStartup {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	static iCS_EditorStartup() {
		iCS_InstallerMgr.Start();
		iCS_CodeGenerator.Start();		
	}
	public static void Start() {}

}
