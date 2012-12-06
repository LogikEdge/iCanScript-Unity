using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    void InitialLayout() {
        // Port position will be initialized by the parent node.
        if(IsPort) return;
        // Wait until node becomes visible.
        if(!IsVisible) return;
        // Iconized nodes will be initialized by the parent node.
        if(IsIconized) return;
        // Folded nodes must initialize size and layout its ports using position ratio.
        if(IsFolded) {
            
            return;
        }
        
    }
}
