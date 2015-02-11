using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P= Prelude;
using Prefs= iCS_PreferencesController;
using iCanScript.Editor;
using iCanScript.Editor.CodeEngineering;

public enum TransactionType { None, Graph, Navigation, Field };

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
           UserTransactionController  myUserTransactionController = new UserTransactionController();
    public TransactionType            myLastTransactionType       = TransactionType.None;
    public int                        myFirstNavigationUndoGroupId= 0;
    public int                        myFirstFieldUndoGroupId     = 0;
           int                        myCurrentUndoGroupId        = 0;
    
    // ======================================================================
    // User transaction management
    // ----------------------------------------------------------------------
    public bool IsUserTransactionActive {
        get { return myUserTransactionController.IsUserTransactionActive; }
    }
    public void ClearUserTransactions() {
        myUserTransactionController.ClearUserTransactions();
    }
    public void OpenUserTransaction() {
        myUserTransactionController.OpenUserTransaction();
        myCurrentUndoGroupId= Undo.GetCurrentGroup();
    }
    public void CloseUserTransaction(string undoMessage= "", TransactionType transactionType= TransactionType.Graph) {
        myUserTransactionController.CloseUserTransaction(this, undoMessage, transactionType);
    }
    public void CancelUserTransaction() {
        myUserTransactionController.CancelUserTransaction();
    }
    
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    void DetectUndoRedo() {
//        // Regenerate internal structures if undo/redo was performed.
//        if(EngineStorage.UndoRedoId != Storage.UndoRedoId) {
//			SynchronizeAfterUndoRedo();
//        }        
    }
    // ----------------------------------------------------------------------
    public void SynchronizeAfterUndoRedo() {
        iCS_UserCommands.UndoRedo(this);
		iCS_EditorController.RepaintVisualEditor();
    }
    // ----------------------------------------------------------------------
    public void SaveWithUndo(string undoMessage, TransactionType transactionType) {
        // Start recording changes for Undo.
        if(UserTransactionController.ShowUserTransaction) {
            Debug.Log("iCanScript: Saving=> "+undoMessage);            
        }
        ++Storage.UndoRedoId;
        // Collapse undo group for same transaction.
        var currentGroupId= Undo.GetCurrentGroup();
        if(currentGroupId != myCurrentUndoGroupId) {
            Undo.CollapseUndoOperations(myCurrentUndoGroupId);
        }
        // Prepare to record modification to group.
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
        // Keep a copy of the selected object global position.
        Storage.SelectedObjectPosition= SelectedObject.GlobalPosition;
        // Tell Unity that our storage has changed.
        iCS_StorageImp.CopyFromTo(Storage, EngineStorage);
        // Commit Undo transaction and forces redraw of inspector window.
        EditorUtility.SetDirty(iCSMonoBehaviour);
        ++ModificationId;
        iCS_EditorController.RepaintAllEditors();
        SystemEvents.AnnouceVisualScriptSaved(this);
    }
    // ----------------------------------------------------------------------
    public void GenerateEditorData() {
        // Duplicate engine storage
        if(Storage == null) {
            Storage= new iCS_VisualScriptData(iCSMonoBehaviour);
        }
        try {
            iCS_VisualScriptData.CopyFromTo(EngineStorage, Storage);            
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
        myLastTransactionType= TransactionType.None;
        SystemEvents.AnnouceVisualScriptReloaded(this);
        CSharpFileUtils.VerifyAndCorrectSourceFile(this);
    }
}
