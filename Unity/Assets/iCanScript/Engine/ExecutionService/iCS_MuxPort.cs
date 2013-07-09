using UnityEngine;
using System.Collections;

public class iCS_MuxPort : iCS_Action, iCS_IParameters {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	object 			 myReturn;
    iCS_Connection[] myConnections;
	
    // ======================================================================
    // IParams implementation
    // ----------------------------------------------------------------------
	public string GetParameterName(int idx) 			{ return Name+"["+idx+"]"; }
    public object GetParameter(int idx) 				{ return idx == 0 ? myReturn : null; }
    public void   SetParameter(int idx, object value)	{ if(idx == 0) myReturn= value; }
    public bool   IsParameterReady(int idx, int frameId) {
		if(idx != 0) return false;
		if(IsCurrent(frameId)) return true;
		return ReadAnyInput(frameId);	
	}
	public void   SetParameterConnection(int idx, iCS_Connection connection) {
		if(idx < 0) {
			Debug.LogWarning("iCanScript: SetConnection index invalid: "+idx);
			return;
		}
		if(idx >= myConnections.Length) {
			var newConnections= new iCS_Connection[idx+1];
			int i= 0;
			for(; i < myConnections.Length; ++i) {
				newConnections[i]= myConnections[i];
			}
			for(; i < newConnections.Length; ++i) {
				newConnections[i]= null;
			}
			myConnections= newConnections;
		}
		myConnections[idx]= connection;
	}

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MuxPort(string name, int priority) : base(name, priority) {
		myConnections= new iCS_Connection[1];
		myConnections[0]= null;
	}

    // ======================================================================
    // Execution (not used)
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
		if(!ReadAnyInput(frameId)) {
	        IsStalled= true;
		}
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
		// Use previous output value.
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
	bool ReadAnyInput(int frameId) {
		for(int i= 1; i < myConnections.Length; ++i) {
            if(myConnections[i].IsConnected && myConnections[i].IsReady(frameId)) {
                myReturn= myConnections[i].Value;
                MarkAsCurrent(frameId);
                return true;
            }			
		}
		return false;		
	}
}
