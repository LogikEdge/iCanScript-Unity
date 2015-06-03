using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //  PORT ATTRIBUTES
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public partial class iCS_EditorObject {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	public object	InitialValue= null;

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
        /// Returns the port that actually produces the data.
    	public iCS_EditorObject SegmentProducerPort {
    		get {
				var producerPort= this;
				while(producerPort.ProducerPort != null) {
					producerPort= producerPort.ProducerPort;
				}
				return producerPort;
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
        /// Returns the list of consumers a producer ports has.
    	public iCS_EditorObject[] SegmentEndConsumerPorts {
    		get {
    			var result= new List<iCS_EditorObject>();
    			BuildListOfSegmentEndConsumerPorts(ref result);
                // -- Remove our self from result. --
                if(result.Count == 1 && result[0] == this) {
                    result.RemoveAt(0);
                }
    			return result.ToArray();
    		}
    	}
    	private void BuildListOfSegmentEndConsumerPorts(ref List<iCS_EditorObject> r) {
    		var consumers= ConsumerPorts;
    		if(consumers.Length == 0) {
    			r.Add(this);
    		} else {
    			foreach(var p in consumers) {
    				p.BuildListOfSegmentEndConsumerPorts(ref r);
    			}
    		}
    	}
    	// ----------------------------------------------------------------------
        /// Determines if the port is the only consumer of the attached producer
        /// producer port.
        public bool IsTheOnlyConsumer {
            get {
                var producerPort= SegmentProducerPort;
                return producerPort.SegmentEndConsumerPorts.Length == 1;
            }
        }
    	// ----------------------------------------------------------------------
    	public P.Tuple<iCS_EditorObject,iCS_EditorObject>[] Connections {
    		get {
    			var result= new List<P.Tuple<iCS_EditorObject,iCS_EditorObject> >();
    			var source= SegmentProducerPort;
    			foreach(var consumer in source.SegmentEndConsumerPorts) {
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
    			var port= SegmentProducerPort;
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
}
