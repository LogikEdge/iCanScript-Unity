using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuUtilities {

    // ----------------------------------------------------------------------
    public static bool ValidateAddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject is WD_StateChart || context.SelectedObject is WD_State;
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddPort(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node selectedObject= context.SelectedObject as WD_Module;
        return selectedObject != null;
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddNode(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node selectedObject= context.SelectedObject as WD_Node;
        return selectedObject != null;
    }
}
