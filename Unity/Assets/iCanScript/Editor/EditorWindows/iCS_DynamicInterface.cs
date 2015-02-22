using UnityEngine;
using System.Collections;

public static class iCS_DynamicInterface {

    public static iCS_VisualScript AddVisualScript(GameObject go) {
        return go.AddComponent<iCS_VisualScript>();
    }
    public static iCS_Behaviour AddBehaviour(GameObject go) {
        return go.AddComponent<iCS_Behaviour>();
    }
    public static iCS_Library AddLibrary(GameObject go) {
        return go.AddComponent<iCS_Library>();
    }
    
}
