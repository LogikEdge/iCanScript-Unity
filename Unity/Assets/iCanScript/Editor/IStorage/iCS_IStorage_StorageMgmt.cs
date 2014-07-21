using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private string myUndoMessage    = "";
    
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
//    public void RegisterUndo(string message= "iCanScript") {
//        Debug.Log("Registering Undo");
//        myUndoMessage= message;
//    }
    // ----------------------------------------------------------------------
    void DetectUndoRedo() {
//        // Regenerate internal structures if undo/redo was performed.
//        if(PersistentStorage.UndoRedoId != Storage.UndoRedoId) {
//			SynchronizeAfterUndoRedo();
//        }        
    }
    // ----------------------------------------------------------------------
    public void SynchronizeAfterUndoRedo() {
        iCS_UserCommands.UndoRedo(this);
		iCS_EditorController.RepaintVisualEditor();
    }
    // ----------------------------------------------------------------------
    void SaveStorageWithUndoRedoSupport() {
        // Start recording changes for Undo.
        ++Storage.UndoRedoId;
        Undo.RecordObject(iCSMonoBehaviour, myUndoMessage);
        SaveStorage();
    }
    // ----------------------------------------------------------------------
    public void SaveStorage(string undoMessage) {
        // Start recording changes for Undo.
//        Debug.Log("Saving visual script");
        ++Storage.UndoRedoId;
        Undo.RecordObject(iCSMonoBehaviour, undoMessage);
        SaveStorage();        
    }
    // ----------------------------------------------------------------------
    public void SaveStorage() {
        // Perform graph cleanup once objects & layout are stable.
        UpdateExecutionPriority();
        for(int retries= 0; retries < 10 && Cleanup(); ++retries);
        // Keep a copy of the selected object global position.
        Storage.SelectedObjectPosition= SelectedObject.GlobalPosition;
        // Tell Unity that our storage has changed.
        iCS_StorageImp.CopyFromTo(Storage, PersistentStorage);
        // Commit Undo transaction and forces redraw of inspector window.
        EditorUtility.SetDirty(iCSMonoBehaviour);
        IsTransactionOpened= false;
        ++ModificationId;
//        iCS_EditorController.Update();
        iCS_EditorController.RepaintAllEditors();
    }
    // ----------------------------------------------------------------------
    public void GenerateEditorData() {
        // Duplicate engine storage
        if(Storage == null) {
            Storage= new iCS_VisualScriptData(iCSMonoBehaviour);
        }
        try {
            iCS_VisualScriptData.CopyFromTo(PersistentStorage, Storage);            
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Unable to copy engine storage: "+e.Message);
        }
        
		// Rebuild Editor Objects from the Engine Objects.
		if(myEditorObjects == null) {
		    myEditorObjects= new List<iCS_EditorObject>();
	    }
		iCS_EditorObject.RebuildFromEngineObjects(this);
		
        // Re-initialize internal values.
        if(EditorObjects.Count > 0 && IsValid(EditorObjects[0])) {
            ForEach(obj=> {
				// Initialize initial port values.
				if(obj.IsInDataOrControlPort) {
					LoadInitialPortValueFromArchive(obj);
				}
            });            
        }
        CleanupUnityObjects();
        
        // Re-initialize multi-selection list.
        var selectedObject= SelectedObject;
        SelectedObject= selectedObject;
    }
}
