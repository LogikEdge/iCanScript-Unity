using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using CompileError  =Prelude.Tuple<int,string>;
using CompileWarning=Prelude.Tuple<int,string>;

public partial class iCS_VisualScriptImp : iCS_MonoBehaviourImp {
    // ----------------------------------------------------------------------
    iCS_Connection BuildVariableProxyConnection(iCS_EngineObject proxyNode, iCS_EngineObject proxyPort, iCS_EngineObject consumerPort) {
        var runtimeNode= GetRuntimeNodeFromReferenceNode(proxyNode);
        if(runtimeNode == null) {
            Debug.LogWarning("Unable to find port proxy node=> "+proxyNode.Name);
            return null;
        }
		iCS_Connection connection= null;
        bool isAlwaysReady= true;
        bool isControlPort= proxyPort.IsControlPort;
		connection= new iCS_Connection(runtimeNode as iCS_ISignature, proxyPort.PortIndex, isAlwaysReady, isControlPort);
        return connection;
    }
    // ----------------------------------------------------------------------
    iCS_ActionWithSignature GetUserFunctionAction(iCS_EngineObject userFunctionCall) {
        var runtimeNode= GetRuntimeNodeFromReferenceNode(userFunctionCall);
        if(runtimeNode == null) {
            Debug.LogWarning("iCanScript: Unable to find user function=> "+userFunctionCall.Name);
            return null;                   
        }
        return runtimeNode as iCS_ActionWithSignature;
    }
    // ----------------------------------------------------------------------
    object GetRuntimeNodeFromReferenceNode(iCS_EngineObject referenceNode) {
        var vs= GetVisualScriptFromReferenceNode(referenceNode);
        if(vs == null) {
            Debug.LogWarning("iCanScript: Unable to find user function=> "+referenceNode.Name);
            return null;
        }
        var runtimeNodeId  = referenceNode.ProxyOriginalNodeId;
        return vs.RuntimeNodes[runtimeNodeId];
    }
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetEngineObjectFromReferenceNode(iCS_EngineObject referenceNode) {
        var vs= GetVisualScriptFromReferenceNode(referenceNode);
        if(vs == null) {
            Debug.LogWarning("iCanScript: Unable to find user function=> "+referenceNode.Name);
            return null;
        }
        var objectId= referenceNode.ProxyOriginalNodeId;
        return vs.EngineObjects[objectId];
    }
    // ----------------------------------------------------------------------
    public bool IsReferenceNodeUsingDynamicBinding(iCS_EngineObject referenceNode) {
        var gameObjectPort= iCS_VisualScriptData.GetInInstancePort(this, referenceNode);
        if(gameObjectPort == null) return false;
        var ProducerPort= iCS_VisualScriptData.GetFirstProducerPort(this, gameObjectPort);
        return ProducerPort != null && ProducerPort != gameObjectPort;
    }
    // ----------------------------------------------------------------------
    public iCS_VisualScriptImp GetVisualScriptFromReferenceNode(iCS_EngineObject referenceNode) {
        var gameObjectPort= iCS_VisualScriptData.GetInInstancePort(this, referenceNode);
        iCS_VisualScriptImp vs= null;
        if(gameObjectPort != null) {
            var gameObject= GetInitialValue(gameObjectPort) as GameObject;
            if(gameObject != null) {
                vs= gameObject.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
            }
        }
        if(vs == null) {
            var tag= referenceNode.ProxyOriginalVisualScriptTag;
            var go= GameObject.FindWithTag(tag);
            if(go != null) {
                vs= go.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;   
            }                
        }
        if(vs == null) {
            Debug.LogWarning("iCanScript: Can't locate game object with tag=> "+tag);
        }
        return vs;
    }
    // ----------------------------------------------------------------------
    iCS_Connection BuildUserFunctionOutputConnection(iCS_EngineObject port, iCS_EngineObject referenceNode, iCS_UserFunctionCall userFunctionCall) {
        var vs= GetVisualScriptFromReferenceNode(referenceNode);
        if(vs == null) {
            return null;
        }
        var runtimeNodeId= referenceNode.ProxyOriginalNodeId;
        var userFunction = vs.EngineObjects[runtimeNodeId];
        var userFunctionPort= iCS_VisualScriptData.GetChildPortWithIndex(vs, userFunction, port.PortIndex);
        if(userFunctionPort == null) return null;
        var sourcePort= iCS_VisualScriptData.GetFirstProducerPort(vs, userFunctionPort);
        var sourcePortParent= vs.GetParentNode(sourcePort);
        var runtimeNode= vs.RuntimeNodes[sourcePortParent.InstanceId];
		iCS_Connection connection= null;
        bool isAlwaysReady= false;
        bool isControlPort= false;
		connection= new iCS_Connection(runtimeNode as iCS_ISignature, sourcePort.PortIndex, isAlwaysReady, isControlPort);
        userFunctionCall.SetConnection(port.PortIndex, connection);
        return connection;        
    }
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetPublicInterfaceFromName(string name) {
        var publicInterfaces= PublicInterfaces;
        foreach(var pi in publicInterfaces) {
            if(pi.Name == name) {
                return pi;
            }
        }
        return null;
    }
}
