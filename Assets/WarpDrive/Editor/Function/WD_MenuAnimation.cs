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
        editorObjects.CreateInstance<WD_AnimBlend>("", parent.InstanceId, context.GraphPosition);
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
        editorObjects.CreateInstance<WD_AnimSelect>("", parent.InstanceId, context.GraphPosition);
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
        editorObjects.CreateInstance<WD_AnimState>("", parent.InstanceId, context.GraphPosition);
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
        editorObjects.CreateInstance<WD_AnimDiversity>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Animation/Diversity", true)]
    public static bool ValidateAddNodeAnimDiversity(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
