using UnityEngine;
using UnityEditor;
using System.Collections;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {

    public class UserTransactionController {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        public  static bool  ShowUserTransaction   = false;
        public  int          myUserTransactionCount= 0;
                
        // ======================================================================
        // User transaction management
        // ----------------------------------------------------------------------
        public bool IsUserTransactionActive {
            get { return myUserTransactionCount != 0; }
        }
        // ----------------------------------------------------------------------
        public void ClearUserTransactions() {
            if(myUserTransactionCount != 0) {
                myUserTransactionCount= 0;
                Debug.LogWarning("iCanScript: Internal Error: User transaction was forced closed.");            
            }
        }
        // ----------------------------------------------------------------------
        /// Opens a new user transaction.
        public void OpenUserTransaction() {
            if(myUserTransactionCount == 0) {
                Undo.IncrementCurrentGroup();
            }
            ++myUserTransactionCount;
            // -- Display user transactions information. --
            if(ShowUserTransaction) {
                Debug.Log("Open: User Transaction Count=> "+myUserTransactionCount);                
            }
        }
        // ----------------------------------------------------------------------
        public void CloseUserTransaction(iCS_IStorage iStorage, string undoMessage= "", TransactionType transactionType= TransactionType.Graph) {
            if(myUserTransactionCount <= 0) {
                Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
                myUserTransactionCount= 0;
                return;
            }
            if(myUserTransactionCount > 0) {
                --myUserTransactionCount;
            }
            if(myUserTransactionCount == 0) {
                // Schedule saving this copy of the visual script.
                iCS_VisualScriptDataController.SaveWithUndo(iStorage, undoMessage, transactionType);
            }
            if(ShowUserTransaction) {
                Debug.Log("Close: User Transaction Count=> "+myUserTransactionCount);                
            }
        }
        // ----------------------------------------------------------------------
        public void CancelUserTransaction() {
            if(myUserTransactionCount <= 0) {
                Debug.LogWarning("iCanScript: Internal Error: Unbalanced user transaction.");
                return;
            }
            if(myUserTransactionCount > 0) {
                --myUserTransactionCount;
            }        
            if(ShowUserTransaction) {
                Debug.Log("Cancel: User Transaction Count=> "+myUserTransactionCount);                
            }
        }

    }
    
}
