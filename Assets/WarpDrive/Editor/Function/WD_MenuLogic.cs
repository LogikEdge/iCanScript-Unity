using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuBoolean {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/CBoolean")]
    public static void AddNodeCBoolean(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_CBoolean function= WD_CBoolean.CreateInstance<WD_CBoolean>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_Not function= WD_Not.CreateInstance<WD_Not>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_And function= WD_And.CreateInstance<WD_And>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_Or function= WD_Or.CreateInstance<WD_Or>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_Xor function= WD_Xor.CreateInstance<WD_Xor>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_Decoder2 function= WD_Decoder2.CreateInstance<WD_Decoder2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
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
        WD_Decoder4 function= WD_Decoder4.CreateInstance<WD_Decoder4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Logic/Decoder4", true)]
    public static bool ValidateAddNodeDecoder4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
