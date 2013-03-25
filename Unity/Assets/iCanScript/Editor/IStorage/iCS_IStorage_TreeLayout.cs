using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // ----------------------------------------------------------------------
	public void ForcedRelayoutOfTree(iCS_EditorObject root) {
        ForEachRecursiveDepthFirst(root,
            obj=> {
            	if(!obj.IsVisibleInLayout) return;
				UpdateTransitionModuleName(obj);
				if(obj.IsNode) {
					obj.LayoutNode(iCS_AnimationControl.None);
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
