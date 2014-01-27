using UnityEngine;
using System.Collections;

public static class iCS_EditorStartup {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	static iCS_EditorStartup() {
		Debug.Log("iCanScript starting...");
		string latestVersion= iCS_UpdateController.GetLatestReleaseId();
		if(latestVersion == null) {
			Debug.Log("iCanScript: Version server not accessible...");
		} else {
			Debug.Log("iCanScript: Latest version is: "+latestVersion);
		}
		iCS_SystemEvents.Start();
		iCS_InstallerMgr.Start();
		iCS_CodeGenerator.Start();		
	}
	public static void Start() {}
}
