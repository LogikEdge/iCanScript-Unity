using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_Debug {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Module/Debug/Log")]
    public static void AddNodeDebugLog(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Log function= AP_Log.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Module/Debug/Log", true)]
    public static bool ValidateAddNodeDebugLog(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
