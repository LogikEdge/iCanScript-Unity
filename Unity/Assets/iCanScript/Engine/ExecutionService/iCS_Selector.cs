using UnityEngine;
using System.Collections;

public class iCS_Selector : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object			myReturn      		= null;		// +1
	protected object			mySelector    		= null;		// +2
	protected object			myDefaultValue		= null;		// +3
	protected iCS_Connection	mySelectorConnection= null;
	
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        if(idx == myParameters.Length) return myReturn;
 		return base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myReturn= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Selector(string name, bool[] paramIsOuts, int priority) : base(name, paramIsOuts, priority) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
		// Wait for the selector source.
		if(mySelectorConnection.IsConnected) {
			if(!mySelectorConnection.IsReady(frameId)) {
				IsStalled= true;
				return;
			}
			mySelector= mySelectorConnection.Value;
		}
		// Use the selector if it exists.
		if(mySelector != null) {
			// Get the index of the selector.
			int idx= -1;
			if(mySelector is bool) {
				idx= (bool)mySelector ? 1 : 0;
			}
			if(mySelector is int) {
				idx= (int)mySelector;
			}
			// Return the default value if the index is invalid.
			if(idx < 0 || idx >= myParameters.Length) {
				if(myDefaultValue != null) {
					myReturn= myDefaultValue;
					MarkAsCurrent(frameId);					
				} else {
					IsStalled= true;
				}
				return;
			}
			// Just read the selected input if it is static.
			if(!myConnections[idx].IsConnected) {
				myReturn= myParameters[idx];
				MarkAsCurrent(frameId);
				return;
			}
			// Wait until the selected input becomes available
			if(!myConnections[idx].IsReady(frameId)) {
				IsStalled= true;
				return;
			}
			myReturn= myConnections[idx].Value;
			MarkAsCurrent(frameId);
			return;
		}
		
        // We don't have a selector so let's use the first connected input that is ready.
        foreach(var id in myInIndexes) {
            if(myConnections[id].IsConnected && myConnections[id].IsReady(frameId)) {
                myReturn= myConnections[id].Value;
                MarkAsCurrent(frameId);
                return;
            }
        }
        IsStalled= true;
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
		// Use previous output value.
        MarkAsCurrent(frameId);
    }

}
