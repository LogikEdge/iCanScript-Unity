using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuDelete {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Delete")]
    public static void DeleteObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        selectedObject.Case<AP_Port, AP_Node>(
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
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Delete",true)]
    public static bool ValidateDeleteObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        
        // For now we can't delete anything on the Top node.
        if(selectedObject.IsTop) return false;

        // Should we start a port deletion?
        bool deletionAllowed= false;
        selectedObject.Case<AP_Port, AP_Node>(
            (port)   => { deletionAllowed= port.Parent is AP_Module; },
            (node)   => { deletionAllowed= node.Parent is AP_Module; },
            (unknown)=> { Debug.Log("Unknown type"); }
        );
        return deletionAllowed;
    }

}
