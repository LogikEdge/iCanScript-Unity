using UnityEngine;
using UnityEditor;
using System.Collections;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private bool undoRedoRunning= false;
    
    // ======================================================================
    // Undo/Redo support
    // ----------------------------------------------------------------------
    public void RegisterUndo(string message= "iCanScript") {
        Undo.RecordObject(Storage, message);
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
    public void SynchronizeAfterUndoRedo() {
        // Avoid reentry.
        if(undoRedoRunning) {
            Debug.LogWarning("iCanScript: Reentering SynchronizeAfterUndoRedo...");
            return;
        }
        undoRedoRunning= true;
        // Keep a copy of the previous display position.
		Rect emptyRect= new Rect(0,0,0,0);
        var previousRect= P.map(o=> IsValid(o) ? o.CachedLayoutRect : emptyRect, EditorObjects);  
        // Rebuild editor data.
        GenerateEditorData();
        // Stamp undo/redo identifier before using IStorage functions.
        Storage.UndoRedoId= ++UndoRedoId;
        // Put back the previous display position
        var rootRect= previousRect[0];
        ForEachRecursiveDepthLast(EditorObjects[0],
            obj=> {
                var r= rootRect;                        
                // only process valid objects.
                if(obj.InstanceId < previousRect.Count && Math3D.Area(previousRect[obj.InstanceId]) > 0.1f) {
                    r= previousRect[obj.InstanceId];                        
                } else {
                    if(obj.IsParentValid && obj.ParentId < previousRect.Count && Math3D.Area(previousRect[obj.ParentId]) > 0.1f) {
                        var parentPos= iCS_EditorObject.PositionFrom(previousRect[obj.ParentId]);
						r= iCS_EditorObject.BuildRect(parentPos, Vector2.zero);
                    }                        
                }
                obj.AnimationStart= r;
            }
        );
        // Rebuild layout
        ForcedRelayoutOfTree(EditorObjects[0]);
        // Animate change.
		var animTime= Mathf.Max(Prefs.MinAnimationTime, 0.5f);
        var timeRatio= iCS_EditorObject.BuildTimeRatio(animTime);
        ForEach(
            obj=> {
                if(obj.IsNode && obj.IsVisibleInLayout) {
                    obj.Animate(obj.LayoutRect, timeRatio);
//					if(obj.Name == ":StateChart") {
//						Debug.Log("AnimStart: "+obj.AnimationStart+" AnimTarget: "+obj.AnimationTarget);
//					}
                }
            }
        );
        undoRedoRunning= false; 
		IsDirty= true;
		iCS_EditorMgr.RepaintVisualEditor();
    }
    
}
