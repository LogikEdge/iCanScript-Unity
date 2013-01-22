using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  STATE / STATECHART ATTRIBUTES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // State specific attributes ---------------------------------------------
    public bool IsEntryState {
		get { return EngineObject.IsEntryState; }
		set {
			var engineObject= EngineObject;
			if(engineObject.IsEntryState == value) return;
			engineObject.IsEntryState= value;
			IsDirty= true;
		}
	}
}
