using UnityEngine;
using System.Collections;
using System.Reflection;

public class AP_InDataPort : AP_DataPort {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public static AP_InDataPort CreateInstance(string portName, AP_Node parent) {
        AP_InDataPort instance= CreateInstance<AP_InDataPort>();
        instance.Init(portName, parent);
        return instance;
    }
    // ----------------------------------------------------------------------
    protected AP_InDataPort Init(string thePortName, AP_Node theParent) {
        base.Init(thePortName, theParent, DirectionEnum.In);
        Edge= EdgeEnum.Left;

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
