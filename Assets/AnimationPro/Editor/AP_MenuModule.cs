using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuModule {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Add State Chart")]
    public static void AddStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Module module= context.SelectedObject as AP_Module;
        AP_StateChart stateChart= AP_StateChart.CreateInstance<AP_StateChart>("", module);
        stateChart.SetInitialPosition(context.GraphPosition);
        // Add initial state.
        AP_State state= AP_State.CreateInstance<AP_State>("", stateChart);
        stateChart.EntryState= state;        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Add State Chart", true)]
    public static bool ValidateAddStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Module module= context.SelectedObject as AP_Module;
        if(module == null) return false;
        return true;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Module module= context.SelectedObject as AP_Module;
        if(EditorUtility.DisplayDialog("Deleting Module", "Are you sure you want to delete module: "+module.NameOrTypeName+"?", "Delete", "Cancel")) {
            module.Dealloc();
        }                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        return context.SelectedObject is AP_Module;
    }

}
