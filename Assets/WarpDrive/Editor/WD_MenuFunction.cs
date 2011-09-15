using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuFunction {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Function/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Function function= context.SelectedObject as WD_Function;
        if(EditorUtility.DisplayDialog("Deleting Function", "Are you sure you want to delete function: "+function.NameOrTypeName+"?", "Delete", "Cancel")) {
            function.Dealloc();
        }                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Function/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject is WD_Function;
    }

}
