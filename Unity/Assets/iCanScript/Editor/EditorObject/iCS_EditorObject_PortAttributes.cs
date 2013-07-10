using UnityEngine;
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
			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
    public int SourceId {
		get { return EngineObject.SourceId; }
		set {
            var engineObject= EngineObject;
            if(engineObject.SourceId == value) return;
			EngineObject.SourceId= value;
			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject Source {
		get { return SourceId != -1 ? myIStorage[SourceId] : null; }
		set { SourceId= (value != null ? value.InstanceId : -1); }
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject SourceEndPort {
		get {
		    var engineObject= Storage.GetSourceEndPort(EngineObject);
		    return engineObject != null ? EditorObjects[engineObject.InstanceId] : this;
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] Destinations {
		get {
			return Filter(c=> c.IsPort && c.SourceId == InstanceId).ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public iCS_EditorObject[] DestinationEndPoints {
		get {
			var result= new List<iCS_EditorObject>();
			BuildDestinationEndPoints(ref result);
			return result.ToArray();
		}
	}
	private void BuildDestinationEndPoints(ref List<iCS_EditorObject> r) {
		var destinations= Destinations;
		if(destinations.Length == 0) {
			r.Add(this);
		} else {
			foreach(var p in destinations) {
				p.BuildDestinationEndPoints(ref r);
			}
		}
	}
	// ----------------------------------------------------------------------
	public P.Tuple<iCS_EditorObject,iCS_EditorObject>[] Connections {
		get {
			var result= new List<P.Tuple<iCS_EditorObject,iCS_EditorObject> >();
			var source= SourceEndPort;
			foreach(var destination in source.DestinationEndPoints) {
				result.Add(new P.Tuple<iCS_EditorObject,iCS_EditorObject>(source, destination));
			}			        
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
	public bool IsPartOfConnection(iCS_EditorObject testedPort) {
		if(this == testedPort) return true;
		var src= Source;
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
			IsDirty= true;
		}
	}
	// ----------------------------------------------------------------------
	public object InitialPortValue {
		get {
			if(!IsInDataPort) return null;
			if(SourceId != -1) return null;
			return InitialValue;			
		}
		set {
			if(!IsInDataPort) return;
			if(SourceId != -1) return;
			InitialValue= value;
	        myIStorage.StoreInitialPortValueInArchive(this);			
		}
	}
	// ----------------------------------------------------------------------
    // Fetches the runtime value if it exists, otherwise returns the initial value
	public object PortValue {
		get {
			if(!IsDataPort) return null;
			var port= SourceEndPort;
			// Get value from port group (ex: ParentMuxPort).
			var funcBase= myIStorage.GetRuntimeObject(port) as iCS_ISignature;
			if(funcBase != null) {
			    return funcBase.GetSignatureDataSource().ReturnValue;
			}
            // Get value from parent node.
			funcBase= myIStorage.GetRuntimeObject(port.Parent) as iCS_ISignature;
			return funcBase == null ? port.InitialPortValue : funcBase.GetSignatureDataSource().Parameters[port.PortIndex];			
		}
		set {
			InitialPortValue= value;
			RuntimePortValue= value;
	        Parent.IsDirty= true;			
		}
	}
	// ----------------------------------------------------------------------
	public object RuntimePortValue {
		get {
            return PortValue;
		}
		set {
	        if(!IsInDataPort) return;
	        // Set the return value for a port group (ex: MuxPort).
			var funcBase= myIStorage.GetRuntimeObject(this) as iCS_ISignature;
	        if(funcBase != null) {
	            funcBase.GetSignatureDataSource().ReturnValue= value;
	            return;
	        }
	        // Propagate value for module port.
	        if(IsKindOfAggregatePort) {
	            iCS_EditorObject[] connectedPorts= Destinations;
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
	        runtimeObject.GetSignatureDataSource().Parameters[PortIndex]= value;			
		}
	}
}
