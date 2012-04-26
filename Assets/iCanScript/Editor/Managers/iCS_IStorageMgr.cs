using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_IStorageMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	static iCS_IStorage 		myIStorage = null;
	static Action<iCS_IStorage>	myCallbacks= null;
	
    // =================================================================================
    // Selection Update.
    // ---------------------------------------------------------------------------------
	public static void Update() {
		GameObject go= Selection.activeGameObject;
		if(go == null) return;
		iCS_Storage storage= go.GetComponent<iCS_Storage>();
		if(myIStorage == null || myIStorage.Storage == storage) {
			myIStorage= new iCS_IStorage(storage);
			if(myCallbacks != null) {
				myCallbacks(myIStorage);
			}
		}
	}

    // =================================================================================
    // Registration.
    // ---------------------------------------------------------------------------------
	public static void Register(Action<iCS_IStorage> action) {
		myCallbacks+= action;
	}
	public static void Unregister(Action<iCS_IStorage> action) {
		myCallbacks-= action;
	}
}
