using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (iCS_Library))]
public class iCS_InspectorLibrary : iCS_Inspector {
    // ---------------------------------------------------------------
    protected override iCS_Editor GetEditor() {
        return iCS_EditorProxy.GetEditor();
    }
}
