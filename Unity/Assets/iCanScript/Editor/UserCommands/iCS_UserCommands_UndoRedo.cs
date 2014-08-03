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
		var previousScrollPosition= iStorage.ScrollPosition;
		var previousScale= iStorage.GuiScale;
        iStorage.AnimateGraph(null,
            _=> {
                // Keep a copy of the animation start Rect.
                // Rebuild editor data.
                try {
                    iStorage.GenerateEditorData();
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
				var visualEditor= iCS_EditorController.FindVisualEditor();
				if(visualEditor != null) {
					var animationTime= iCS_PreferencesController.AnimationTime;
					visualEditor.AnimateScrollPosition(previousScrollPosition,
													   iStorage.ScrollPosition,
													   animationTime);
					visualEditor.AnimateScale(previousScale,
											  iStorage.GuiScale,
											  animationTime);
				}
            }
        );
        // Update central visual script data
        iCS_VisualScriptDataController.Update();
        // Repaint all windows that could have changed.
        iCS_EditorController.RepaintAllEditors();
        // Force redraw of Inspector Window.
        EditorUtility.SetDirty(iStorage.iCSMonoBehaviour);
    }
}
