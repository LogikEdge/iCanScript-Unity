using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_Debug {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/Log")]
    public static void AddNodeDebugLog(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_EditorObject parent= context.SelectedObject;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Log>("", parent.InstanceId, context.GraphPosition);
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
        WD_EditorObject parent= context.SelectedObject;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_LogError>("", parent.InstanceId, context.GraphPosition);
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
        WD_EditorObject parent= context.SelectedObject;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_LogWarning>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Debug/LogWarning", true)]
    public static bool ValidateAddNodeDebugLogWarning(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
