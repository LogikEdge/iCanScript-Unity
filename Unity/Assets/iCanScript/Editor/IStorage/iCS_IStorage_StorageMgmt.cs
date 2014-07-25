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
    public  int    UserTransactionCount= 0;
    
    
    // ======================================================================
    // User transaction management
    // ----------------------------------------------------------------------
    public bool IsUserTransactionActive {
        get { return UserTransactionCount != 0; }
    }
    public void ClearUserTransactions() {
        if(UserTransactionCount != 0) {
            SaveStorage();
            UserTransactionCount= 0;
            Debug.LogWarning("iCanScript: Internal Error: User transaction was forced closed.");            
        }
    }
    public void OpenUserTransaction() {
        if(UserTransactionCount == 0) {
            SaveSelectedObjectPosition();
        }
        ++UserTransactionCount;
    }
    public void CloseUserTransaction(string undoMessage= "") {
        if(UserTransactionCount <= 0) {
            Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
        }
        if(UserTransactionCount > 0) {
            --UserTransactionCount;
        }
        if(UserTransactionCount == 0) {
            SaveStorage(undoMessage);
        }
    }
    public void CancelUserTransaction() {
        if(UserTransactionCount <= 0) {
            Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
        }
        if(UserTransactionCount > 0) {
            --UserTransactionCount;
        }        
    }
    
    // ======================================================================
    // Undo/Redo support
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
    private void SaveStorage(string undoMessage) {
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
        ClearUserTransactions();
        ++ModificationId;
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
    // ----------------------------------------------------------------------
    public void SaveSelectedObjectPosition() {
//        var selectedObject= SelectedObject.IsNode ? SelectedObject : SelectedObject.ParentNode;
        var selectedPos= SelectedObject.GlobalPosition;
        Storage.SelectedObjectPosition= selectedPos;
        PersistentStorage.SelectedObjectPosition= selectedPos;
        Debug.Log("Saving position for=> "+SelectedObject.Name+" "+selectedPos);
    }
    // ----------------------------------------------------------------------
    public void FlushLayoutData() {
        FlushSelectedObject();
        FlushScrollpositionAndGuiScale();
        FlushDisplayRoot();
    }
    // ----------------------------------------------------------------------
    public void FlushSelectedObject() {
        PersistentStorage.SelectedObject= Storage.SelectedObject;
    }
    // ----------------------------------------------------------------------
    public void FlushScrollpositionAndGuiScale() {
        FlushScrollposition();
        FlushGuiScale();
    }
    // ----------------------------------------------------------------------
    public void FlushScrollposition() {
        PersistentStorage.ScrollPosition= Storage.ScrollPosition;
    }
    // ----------------------------------------------------------------------
    public void FlushGuiScale() {
        PersistentStorage.GuiScale= Storage.GuiScale;
    }
    // ----------------------------------------------------------------------
    public void FlushDisplayRoot() {
        PersistentStorage.DisplayRoot= Storage.DisplayRoot;
        PersistentStorage.ShowDisplayRootNode= Storage.ShowDisplayRootNode;
    }
}
