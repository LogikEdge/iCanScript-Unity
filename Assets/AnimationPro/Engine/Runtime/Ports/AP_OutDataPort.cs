using UnityEngine;
using System.Collections;
using System.Reflection;

public class AP_OutDataPort : AP_DataPort {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public static AP_OutDataPort CreateInstance(string portName, AP_Node parent) {
        AP_OutDataPort instance= CreateInstance<AP_OutDataPort>();
        instance.Init(portName, parent);
        return instance;
    }
    // ----------------------------------------------------------------------
    protected AP_OutDataPort Init(string thePortName, AP_Node theParent) {
        base.Init(thePortName, theParent, DirectionEnum.In);
        Edge= EdgeEnum.Right;

        // Allow streams to also be used as non-stream ports.
        if(IsStream) {
            FieldInfo fieldInfo= Parent.GetType().GetField(Name);
            System.Type fieldType= fieldInfo.FieldType;
            System.Array array= fieldInfo.GetValue(Parent) as System.Array;
            if(array == null || array.Length == 0) fieldInfo.SetValue(Parent, System.Array.CreateInstance(fieldType.GetElementType(), 1));
        }
        return this;
    }

}
