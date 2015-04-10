using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;
using Subspace;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  PORT ATTRIBUTES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public object	InitialValue= null;

    // ======================================================================
	// Port iteration signature
	// ----------------------------------------------------------------------
    public PortIterationSignatureEnum PortIterationSignature {
        get { return EngineObject.PortIterationSignature; }
        set { EngineObject.PortIterationSignature= value; }
    }
    
    // ======================================================================
	// Port source related attributes.
	// ----------------------------------------------------------------------
    public bool IsPortDisabled { get { return DisplayOption == iCS_DisplayOptionEnum.Disabled; }}
    
    // ======================================================================
	// Port source related attributes.
	// ----------------------------------------------------------------------
    public int PortIndex {
		get { return EngineObject.PortIndex; }
		set {
            var engineObject= EngineObject;
			if(engineObject.PortIndex == value) return;
			engineObject.PortIndex= value;
		}
	}
	// ----------------------------------------------------------------------
    public int ProducerPortId {
		get { return EngineObject.SourceId; }
		set { EngineObject.SourceId= value; }
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject ProducerPort {
		get { return ProducerPortId != -1 ? myIStorage[ProducerPortId] : null; }
		set { ProducerPortId= (value != null ? value.InstanceId : -1); }
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject VisibleProducerPort {
		get {
            var providerPort= ProducerPort;
            if(providerPort != null) {
                var providerNode= providerPort.Parent;
                if(providerNode.IsHidden) {
                    if(providerNode.IsTypeCast) {
                        providerNode.ForEachChild(c=> P.executeIf(c, t=> t.IsInDataPort && !t.IsFloating, f=> providerPort= f.ProducerPort));
                    }
                    else {
                        Debug.LogWarning("iCanScript: Internal warning=> Need to update VisibleProducerPort filtering.");
                    }                    
                }
            }
            return providerPort;
        }
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject FirstProducerPort {
		get {
		    var engineObject= Storage.GetFirstProducerPort(EngineObject);
            if(engineObject == null) return this;
            var firstProducer= EditorObjects[engineObject.InstanceId];
            return firstProducer ?? this;
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] ConsumerPorts {
		get {
			return EditorObjects[0].Filter(
                c=> c != this && c.IsPort && c.ProducerPortId == InstanceId
            ).ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] EndConsumerPorts {
		get {
			var result= new List<iCS_EditorObject>();
			BuildListOfEndConsumerPorts(ref result);
            // Remove our self from result.
            if(result.Count == 1 && result[0] == this) {
                result.RemoveAt(0);
            }
			return result.ToArray();
		}
	}
	private void BuildListOfEndConsumerPorts(ref List<iCS_EditorObject> r) {
		var consumers= ConsumerPorts;
		if(consumers.Length == 0) {
			r.Add(this);
		} else {
			foreach(var p in consumers) {
				p.BuildListOfEndConsumerPorts(ref r);
			}
		}
	}
	// ----------------------------------------------------------------------
	public P.Tuple<iCS_EditorObject,iCS_EditorObject>[] Connections {
		get {
			var result= new List<P.Tuple<iCS_EditorObject,iCS_EditorObject> >();
			var source= FirstProducerPort;
			foreach(var consumer in source.EndConsumerPorts) {
				result.Add(new P.Tuple<iCS_EditorObject,iCS_EditorObject>(source, consumer));
			}			        
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public bool IsPartOfConnection(iCS_EditorObject testedPort) {
		if(this == testedPort) return true;
		var src= ProducerPort;
		if(src == null) return false;
		return src.IsPartOfConnection(testedPort);
	} 
	
    // ======================================================================
	// Port value attributes.
	// ----------------------------------------------------------------------
    public string InitialValueArchive {
		get { return EngineObject.InitialValueArchive; }
		set {
            var engineObject= EngineObject;
			if(engineObject.InitialValueArchive == value) return;
			engineObject.InitialValueArchive= value;
		}
	}
	// ----------------------------------------------------------------------
	public object InitialPortValue {
		get {
			if(!IsInDataOrControlPort) return null;
			if(ProducerPortId != -1) return null;
			return InitialValue;			
		}
		set {
			if(!IsInDataOrControlPort) return;
			if(ProducerPortId != -1) return;
			InitialValue= value;
	        myIStorage.StoreInitialPortValueInArchive(this);			
		}
	}
	// ----------------------------------------------------------------------
    // Fetches the runtime value if it exists, otherwise returns the initial value
	public object PortValue {
		get {
			if(!IsDataOrControlPort) return null;
			var port= FirstProducerPort;
            // Get value from parent node.
            return port.InitialPortValue;
		}
		set {
			InitialPortValue= value;
			RuntimePortValue= value;
		}
	}
	// ----------------------------------------------------------------------
	public object RuntimePortValue {
		get {
            return PortValue;
		}
		set {
            // TODO: Implement runtime value change for iCS2.
		}
	}
}
