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
        node.IStorage.RegisterUndo("Node Drag");
    }
    public static void EndNodeDrag(iCS_EditorObject node) {
        node.IStorage.IsDirty= true;
    }
    public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
        iStorage.RegisterUndo("Multi-Selection Node Drag");
    }
    public static void EndMultiSelectionDrag(iCS_IStorage iStorage) {
        iStorage.IsDirty= true;
    }
    public static void StartNodeRelocation(iCS_EditorObject node) {
        node.IStorage.RegisterUndo("Node Relocation");
        Debug.Log("iCanScript: Undo Group => "+Undo.GetCurrentGroup());
        node.IStorage.RegisterUndo("Node Relocation");
        Debug.Log("iCanScript: Undo Group => "+Undo.GetCurrentGroup());        
    }
    public static void EndNodeRelocation(iCS_EditorObject node, iCS_EditorObject oldParent, iCS_EditorObject newParent) {
        var iStorage= node.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                if(oldParent != newParent) {
                    iStorage.ChangeParent(node, newParent); 
                    oldParent.LayoutNodeAndParents();                   
                }
                node.LayoutNodeAndParents();
            }
        );
        node.IStorage.IsDirty= true;
    }
    public static void StartPortDrag(iCS_EditorObject port) {
        port.IStorage.RegisterUndo("Port Drag");
    }
    public static void EndPortDrag(iCS_EditorObject port) {
        port.IStorage.AnimateGraph(null,
            _=> {
                port.ParentNode.LayoutNodeAndParents();                
            }
        );
        port.IStorage.IsDirty= true;
    }
}
