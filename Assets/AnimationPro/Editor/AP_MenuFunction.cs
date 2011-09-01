using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuFunction {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Function/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Function function= context.SelectedObject as AP_Function;
        if(EditorUtility.DisplayDialog("Deleting Function", "Are you sure you want to delete function: "+function.NameOrTypeName+"?", "Delete", "Cancel")) {
            function.Dealloc();
        }                
    }
    [MenuItem("CONTEXT/AnimationPro/Function/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        return context.SelectedObject is AP_Function;
    }

}
