using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_VisualScriptDataController {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        static bool             myIsPlaying       = false;
    	static iCS_IStorage 	myIStorage        = null;
	
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
        public static iCS_IStorage          IStorage         { get { return myIStorage; }}
        public static iCS_VisualScriptData  Storage          { get { return IStorage != null ? IStorage.Storage : null; }}
        public static iCS_EditorObject      SelectedObject   {
            get { return IStorage != null ? IStorage.SelectedObject : null; }
            set {
                if(IStorage != null) {
                    iCS_UserCommands.Select(value, IStorage);                
                }
            }
        }
	
        // =================================================================================
        // Installation
        // ---------------------------------------------------------------------------------
        static iCS_VisualScriptDataController() {
            mySaveTimer= TimerService.CreateTimedAction(0.5f, PerformSaveWithUndo);
        }
        public static void Start() {}
        public static void Shutdown() {}
    
        // ---------------------------------------------------------------------------------
        public static bool IsSameVisualScript(iCS_IStorage iStorage, iCS_VisualScriptData storage) {
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
        // ---------------------------------------------------------------------------------
        public static bool IsInUse(iCS_MonoBehaviourImp monoBehaviour) {
            if(IStorage == null) return false;
            return IStorage.VisualScript == monoBehaviour;
        }
    
        // =================================================================================
        // Storage & Selected object Update.  This update is called by the Editors.
        // ---------------------------------------------------------------------------------
    	public static void Update() {
            // -- Use previous game object if new selection does not include a visual script. --
    		GameObject go= Selection.activeGameObject;
            var monoBehaviour= go != null ? go.GetComponent<iCS_MonoBehaviourImp>() : null;
            // -- Verify if we should save visual script. --
            if(myIStorage != null && myIStorage.iCSMonoBehaviour != monoBehaviour) {
                var previousMonobehaviour= myIStorage.iCSMonoBehaviour;
                if(previousMonobehaviour != null) {
                    ImmediateSaveWithUndo();
                    SaveAndLoad.Save(myIStorage);
                }
            }
            if(monoBehaviour == null) {
                // Clear if previous game object is not valid.
                ImmediateSaveWithUndo();
                myIStorage= null;
                myIsPlaying= Application.isPlaying;
                return;
            }
    		// Verify for storage change.
            bool isPlaying= Application.isPlaying;
    		if(myIStorage == null || myIStorage.iCSMonoBehaviour != monoBehaviour || myIsPlaying != isPlaying) {
                ImmediateSaveWithUndo();
                myIsPlaying= isPlaying;
    			myIStorage= new iCS_IStorage(monoBehaviour);
    			return;
    		}
    	}

        // =================================================================================
        // Save & Undo
        // ---------------------------------------------------------------------------------
        static TimedAction      mySaveTimer= null;
        static iCS_IStorage     mySaveIStorage= null;
        static string           mySaveUndoMessage= null;
        static TransactionType  mySaveTransactionType= TransactionType.None;
    
        public static void SaveWithUndo(iCS_IStorage iStorage, string undoMessage, TransactionType transactionType) {
            if(mySaveIStorage != null && mySaveIStorage != iStorage) {
                ImmediateSaveWithUndo();
            }
            mySaveIStorage= iStorage;
            mySaveUndoMessage= undoMessage;
            mySaveTransactionType= transactionType;
            mySaveTimer.Restart();
        }
    
        static void ImmediateSaveWithUndo() {
            if(mySaveTimer.IsElapsed) return;
            mySaveTimer.Stop();
            PerformSaveWithUndo();
        }
        static void PerformSaveWithUndo() {
            if(mySaveIStorage == null) return;
            var iStorageToSave= mySaveIStorage;
            mySaveIStorage= null;
            iStorageToSave.SaveWithUndo(mySaveUndoMessage, mySaveTransactionType);
        }
    }

}
