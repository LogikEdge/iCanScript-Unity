#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;

#if DEBUG
public static class iCS_DevelopmentMenu {
    // ======================================================================
    // Sanity Check
	[MenuItem("DEVELOPMENT/Sanity Check Selection",false,1000)]
	public static void MenuSanityCheck() {
		iCS_IStorage storage= iCS_StorageMgr.IStorage;
		if(storage == null) return;
		Debug.Log("iCanScript: Start Sanity Check on: "+storage.Storage.name);
		storage.SanityCheck();
		Debug.Log("iCanScript: Completed Sanity Check on: "+storage.Storage.name);
	}
    // ======================================================================
    // Trigger Periodic Software Update Verification
	[MenuItem("DEVELOPMENT/Invoke Periodic Software Update Verification",false,1001)]
	public static void MenuPeriodicSoftwareUpdateVerification() {
		iCS_SoftwareUpdateController.PeriodicUpdateVerification();
	}	
}
#endif
