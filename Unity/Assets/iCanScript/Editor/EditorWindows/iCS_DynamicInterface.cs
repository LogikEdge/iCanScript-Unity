using UnityEngine;
using System.Collections;

public static class iCS_DynamicInterface {

    public static iCS_VisualScript AddVisualScript(GameObject go) {
        return go.AddComponent<iCS_VisualScript>();
    }
    
}
