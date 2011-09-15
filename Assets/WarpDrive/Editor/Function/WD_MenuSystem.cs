using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuSystem {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/System/FrameTime")]
    public static void AddNodeGameTimeFrameTime(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Time function= WD_Time.CreateInstance<WD_Time>("", parent);
        function.SetInitialPosition(context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/System/FrameTime", true)]
    public static bool ValidateAddNodeTimeFrameTime(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
