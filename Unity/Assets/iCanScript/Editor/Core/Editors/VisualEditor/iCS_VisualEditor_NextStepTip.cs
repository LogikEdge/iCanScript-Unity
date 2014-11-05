using UnityEngine;
using UnityEditor;
using System.Collections;


public partial class iCS_VisualEditor : iCS_EditorBase {
    void ShowNextStepHelp(Rect r) {
        if(Selection.activeGameObject == null) {
            Debug.Log("No game object selected");
        }
    }
}
