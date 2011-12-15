using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
    // ======================================================================
    // Runtime code generation
    // ----------------------------------------------------------------------
    public static void GenerateRuntimeCodeCallback(UK_Behaviour behaviour, object _iStorage) {
        Debug.Log("Code generation callback invoked.");
        UK_IStorage iStorage= _iStorage == null ? new UK_IStorage(behaviour) : _iStorage as UK_IStorage;
        iStorage.GenerateRuntimeCode();
        behaviour.SetCodeGenerationAction(null, null);
    }
    // ----------------------------------------------------------------------
    public void GenerateRuntimeCode() {
        // Only generate runtime code for behaviours.
        UK_Behaviour rtBehaviour= Storage as UK_Behaviour;
        if(rtBehaviour == null || EditorObjects.Count == 0) return;
        // Remove any previous runtime data.
        UK_EditorObject edBehaviour= EditorObjects[0];
        if(!edBehaviour.IsBehaviour) {
            Debug.LogError("Could not locate Behaviour object.  Aborting code generation.");
        }
        // Remove any previous runtime object creation.
        rtBehaviour.ClearGeneratedCode();
        // Create all the runtime nodes.
        rtBehaviour.GenerateRuntimeNodes();
        // Connect the runtime nodes.
        rtBehaviour.ConnectRuntimeNodes();
    }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public UK_EditorObject GetDataConnectionSource(UK_EditorObject port) {
        return Storage.GetDataConnectionSource(port);
    }
    
}
