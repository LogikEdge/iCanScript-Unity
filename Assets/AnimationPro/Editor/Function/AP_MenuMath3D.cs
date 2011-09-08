using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuVector {
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Const Int")]
    public static void AddNodeCInt(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ConstInt function= AP_ConstInt.CreateInstance<AP_ConstInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Const Int", true)]
    public static bool ValidateAddNodeCInt(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Const Float")]
    public static void AddNodeCFloat(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ConstFloat function= AP_ConstFloat.CreateInstance<AP_ConstFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Const Float", true)]
    public static bool ValidateAddNodeCFloat(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector2")]
    public static void AddNodeScaleVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ScaleVector2 function= AP_ScaleVector2.CreateInstance<AP_ScaleVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector2", true)]
    public static bool ValidateAddNodeScaleVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector3")]
    public static void AddNodeScaleVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ScaleVector3 function= AP_ScaleVector3.CreateInstance<AP_ScaleVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector3", true)]
    public static bool ValidateAddNodeScaleVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector4")]
    public static void AddNodeScaleVector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ScaleVector4 function= AP_ScaleVector4.CreateInstance<AP_ScaleVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ScaleVector4", true)]
    public static bool ValidateAddNodeScaleVector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector2")]
    public static void AddNodeScale2Vector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Scale2Vector2 function= AP_Scale2Vector2.CreateInstance<AP_Scale2Vector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector2", true)]
    public static bool ValidateAddNodeScale2Vector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector3")]
    public static void AddNodeScale2Vector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Scale2Vector3 function= AP_Scale2Vector3.CreateInstance<AP_Scale2Vector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector3", true)]
    public static bool ValidateAddNodeScale2Vector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector4")]
    public static void AddNodeScale2Vector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Scale2Vector4 function= AP_Scale2Vector4.CreateInstance<AP_Scale2Vector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);                
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Scale2Vector4", true)]
    public static bool ValidateAddNodeScale2Vector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddInt")]
    public static void AddNodeAddInt(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AddInt function= AP_AddInt.CreateInstance<AP_AddInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddInt", true)]
    public static bool ValidateAddNodeAddInt(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddFloat")]
    public static void AddNodeAddFloat(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AddFloat function= AP_AddFloat.CreateInstance<AP_AddFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddFloat", true)]
    public static bool ValidateAddNodeAddFloat(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector2")]
    public static void AddNodeAddVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AddVector2 function= AP_AddVector2.CreateInstance<AP_AddVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector2", true)]
    public static bool ValidateAddNodeAddVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector3")]
    public static void AddNodeAddVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AddVector3 function= AP_AddVector3.CreateInstance<AP_AddVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector3", true)]
    public static bool ValidateAddNodeAddVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector4")]
    public static void AddNodeAddVector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_AddVector4 function= AP_AddVector4.CreateInstance<AP_AddVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/AddVector4", true)]
    public static bool ValidateAddNodeAddVector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/CosWave")]
    public static void AddNodeCosWave(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_CosWave function= AP_CosWave.CreateInstance<AP_CosWave>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/CosWave", true)]
    public static bool ValidateAddNodeCosWave(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/SinWave")]
    public static void AddNodeSinWave(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_SinWave function= AP_SinWave.CreateInstance<AP_SinWave>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/SinWave", true)]
    public static bool ValidateAddNodeSinWave(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector2")]
    public static void AddNodeToVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ToVector2 function= AP_ToVector2.CreateInstance<AP_ToVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector2", true)]
    public static bool ValidateAddNodeToVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector3")]
    public static void AddNodeToVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ToVector3 function= AP_ToVector3.CreateInstance<AP_ToVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector3", true)]
    public static bool ValidateAddNodeToVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector4")]
    public static void AddNodeToVector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_ToVector4 function= AP_ToVector4.CreateInstance<AP_ToVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/ToVector4", true)]
    public static bool ValidateAddNodeToVector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector2")]
    public static void AddNodeFromVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_FromVector2 function= AP_FromVector2.CreateInstance<AP_FromVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector2", true)]
    public static bool ValidateAddNodeFromVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector3")]
    public static void AddNodeFromVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_FromVector3 function= AP_FromVector3.CreateInstance<AP_FromVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector3", true)]
    public static bool ValidateAddNodeFromVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector4")]
    public static void AddNodeFromVector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_FromVector4 function= AP_FromVector4.CreateInstance<AP_FromVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/FromVector4", true)]
    public static bool ValidateAddNodeFromVector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpInt")]
    public static void AddNodeLerpInt(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LerpInt function= AP_LerpInt.CreateInstance<AP_LerpInt>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpInt", true)]
    public static bool ValidateAddNodeLerpInt(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpFloat")]
    public static void AddNodeLerpFloat(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LerpFloat function= AP_LerpFloat.CreateInstance<AP_LerpFloat>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpFloat", true)]
    public static bool ValidateAddNodeLerpFloat(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector2")]
    public static void AddNodeLerpVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LerpVector2 function= AP_LerpVector2.CreateInstance<AP_LerpVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector2", true)]
    public static bool ValidateAddNodeLerpVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector3")]
    public static void AddNodeLerpVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LerpVector3 function= AP_LerpVector3.CreateInstance<AP_LerpVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector3", true)]
    public static bool ValidateAddNodeLerpVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector4")]
    public static void AddNodeLerpVector4(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_LerpVector4 function= AP_LerpVector4.CreateInstance<AP_LerpVector4>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/LerpVector4", true)]
    public static bool ValidateAddNodeLerpVector4(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Random")]
    public static void AddNodeRandom(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Random function= AP_Random.CreateInstance<AP_Random>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/Random", true)]
    public static bool ValidateAddNodeRandom(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/RandomVector2")]
    public static void AddNodeRandomVector2(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_RandomVector2 function= AP_RandomVector2.CreateInstance<AP_RandomVector2>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/RandomVector2", true)]
    public static bool ValidateAddNodeRandomVector2(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/RandomVector3")]
    public static void AddNodeRandomVector3(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_RandomVector3 function= AP_RandomVector3.CreateInstance<AP_RandomVector3>("", parent);
        function.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Module/Math3D/RandomVector3", true)]
    public static bool ValidateAddNodeRandomVector3(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddNode(command);
    }

}
