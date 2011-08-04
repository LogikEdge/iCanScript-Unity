using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuCommon {
    // ----------------------------------------------------------------------
    public static void DeleteObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        selectedObject.Case<AP_Port, AP_Node, AP_State>(
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
            (state)=> {
                if(EditorUtility.DisplayDialog("Deleting State", "Are you sure you want to delete state: "+state.NameOrTypeName+" and all of its children?", "Delete", "Cancel")) {
                    state.Dealloc();
                }                                
            },
            (unknown)=> { Debug.Log("Unknown type"); }
        );
    }
    public static bool ValidateDeleteObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        
        // For now we can't delete anything on the Top node.
        if(selectedObject.IsTop) return false;

        // Should we start a port deletion?
        bool deletionAllowed= false;
        selectedObject.Case<AP_Port, AP_Node, AP_State>(
            (port)   => { deletionAllowed= port.Parent is AP_Module; },
            (node)   => { deletionAllowed= node.Parent is AP_Module; },
            (state)  => { deletionAllowed= state.Parent is AP_State; },
            (unknown)=> { Debug.Log("Unknown type"); }
        );
        return deletionAllowed;
    }

    // ----------------------------------------------------------------------
    public static void SelectDisplayRoot(MenuCommand command) {
        
    }
    public static bool ValidateSelectDisplayRoot(MenuCommand command) {
        return false;
    }
}
