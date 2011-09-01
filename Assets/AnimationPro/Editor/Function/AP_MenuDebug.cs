using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_Debug {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/Log")]
    public static void AddNodeDebugLog(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Log function= AP_Log.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/Log", true)]
    public static bool ValidateAddNodeDebugLog(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/LogError")]
    public static void AddNodeDebugLogError(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LogError function= AP_LogError.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/LogError", true)]
    public static bool ValidateAddNodeDebugLogError(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/LogWarning")]
    public static void AddNodeDebugLogWarning(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LogWarning function= AP_LogWarning.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Debug/LogWarning", true)]
    public static bool ValidateAddNodeDebugLogWarning(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
