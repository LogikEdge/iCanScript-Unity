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
        OpenTransaction(node.IStorage);
    }
    public static void EndNodeDrag(iCS_EditorObject node) {
        CloseTransaction(node.IStorage, "Drag "+node.Name);
    }
    public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
        OpenTransaction(iStorage);
    }
    public static void EndMultiSelectionDrag(iCS_IStorage iStorage) {
        CloseTransaction(iStorage, "Multi-Selection Node Drag");
    }
    public static void StartNodeRelocation(iCS_EditorObject node) {
        var iStorage= node.IStorage;
        OpenTransaction(iStorage);
    }
    public static void EndNodeRelocation(iCS_EditorObject node, iCS_EditorObject oldParent, iCS_EditorObject newParent) {
        var iStorage= node.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                var globalPosition= node.GlobalPosition;
                if(oldParent != newParent) {
                    iStorage.ChangeParent(node, newParent); 
                }
                node.LocalAnchorFromGlobalPosition= globalPosition;
				iStorage.AutoLayoutPortOnNode(node);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Relocate "+node.Name);
    }
    public static void CancelNodeRelocation(iCS_EditorObject node) {
        CancelTransaction(node.IStorage);
    }

	// ----------------------------------------------------------------------
	// We are not certain if we shoud be animating or not..
    public static void StartPortDrag(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        OpenTransaction(iStorage);
    }
    public static void EndPortConnection(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Port Connection=> "+port.Name);
    }
    public static void EndPortPublishing(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Port Publishing=> "+port.Name);
    }


    public static void EndPortDrag(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Drag "+port.Name);
    }
}
