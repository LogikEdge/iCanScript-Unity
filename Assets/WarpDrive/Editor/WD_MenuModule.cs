using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuModule {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Add State Chart")]
    public static void AddStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Module module= context.SelectedObject as WD_Module;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_StateChart stateChart= WD_StateChart.CreateInstance<WD_StateChart>("", module);
        editorObjects.SetInitialPosition(editorObjects[stateChart.InstanceId], context.GraphPosition);
        // Add initial state.
        WD_State state= WD_State.CreateInstance<WD_State>("", stateChart);
        stateChart.EntryState= state;        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Add State Chart", true)]
    public static bool ValidateAddStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Module module= context.SelectedObject as WD_Module;
        if(module == null) return false;
        return true;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Module module= context.SelectedObject as WD_Module;
        if(EditorUtility.DisplayDialog("Deleting Module", "Are you sure you want to delete module: "+module.NameOrTypeName+"?", "Delete", "Cancel")) {
            module.Dealloc();
        }                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject is WD_Module;
    }

}
