using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ======================================================================
        // Undo/Redo is a condition detected by the storage.  It simulates
        // a User Command.
        static void ReloadEditorData(iCS_IStorage iStorage, Action reloadAction) {
            var animationStarts= new Rect[iStorage.EditorObjects.Count];
            iStorage.ForEach(obj=> { animationStarts[obj.InstanceId]= obj.AnimationTargetRect;});
    		var previousScrollPosition= iStorage.ScrollPosition;
    		var previousScale= iStorage.GuiScale;
            iStorage.AnimateGraph(null,
                _=> {
                    // Keep a copy of the animation start Rect.
                    // Rebuild editor data.
                    try {
                        reloadAction();
                    }
                    catch(System.Exception e) {
                        Debug.LogWarning("iCanScript: Problem found regenerating data: "+e.Message);
                    }
                    // Rebuild layout
                    iStorage.ForcedRelayoutOfTree();
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
    					var animationTime= PreferencesController.AnimationTime;
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
    		// Annouce that an undo occured.
    		SystemEvents.AnnouceVisualScriptUndo(iStorage);
        }

        // ======================================================================
        // Undo/Redo is a condition detected by the storage.  It simulates
        // a User Command.
        public static void UndoRedo(iCS_IStorage iStorage) {
            ReloadEditorData(iStorage, iStorage.GenerateEditorData);
        }

        // ======================================================================
        // Initialize the editor data.
        public static void InitEditorData(iCS_IStorage iStorage) {
            ReloadEditorData(iStorage, iStorage.InitEditorData);
        }
    }

}

