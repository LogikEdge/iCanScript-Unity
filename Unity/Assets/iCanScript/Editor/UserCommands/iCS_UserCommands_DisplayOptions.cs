//
// File: iCS_UserCommands_DisplayOptions
//
//#define DEBUG
using UnityEngine;
using System.Collections;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Change in display options.
	// ----------------------------------------------------------------------
    // OK
    public static void Unfold(iCS_EditorObject node) {
#if DEBUG
        Debug.Log("iCanScript: Unfold => "+node.Name);
#endif
        if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            return;
        }
        var iStorage= node.IStorage;
        SendStartRelayoutOfTree(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                var oldNodePos= node.LayoutPosition;
                node.Unfold();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                node.ClearLayoutPriority();
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
                    visualEditor.ReframeOn(node, oldNodePos, node);
                    visualEditor.MakeVisibleInViewport(node);
//                    visualEditor.ReduceEmptyViewport();
                }
            }
        );
        SendEndRelayoutOfTree(iStorage);
        iStorage.SaveStorage("Unfold "+node.Name);
    }
	// ----------------------------------------------------------------------
    // OK
    public static void Fold(iCS_EditorObject node) {
#if DEBUG
        Debug.Log("iCanScript: Fold => "+node.Name);
#endif        
        if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Folded) {
            return;
        }
        var iStorage= node.IStorage;        
        SendStartRelayoutOfTree(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                var oldNodePos= node.LayoutPosition;
                node.Fold();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                node.ClearLayoutPriority();
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
                    parent= node.ParentNode;
                    if(parent == null || parent.IsParentOf(DisplayRoot)) {
                        parent= DisplayRoot;
                    }
                    visualEditor.ReframeOn(node, oldNodePos, parent);
                    var newPos= node.LayoutPosition;
                    visualEditor.MakeVisibleInViewport(node);
//                    visualEditor.ReduceEmptyViewport();
                }
            }
        );
        SendEndRelayoutOfTree(iStorage);
        iStorage.SaveStorage("Fold "+node.Name);
    }
	// ----------------------------------------------------------------------
    // OK
    public static void Iconize(iCS_EditorObject node) {
#if DEBUG
        Debug.Log("iCanScript: Iconize => "+node.Name);
#endif        
        if(!node.IsNode || node.DisplayOption == iCS_DisplayOptionEnum.Iconized) {
            return;
        }
        var iStorage= node.IStorage;
        SendStartRelayoutOfTree(iStorage);
        iStorage.AnimateGraph(null,
            _=> {
                var oldNodePos= node.LayoutPosition;
                node.Iconize();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                node.ClearLayoutPriority();
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
                    parent= node.ParentNode;
                    if(parent == null || parent.IsParentOf(DisplayRoot)) {
                        parent= DisplayRoot;
                    }
                    visualEditor.ReframeOn(node, oldNodePos, parent);
                    visualEditor.MakeVisibleInViewport(node);
//                    visualEditor.ReduceEmptyViewport();
                }
            }
        );
        SendEndRelayoutOfTree(iStorage);       
        iStorage.SaveStorage("Iconize "+node.Name);
    }

}
