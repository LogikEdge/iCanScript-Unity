//
// File: iCS_UserCommands_UndoRedo
//
//#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Undo/Redo is a condition detected by the storage.  It simulates
    // a User Command.
	// ----------------------------------------------------------------------
    // OK
    public static void UndoRedo(iCS_IStorage iStorage) {
#if DEBUG
        Debug.Log("iCanScript: Undo/Redo. Undo Group => "+Undo.GetCurrentGroup());
#endif
        var animationStarts= new Rect[iStorage.EditorObjects.Count];
        iStorage.ForEach(obj=> { animationStarts[obj.InstanceId]= obj.AnimationTargetRect;});
        iStorage.AnimateGraph(null,
            _=> {
                // Keep a copy of the animation start Rect.
                // Rebuild editor data.
                try {
                    iStorage.GenerateEditorData();                    
                }
                catch(System.Exception) {
                    Debug.LogWarning("iCanScript: Problem found regenerating data");
                }
                // Rebuild layout
                iStorage.ForcedRelayoutOfTree(iStorage.EditorObjects[0]);
                // Put back the animation start Rect.
                int len= animationStarts.Length;
                for(int id= 0; id < len; ++id) {
                    if(iStorage.IsValid(id)) {
                        var obj= iStorage.EditorObjects[id];
                        obj.ResetAnimationRect(animationStarts[id]);
                    }
                }
            }
        );
    }
}
