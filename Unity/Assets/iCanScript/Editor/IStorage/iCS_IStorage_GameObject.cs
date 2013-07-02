using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {

    // ----------------------------------------------------------------------
    // Scans through the behaviour
    // Returns the next available port index.
    public int UpdateBehaviourInputPorts(iCS_EditorObject node, int portIdx) {
        string portName            = "gameObject";
        Type valueType             = typeof(GameObject);
        iCS_ObjectTypeEnum portType= iCS_ObjectTypeEnum.InFixPort;
        if(!DoesPortExist(node, portName, valueType, portType)) {
    	    var goPort= CreatePort(portName, node.InstanceId, valueType, portType);
            goPort.PortIndex= portIdx++;
            goPort.InitialPortValue= Storage.gameObject;            
        }
        return portIdx;
    }
    // ----------------------------------------------------------------------
    public bool DoesPortExist(iCS_EditorObject node, string portName, Type valueType, iCS_ObjectTypeEnum portType) {
        return node.UntilMatchingChild(p=> p.Name == portName && p.ObjectType == portType && p.RuntimeType == valueType);
    }
    // ----------------------------------------------------------------------
    public int NextAvailablePortIdx(int startingPortId= 0) {
        return ++startingPortId;
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject[] GetCurrentFixPorts(iCS_EditorObject node) {
        var fixPorts= new List<iCS_EditorObject>();
        node.ForEachChildPort(p => { if(p.IsFixPort) fixPorts.Add(p); });
        return fixPorts.ToArray();
    }
}
