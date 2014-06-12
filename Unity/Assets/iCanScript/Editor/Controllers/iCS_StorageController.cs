using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_StorageController {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static bool             myIsPlaying       = false;
	static iCS_IStorage 	myIStorage        = null;
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static iCS_IStorage     IStorage         { get { return myIStorage; }}
    public static iCS_StorageImp   Storage          { get { return IStorage != null ? IStorage.Storage : null; }}
    public static iCS_EditorObject SelectedObject   {
        get { return IStorage != null ? IStorage.SelectedObject : null; }
        set {
            if(IStorage != null) {
                iCS_UserCommands.Select(value, IStorage);                
            }
        }
    }
	
    // =================================================================================
    // Storage & Selected object Update.
    // ---------------------------------------------------------------------------------
	public static void Update() {
        // Use previous game object if new selection does not include a visual script.
		GameObject go= Selection.activeGameObject;
        var monoBehaviour= go != null ? go.GetComponent<iCS_MonoBehaviourImp>() : null;
        if(monoBehaviour == null) {
            // Clear if previous game object is not valid.
                myIStorage= null;
                myIsPlaying= Application.isPlaying;
                return;                
        }
        // Create a root object if one does not exist.
        var visualScript= monoBehaviour as iCS_VisualScriptImp;
        if(visualScript != null) {
            var storage= monoBehaviour.Storage;
            if(storage == null) return;
            if(storage.EngineObjects.Count == 0) {
                CreateRootBehaviourNode(visualScript);
            }
        }
		// Verify for storage change.
        bool isPlaying= Application.isPlaying;
		if(myIStorage == null || myIStorage.iCSMonoBehaviour != monoBehaviour || myIsPlaying != isPlaying) {
            myIsPlaying= isPlaying;
			myIStorage= new iCS_IStorage(monoBehaviour);
			return;
		}
	}
    // ---------------------------------------------------------------------------------
    public static void CreateRootBehaviourNode(iCS_VisualScriptImp visualScript) {
        var behaviour= new iCS_EngineObject(0, visualScript.name+"::Behaviour", typeof(iCS_VisualScriptImp), -1, iCS_ObjectTypeEnum.Behaviour);
        visualScript.Storage.EngineObjects.Add(behaviour);        
    }

    // ---------------------------------------------------------------------------------
    public static bool IsSameVisualScript(iCS_IStorage iStorage, iCS_StorageImp storage) {
        if(iStorage == null || storage == null) return false;
        if(iStorage.Storage == storage) return true;
        return false;
    }
    // ---------------------------------------------------------------------------------
    public static bool IsSameVisualScript(iCS_MonoBehaviourImp monoBehaviour, iCS_IStorage iStorage) {
        if(monoBehaviour == null || iStorage == null) return false;
        if(iStorage.iCSMonoBehaviour == monoBehaviour) return true;
        return false;
    }
}
