using UnityEngine;
using UnityEditor;
using System.Collections;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private bool undoRedoRunning= false;
    
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    public void RegisterUndo(string message= "iCanScript") {
        Debug.Log("Registering Undo");
        Undo.RecordObject(Storage, message);
        Storage.UndoRedoId= ++UndoRedoId;
    }
    // ----------------------------------------------------------------------
    void DetectUndoRedo() {
//        // Regenerate internal structures if undo/redo was performed.
//        if(Storage.UndoRedoId != UndoRedoId) {
//			SynchronizeAfterUndoRedo();
//        }        
    }
    // ----------------------------------------------------------------------
    public void SynchronizeAfterUndoRedo() {
        // Avoid reentry.
        if(undoRedoRunning) {
            Debug.LogWarning("iCanScript: Reentering SynchronizeAfterUndoRedo...");
            return;
        }
        undoRedoRunning= true;
        Storage.UndoRedoId= ++UndoRedoId;
        iCS_UserCommands.UndoRedo(this);
        undoRedoRunning= false; 
		IsDirty= true;
		iCS_EditorMgr.RepaintVisualEditor();
    }
    // ----------------------------------------------------------------------
    void SaveStorage() {
        // Tell Unity that our storage has changed.
        Debug.Log("Saving visual script");
        EditorUtility.SetDirty(Storage);
    }
    // ----------------------------------------------------------------------
    public void GenerateEditorData() {
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
        
        // Cleanup Visual Editor display root.
        DisplayRoot= DisplayRoot;
    }
}
