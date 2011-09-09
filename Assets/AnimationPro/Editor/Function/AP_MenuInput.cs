using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuInput {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Input/GameController")]
    public static void AddNodeGameInputGameController(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_GameController function= AP_GameController.CreateInstance<AP_GameController>("", parent);
        function.SetInitialPosition(context.GraphPosition);
        AP_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Input/GameController", true)]
    public static bool ValidateAddNodeInputGameController(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
