using UnityEngine;
using System.Collections;

public static class iCS_MenuInterface {

    public static void AddVisualScript(GameObject go) {
        go.AddComponent<iCS_VisualScript>();
    }
    public static void AddBehaviour(GameObject go) {
        go.AddComponent<iCS_Behaviour>();
    }
    
}
