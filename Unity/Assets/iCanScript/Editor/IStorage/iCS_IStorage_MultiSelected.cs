using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// -------------------------------------------------------------------------
	public void ClearMultiSelected() {
		ForEachNode(n => { n.IsMultiSelected= false; });
	}
	// -------------------------------------------------------------------------
	public void ToggleMultiSelected(iCS_EditorObject obj) {
		if(!IsMultiSelectedAllowed(obj)) return;
		obj.IsMultiSelected= !obj.IsMultiSelected;
	}
	// -------------------------------------------------------------------------
	public void SetMultiSelected(iCS_EditorObject obj) {
		if(!IsMultiSelectedAllowed(obj)) return;
		obj.IsMultiSelected= true;
	}
	// -------------------------------------------------------------------------
	public bool IsMultiSelectedAllowed(iCS_EditorObject obj) {
		if(obj == null || !obj.IsNode) return false;
		if(obj.IsBehaviour) return false;
		return true;
	}
}
