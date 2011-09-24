using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuBoolean {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/CBoolean")]
    public static void AddNodeCBoolean(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_CBoolean>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/CBoolean", true)]
    public static bool ValidateAddNodeCBoolean(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }
    
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Not")]
    public static void AddNodeNot(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Not>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Not", true)]
    public static bool ValidateAddNodeNot(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }
    
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/And")]
    public static void AddNodeAnd(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_And>("", parent.InstanceId, context.GraphPosition);
        WD_And function= WD_And.CreateInstance<WD_And>("", parent);
        editorObjects.SetInitialPosition(editorObjects[function.InstanceId], context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/And", true)]
    public static bool ValidateAddNodeAnd(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }
    
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Or")]
    public static void AddNodeOr(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Or>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Or", true)]
    public static bool ValidateAddNodeOr(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }
    
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Xor")]
    public static void AddNodeXor(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Xor>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Xor", true)]
    public static bool ValidateAddNodeXor(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Decoder2")]
    public static void AddNodeDecoder2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Decoder2>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Decoder2", true)]
    public static bool ValidateAddNodeDecoder2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Decoder4")]
    public static void AddNodeDecoder4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Decoder4>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Decoder4", true)]
    public static bool ValidateAddNodeDecoder4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
