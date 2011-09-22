using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuAnimation {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Blend")]
    public static void AddNodeAnimBlend(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_AnimBlend function= WD_AnimBlend.CreateInstance<WD_AnimBlend>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Blend", true)]
    public static bool ValidateAddNodeAnimBlend(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Select")]
    public static void AddNodeAnimSelect(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_AnimSelect function= WD_AnimSelect.CreateInstance<WD_AnimSelect>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Select", true)]
    public static bool ValidateAddNodeAnimSelect(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/State")]
    public static void AddNodeAnimState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_AnimState function= WD_AnimState.CreateInstance<WD_AnimState>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/State", true)]
    public static bool ValidateAddNodeAnimState(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Diversity")]
    public static void AddNodeAnimDiversity(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        WD_AnimDiversity function= WD_AnimDiversity.CreateInstance<WD_AnimDiversity>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Diversity", true)]
    public static bool ValidateAddNodeAnimDiversity(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
