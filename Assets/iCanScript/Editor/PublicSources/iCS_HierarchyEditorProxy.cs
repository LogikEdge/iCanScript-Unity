using UnityEngine;
using System.Collections;

public class iCS_HierarchyEditorProxy : iCS_HierarchyEditor {
    public static iCS_HierarchyEditor GetGraphTreeEditor() {
        return GetWindow(typeof(iCS_HierarchyEditorProxy), false, "iCS Hierarchy") as iCS_HierarchyEditorProxy;
    }
}
