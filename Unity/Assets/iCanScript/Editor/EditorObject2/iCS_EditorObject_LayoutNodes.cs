using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Resolve collision between children then wrap node around the children.
    public void LayoutNodes() {
        UpdateNodeLayoutSize();
    }

}

