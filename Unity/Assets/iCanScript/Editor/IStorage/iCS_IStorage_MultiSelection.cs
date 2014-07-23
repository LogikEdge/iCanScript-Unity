using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // =========================================================================
    // Fields
	// -------------------------------------------------------------------------
    List<iCS_EditorObject>  mySelectedObjects= new List<iCS_EditorObject>();
    iCS_EditorObject[]      myMultiSelectNodeDragObjects= null;
    Vector2[]               myMultiSelectNodeDragStartPosition= null;
    
    // =========================================================================
    // Properties
	// -------------------------------------------------------------------------
    int SelectedObjectId {
        get { return Storage.SelectedObject; }
        set {
            Storage.SelectedObject= value;
            if(!IsUserTransactionActive) {
                PersistentStorage.SelectedObject= value;                
            }
            EditorUtility.SetDirty(iCSMonoBehaviour);
            ++ModificationId;
        }
    }
    public iCS_EditorObject SelectedObject {
        get {
            int id= SelectedObjectId;
            return id == -1 ? DisplayRoot : this[id];
        }
        set {
            mySelectedObjects.Clear();
            if(value != null) {
                mySelectedObjects.Add(value);
            }
            var selectedId= value != null ? value.InstanceId : DisplayRootId;
            if(selectedId != SelectedObjectId) {
                SelectedObjectId= selectedId;
                SaveSelectedObjectPosition();                
            }
        }
    }
    // -------------------------------------------------------------------------
    public bool IsMultiSelectionActive {
        get { return mySelectedObjects.Count > 1; }
    }
    
    // =========================================================================
    // Public methods
    // -------------------------------------------------------------------------
    public void ClearSelected() {
        SelectedObject= null;
    }
	// -------------------------------------------------------------------------
	public void ClearMultiSelection() {
        while(mySelectedObjects.Count > 1) {
            mySelectedObjects.RemoveAt(1);
        }
	}
	// -------------------------------------------------------------------------
	public bool ToggleMultiSelection(iCS_EditorObject obj) {
		if(!IsMultiSelectionAllowed(obj)) return false;
        if(IsSelectedOrMultiSelected(obj)) {
            RemoveFromSelectedObjects(obj);
            return false;
        }
        AddToSelectedObjects(obj);
        return true;
	}
	// -------------------------------------------------------------------------
    public bool IsSelectedOrMultiSelected(iCS_EditorObject obj) {
        return mySelectedObjects.Contains(obj);
    }
	// -------------------------------------------------------------------------
	public iCS_EditorObject[] GetMultiSelectedObjects() {
		return mySelectedObjects.ToArray(); 
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
    public iCS_EditorObject[] FilterMultiSelectionForMove() {
        return FilterMultiSelectionUnderSameParent();
    }
	// -------------------------------------------------------------------------
    public void StartMultiSelectionNodeDrag() {
        myMultiSelectNodeDragObjects= FilterMultiSelectionForMove();
        var len= myMultiSelectNodeDragObjects.Length;
        myMultiSelectNodeDragStartPosition= new Vector2[len];
        for(int i= 0; i < len; ++i) {
            myMultiSelectNodeDragStartPosition[i]= myMultiSelectNodeDragObjects[i].GlobalPosition;
        }
    }
	// -------------------------------------------------------------------------
    public void MoveMultiSelectedNodesBy(Vector2 delta) {
        var len= myMultiSelectNodeDragObjects.Length;
        for(int i= 0; i < len; ++i) {
            var newPos= myMultiSelectNodeDragStartPosition[i]+delta;
            myMultiSelectNodeDragObjects[i].NodeDragTo(newPos);
        }
    }
	// -------------------------------------------------------------------------
    public void EndMultiSelectionNodeDrag() {
        // Remove sticky on parent nodes.
        foreach(var node in myMultiSelectNodeDragObjects) {
            node.EndNodeDrag();                    
        }
        myMultiSelectNodeDragObjects= null;
        myMultiSelectNodeDragStartPosition= null;
    }
    
    // =========================================================================
    // Utilities
	// -------------------------------------------------------------------------
    void RemoveFromSelectedObjects(iCS_EditorObject obj) {
        int idx= mySelectedObjects.IndexOf(obj);
        if(idx == -1) return;
        mySelectedObjects.RemoveAt(idx);
        if(idx == 0) {
            SelectedObjectId= mySelectedObjects.Count == 0 ? -1 : mySelectedObjects[0].InstanceId;
        }
    }
	// -------------------------------------------------------------------------
    void AddToSelectedObjects(iCS_EditorObject obj) {
        if(mySelectedObjects.Count == 0) {
            SelectedObject= obj;
        }
        else {
            mySelectedObjects.Add(obj);            
        }
    }
	// -------------------------------------------------------------------------
	bool IsMultiSelectionAllowed(iCS_EditorObject obj) {
		if(obj == null) return false;
		if(obj.IsBehaviour) return false;
        if(obj.IsFixDataPort) return false;
		return true;
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
