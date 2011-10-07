//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//public class WD_MenuInput {
//
//    // ----------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Module/Input/GameController")]
//    public static void AddNodeGameInputGameController(MenuCommand command) {
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_EditorObject parent= context.SelectedObject;
//        context.EditorObjects.CreateInstance<WD_GameController>("", parent.InstanceId, context.GraphPosition);
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Module/Input/GameController", true)]
//    public static bool ValidateAddNodeInputGameController(MenuCommand command) {
//        return WD_MenuUtilities.ValidateAddNode(command);
//    }
//
//}
//