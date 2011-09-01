using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuGameObject {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/GameObject/CGameObject")]
    public static void AddNodeCGameObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_CGameObject function= AP_CGameObject.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/GameObject/CGameObject", true)]
    public static bool ValidateAddNodeCGameObject(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/GameObject/Transform")]
    public static void AddNodeGameObjectTransform(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Transform function= AP_Transform.CreateInstance("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/GameObject/Transform", true)]
    public static bool ValidateAddNodeGameObjectTransform(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
