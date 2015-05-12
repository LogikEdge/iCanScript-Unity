//
// iCS_UserCommands_Others
//
//#define DEBUG
using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public static partial class iCS_UserCommands {
        // ======================================================================
        // Miscellanious User Commands.
    	// ----------------------------------------------------------------------
        // OK
        public static iCS_EditorObject SetAsStateEntry(iCS_EditorObject state) {
    #if DEBUG
            Debug.Log("iCanScript: Set As Entry State => "+state.DisplayName);
    #endif
            if(state == null) return null;
            var iStorage= state.IStorage;
            OpenTransaction(iStorage);
            iStorage.ForEachChild(state.Parent,
                child=>{
                    if(child.IsEntryState) {
                        child.IsEntryState= false;
                    }
                }
            );
            state.IsEntryState= true;
            var name= state.DisplayName;
            CloseTransaction(iStorage, "Set As Entry "+name);
            return state;
        }
    	// ----------------------------------------------------------------------
        public static void ShowInHierarchy(iCS_EditorObject obj) {
            var editor= iCS_EditorController.FindTreeViewEditor();
            if(editor != null) {
                editor.ShowElement(obj);
            }
        }
        // ----------------------------------------------------------------------
        public static void ChangeName(iCS_EditorObject obj, string name) {
            if(name == null) return;
            name.Trim();
            if(string.Compare(obj.DisplayName, name) == 0) return;
            var iStorage= obj.IStorage;
            if(obj.IsNode && string.IsNullOrEmpty(name)) {
                name= NameUtility.ToDisplayName(obj.CodeName);
            }
            OpenTransaction(iStorage);
            try {
                iStorage.AnimateGraph(null,
                    _=> {
                        obj.DisplayName= name;
                        if(obj.IsNode) {
                            iStorage.ForcedRelayoutOfTree();
                        }
                        else if(obj.IsDataOrControlPort) {
                            iStorage.ForcedRelayoutOfTree();
                        }
                    }
                );            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Change name => "+name, TransactionType.Field);
            iCS_EditorController.RepaintEditorsWithLabels();
    		SystemEvents.AnnounceVisualScriptElementNameChanged(obj);
        }
        // ----------------------------------------------------------------------
        public static void ChangeDescription(iCS_EditorObject obj, string description) {
            var iStorage= obj.IStorage;
            if(string.IsNullOrEmpty(description)) {
                OpenTransaction(iStorage);
                obj.Description= null;
                CloseTransaction(iStorage, "Change tooltip for "+obj.DisplayName, TransactionType.Field);
                return;
            }
            description.Trim();
            if(string.Compare(obj.Description, description) == 0) return;
            OpenTransaction(iStorage);
            obj.Description= description;
            CloseTransaction(iStorage, "Change description for "+obj.DisplayName, TransactionType.Field);
        }
        // ----------------------------------------------------------------------
    	public static void ChangePortValue(iCS_EditorObject port, object newValue) {
    		if(port == null) return;
    		var iStorage= port.IStorage;
            OpenTransaction(iStorage);
    		try {
    			port.PortValue= newValue;
    		}
    		catch(System.Exception) {
    			CancelTransaction(iStorage);
    		}
            iCS_UserCommands.CloseTransaction(iStorage, "Change port value => "+port.DisplayName, TransactionType.Field);
            iCS_EditorController.RepaintEditorsWithValues();
    	}
        // ----------------------------------------------------------------------
        public static void AutoLayoutPort(iCS_EditorObject port) {
            if(port == null) return;
            var iStorage= port.IStorage;
            OpenTransaction(iStorage);
            try {
                iStorage.AnimateGraph(null,
                    _=> {
        				iStorage.AutoLayoutPort(port);
        				iStorage.ForcedRelayoutOfTree();
        		    }
        		);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            // Save result.
            CloseTransaction(iStorage, "AutoLayout Port => "+port.DisplayName);
        }
        // ----------------------------------------------------------------------
        public static void AutoLayoutPortsOnNode(iCS_EditorObject node) {
            if(node == null || !node.IsNode) return;
            var iStorage= node.IStorage;
            OpenTransaction(iStorage);
            try {
                iStorage.AnimateGraph(null,
                    _=> {
        				iStorage.AutoLayoutPortOnNode(node);
        				iStorage.ForcedRelayoutOfTree();
        		    }
        		);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            // Save result.
            CloseTransaction(iStorage, "AutoLayout Ports on=> "+node.DisplayName);
        }
        // ----------------------------------------------------------------------
        public static void SetScrollPosition(iCS_IStorage iStorage, Vector2 newPosition) {
            OpenTransaction(iStorage);
            iStorage.ScrollPosition= newPosition;
            CloseTransaction(iStorage, "Scroll", TransactionType.Navigation);
        }
        // ----------------------------------------------------------------------
        public static void SetZoom(iCS_IStorage iStorage, float newZoom) {
            OpenTransaction(iStorage);
            if(newZoom > 2f) newZoom= 2f;
            if(newZoom < 0.15f) newZoom= 0.15f;
            iStorage.GuiScale= newZoom;
            CloseTransaction(iStorage, "Zoom", TransactionType.Navigation);
        }
    }

}

