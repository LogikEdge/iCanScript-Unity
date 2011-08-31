using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuUtilities {

    // ----------------------------------------------------------------------
    public static bool ValidateAddState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        return context.SelectedObject is AP_StateChart || context.SelectedObject is AP_State;
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddPort(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node selectedObject= context.SelectedObject as AP_Module;
        return selectedObject != null;
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddNode(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node selectedObject= context.SelectedObject as AP_Node;
        return selectedObject != null;
    }
}
