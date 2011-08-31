using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuModule {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Module/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Module module= context.SelectedObject as AP_Module;
        if(EditorUtility.DisplayDialog("Deleting Module", "Are you sure you want to delete module: "+module.NameOrTypeName+"?", "Delete", "Cancel")) {
            module.Dealloc();
        }                
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Module/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        return context.SelectedObject is AP_Module;
    }

}
