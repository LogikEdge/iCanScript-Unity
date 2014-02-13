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
        iStorage.RegisterUndo("Unfold "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                node.Unfold();
                node.LayoutNodeAndParents();
            }
        );
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
        iStorage.RegisterUndo("Fold "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                if(node.IsKindOfFunction || node.IsInstanceNode) {
                    node.Unfold();
                    node.LayoutNodeAndParents();
                }
                else {
                    node.Fold();
                    node.LayoutNodeAndParents();
                }
            }
        );

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
        iStorage.RegisterUndo("Iconize "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                node.Iconize();
                node.LayoutNodeAndParents();
            }
        );
    }
}
