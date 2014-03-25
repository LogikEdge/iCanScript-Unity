using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  PORT ATTRIBUTES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public object		    InitialValue= null;

    // ======================================================================
	// Port source related attributes.
	// ----------------------------------------------------------------------
    public int PortIndex {
		get { return EngineObject.PortIndex; }
		set {
            var engineObject= EngineObject;
			if(engineObject.PortIndex == value) return;
			engineObject.PortIndex= value;
//			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
    public int SourceId {
		get { return EngineObject.SourceId; }
		set {
            var engineObject= EngineObject;
            if(engineObject.SourceId == value) return;
			EngineObject.SourceId= value;
//			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject ProviderPort {
		get { return SourceId != -1 ? myIStorage[SourceId] : null; }
		set { SourceId= (value != null ? value.InstanceId : -1); }
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject FirstProviderPort {
		get {
		    var engineObject= Storage.GetFirstProviderPort(EngineObject);
		    return engineObject != null ? EditorObjects[engineObject.InstanceId] : this;
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] ConsumerPorts {
		get {
			return Filter(c=> c.IsPort && c.SourceId == InstanceId).ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] EndConsumerPorts {
		get {
			var result= new List<iCS_EditorObject>();
			BuildListOfEndConsumerPorts(ref result);
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
			var source= FirstProviderPort;
			foreach(var consumer in source.EndConsumerPorts) {
				result.Add(new P.Tuple<iCS_EditorObject,iCS_EditorObject>(source, consumer));
			}			        
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public bool IsPartOfConnection(iCS_EditorObject testedPort) {
		if(this == testedPort) return true;
		var src= ProviderPort;
		if(src == null) return false;
		return src.IsPartOfConnection(testedPort);
	} 
	
    // ======================================================================
	// Port value attributes.
    public string InitialValueArchive {
		get { return EngineObject.InitialValueArchive; }
		set {
            var engineObject= EngineObject;
			if(engineObject.InitialValueArchive == value) return;
			engineObject.InitialValueArchive= value;
//			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
	public object InitialPortValue {
		get {
			if(!IsInDataOrControlPort) return null;
			if(SourceId != -1) return null;
			return InitialValue;			
		}
		set {
			if(!IsInDataOrControlPort) return;
			if(SourceId != -1) return;
			InitialValue= value;
	        myIStorage.StoreInitialPortValueInArchive(this);			
		}
	}
	// ----------------------------------------------------------------------
    // Fetches the runtime value if it exists, otherwise returns the initial value
	public object PortValue {
		get {
			if(!IsDataOrControlPort) return null;
			var port= FirstProviderPort;
			// Get value from port group (ex: ParentMuxPort).
			var funcBase= myIStorage.GetRuntimeObject(port) as iCS_ISignature;
			if(funcBase != null) {
			    object returnValue= funcBase.GetSignatureDataSource().ReturnValue;
				return returnValue;
			}
            // Get value from parent node.
			funcBase= myIStorage.GetRuntimeObject(port.Parent) as iCS_ISignature;
			return funcBase == null ? port.InitialPortValue : funcBase.GetSignatureDataSource().GetValue(port.PortIndex);			
		}
		set {
			InitialPortValue= value;
			RuntimePortValue= value;
//	        Parent.IsDirty= true;			
		}
	}
	// ----------------------------------------------------------------------
	public object RuntimePortValue {
		get {
            return PortValue;
		}
		set {
	        if(!IsInDataOrControlPort) return;
	        // Set the return value for a port group (ex: MuxPort).
			var funcBase= myIStorage.GetRuntimeObject(this) as iCS_ISignature;
	        if(funcBase != null) {
	            funcBase.GetSignatureDataSource().ReturnValue= value;
	            return;
	        }
	        // Propagate value for module port.
	        if(IsKindOfPackagePort) {
	            iCS_EditorObject[] connectedPorts= ConsumerPorts;
	            foreach(var cp in connectedPorts) {
	                cp.RuntimePortValue= value;
	            }
	            return;
	        }
	        if(PortIndex < 0) return;
	        iCS_EditorObject parent= Parent;
	        if(parent == null) return;
	        // Get runtime object if it exists.
	        var runtimeObject= myIStorage.GetRuntimeObject(parent) as iCS_ISignature;
	        if(runtimeObject == null) return;
	        runtimeObject.GetSignatureDataSource().SetValue(PortIndex, value);			
		}
	}
}
