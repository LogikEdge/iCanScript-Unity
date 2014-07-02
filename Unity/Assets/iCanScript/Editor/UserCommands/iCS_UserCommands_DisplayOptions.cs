﻿//
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
                node.Unfold();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
//                    visualEditor.MakeVisibleInViewport(node);
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
                var oldPos= node.LayoutPosition;
                node.Fold();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
//                    var newPos= node.LayoutPosition;
//                    if(Math3D.IsNotEqual(oldPos, newPos)){
//                        visualEditor.ScrollBy(newPos-oldPos);
//                    }
//                    visualEditor.MakeVisibleInViewport(node);
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
                var oldPos= node.LayoutPosition;
                node.Iconize();
                node.SetAsHighestLayoutPriority();
                iStorage.ForcedRelayoutOfTree(iStorage.DisplayRoot);
                var visualEditor= iCS_EditorController.FindVisualEditor();
                if(visualEditor != null) {
//                    var newPos= node.LayoutPosition;
//                    if(Math3D.IsNotEqual(oldPos, newPos)){
//                        visualEditor.ScrollBy(newPos-oldPos);
//                    }
//                    visualEditor.MakeVisibleInViewport(node);
//                    visualEditor.ReduceEmptyViewport();
                }
            }
        );
        SendEndRelayoutOfTree(iStorage);       
        iStorage.SaveStorage("Iconize "+node.Name);
    }

}
