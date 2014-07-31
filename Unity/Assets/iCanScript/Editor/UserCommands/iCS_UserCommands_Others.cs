//
// iCS_UserCommands_Others
//
//#define DEBUG
using UnityEngine;
using System.Collections;
using Pref= iCS_PreferencesController;


public static partial class iCS_UserCommands {
    // ======================================================================
    // Miscellanious User Commands.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject SetAsStateEntry(iCS_EditorObject state) {
#if DEBUG
        Debug.Log("iCanScript: Set As Entry State => "+state.Name);
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
        var name= state.Name;
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
        if(string.Compare(obj.RawName, name) == 0) return;
        var iStorage= obj.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                obj.Name= name;
                if(obj.IsNode) {
                    iStorage.ForcedRelayoutOfTree();
                }
                else if(obj.IsDataOrControlPort) {
                    iStorage.ForcedRelayoutOfTree();
                }
            }
        );
        CloseTransaction(iStorage, "Change name => "+name, TransactionType.Field);
        iCS_EditorController.RepaintEditorsWithLabels();
    }
    // ----------------------------------------------------------------------
    public static void AutoLayoutPort(iCS_EditorObject port) {
        if(port == null) return;
        var iStorage= port.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
				iStorage.AutoLayoutPort(port);
				iStorage.ForcedRelayoutOfTree();
		    }
		);
        // Save result.
        CloseTransaction(iStorage, "AutoLayout Port => "+port.Name);
    }
    // ----------------------------------------------------------------------
    public static void AutoLayoutPortsOnNode(iCS_EditorObject node) {
        if(node == null || !node.IsNode) return;
        var iStorage= node.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
				iStorage.AutoLayoutPortOnNode(node);
				iStorage.ForcedRelayoutOfTree();
		    }
		);
        // Save result.
        CloseTransaction(iStorage, "AutoLayout Ports on=> "+node.Name);
    }
    // ----------------------------------------------------------------------
    public static void UpdateMessageHandlerPorts(iCS_EditorObject messageHandler) {
        if(messageHandler == null) return;
        var iStorage= messageHandler.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.UpdateBehaviourMessagePorts(messageHandler);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Update Ports=> "+messageHandler.Name);
    }
}
