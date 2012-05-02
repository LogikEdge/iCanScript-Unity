using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_StorageMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	static bool						myIsOutdated				   = true;
	static iCS_IStorage 		    myIStorage                     = null;
	static iCS_EditorObject			mySelectedObject			   = null;
	static Action<iCS_IStorage>	    myStorageChangeCallbacks       = null;
	static Action<iCS_EditorObject> mySelectedObjectChangeCallBacks= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static iCS_IStorage     IStorage             { get { return myIStorage; }}
    public static iCS_Storage      Storage              { get { return IStorage != null ? IStorage.Storage : null; }}
    public static iCS_EditorObject SelectedObject   	{ get { return IStorage != null ? IStorage.SelectedObject : null; }}
	public static bool			   IsOutdated			{ get { return myIsOutdated; }}
	
    // =================================================================================
    // Selection Update.
    // ---------------------------------------------------------------------------------
	public static void Update() {
		GameObject go= Selection.activeGameObject;
		if(go == null) { myIsOutdated= true; return; }
		iCS_Storage storage= go.GetComponent<iCS_Storage>();
        if(storage == null) { myIsOutdated= true; return; }
		// Verify for storage change.
		if(myIStorage == null || myIStorage.Storage != storage) {
			myIStorage= new iCS_IStorage(storage);
			mySelectedObject= myIStorage.SelectedObject;
			myIsOutdated= false;
			Debug.Log("Storage has changed");
			if(myStorageChangeCallbacks != null) {
				myStorageChangeCallbacks(myIStorage);
			}
			if(mySelectedObjectChangeCallBacks != null) {
				mySelectedObjectChangeCallBacks(mySelectedObject);
			}
			return;
		}
		// Verify for selected object change.
		if(mySelectedObject != myIStorage.SelectedObject) {
			mySelectedObject= myIStorage.SelectedObject;
			myIsOutdated= false;
			if(mySelectedObjectChangeCallBacks != null) {
				mySelectedObjectChangeCallBacks(mySelectedObject);
			}
		}
	}

    // =================================================================================
    // Registration.
    // ---------------------------------------------------------------------------------
	public static void RegisterStorageChangeNotification(Action<iCS_IStorage> action) {
		myStorageChangeCallbacks+= action;
	}
	public static void UnregisterStorageChangeNotification(Action<iCS_IStorage> action) {
		myStorageChangeCallbacks-= action;
	}
	public static void RegisterSelectedObjectChangeNotification(Action<iCS_EditorObject> action) {
		mySelectedObjectChangeCallBacks+= action;
	}
	public static void UnregisterSelectedObjectChangeNotification(Action<iCS_EditorObject> action) {
		mySelectedObjectChangeCallBacks-= action;
	}
}
