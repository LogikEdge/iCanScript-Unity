//
// File: iCS_UserCommands_ObjectDrag
//
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ======================================================================
        // Node drag
    	// ----------------------------------------------------------------------
        public static void StartNodeDrag(iCS_EditorObject node) {
            OpenTransaction(node.IStorage);
        }
        public static void EndNodeDrag(iCS_EditorObject node) {
            CloseTransaction(node.IStorage, "Drag "+node.DisplayName);
        }
        public static void StartMultiSelectionNodeDrag(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
        }
        public static void EndMultiSelectionDrag(iCS_IStorage iStorage) {
            CloseTransaction(iStorage, "Multi-Selection Node Drag");
        }
        // ======================================================================
        // Node Relocation
    	// ----------------------------------------------------------------------
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
            CloseTransaction(iStorage, "Relocate "+node.DisplayName);
        }
        public static void CancelNodeRelocation(iCS_EditorObject node) {
            CancelTransaction(node.IStorage);
        }
        public static void StartMultiSelectionNodeRelocation(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
            // Keep a copy of the original position.
            var selectedNodes= iStorage.FilterMultiSelectionForMove();
            foreach(var node in selectedNodes) {
                node.AnimationTargetRect= node.GlobalRect;
            }
        }
        public static void EndMultiSelectionNodeRelocation(iCS_EditorObject[] selectedNodes, iCS_EditorObject oldParent, iCS_EditorObject newParent) {
            var iStorage= newParent.IStorage;
            iStorage.AnimateGraph(null,
                _=> {
                    foreach(var node in selectedNodes) {
                        var globalPosition= node.GlobalPosition;
                        if(oldParent != newParent) {
                            iStorage.ChangeParent(node, newParent); 
                        }
                        node.LocalAnchorFromGlobalPosition= globalPosition;
        				iStorage.AutoLayoutPortOnNode(node);                    
                    }
                    iStorage.ForcedRelayoutOfTree();
                }
            );
            CloseTransaction(iStorage, "Multi-Selection Node Relocation");
        }
        public static void CancelMultiSelectionNodeRelocation(iCS_IStorage iStorage) {
            // Animate node back to its original position.
            iStorage.AnimateGraph(null,
                _=> {
                    iStorage.CancelMultiSelectionNodeRelocation();
                    iStorage.ForcedRelayoutOfTree();
                }
            );        
            CancelTransaction(iStorage);
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
            CloseTransaction(iStorage, "Port Connection=> "+port.DisplayName);
        }
        public static void EndPortPublishing(iCS_EditorObject port) {
            var iStorage= port.IStorage;
            iStorage.AnimateGraph(null,
                _=> {
                    iStorage.ForcedRelayoutOfTree();
                }
            );
            CloseTransaction(iStorage, "Port Publishing=> "+port.DisplayName);
        }


        public static void EndPortDrag(iCS_EditorObject port) {
            var iStorage= port.IStorage;
            iStorage.AnimateGraph(null,
                _=> {
                    iStorage.ForcedRelayoutOfTree();
                }
            );
            CloseTransaction(iStorage, "Drag "+port.DisplayName);
        }
    }

}

