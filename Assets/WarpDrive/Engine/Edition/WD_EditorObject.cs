using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public int              InstanceId = -1;
    public int              ParentId   = -1;
    public string           QualifiedType= "";
    public string           Name= "";
    public bool             IsDirty= true;
    public bool             IsVisible= true;
    public Rect             Position= new Rect(0,0,0,0);
    public int              PortSource= -1;

    // ======================================================================
    // Object Serialization
    // ----------------------------------------------------------------------
    public void Serialize(WD_Object obj, int id) {
        InstanceId= obj.InstanceId= id;
        ParentId= obj.Parent != null ? obj.Parent.InstanceId : -1;
        Type t= obj.GetType();
        QualifiedType= t.AssemblyQualifiedName;
        Name= obj.name;
        obj.Case<WD_Node, WD_Port>(
            (node) => { Position= node.LocalPosition; },
            (port) => {
                Position.x= port.LocalPosition.x;
                Position.y= port.LocalPosition.y;
                port.ExecuteIf<WD_DataPort>(
                    (dataPort) => { if(dataPort.Source != null) PortSource= dataPort.Source.InstanceId; }
                );
                
            }
        );
    }
    // ----------------------------------------------------------------------
    public WD_Object Deserialize() {
        return null;
    }

}
