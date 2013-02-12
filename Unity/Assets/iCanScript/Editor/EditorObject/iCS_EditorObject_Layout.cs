using UnityEngine;
using System.Collections;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Accessors ============================================================
    public Vector2 LayoutSize {
		get {
			if(!IsVisibleInLayout) return Vector2.zero;
            return myLayoutSize;
		}
		set {
            // Avoid propagating change if we did not change size
            if(Math3D.IsEqual(myLayoutSize, value)) return;
            myLayoutSize= value;
            if(IsNode && !IsIconizedInLayout && IsVisibleInLayout) {
                LayoutPorts();
                if(IsParentValid) {
                    Parent.IsDirty= true;
                }
            }
		}
	}
}

