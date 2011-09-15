using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuVector {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Const Int")]
    public static void AddNodeCInt(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ConstInt function= WD_ConstInt.CreateInstance<WD_ConstInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Const Int", true)]
    public static bool ValidateAddNodeCInt(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Const Float")]
    public static void AddNodeCFloat(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ConstFloat function= WD_ConstFloat.CreateInstance<WD_ConstFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Const Float", true)]
    public static bool ValidateAddNodeCFloat(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector2")]
    public static void AddNodeScaleVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ScaleVector2 function= WD_ScaleVector2.CreateInstance<WD_ScaleVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector2", true)]
    public static bool ValidateAddNodeScaleVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector3")]
    public static void AddNodeScaleVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ScaleVector3 function= WD_ScaleVector3.CreateInstance<WD_ScaleVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector3", true)]
    public static bool ValidateAddNodeScaleVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector4")]
    public static void AddNodeScaleVector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ScaleVector4 function= WD_ScaleVector4.CreateInstance<WD_ScaleVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ScaleVector4", true)]
    public static bool ValidateAddNodeScaleVector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector2")]
    public static void AddNodeScale2Vector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Scale2Vector2 function= WD_Scale2Vector2.CreateInstance<WD_Scale2Vector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector2", true)]
    public static bool ValidateAddNodeScale2Vector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector3")]
    public static void AddNodeScale2Vector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Scale2Vector3 function= WD_Scale2Vector3.CreateInstance<WD_Scale2Vector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector3", true)]
    public static bool ValidateAddNodeScale2Vector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector4")]
    public static void AddNodeScale2Vector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Scale2Vector4 function= WD_Scale2Vector4.CreateInstance<WD_Scale2Vector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Scale2Vector4", true)]
    public static bool ValidateAddNodeScale2Vector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddInt")]
    public static void AddNodeAddInt(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_AddInt function= WD_AddInt.CreateInstance<WD_AddInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddInt", true)]
    public static bool ValidateAddNodeAddInt(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddFloat")]
    public static void AddNodeAddFloat(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_AddFloat function= WD_AddFloat.CreateInstance<WD_AddFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddFloat", true)]
    public static bool ValidateAddNodeAddFloat(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector2")]
    public static void AddNodeAddVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_AddVector2 function= WD_AddVector2.CreateInstance<WD_AddVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector2", true)]
    public static bool ValidateAddNodeAddVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector3")]
    public static void AddNodeAddVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_AddVector3 function= WD_AddVector3.CreateInstance<WD_AddVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector3", true)]
    public static bool ValidateAddNodeAddVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector4")]
    public static void AddNodeAddVector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_AddVector4 function= WD_AddVector4.CreateInstance<WD_AddVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/AddVector4", true)]
    public static bool ValidateAddNodeAddVector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/CosWave")]
    public static void AddNodeCosWave(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_CosWave function= WD_CosWave.CreateInstance<WD_CosWave>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/CosWave", true)]
    public static bool ValidateAddNodeCosWave(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/SinWave")]
    public static void AddNodeSinWave(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_SinWave function= WD_SinWave.CreateInstance<WD_SinWave>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/SinWave", true)]
    public static bool ValidateAddNodeSinWave(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector2")]
    public static void AddNodeToVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ToVector2 function= WD_ToVector2.CreateInstance<WD_ToVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector2", true)]
    public static bool ValidateAddNodeToVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector3")]
    public static void AddNodeToVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ToVector3 function= WD_ToVector3.CreateInstance<WD_ToVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector3", true)]
    public static bool ValidateAddNodeToVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector4")]
    public static void AddNodeToVector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_ToVector4 function= WD_ToVector4.CreateInstance<WD_ToVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/ToVector4", true)]
    public static bool ValidateAddNodeToVector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector2")]
    public static void AddNodeFromVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_FromVector2 function= WD_FromVector2.CreateInstance<WD_FromVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector2", true)]
    public static bool ValidateAddNodeFromVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector3")]
    public static void AddNodeFromVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_FromVector3 function= WD_FromVector3.CreateInstance<WD_FromVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector3", true)]
    public static bool ValidateAddNodeFromVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector4")]
    public static void AddNodeFromVector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_FromVector4 function= WD_FromVector4.CreateInstance<WD_FromVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/FromVector4", true)]
    public static bool ValidateAddNodeFromVector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpInt")]
    public static void AddNodeLerpInt(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LerpInt function= WD_LerpInt.CreateInstance<WD_LerpInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpInt", true)]
    public static bool ValidateAddNodeLerpInt(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpFloat")]
    public static void AddNodeLerpFloat(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LerpFloat function= WD_LerpFloat.CreateInstance<WD_LerpFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpFloat", true)]
    public static bool ValidateAddNodeLerpFloat(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector2")]
    public static void AddNodeLerpVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LerpVector2 function= WD_LerpVector2.CreateInstance<WD_LerpVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector2", true)]
    public static bool ValidateAddNodeLerpVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector3")]
    public static void AddNodeLerpVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LerpVector3 function= WD_LerpVector3.CreateInstance<WD_LerpVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector3", true)]
    public static bool ValidateAddNodeLerpVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector4")]
    public static void AddNodeLerpVector4(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_LerpVector4 function= WD_LerpVector4.CreateInstance<WD_LerpVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/LerpVector4", true)]
    public static bool ValidateAddNodeLerpVector4(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Random")]
    public static void AddNodeRandom(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_Random function= WD_Random.CreateInstance<WD_Random>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/Random", true)]
    public static bool ValidateAddNodeRandom(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/RandomVector2")]
    public static void AddNodeRandomVector2(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_RandomVector2 function= WD_RandomVector2.CreateInstance<WD_RandomVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/RandomVector2", true)]
    public static bool ValidateAddNodeRandomVector2(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/RandomVector3")]
    public static void AddNodeRandomVector3(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_RandomVector3 function= WD_RandomVector3.CreateInstance<WD_RandomVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Module/Math3D/RandomVector3", true)]
    public static bool ValidateAddNodeRandomVector3(MenuCommand command) {
        return WD_MenuUtilities.ValidateAddNode(command);
    }

}
