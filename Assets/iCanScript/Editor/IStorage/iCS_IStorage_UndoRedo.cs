using UnityEngine;
using UnityEditor;
using System.Collections;
using P= Prelude;

public partial class iCS_IStorage {
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    public void RegisterUndo(string message= "iCanScript") {
        Undo.RegisterUndo(Storage, message);
        Storage.UndoRedoId= ++UndoRedoId;        
    }
    // ----------------------------------------------------------------------
    void DetectUndoRedo() {
        // Regenerate internal structures if undo/redo was performed.
        if(Storage.UndoRedoId != UndoRedoId) {
			SynchronizeAfterUndoRedo();
        }        
    }
    // ----------------------------------------------------------------------
    void SynchronizeAfterUndoRedo() {
        // Keep a copy of the previous display position.
        var displayPositions= P.map(o=> new P.Tuple<bool,Rect>(IsValid(o),GetDisplayPosition(o)), EditorObjects);  
        // Rebuild editor data.
        GenerateEditorData();
        // Put back the previous display position
        Vector2 rootCenter= Math3D.Middle(displayPositions[0].Item2);
        ForEach(
            obj=> {
                // Default to root center.
                Rect displayPos= new Rect(rootCenter.x, rootCenter.y, 0, 0);                        
                if(obj.InstanceId < displayPositions.Count && displayPositions[obj.InstanceId].Item1) {
                    displayPos= displayPositions[obj.InstanceId].Item2;
                } else {
                    if(obj.IsParentValid && obj.ParentId < displayPositions.Count && displayPositions[obj.ParentId].Item1) {
                        Vector2 parentCenter= Math3D.Middle(displayPositions[obj.ParentId].Item2);
                        displayPos= new Rect(parentCenter.x, parentCenter.y, 0, 0);
                    }
                }
                SetDisplayPosition(obj, displayPos);
            }
        );
        // Set all object dirty.
        ForEach(SetDirty);
        Storage.UndoRedoId= ++UndoRedoId;        
    }
    
}
