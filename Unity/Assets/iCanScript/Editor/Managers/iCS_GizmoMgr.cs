using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_GizmoMgr {
    // ---------------------------------------------------------------------------------
    [DrawGizmo(GizmoType.NotSelected | GizmoType.Selected)]
    public static void DrawGizmos(iCS_Storage storage, GizmoType gizmoType) {
        var go= storage.gameObject;
        var p= go.transform.position;
        Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
        if(go.renderer != null) {
            for(int intensity= 5; intensity >= 0; --intensity) {
                Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);                
            }
        }
     }
}
