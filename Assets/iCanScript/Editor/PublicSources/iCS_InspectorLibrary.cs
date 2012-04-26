using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (iCS_Library))]
public class iCS_InspectorLibrary : iCS_Inspector {
    // ---------------------------------------------------------------
    protected override iCS_GraphEditor GetEditor() {
        return iCS_GraphEditorProxy.GetEditor();
    }
}
