using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuGameObject {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/GameObject/CGameObject")]
    public static void AddNodeCGameObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_EditorObject parent= context.SelectedObject;
        context.EditorObjects.CreateInstance<WD_LogWarning>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/GameObject/CGameObject", true)]
    public static bool ValidateAddNodeCGameObject(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/GameObject/Transform")]
    public static void AddNodeGameObjectTransform(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_EditorObject parent= context.SelectedObject;
        context.EditorObjects.CreateInstance<WD_Transform>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/GameObject/Transform", true)]
    public static bool ValidateAddNodeGameObjectTransform(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
