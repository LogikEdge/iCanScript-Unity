using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuDelete {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        selectedObject.Case<WD_Port, WD_Node>(
            (port)=> {
                if(EditorUtility.DisplayDialog("Deleting Port", "Are you sure you want to delete port: "+port.NameOrTypeName+"?", "Delete", "Cancel")) {
                    port.Dealloc();
                }
            },
            (node)=> {
                if(EditorUtility.DisplayDialog("Deleting Node", "Are you sure you want to delete node: "+node.NameOrTypeName+"?", "Delete", "Cancel")) {
                    node.Dealloc();
                }                
            },
            (unknown)=> { Debug.Log("Unknown type"); }
        );
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        
        // For now we can't delete anything on the Top node.
        if(selectedObject.IsTop) return false;

        // Should we start a port deletion?
        bool deletionAllowed= false;
        selectedObject.Case<WD_Port, WD_Node>(
            (port)   => { deletionAllowed= port.Parent is WD_Module; },
            (node)   => { deletionAllowed= node.Parent is WD_Module; },
            (unknown)=> { Debug.Log("Unknown type"); }
        );
        return deletionAllowed;
    }

}
