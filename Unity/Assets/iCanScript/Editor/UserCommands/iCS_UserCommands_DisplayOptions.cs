//
// File: iCS_UserCommands_DisplayOptions
//
#define DEBUG
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
        if(!node.IsNode) return;
        var iStorage= node.IStorage;
        iStorage.RegisterUndo("Unfold "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                node.Unfold();
            }
        );
    }
	// ----------------------------------------------------------------------
    // OK
    public static void Fold(iCS_EditorObject node) {
#if DEBUG
        Debug.Log("iCanScript: Fold => "+node.Name);
#endif        
        if(!node.IsNode) return;
        var iStorage= node.IStorage;        
        iStorage.RegisterUndo("Fold "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                if(node.IsKindOfFunction || node.IsInstanceNode) {
                    node.Unfold();
                }
                else {
                    node.Fold();
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
        if(!node.IsNode) return;
        var iStorage= node.IStorage;
        iStorage.RegisterUndo("Iconize "+node.Name);
        iStorage.AnimateGraph(null,
            _=> {
                node.Iconize();
            }
        );
    }
}
