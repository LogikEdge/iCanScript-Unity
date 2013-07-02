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
    public int UpdateBehaviourMessagePorts(iCS_EditorObject node, int portIdx) {
        var neededPorts= BuildListOfPortInfoForBehaviourMessage(node.Parent);
        var changed= CleanupExistingFixPorts(node, neededPorts);
        changed |= BuildMissingPorts(node, neededPorts, portIdx);
        if(changed) node.LayoutNode();
        return portIdx;
    }

    // ----------------------------------------------------------------------
    // Creates the missing fix ports
    public bool BuildMissingPorts(iCS_EditorObject node, PortInfo[] neededPorts, int portIdx) {
        bool changed= false;
        foreach(var pi in neededPorts) {
            if(!DoesPortExist(node, pi.Name, pi.ValueType, pi.PortType)) {
        	    var port= CreatePort(pi.Name, node.InstanceId, pi.ValueType, pi.PortType);
                port.PortIndex= portIdx++;
                port.InitialPortValue= pi.InitialValue;            
                changed= true;
            }
        }
        myIsDirty|= changed;
        return changed;
    }
    // ----------------------------------------------------------------------
    // Removes the non needed fix ports.
    public bool CleanupExistingFixPorts(iCS_EditorObject node, PortInfo[] neededPorts) {
        bool changed= false;
        var currentFixPorts= GetCurrentFixPorts(node);
        foreach(var cp in currentFixPorts) {
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
        myIsDirty|= changed;
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
    public iCS_EditorObject[] GetCurrentFixPorts(iCS_EditorObject node) {
        var fixPorts= new List<iCS_EditorObject>();
        node.ForEachChildPort(p => { if(p.IsFixPort) fixPorts.Add(p); });
        return fixPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    public PortInfo[] BuildListOfPortInfoForBehaviourMessage(iCS_EditorObject behaviour) {
        var go= behaviour.Storage.gameObject;
        var portInfos= new List<PortInfo>();        
        portInfos.Add(new PortInfo("gameObject", typeof(GameObject), iCS_ObjectTypeEnum.InFixPort, go));
        return BuildListOfPortInfoForGameObject(go, portInfos);
    }
    // ----------------------------------------------------------------------
    public PortInfo[] BuildListOfPortInfoForGameObject(GameObject gameObject, List<PortInfo> portInfos= null) {
        if(portInfos == null) {
            portInfos= new List<PortInfo>();
        }
        var rigidbody= gameObject.rigidbody;
        if(rigidbody != null) {
            portInfos.Add(new PortInfo("rigidbody", rigidbody.GetType(), iCS_ObjectTypeEnum.InFixPort, rigidbody));
        }
        var camera= gameObject.camera;
        if(camera != null) {
            portInfos.Add(new PortInfo("camera", camera.GetType(), iCS_ObjectTypeEnum.InFixPort, camera));
        }
        var light= gameObject.light;
        if(light != null) {
            portInfos.Add(new PortInfo("light", light.GetType(), iCS_ObjectTypeEnum.InFixPort, light));            
        }
        var animation= gameObject.animation;
        if(animation != null) {
            portInfos.Add(new PortInfo("animation", animation.GetType(), iCS_ObjectTypeEnum.InFixPort, animation));            
        }
        var constantForce= gameObject.constantForce;
        if(constantForce != null) {
            portInfos.Add(new PortInfo("constantForce", constantForce.GetType(), iCS_ObjectTypeEnum.InFixPort, constantForce));            
        }
        var renderer= gameObject.renderer;
        if(renderer != null) {
            portInfos.Add(new PortInfo("renderer", renderer.GetType(), iCS_ObjectTypeEnum.InFixPort, renderer));            
        }
        var audio= gameObject.audio;
        if(audio != null) {
            portInfos.Add(new PortInfo("audio", audio.GetType(), iCS_ObjectTypeEnum.InFixPort, audio));            
        }
        var guiText= gameObject.guiText;
        if(guiText != null) {
            portInfos.Add(new PortInfo("guiText", guiText.GetType(), iCS_ObjectTypeEnum.InFixPort, guiText));            
        }
        var networkView= gameObject.networkView;
        if(networkView != null) {
            portInfos.Add(new PortInfo("networkView", networkView.GetType(), iCS_ObjectTypeEnum.InFixPort, networkView));            
        }
        var guiTexture= gameObject.guiTexture;
        if(guiTexture != null) {
            portInfos.Add(new PortInfo("guiTexture", guiTexture.GetType(), iCS_ObjectTypeEnum.InFixPort, guiTexture));            
        }
        var collider= gameObject.collider;
        if(collider != null) {
            portInfos.Add(new PortInfo("collider", collider.GetType(), iCS_ObjectTypeEnum.InFixPort, collider));            
        }
        var hingeJoint= gameObject.hingeJoint;
        if(hingeJoint != null) {
            portInfos.Add(new PortInfo("hingeJoint", hingeJoint.GetType(), iCS_ObjectTypeEnum.InFixPort, hingeJoint));            
        }
        var particleEmitter= gameObject.particleEmitter;
        if(particleEmitter != null) {
            portInfos.Add(new PortInfo("particleEmitter", particleEmitter.GetType(), iCS_ObjectTypeEnum.InFixPort, particleEmitter));            
        }
        var particleSystem= gameObject.particleSystem;
        if(particleSystem != null) {
            portInfos.Add(new PortInfo("particleSystem", particleSystem.GetType(), iCS_ObjectTypeEnum.InFixPort, particleSystem));            
        }
        var tag= gameObject.tag;
        if(tag != null) {
            portInfos.Add(new PortInfo("tag", tag.GetType(), iCS_ObjectTypeEnum.InFixPort, tag));            
        }


        return portInfos.ToArray();
    }
}
