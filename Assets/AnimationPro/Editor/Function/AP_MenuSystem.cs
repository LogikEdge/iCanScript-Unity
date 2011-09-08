using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuSystem {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/System/FrameTime")]
    public static void AddNodeGameTimeFrameTime(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Time function= AP_Time.CreateInstance<AP_Time>("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/System/FrameTime", true)]
    public static bool ValidateAddNodeTimeFrameTime(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
