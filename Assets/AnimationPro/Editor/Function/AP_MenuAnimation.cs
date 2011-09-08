using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuAnimation {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Blend")]
    public static void AddNodeAnimBlend(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AnimBlend function= AP_AnimBlend.CreateInstance<AP_AnimBlend>("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Blend", true)]
    public static bool ValidateAddNodeAnimBlend(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Select")]
    public static void AddNodeAnimSelect(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AnimSelect function= AP_AnimSelect.CreateInstance<AP_AnimSelect>("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Select", true)]
    public static bool ValidateAddNodeAnimSelect(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/State")]
    public static void AddNodeAnimState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AnimState function= AP_AnimState.CreateInstance<AP_AnimState>("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/State", true)]
    public static bool ValidateAddNodeAnimState(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Diversity")]
    public static void AddNodeAnimDiversity(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AnimDiversity function= AP_AnimDiversity.CreateInstance<AP_AnimDiversity>("", parent);
        function.SetInitialPosition(context.GraphPosition);
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Animation/Diversity", true)]
    public static bool ValidateAddNodeAnimDiversity(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }



}
