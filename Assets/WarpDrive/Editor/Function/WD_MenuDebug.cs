using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_Debug {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/Log")]
    public static void AddNodeDebugLog(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Log function= WD_Log.CreateInstance<WD_Log>("", parent);
        function.SetInitialPosition(context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/Log", true)]
    public static bool ValidateAddNodeDebugLog(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/LogError")]
    public static void AddNodeDebugLogError(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LogError function= WD_LogError.CreateInstance<WD_LogError>("", parent);
        function.SetInitialPosition(context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/LogError", true)]
    public static bool ValidateAddNodeDebugLogError(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/LogWarning")]
    public static void AddNodeDebugLogWarning(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LogWarning function= WD_LogWarning.CreateInstance<WD_LogWarning>("", parent);
        function.SetInitialPosition(context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/LogWarning", true)]
    public static bool ValidateAddNodeDebugLogWarning(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
