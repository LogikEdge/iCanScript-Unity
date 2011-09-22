using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuGeneric {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Generic/Module")]
    public static void AddNodeModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_Module function= WD_Module.CreateInstance<WD_Module>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Generic/Module", true)]
    public static bool ValidateAddNodeModule(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
