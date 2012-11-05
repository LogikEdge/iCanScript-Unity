using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public object		    InitialValue= null;

    // ======================================================================
    // Proxy Methods
    // ----------------------------------------------------------------------
    public iCS_EdgeEnum Edge {
		get { return EngineObject.Edge; }
		set { EngineObject.Edge= value; }
	}
    public int SourceId {
		get { return EngineObject.SourceId; }
		set { EngineObject.SourceId= value; }
	}
    public int PortIndex {
		get { return EngineObject.PortIndex; }
		set { EngineObject.PortIndex= value; }
	}
    public string InitialValueArchive {
		get { return EngineObject.InitialValueArchive; }
		set { EngineObject.InitialValueArchive= value;}
	}
    
    // Port layout attributes -----------------------------------------------
    public bool IsOnLeftEdge      { get { return EngineObject.IsOnLeftEdge; }}
    public bool IsOnRightEdge     { get { return EngineObject.IsOnRightEdge; }}
    public bool IsOnTopEdge       { get { return EngineObject.IsOnTopEdge; }}
    public bool IsOnBottomEdge    { get { return EngineObject.IsOnBottomEdge; }}


    // Initial port value ---------------------------------------------------
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
}
