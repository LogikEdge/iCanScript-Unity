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
        OpenTransaction(node.IStorage);
        CloseTransaction(node.IStorage, "Node Drag");
    }
    public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
    }
    public static void EndMultiSelectionDrag(iCS_IStorage iStorage) {
        OpenTransaction(iStorage);
        CloseTransaction(iStorage, "Multi-Selection Node Drag");
    }
    public static void StartNodeRelocation(iCS_EditorObject node) {
    }
    public static void EndNodeRelocation(iCS_EditorObject node, iCS_EditorObject oldParent, iCS_EditorObject newParent) {
        var iStorage= node.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                if(oldParent != newParent) {
                    iStorage.ChangeParent(node, newParent); 
                }
                iStorage.ForcedRelayoutOfTree();
				iStorage.AutoLayoutPortOnNode(node);
            }
        );
        CloseTransaction(iStorage, "Node Relocation");
    }


	// ----------------------------------------------------------------------
	// We are not certain if we shoud be animating or not..
    public static void StartPortDrag(iCS_EditorObject port) {
    }

    public static void StartPortConnection(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.PrepareToAnimate();
    }
    public static void EndPortConnection(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        OpenTransaction(iStorage);
        iStorage.ForcedRelayoutOfTree();
        iStorage.StartToAnimate();
        CloseTransaction(iStorage, "Port Connection=> "+port.Name);
    }
    public static void EndPortPublishing(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        OpenTransaction(iStorage);
        iStorage.ForcedRelayoutOfTree();
        CloseTransaction(iStorage, "Port Publishing=> "+port.Name);
    }


    public static void EndPortDrag(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        OpenTransaction(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Port Drag");
    }
}
