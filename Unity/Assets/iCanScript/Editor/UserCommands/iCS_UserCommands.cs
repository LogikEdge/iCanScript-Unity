//
// File: iCS_UserCommands
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Transaction State
	// ----------------------------------------------------------------------
    // Opens a transaction that requires several steps.
    public static void OpenTransaction(iCS_IStorage iStorage) {
        iStorage.IsTransactionOpened= true;
    }
	// ----------------------------------------------------------------------
    // Saves the storage and prepare for a possible Undo operation.
    public static void CloseTransaction(iCS_IStorage iStorage, string undoMessage) {
        iStorage.SaveStorage(undoMessage);
    }
	// ----------------------------------------------------------------------
    // Retruns true if a transaction is opened.
    public static bool IsLastTransactionOpened(iCS_IStorage iStorage) {
        return iStorage.IsTransactionOpened == false;
    }
    
    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
    static Rect BuildRect(Vector2 p, Vector2 s) {
        return new Rect(p.x, p.y, s.x, s.y);
    }
	// ----------------------------------------------------------------------
    // Shows a notification in the Visual Editor window.
    static void ShowNotification(string message) {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null) {
            visualEditor.ShowNotification(new GUIContent(message));
        }
        else {
            Debug.LogWarning("iCanScript: "+message);
        }
    }
	// ----------------------------------------------------------------------
    static void SendDisplayRootChange(iCS_IStorage iStorage) {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == iStorage) {
            visualEditor.OnDisplayRootChange();
        }        
    }
	// ----------------------------------------------------------------------
    static void SendStartRelayoutOfTree(iCS_IStorage iStorage) {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == iStorage) {
            visualEditor.OnStartRelayoutOfTree();
        }
    }
	// ----------------------------------------------------------------------
    static void SendEndRelayoutOfTree(iCS_IStorage iStorage) {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == iStorage) {
            visualEditor.OnEndRelayoutOfTree();
        }
    }
	// ----------------------------------------------------------------------
    private static bool IsCreationAllowed() {
        if(Application.isPlaying) {
            var visualEditor= iCS_EditorController.FindVisualEditor();
            if(visualEditor != null) {
                visualEditor.ShowNotification(new GUIContent("PLEASE STOP ENGINE to add new nodes !!!"));
            }
            return false;
        }
        return true;
    }
	// ----------------------------------------------------------------------
    private static bool IsDeletionAllowed() {
        if(Application.isPlaying) {
            var visualEditor= iCS_EditorController.FindVisualEditor();
            if(visualEditor != null) {
                visualEditor.ShowNotification(new GUIContent("PLEASE STOP ENGINE to remove nodes !!!"));
            }
            return false;
        }
        return true;
    }
}
