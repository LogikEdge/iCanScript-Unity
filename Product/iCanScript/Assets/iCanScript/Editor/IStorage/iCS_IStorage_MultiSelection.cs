using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
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
                    EngineStorage.SelectedObject= value;                
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
                    AddToSelectedObjects(value);
                }
                var selectedId= value != null ? value.InstanceId : DisplayRootId;
                SelectedObjectId= selectedId;
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
            // Ignore display root if selected
            if(P.length(mySelectedObjects) == 1 && mySelectedObjects[0] == DisplayRoot) {
                mySelectedObjects.Clear();
            }
            // Update SelectedObject if multi-select list is empty.
            if(P.length(mySelectedObjects) == 0) {
                SelectedObject= obj;
            }
            else {
                AddToSelectedObjects(obj);
            }
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
        // =========================================================================
        // Multi-selection Node Drag
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
        public void DragMultiSelectedNodesBy(Vector2 delta) {
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
        // Multi-selection Node Relocate
    	// -------------------------------------------------------------------------
        public void StartMultiSelectionNodeRelocation() {
            myMultiSelectNodeDragObjects= FilterMultiSelectionForMove();
            var len= myMultiSelectNodeDragObjects.Length;
            myMultiSelectNodeDragStartPosition= new Vector2[len];
            for(int i= 0; i < len; ++i) {
                myMultiSelectNodeDragObjects[i].StartNodeRelocate();
                myMultiSelectNodeDragStartPosition[i]= myMultiSelectNodeDragObjects[i].GlobalPosition;
            }
        }
    	// -------------------------------------------------------------------------
        public void RelocateMultiSelectedNodesBy(Vector2 delta) {
            var len= myMultiSelectNodeDragObjects.Length;
            for(int i= 0; i < len; ++i) {
                var newPos= myMultiSelectNodeDragStartPosition[i]+delta;
                myMultiSelectNodeDragObjects[i].NodeRelocateTo(newPos);
            }
        }
    	// -------------------------------------------------------------------------
        public void EndMultiSelectionNodeRelocation() {
            // Remove sticky on parent nodes.
            foreach(var node in myMultiSelectNodeDragObjects) {
                node.EndNodeRelocate();
            }
            myMultiSelectNodeDragObjects= null;
            myMultiSelectNodeDragStartPosition= null;
        }
    	// -------------------------------------------------------------------------
        public void CancelMultiSelectionNodeRelocation() {
            // Remove sticky on parent nodes.
            var len= myMultiSelectNodeDragObjects.Length;
            for(int i= 0; i < len; ++i) {
                var node= myMultiSelectNodeDragObjects[i];
                node.LocalAnchorFromGlobalPosition= myMultiSelectNodeDragStartPosition[i];
                node.EndNodeRelocate();
            }
            myMultiSelectNodeDragObjects= null;
            myMultiSelectNodeDragStartPosition= null;
        }
    
        // =========================================================================
        // Utilities
    	// -------------------------------------------------------------------------
        // Don't use this function directly !!!
        // Use ToggleMultiSelection or SelectedObject= value to add an object to the
        // selection list.
        void RemoveFromSelectedObjects(iCS_EditorObject obj) {
            int idx= mySelectedObjects.IndexOf(obj);
            if(idx == -1) return;
            mySelectedObjects.RemoveAt(idx);
            if(idx == 0) {
                SelectedObjectId= mySelectedObjects.Count == 0 ? -1 : mySelectedObjects[0].InstanceId;
            }
            obj.ForEachConnectedProducerTypeCast(n=> RemoveFromSelectedObjects(n));
        }
    	// -------------------------------------------------------------------------
        // Don't use this function directly !!!
        // Use ToggleMultiSelection or SelectedObject= value to add an object to the
        // selection list.
        void AddToSelectedObjects(iCS_EditorObject obj) {
            // Add new object to selection list.
            mySelectedObjects.Add(obj);
            obj.ForEachConnectedProducerTypeCast(n=> AddToSelectedObjects(n));
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
                Debug.LogWarning("No selected object found!!!");
                return null;
            }
            // Find common parent.
            if(multiSelectedObjects.Length == 1) {
                return multiSelectedObjects;
            }
            var commonParent= GraphInfo.GetCommonParent(multiSelectedObjects);
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

}
