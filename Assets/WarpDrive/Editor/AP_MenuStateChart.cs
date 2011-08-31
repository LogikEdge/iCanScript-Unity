using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuStateChart {

    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/StateChart/Add State")]
    public static void AddState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_StateChart parent= context.SelectedObject as AP_StateChart;
        AP_State instance= AP_State.CreateInstance("", parent);
        instance.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/StateChart/Add State", true)]
    public static bool ValidateAddState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_StateChart stateChart= context.SelectedObject as AP_StateChart;
        return stateChart != null;
    }

    // ======================================================================
    // COMMON AREA
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/StateChart/Delete")]
    public static void Delete(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_StateChart stateChart= context.SelectedObject as AP_StateChart;
        if(EditorUtility.DisplayDialog("Deleting State Chart", "Are you sure you want to delete state chart: "+stateChart.NameOrTypeName+" and all of its children?", "Delete", "Cancel")) {
            stateChart.Dealloc();
        }                                
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/StateChart/Delete", true)]
    public static bool ValidateDelete(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_StateChart stateChart= context.SelectedObject as AP_StateChart;
        return stateChart is AP_StateChart;
    }
}
