using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Computes the size of the node.  We assume that all children have been
    // previously layed out prior to updating the node size.
    public Vector2 ComputeNodeDisplaySize() {
        return Vector2.zero;
    }
    
    // ----------------------------------------------------------------------
    // Returns the minimium height needed for the left / right ports.
    public float MinimumHeightForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            return nbOfPorts*iCS_EditorConfig.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimum width needed for the top / bottom ports.
    public float MinimumWidthForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfTopPorts, NbOfBottomPorts);
            return nbOfPorts*iCS_EditorConfig.MinimumPortSeparation;                                            
        }
    }
    
}
