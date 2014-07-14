//
// File: iCS_UserCommands_ObjectDrag
//
using UnityEngine;
using UnityEditor;
using System.Collections;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Node drag
	// ----------------------------------------------------------------------
    public static void StartNodeDrag(iCS_EditorObject node) {
    }
    public static void EndNodeDrag(iCS_EditorObject node) {
        node.IStorage.SaveStorage("Node Drag");
    }
    public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
    }
    public static void EndMultiSelectionDrag(iCS_IStorage iStorage) {
        iStorage.SaveStorage("Multi-Selection Node Drag");
    }
    public static void StartNodeRelocation(iCS_EditorObject node) {
    }
    public static void EndNodeRelocation(iCS_EditorObject node, iCS_EditorObject oldParent, iCS_EditorObject newParent) {
        var iStorage= node.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                if(oldParent != newParent) {
                    iStorage.ChangeParent(node, newParent); 
                }
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
            }
        );
        iStorage.SaveStorage("Node Relocation");
    }
    public static void StartPortDrag(iCS_EditorObject port) {
    }
    public static void EndPortDrag(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
            }
        );
        iStorage.SaveStorage("Port Drag");
    }
}
