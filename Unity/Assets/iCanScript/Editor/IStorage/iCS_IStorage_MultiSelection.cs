using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public iCS_EditorObject[] GetMultiSelectedObjects() {
		var multiSelectedObjects= Filter(obj => obj.IsMultiSelected);
		if(multiSelectedObjects == null || multiSelectedObjects.Count == 0) return null;
		if(SelectedObject != null) {
			multiSelectedObjects.Add(SelectedObject);
		}
		return multiSelectedObjects.ToArray(); 
	}
	// -------------------------------------------------------------------------
    public iCS_EditorObject[] FilterMultiSelectionForDelete(ref iCS_EditorObject[] invalidList) {
        var multiSelectedObjects= GetMultiSelectedObjects();
        List<iCS_EditorObject> invalid= new List<iCS_EditorObject>();
        List<iCS_EditorObject> valid= new List<iCS_EditorObject>();
        foreach(var obj in multiSelectedObjects) {
            if(obj.IsFixDataPort) {
                invalid.Add(obj);
            }
            else {
                valid.Add(obj);
            }
        }
        invalidList= valid.ToArray();
        return valid.ToArray();
    }
	// -------------------------------------------------------------------------
    public iCS_EditorObject[] FilterMultiSelectionForWrapInPackage() {
        return FilterMultiSelectionUnderSameParent();
    }
	// -------------------------------------------------------------------------
    public iCS_EditorObject[] FilterMultiSelectionForMove(ref iCS_EditorObject[] invalidList) {
        return FilterMultiSelectionUnderSameParent();
    }
    
    // =========================================================================
    // Utilities
	// -------------------------------------------------------------------------
	bool IsMultiSelectionAllowed(iCS_EditorObject obj) {
		if(obj == null) return false;
		if(obj.IsBehaviour) return false;
        if(obj.IsFixDataPort) return false;
		return true;
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
    iCS_EditorObject[] FilterMultiSelectionUnderSameParent() {
        var multiSelectedObjects= GetMultiSelectedObjects();
        if(multiSelectedObjects == null || multiSelectedObjects.Length == 0) {
            return null;
        }
        // Find common parent.
        if(multiSelectedObjects.Length == 1) {
            return multiSelectedObjects;
        }
        var commonParent= multiSelectedObjects[0];
        for(int i= 0; i < multiSelectedObjects.Length-1; ++i) {
            var sharedParent= multiSelectedObjects[i].GetCommonParent(multiSelectedObjects[i+1]);
            if(sharedParent != commonParent) {
                commonParent= commonParent.GetCommonParent(sharedParent);
            }
        }
        // Special case for when the common parent is one of the selected objects.
        List<iCS_EditorObject> valid= new List<iCS_EditorObject>();
        foreach(var obj in multiSelectedObjects) {
            if(obj == commonParent) {
                valid.Add(obj);
            }
        }
        if(valid.Count != 0) {
            return valid.ToArray();
        }
        // Find the proper node just below common parent.
        foreach(var o in multiSelectedObjects) {
            var obj= o;
            while(obj.ParentNode != null && obj.ParentNode != commonParent) obj= obj.ParentNode;
            if(obj.ParentNode == commonParent) {
                valid.Add(obj);
            }
        }
        /*
            TODO : Filter for uniqu entries.
        */
        return valid.ToArray();                
    }
}
