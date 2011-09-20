using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_Command {
    public enum CommandTypeEnum { NOP, Add, Remove, Replace };
    public CommandTypeEnum  CommandType= CommandTypeEnum.NOP;
    public int              InstanceId = -1;
    public int              ParentId   = -1;
    public string           QualifiedType= "";
    public string           Name= "";
    public Rect             Position= new Rect(0,0,0,0);
    public int              PortSource= -1;

    public WD_Command(WD_Object obj) {
        ParentId= obj.Parent != null ? obj.Parent.InstanceId : -1;
        Type t= obj.GetType();
        QualifiedType= t.AssemblyQualifiedName;
        Name= obj.name;
        obj.Case<WD_Node, WD_Port>(
            (node) => { Position= node.Position; },
            (port) => {
                Position.x= port.LocalPosition.x;
                Position.y= port.LocalPosition.y;
                port.ExecuteIf<WD_DataPort>(
                    (dataPort) => { if(dataPort.Source != null) PortSource= dataPort.Source.InstanceId; }
                );
                
            }
        );
    }
}
