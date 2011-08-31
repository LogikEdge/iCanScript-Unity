using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuPort {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Add Input")]
    public static void AddInputPort(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node selectedObject= context.SelectedObject as AP_Node;
        AP_VirtualDataPort.CreateInstance("", selectedObject, AP_DataPort.DirectionEnum.In);
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Add Input",true)]
    public static bool ValidateAddInputPort(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddPort(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Add Output")]
    public static void AddOutputPort(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node selectedObject= context.SelectedObject as AP_Node;
        AP_VirtualDataPort.CreateInstance("", selectedObject, AP_DataPort.DirectionEnum.Out);
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Add Output",true)]
    public static bool ValidateAddOutputPort(MenuCommand command) {
        return AP_MenuUtilities.ValidateAddPort(command);
    }

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Create Property/Float")]
    public static void PortCreatePropertyFloat(MenuCommand command) {    
//        AP_MenuContext context= command.context as AP_MenuContext;
//        AP_Port selectedObject= context.SelectedObject as AP_Port;
        /*
            TODO To Be Completed...
        */
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Port/Create Property/Float",true)]
    public static bool ValidatePortCreatePropertyFloat(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_DataPort selectedObject= context.SelectedObject as AP_DataPort;
        if(selectedObject != null && selectedObject.Source == null) {
            return true;
        }
        return false;
    }

}
