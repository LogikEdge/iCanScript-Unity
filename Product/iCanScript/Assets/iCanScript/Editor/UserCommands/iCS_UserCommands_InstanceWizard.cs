//
// File: iCS_UserCommands_PropertiesWizard
//
//#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
    	// ----------------------------------------------------------------------
        // OK
        public static iCS_EditorObject CreatePropertiesWizardElement(iCS_EditorObject parent, LibraryObject libraryObject) {
#if DEBUG
            Debug.Log("iCanScript: Create Instance Element => "+libraryObject.displayString);
#endif
            if(parent == null) return null;
            var iStorage= parent.IStorage;

            iCS_EditorObject instance= null;
            OpenTransaction(iStorage);
            try {
                SendStartRelayoutOfTree(iStorage);
                iStorage.AnimateGraph(null,
                    _=> {
                        instance= iStorage.PropertiesWizardCreate(parent, libraryObject);
                        instance.SetInitialPosition(parent.GlobalPosition);
                        instance.Iconize();
                        iStorage.ForcedRelayoutOfTree();
                    }
                );                
                SendEndRelayoutOfTree(iStorage);            
            }
            catch(System.Exception) {
                instance= null;
            }
            if(instance == null) {
                CancelTransaction(iStorage);
                return null;
            }
            CloseTransaction(iStorage, "Create "+libraryObject.nodeName);            
            return instance;
        }
    	// ----------------------------------------------------------------------
        public static void DeletePropertiesWizardElement(iCS_EditorObject parent, LibraryObject libraryObject) {
#if DEBUG
            Debug.Log("iCanScript: Delete Instance Element => "+libraryObject.displayString);
#endif
            if(parent == null || libraryObject == null) return;
            var iStorage= parent.IStorage;
            OpenTransaction(iStorage);
            try {
                SendStartRelayoutOfTree(iStorage);
                iStorage.AnimateGraph(null,
                    _=> {
                        iStorage.PropertiesWizardDestroy(parent, libraryObject);
                        iStorage.ForcedRelayoutOfTree();
                    }
                );                
                SendEndRelayoutOfTree(iStorage);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Delete "+libraryObject.nodeName);            
        }
	}
}

