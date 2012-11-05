using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // State specific attributes ---------------------------------------------
    public bool IsRawEntryState {
		get { return EngineObject.IsRawEntryState; }
		set { EngineObject.IsRawEntryState= value; }
	}
    public bool IsEntryState {
		get { return EngineObject.IsEntryState; }
		set { EngineObject.IsEntryState= value; }
	}
}
