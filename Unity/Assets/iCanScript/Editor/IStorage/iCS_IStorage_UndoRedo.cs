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
        var displayRects= P.map(o=> new P.Tuple<bool,Rect>(IsValid(o),o.GlobalDisplayRect), EditorObjects);  
        // Rebuild editor data.
        GenerateEditorData();
        // Put back the previous display position
        Vector2 rootPos= iCS_EditorObject.PositionFrom(displayRects[0].Item2);
        ForEach(
            obj=> {
                // Default to root center.
                var displayRect= new Rect(rootPos.x, rootPos.y, 0, 0);                        
                if(obj.InstanceId < displayRects.Count && displayRects[obj.InstanceId].Item1) {
                    displayRect= displayRects[obj.InstanceId].Item2;
                } else {
                    if(obj.IsParentValid && obj.ParentId < displayRects.Count && displayRects[obj.ParentId].Item1) {
                        var parentPos= iCS_EditorObject.PositionFrom(displayRects[obj.ParentId].Item2);
                        displayRect= new Rect(parentPos.x, parentPos.y, 0, 0);
                    }
                }
                if(obj.IsPort) {
                    obj.AnimatePosition(iCS_EditorObject.PositionFrom(displayRect));                    
                } else {
                    obj.AnimateRect(displayRect);                    
                }
            }
        );
        // Set all object dirty.
        ForEach(o=> o.IsDirty= true);
        Storage.UndoRedoId= ++UndoRedoId;        
    }
    
}
