using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuPort {

//    // ----------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Port/Add Input")]
//    public static void AddInputPort(MenuCommand command) {    
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_Node selectedObject= context.SelectedObject as WD_Node;
//        WD_VirtualDataPort.CreateInstance("", selectedObject, WD_DataPort.DirectionEnum.In);
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Port/Add Input",true)]
//    public static bool ValidateAddInputPort(MenuCommand command) {
//        return WD_MenuUtilities.ValidateAddPort(command);
//    }
//
//    // ----------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Port/Add Output")]
//    public static void AddOutputPort(MenuCommand command) {    
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_Node selectedObject= context.SelectedObject as WD_Node;
//        WD_VirtualDataPort.CreateInstance("", selectedObject, WD_DataPort.DirectionEnum.Out);
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Port/Add Output",true)]
//    public static bool ValidateAddOutputPort(MenuCommand command) {
//        return WD_MenuUtilities.ValidateAddPort(command);
//    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Port/Create Property/Float")]
    public static void PortCreatePropertyFloat(MenuCommand command) {    
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_Port selectedObject= context.SelectedObject as WD_Port;
        /*
            TODO To Be Completed...
        */
    }
    [MenuItem("CONTEXT/WarpDrive/Port/Create Property/Float",true)]
    public static bool ValidatePortCreatePropertyFloat(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_DataPort selectedObject= context.SelectedObject as WD_DataPort;
        if(selectedObject != null && selectedObject.Source == null) {
            return true;
        }
        return false;
    }

}
