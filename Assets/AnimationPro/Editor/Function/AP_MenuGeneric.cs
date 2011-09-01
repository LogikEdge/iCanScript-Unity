using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuGeneric {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Generic/Module")]
    public static void AddNodeModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Module function= AP_Module.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Generic/Module", true)]
    public static bool ValidateAddNodeModule(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
