using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // ----------------------------------------------------------------------
	void ForcedRelayoutOfTree(iCS_EditorObject root) {
        ForEachRecursiveDepthFirst(root,
            obj=> {
            	if(!obj.IsVisibleInLayout) return;
				UpdateTransitionModuleName(obj);
				if(obj.IsNode) {
					obj.LayoutNode();
				}
            }
        );    		
	}
    // ----------------------------------------------------------------------
	void ForcedRedisplayOfTree(iCS_EditorObject root) {
        ForEachRecursiveDepthFirst(root,
            obj=> {
            	if(!obj.IsVisibleOnDisplay) return;
				UpdateTransitionModuleName(obj);
				if(obj.IsNode) {
					obj.LayoutNode();
				}
            }
        );    		
	}
    // ----------------------------------------------------------------------
	void UpdateTransitionModuleName(iCS_EditorObject obj) {
		if(!obj.IsTransitionModule) return;
        GetTransitionName(obj);		
	}
	
}
