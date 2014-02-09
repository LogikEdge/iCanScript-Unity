using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// -------------------------------------------------------------------------
	public void ClearMultiSelection() {
		ForEach(n => { n.IsMultiSelected= false; });
	}
	// -------------------------------------------------------------------------
	public bool ToggleMultiSelection(iCS_EditorObject obj) {
		if(!IsMultiSelectionAllowed(obj)) return false;
        TransformSelectedToMultiSelected();
		obj.IsMultiSelected= !obj.IsMultiSelected;			
        return obj.IsMultiSelected;
	}
	// -------------------------------------------------------------------------
	public void SetMultiSelection(iCS_EditorObject obj) {
		if(!IsMultiSelectionAllowed(obj)) return;
        TransformSelectedToMultiSelected();
		obj.IsMultiSelected= true;
	}
	// -------------------------------------------------------------------------
    public bool IsSelectedOrMultiSelected(iCS_EditorObject obj) {
        return obj == SelectedObject || obj.IsMultiSelected;
    }
	// -------------------------------------------------------------------------
    void TransformSelectedToMultiSelected() {
        if(SelectedObject != null) {
            if(SelectedObject.IsValid) {
                SelectedObject.IsMultiSelected= true;
            }
            SelectedObject= null;
        }
    }
	// -------------------------------------------------------------------------
	public bool IsMultiSelectionAllowed(iCS_EditorObject obj) {
		if(obj == null) return false;
		if(obj.IsBehaviour) return false;
        if(obj.IsFixDataPort) return false;
		return true;
	}
	// -------------------------------------------------------------------------
	public iCS_EditorObject[] GetMultiSelectedObjects() {
		var multiSelectedObjects= Filter(obj => obj.IsMultiSelected);
		if(multiSelectedObjects == null || multiSelectedObjects.Count == 0) return null;
		if(SelectedObject != null) {
			multiSelectedObjects.Add(SelectedObject);
		}
		return multiSelectedObjects.ToArray(); 
	}
}
