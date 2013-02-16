using UnityEngine;
using System.Collections;

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
    public int PortIndex {
		get { return EngineObject.PortIndex; }
		set {
            var engineObject= EngineObject;
			if(engineObject.PortIndex == value) return;
			engineObject.PortIndex= value;
			IsDirty= true;
		}
	}
    public int SourceId {
		get { return EngineObject.SourceId; }
		set {
            var engineObject= EngineObject;
            if(engineObject.SourceId == value) return;
			EngineObject.SourceId= value;
			IsDirty= true;
		}
	}
    public iCS_EditorObject Source {
		get { return SourceId != -1 ? myIStorage[SourceId] : null; }
		set { SourceId= (value != null ? value.InstanceId : -1); }
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
			var port= this;
            int retry= 0;
			while(port.Source != null) {
			    port= port.Source;
                if(++retry > 100) {
                    Debug.LogWarning("iCanScript: Circular port connection detected on: "+port.ParentNode.Name+"."+port.Name);
                    return null;
                }
		    }
			iCS_IParams funcBase= myIStorage.GetRuntimeObject(port) as iCS_IParams;
			if(funcBase != null) {
			    return funcBase.GetParameter(0);
			}
			funcBase= myIStorage.GetRuntimeObject(port.Parent) as iCS_IParams;
			return funcBase == null ? port.InitialPortValue : funcBase.GetParameter(port.PortIndex);			
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
			if(!IsDataPort) return null;
			var port= this;
			while(port.Source != null) port= port.Source;
			iCS_IParams funcBase= myIStorage.GetRuntimeObject(port) as iCS_IParams;
			if(funcBase != null) {
			    return funcBase.GetParameter(0);
			}
			funcBase= myIStorage.GetRuntimeObject(port.Parent) as iCS_IParams;
			return funcBase == null ? port.InitialPortValue : null;			
		}
		set {
	        if(!IsInDataPort) return;
	        // Just set the port if it has its own runtime.
			iCS_IParams funcBase= myIStorage.GetRuntimeObject(this) as iCS_IParams;
	        if(funcBase != null) {
	            funcBase.SetParameter(0, value);
	            return;
	        }
	        // Propagate value for module port.
	        if(IsModulePort) {
	            iCS_EditorObject[] connectedPorts= myIStorage.FindConnectedPorts(this);
	            foreach(var cp in connectedPorts) {
	                cp.RuntimePortValue= value;
	            }
	            return;
	        }
	        if(PortIndex < 0) return;
	        iCS_EditorObject parent= Parent;
	        if(parent == null) return;
	        // Get runtime object if it exists.
	        iCS_IParams runtimeObject= myIStorage.GetRuntimeObject(parent) as iCS_IParams;
	        if(runtimeObject == null) return;
	        runtimeObject.SetParameter(PortIndex, value);			
		}
	}
}
