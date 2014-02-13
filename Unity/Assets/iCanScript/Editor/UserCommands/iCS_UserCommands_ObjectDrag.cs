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
        var iStorage= node.IStorage;
        iStorage.RegisterUndo("Node Drag");
    }
    public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
        iStorage.RegisterUndo("Multi-Selection Node Drag");
    }
    public static void StartNodeRelocation(iCS_EditorObject node) {
        var iStorage= node.IStorage;
        iStorage.RegisterUndo("Node Relocation");
    }
    public static void StartPortDrag(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.RegisterUndo("Port Drag");
    }
}
