using UnityEngine;
using UnityEditor;
using System.Collections;


public partial class iCS_VisualEditor : iCS_EditorBase {
    // ----------------------------------------------------------------------
    void ShowNextStepHelp() {
        if(Selection.activeGameObject == null) {
            ShowNextStepHelp("Select a Game Object");
        }
    }
    // ----------------------------------------------------------------------
    void ShowNextStepHelp(string message) {
        var r= position; /*WindowRectForGraph;*/
        GUI.Label(r, message);
//        Debug.Log("Message=> "+message+" r=> "+r);
    }
}
