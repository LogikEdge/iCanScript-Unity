using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuStateChart {

    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/StateChart/Add State")]
    public static void AddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_EditorObject parent= context.SelectedObject;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_State>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/StateChart/Add State", true)]
    public static bool ValidateAddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject.IsRuntimeA<WD_StateChart>();
    }

    // ======================================================================
    // COMMON AREA
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/StateChart/Delete")]
    public static void Delete(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_EditorObject stateChart= context.SelectedObject;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        if(EditorUtility.DisplayDialog("Deleting State Chart", "Are you sure you want to delete state chart: "+stateChart.NameOrTypeName+" and all of its children?", "Delete", "Cancel")) {
            editorObjects.DestroyInstance(stateChart);
        }                                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/StateChart/Delete", true)]
    public static bool ValidateDelete(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject.IsRuntimeA<WD_StateChart>();
    }
}
