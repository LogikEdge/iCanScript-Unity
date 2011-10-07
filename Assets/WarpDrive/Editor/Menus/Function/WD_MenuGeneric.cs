//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//public class WD_MenuGeneric {
//
//    // ----------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Module/Generic/Module")]
//    public static void AddNodeModule(MenuCommand command) {
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_EditorObject parent= context.SelectedObject;
//        context.EditorObjects.CreateInstance<WD_Module>("", parent.InstanceId, context.GraphPosition);
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Module/Generic/Module", true)]
//    public static bool ValidateAddNodeModule(MenuCommand command) {
//        return WD_MenuUtilities.ValidateAddNode(command);
//    }
//
//}
//