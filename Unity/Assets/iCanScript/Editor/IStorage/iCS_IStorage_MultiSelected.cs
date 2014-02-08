using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// -------------------------------------------------------------------------
	public void ClearMultiSelected() {
		ForEachNode(n => { n.IsMultiSelected= false; });
	}
	// -------------------------------------------------------------------------
	public void ToggleMultiSelect(iCS_EditorObject obj) {
		if(!IsMultiSelectAllowed(obj)) return;
		obj.IsMultiSelected= !obj.IsMultiSelected;
	}
	// -------------------------------------------------------------------------
	public void SetMultiSelect(iCS_EditorObject obj) {
		if(!IsMultiSelectAllowed(obj)) return;
		obj.IsMultiSelected= true;
	}
	// -------------------------------------------------------------------------
	public bool IsMultiSelectAllowed(iCS_EditorObject obj) {
		if(obj == null || !obj.IsNode) return false;
		if(obj.IsBehaviour) return false;
		return true;
	}
}
