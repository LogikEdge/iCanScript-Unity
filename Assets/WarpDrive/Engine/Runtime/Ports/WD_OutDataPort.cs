using UnityEngine;
using System.Collections;
using System.Reflection;

public sealed class WD_OutDataPort : WD_DataPort {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public static WD_OutDataPort CreateInstance(string portName, WD_Node parent) {
        WD_OutDataPort instance=  CreateInstance<WD_OutDataPort>();
        instance.Init(portName, parent);
        return instance;
    }
    // ----------------------------------------------------------------------
    new WD_OutDataPort Init(string thePortName, WD_Node theParent) {
        base.Init(thePortName, theParent);

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
