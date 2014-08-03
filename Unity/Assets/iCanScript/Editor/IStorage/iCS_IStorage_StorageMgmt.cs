using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public enum TransactionType { Graph, Navigation, Field };

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public bool             ShowUserTransaction         = false;
    public int              UserTransactionCount        = 0;
    public TransactionType  myLastTransactionType       = TransactionType.Graph;
    public int              myFirstNavigationUndoGroupId= 0;
    public int              myFirstFieldUndoGroupId     = 0;
    
    // ======================================================================
    // User transaction management
    // ----------------------------------------------------------------------
    public bool IsUserTransactionActive {
        get { return UserTransactionCount != 0; }
    }
    public void ClearUserTransactions() {
        if(UserTransactionCount != 0) {
            UserTransactionCount= 0;
            Debug.LogWarning("iCanScript: Internal Error: User transaction was forced closed.");            
        }
    }
    public void OpenUserTransaction() {
        ++UserTransactionCount;
//        Debug.Log("Open: User Transaction Count=> "+UserTransactionCount);
    }
    public void CloseUserTransaction(string undoMessage= "", TransactionType transactionType= TransactionType.Graph) {
        if(UserTransactionCount <= 0) {
            Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
            UserTransactionCount= 0;
            return;
        }
        if(UserTransactionCount > 0) {
            --UserTransactionCount;
        }
        if(UserTransactionCount == 0) {
            SaveStorageWithUndo(undoMessage, transactionType);
        }
//        Debug.Log("Close: User Transaction Count=> "+UserTransactionCount);
    }
    public void CancelUserTransaction() {
        if(UserTransactionCount <= 0) {
            Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
            return;
        }
        if(UserTransactionCount > 0) {
            --UserTransactionCount;
        }        
//        Debug.Log("Cancel: User Transaction Count=> "+UserTransactionCount);
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
    private void SaveStorageWithUndo(string undoMessage, TransactionType transactionType) {
        // Start recording changes for Undo.
        if(ShowUserTransaction) {
            Debug.Log("iCanScript: Saving=> "+undoMessage);            
        }
        ++Storage.UndoRedoId;
        Undo.RecordObject(iCSMonoBehaviour, undoMessage);
        SaveStorage();        
        // Save type of user operation
        if(transactionType == myLastTransactionType) {
            if(myLastTransactionType == TransactionType.Navigation) {
                Undo.CollapseUndoOperations(myFirstNavigationUndoGroupId);
            }
            if(myLastTransactionType == TransactionType.Field) {
                Undo.CollapseUndoOperations(myFirstFieldUndoGroupId);                
            }            
        }
        // New user operation type
        else {
            // Take a snapshot of the first navigation transaction.
            if(transactionType == TransactionType.Navigation) {
                myFirstNavigationUndoGroupId= Undo.GetCurrentGroup();
            } 
            // Take a snapshot of the first field transaction.
            if(transactionType == TransactionType.Field) {
                myFirstFieldUndoGroupId= Undo.GetCurrentGroup();
            }             
        }
        myLastTransactionType= transactionType;
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
        myLastTransactionType= TransactionType.Graph;
    }
}
