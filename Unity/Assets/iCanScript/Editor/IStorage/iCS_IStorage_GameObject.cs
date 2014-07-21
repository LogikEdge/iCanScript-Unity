using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ======================================================================
    // Nested Types
    // ----------------------------------------------------------------------
    public struct PortInfo {
        public string               Name;
        public Type                 ValueType;
        public iCS_ObjectTypeEnum   PortType;
        public object               InitialValue;
        public PortInfo(string name, Type valueType, iCS_ObjectTypeEnum portType, object initialValue) {
            Name        = name;
            ValueType   = valueType;
            PortType    = portType;
            InitialValue= initialValue;
        }
    }
    
    // ----------------------------------------------------------------------
    // Scans through the behaviour
    // Returns the next available port index.
    public void UpdateBehaviourMessagePorts(iCS_EditorObject node) {
        var neededPorts= BuildListOfPortInfoForBehaviourMessage(node.Parent);
        var changed= CleanupExistingProposedPorts(node, neededPorts);
        changed |= BuildMissingPorts(node, neededPorts);
    }

    // ----------------------------------------------------------------------
    // Creates the missing fix ports
    public bool BuildMissingPorts(iCS_EditorObject node, PortInfo[] neededPorts) {
        bool changed= false;
        foreach(var pi in neededPorts) {
            if(!DoesPortExist(node, pi.Name, pi.ValueType, pi.PortType)) {
        	    var port= CreatePort(pi.Name, node.InstanceId, pi.ValueType, pi.PortType);
				port.IsNameEditable= false;
                port.PortValue= pi.InitialValue;            
                changed= true;
            }
        }
        return changed;
    }
    // ----------------------------------------------------------------------
    // Removes the non needed fix ports.
    public bool CleanupExistingProposedPorts(iCS_EditorObject node, PortInfo[] neededPorts) {
        bool changed= false;
        var currentProposedPorts= GetCurrentProposedPorts(node);
        foreach(var cp in currentProposedPorts) {
            bool found= false;
            foreach(var np in neededPorts) {
                if(np.Name == cp.Name && np.PortType == cp.ObjectType && np.ValueType == cp.RuntimeType) {
                    found= true;
                }
            }
            if(!found) {
                DestroyInstance(cp);
                changed= true;
            }
        }
        return changed;
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
    public iCS_EditorObject[] GetCurrentProposedPorts(iCS_EditorObject node) {
        var proposedPorts= new List<iCS_EditorObject>();
        node.ForEachChildPort(p => { if(p.IsProposedDataPort) proposedPorts.Add(p); });
        return proposedPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    public PortInfo[] BuildListOfPortInfoForBehaviourMessage(iCS_EditorObject behaviour) {
        var go= behaviour.iCSMonoBehaviour.gameObject;
        var portInfos= new List<PortInfo>();        
        portInfos.Add(new PortInfo("gameObject", typeof(GameObject), iCS_ObjectTypeEnum.InProposedDataPort, go));
        return BuildListOfPortInfoForGameObject(go, portInfos);
    }
    // ----------------------------------------------------------------------
    public PortInfo[] BuildListOfPortInfoForGameObject(GameObject gameObject, List<PortInfo> portInfos= null) {
        if(portInfos == null) {
            portInfos= new List<PortInfo>();
        }
        foreach(var component in gameObject.GetComponents<Component>()) {
            if(component == null || component is iCS_VisualScriptImp || component is iCS_MonoBehaviourImp) {
                continue;
            }
            var componentType= component.GetType();
            portInfos.Add(new PortInfo(componentType.Name, componentType, iCS_ObjectTypeEnum.InProposedDataPort, component));                
        }
        return portInfos.ToArray();
    }
}
