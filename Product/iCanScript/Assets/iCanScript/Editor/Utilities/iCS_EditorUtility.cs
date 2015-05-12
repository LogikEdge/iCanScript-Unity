using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_EditorUtility {
        // ======================================================================
        // Tree Navigation
    	// ----------------------------------------------------------------------
        public static iCS_EditorObject GetFirstChild(iCS_EditorObject parent, iCS_IStorage storage) {
            if(parent == null) return null;
            iCS_EditorObject firstChild= parent;
            storage.UntilMatchingChildNode(parent,
                child=> {
                    firstChild= child;
                    return true;
                }
            );
            return firstChild;
        }
    	// ----------------------------------------------------------------------
        public static iCS_EditorObject GetNextSibling(iCS_EditorObject node, iCS_IStorage storage) {
            if(node == null || node.Parent == null) return null;
            iCS_EditorObject nextSibling= null;
            iCS_EditorObject firstChild= null;
            bool nodeFound= false;
            storage.UntilMatchingChildNode(node.Parent,
                child=> {
                    if(firstChild == null) {
                        firstChild= child;
                    }
                    if(child == node) {
                        nodeFound= true;
                        return false;
                    }
                    if(nodeFound) {
                        nextSibling= child;
                        return true;
                    }
                    return false;
                }
            );
            if(!nodeFound) return node;
            return nextSibling ?? firstChild;
        }
    	// ----------------------------------------------------------------------
        public static iCS_EditorObject GetPreviousSibling(iCS_EditorObject node, iCS_IStorage storage) {
            if(node == null || node.Parent == null) return null;
            iCS_EditorObject prevSibling= null;
            storage.UntilMatchingChildNode(node.Parent,
                child=> {
                    if(child == node && prevSibling != null) {
                        return true;
                    }
                    prevSibling= child;
                    return false;
                }
            );
            return prevSibling ?? node;
        }


        // ======================================================================
        // GUI helpers
    	// ----------------------------------------------------------------------
        public static int SafeSelectAndMakeVisible(iCS_EditorObject selected, iCS_IStorage iStorage) {
            iCS_UserCommands.OpenTransaction(iStorage);
            iCS_UserCommands.Select(selected, iStorage);        
            CenterOn(selected, iStorage);
            iCS_UserCommands.CloseTransaction(iStorage, "Focus on: "+selected.DisplayName);
            return iStorage.UndoRedoId;
        }
        public static void MakeVisible(iCS_EditorObject eObj, iCS_IStorage iStorage) {
            if(eObj == null || iStorage == null) return;
            if(eObj.IsNode) {
                for(var parent= eObj.Parent; parent != null && parent.InstanceId != 0; parent= parent.Parent) {
                    parent.Unfold();
                }            
                return;
            }
            if(eObj.IsPort) {
                var portParent= eObj.ParentNode;
                if(!portParent.IsVisibleInLayout || portParent.IsIconizedInLayout) {
                    portParent.Fold();
                }
                MakeVisible(portParent, iStorage);
                return;            
            }
        }
    	// ----------------------------------------------------------------------
        public static void SafeCenterOn(iCS_EditorObject eObj, iCS_IStorage iStorage) {
            iCS_UserCommands.OpenTransaction(iStorage);
            CenterOn(eObj, iStorage);
            iCS_UserCommands.CloseTransaction(iStorage, "Focus on "+eObj.DisplayName);
        }
        public static void CenterOn(iCS_EditorObject eObj, iCS_IStorage iStorage) {
            MakeVisible(eObj, iStorage);
            var graphEditor= iCS_EditorController.FindVisualEditor();
            if(graphEditor != null) graphEditor.CenterAndScaleOn(eObj);        
        }
    	// ----------------------------------------------------------------------
        public static bool IsCurrentUndoRedoId(int modificationId, iCS_IStorage iStorage) {
            return modificationId == iStorage.UndoRedoId;
        } 
        public static void UndoIfUndoRedoId(int modificationId, iCS_IStorage iStorage) {
            if(IsCurrentUndoRedoId(modificationId, iStorage)) {
                Undo.PerformUndo();
            }
        }

    	// ----------------------------------------------------------------------
    	public static float GetGUIStyleHeight(GUIStyle style) {
    		float height= style.lineHeight+style.border.vertical;
    		Texture backgroundTexture= style.normal.background;
    		if(backgroundTexture != null) {
    			height= backgroundTexture.height;
    		}
    		return height;		
    	}
    }
}

