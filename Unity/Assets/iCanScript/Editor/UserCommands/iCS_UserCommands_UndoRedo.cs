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
//        Debug.Log("iCanScript: Display Root after Undo => "+iStorage.iCSMonoBehaviour.Storage.DisplayRoot);

        var animationStarts= new Rect[iStorage.EditorObjects.Count];
        iStorage.ForEach(obj=> { animationStarts[obj.InstanceId]= obj.AnimationTargetRect;});
        iStorage.AnimateGraph(null,
            _=> {
                // Keep a copy of the animation start Rect.
                // Rebuild editor data.
                try {
                    iCS_StorageImp.CopyFromTo(iStorage.PersistentStorage, iStorage.Storage);
                    iStorage.GenerateEditorData();
//                    var navigationHistory= new iCS_NavigationHistory();
//                    navigationHistory.CopyFrom(iStorage.Storage.NavigationHistory);
//                    var displayRoot= iStorage.Storage.DisplayRoot;
//                    var scrollPosition= iStorage.Storage.ScrollPosition;
//                    iStorage.GenerateEditorData();
//                    if(displayRoot != iStorage.Storage.DisplayRoot ||
//                       !navigationHistory.IsEquivalentTo(iStorage.Storage.NavigationHistory)) {
//                         iStorage.Storage.ScrollPosition= scrollPosition;   
//                    }
//                    iStorage.Storage.NavigationHistory.CopyFrom(navigationHistory);
//                    iStorage.Storage.DisplayRoot= displayRoot;
                }
                catch(System.Exception e) {
                    Debug.LogWarning("iCanScript: Problem found regenerating data: "+e.Message);
                }
                // Rebuild layout
                iStorage.ForcedRelayoutOfTree(iStorage.SelectedObject, iStorage.Storage.SelectedObjectPosition);
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
        // Update central visual script data
        iCS_VisualScriptDataController.Update();
        // Repaint all windows that could have changed.
        iCS_EditorController.RepaintVisualEditor();
        iCS_EditorController.RepaintTreeViewEditor();
        iCS_EditorController.RepaintInstanceEditor();
        // Force redraw of Inspector Window.
        EditorUtility.SetDirty(iStorage.iCSMonoBehaviour);
    }
}
