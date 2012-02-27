using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (iCS_Behaviour))]
public class iCS_InspectorBehaviour : iCS_Inspector {
    // ---------------------------------------------------------------
    protected override iCS_Editor GetEditor() {
        return iCS_EditorProxy.GetEditor();
    }
}
