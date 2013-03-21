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
        var objectMementos= P.map(o=> IsValid(o) ? o.CreateMemento() : null, EditorObjects);  
        // Rebuild editor data.
        GenerateEditorData();
        // Stamp undo/redo identifier before using IStorage functions.
        Storage.UndoRedoId= ++UndoRedoId;
        // Put back the previous display position
        var rootMemento= objectMementos[0];
        ForEachRecursiveDepthLast(EditorObjects[0],
            obj=> {
                iCS_EditorObject.PositionMemento objMemento= null;                        
                // only process valid objects.
                if(obj.InstanceId < objectMementos.Count && objectMementos[obj.InstanceId] != null) {
                    objMemento= objectMementos[obj.InstanceId];                        
                } else {
                    if(obj.IsParentValid && obj.ParentId < objectMementos.Count && objectMementos[obj.ParentId] != null) {
                        var parentMemento= objectMementos[obj.ParentId];
                        objMemento= new iCS_EditorObject.PositionMemento();
                        objMemento.GlobalPosition= parentMemento.GlobalPosition;
                    }                        
                }
                obj.SetMemento(objMemento ?? rootMemento);
                obj.PrepareToAnimateRect();
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
