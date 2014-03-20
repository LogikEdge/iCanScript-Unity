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
    
    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
    static Rect BuildRect(Vector2 p, Vector2 s) {
        return new Rect(p.x, p.y, s.x, s.y);
    }
}
