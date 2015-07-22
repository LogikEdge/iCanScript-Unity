//
// File: iCS_UserCommands_DisplayOptions
//
//#define DEBUG
using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ======================================================================
        // Change in display options.
    	// ----------------------------------------------------------------------
        // OK
        public static void Unfold(iCS_EditorObject node) {
#if DEBUG
            Debug.Log("iCanScript: Unfold => "+node.DisplayName);
#endif
            if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
                return;
            }
            var iStorage= node.IStorage;
            OpenTransaction(iStorage);
            try {
                SendStartRelayoutOfTree(iStorage);
                iStorage.AnimateGraph(null,
                    _=> {
                        node.Unfold();
                        node.SetAsHighestLayoutPriority();
                        iStorage.ForcedRelayoutOfTree();
                        node.ClearLayoutPriority();
                        var visualEditor= iCS_EditorController.FindVisualEditor();
                        if(visualEditor != null) {
                            visualEditor.SmartFocusOn(node);
                        }
                    }
                );
                SendEndRelayoutOfTree(iStorage);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Unfold "+node.DisplayName);
        }
    	// ----------------------------------------------------------------------
        // OK
        public static void Fold(iCS_EditorObject node) {
#if DEBUG
            Debug.Log("iCanScript: Fold => "+node.DisplayName);
#endif        
            if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Folded) {
                return;
            }
            var iStorage= node.IStorage;
            OpenTransaction(iStorage);
            try {
                SendStartRelayoutOfTree(iStorage);
                iStorage.AnimateGraph(null,
                    _=> {
                        node.Fold();
                        node.SetAsHighestLayoutPriority();
                        iStorage.ForcedRelayoutOfTree();
                        node.ClearLayoutPriority();
                        var visualEditor= iCS_EditorController.FindVisualEditor();
                        if(visualEditor != null) {
                            visualEditor.SmartFocusOn(node);
                        }
                    }
                );
                SendEndRelayoutOfTree(iStorage);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Fold "+node.DisplayName);
        }
    	// ----------------------------------------------------------------------
        // OK
        public static void Iconize(iCS_EditorObject node) {
#if DEBUG
            Debug.Log("iCanScript: Iconize => "+node.DisplayName);
#endif        
            if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Iconized) {
                return;
            }
            var iStorage= node.IStorage;
            OpenTransaction(iStorage);
            try {
                SendStartRelayoutOfTree(iStorage);
                iStorage.AnimateGraph(null,
                    _=> {
                        node.Iconize();
                        node.SetAsHighestLayoutPriority();
                        iStorage.ForcedRelayoutOfTree();
                        node.ClearLayoutPriority();
                        var visualEditor= iCS_EditorController.FindVisualEditor();
                        if(visualEditor != null) {
                            var focusNode= node.Parent;
                            if(focusNode == null) focusNode= node;
                            visualEditor.SmartFocusOn(focusNode);
                        }
                    }
                );
                SendEndRelayoutOfTree(iStorage);                   
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Iconize "+node.DisplayName);
        }
    	// ----------------------------------------------------------------------
        public static void FocusOn(iCS_EditorObject obj) {
            var visualEditor= iCS_EditorController.FindVisualEditor();
            if(visualEditor != null) {
                var iStorage= obj.IStorage;
                OpenTransaction(iStorage);
                visualEditor.CenterAndScaleOn(obj);
                CloseTransaction(iStorage, "Focus on "+obj.DisplayName);
            }
        }
    	// ----------------------------------------------------------------------
        public static void SmartFocusOn(iCS_EditorObject obj) {
            var visualEditor= iCS_EditorController.FindVisualEditor();
            if(visualEditor != null) {
                var iStorage= obj.IStorage;
                OpenTransaction(iStorage);
                visualEditor.SmartFocusOn(obj);
                CloseTransaction(iStorage, "Focus on "+obj.DisplayName);
            }
        }
    }

}

