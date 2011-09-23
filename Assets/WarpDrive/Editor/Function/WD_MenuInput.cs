using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuInput {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Input/GameController")]
    public static void AddNodeGameInputGameController(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_GameController>("", parent.InstanceId, context.GraphPosition);
//        WD_GameController function= WD_GameController.CreateInstance<WD_GameController>("", parent);
//        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Input/GameController", true)]
    public static bool ValidateAddNodeInputGameController(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
