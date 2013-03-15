using UnityEngine;
using UnityEditor;
using System.Collections;
using P= Prelude;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private bool undoRedoRunning= false;
    
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
        // Avoid reentry.
        if(undoRedoRunning) {
            Debug.LogWarning("iCanScript: Reentering SynchronizeAfterUndoRedo...");
            return;
        }
        undoRedoRunning= true;
        // Keep a copy of the previous display position.
        var displayRects= P.map(
            o=> {
                if(IsValid(o)) {
                    return new P.Tuple<bool,Rect>(true, o.GlobalDisplayRect);
//                    return o.CreateMemento();
                }
                return new P.Tuple<bool,Rect>(false, new Rect(0,0,0,0));
//                return null;
            },
            EditorObjects
        );  
        // Rebuild editor data.
        GenerateEditorData();
        // Stamp undo/redo identifier before using IStorage functions.
        Storage.UndoRedoId= ++UndoRedoId;
        // Put back the previous display position
        Vector2 rootPos= iCS_EditorObject.PositionFrom(displayRects[0].Item2);
        ForEachRecursiveDepthLast(EditorObjects[0],
            obj=> {
                // Default to root center.
                var displayRect= new Rect(rootPos.x, rootPos.y, 0, 0);                        
                // only process valid objects.
                if(obj.InstanceId < displayRects.Count) {
                    if(displayRects[obj.InstanceId].Item1) {
                        displayRect= displayRects[obj.InstanceId].Item2;                        
                    } else {
                        if(obj.IsParentValid && obj.ParentId < displayRects.Count && displayRects[obj.ParentId].Item1) {
                            var parentPos= iCS_EditorObject.PositionFrom(displayRects[obj.ParentId].Item2);
                            displayRect= new Rect(parentPos.x, parentPos.y, 0, 0);
                        }                        
                    }
                }
                if(obj.IsPort) {
                    obj.GlobalDisplayPosition= iCS_EditorObject.PositionFrom(displayRect);                    
                } else {
                    obj.GlobalDisplayRect= displayRect;
                    obj.PrepareToAnimateRect();                    
                }
            }
        );
        // Rebuild layout
        ForcedRelayoutOfTree(EditorObjects[0]);
        // Animate change.
        var timeRatio= iCS_EditorObject.BuildTimeRatio(0.5f);  // Half second animation.
        ForEach(
            obj=> {
                if(obj.IsNode) {
                    obj.AnimateRect(timeRatio);
                }
            }
        );
        undoRedoRunning= false;        
    }
    
}
