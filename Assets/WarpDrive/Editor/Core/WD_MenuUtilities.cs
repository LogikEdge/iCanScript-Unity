using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuUtilities {

    // ----------------------------------------------------------------------
    public static bool ValidateAddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject.IsRuntimeA<WD_StateChart>() || context.SelectedObject.IsRuntimeA<WD_State>();
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddPort(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject.IsRuntimeA<WD_Module>();
    }
    // ----------------------------------------------------------------------
    public static bool ValidateAddNode(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject.IsRuntimeA<WD_Node>();
    }
}
