using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_StorageMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static bool             myIsPlaying     = false;
    static bool             myIsOutdated    = false;
	static iCS_IStorage 	myIStorage      = null;
	static iCS_EditorObject mySelectedObject= null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static iCS_IStorage     IStorage         { get { return myIStorage; }}
    public static iCS_Storage      Storage          { get { return IStorage != null ? IStorage.Storage : null; }}
    public static iCS_EditorObject SelectedObject   { get { return IStorage != null ? IStorage.SelectedObject : null; } set { if(IStorage != null) IStorage.SelectedObject= value; }}
	
    // =================================================================================
    // Selection Update.
    // ---------------------------------------------------------------------------------
	public static void Update(iCS_IStorage iStorage, Action<iCS_IStorage> notif) {
		Update();
		if(myIStorage != iStorage) {
			notif(myIStorage);
		}
	}
    // ---------------------------------------------------------------------------------
	public static void Update(iCS_EditorObject eObj, Action<iCS_IStorage, iCS_EditorObject> notif) {
		Update();
		if(mySelectedObject != eObj) {
			notif(myIStorage, mySelectedObject);
		}
	}
    // ---------------------------------------------------------------------------------
	public static void Update() {
		GameObject go= Selection.activeGameObject;
		if(go == null) {
            myIsOutdated= true;
		    return;
		}
		iCS_Storage storage= go.GetComponent<iCS_Storage>();
        if(storage == null) {
            myIsOutdated= true;
            return;
        }
		// Verify for storage change.
        bool isPlaying= Application.isPlaying;
		if(myIStorage == null || myIStorage.Storage != storage || myIsPlaying != isPlaying) {
            myIsPlaying= isPlaying;
			myIStorage= new iCS_IStorage(storage);
			mySelectedObject= myIStorage.SelectedObject;
            myIsOutdated= false;
			return;
		}
		// Verify for selected object change.
		if(mySelectedObject != myIStorage.SelectedObject) {
			mySelectedObject= myIStorage.SelectedObject;
            myIsOutdated= false;
		}
	}
}
