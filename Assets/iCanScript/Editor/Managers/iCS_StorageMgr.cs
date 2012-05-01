using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_StorageMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static GameObject               myGameObject                   = null;
	static iCS_IStorage 		    myIStorage                     = null;
	static Action<iCS_IStorage>	    myStorageChangeCallbacks       = null;
	static Action<iCS_EditorObject> mySelectedObjectChangeCallBacks= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static GameObject       SelectedGameObject	{ get { return myGameObject; }}
    public static iCS_IStorage     IStorage             { get { return myIStorage; }}
    public static iCS_Storage      Storage              { get { return IStorage != null ? IStorage.Storage : null; }}
    public static iCS_EditorObject SelectedObject   	{ get { return IStorage != null ? IStorage.SelectedObject : null; }}

    // =================================================================================
    // Selection Update.
    // ---------------------------------------------------------------------------------
	public static void Update() {
		GameObject go= Selection.activeGameObject;
		if(go == null) return;
		iCS_Storage storage= go.GetComponent<iCS_Storage>();
        if(storage == null) return;
		if(myIStorage == null || myIStorage.Storage != storage) {
			myGameObject= go;
			myIStorage= new iCS_IStorage(storage);
			if(myStorageChangeCallbacks != null) {
				myStorageChangeCallbacks(myIStorage);
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
