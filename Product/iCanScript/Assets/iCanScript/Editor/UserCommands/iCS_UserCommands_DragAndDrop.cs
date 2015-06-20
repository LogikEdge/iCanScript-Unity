//
// File: iCS_UserCommands_DragAndDrop
//
//#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ======================================================================
        // 
    	// ----------------------------------------------------------------------
        // OK
        public static bool ChangeIcon(iCS_EditorObject node, Texture newTexture) {
            if(node == null) return false;
    #if DEBUG
            Debug.Log("iCanScript: Change Icon => "+node.DisplayName);
    #endif
            if(!IsCreationAllowed()) return false;
            var iStorage= node.IStorage;
            OpenTransaction(iStorage);
            try {
                string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
                node.IconGUID= iconGUID;                    
                iStorage.ForcedRelayoutOfTree();
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return false;
            }
    	    CloseTransaction(iStorage, "Change Icon");
            iCS_EditorController.RepaintEditorsWithLabels();			
            return true;
        }
    	// ----------------------------------------------------------------------
        public static void PasteIntoGraph(iCS_MonoBehaviourImp sourceMonoBehaviour, iCS_EngineObject sourceRoot,
                                          iCS_IStorage iStorage, iCS_EditorObject parent, Vector2 globalPos) {
            if(!IsCreationAllowed()) return;
            OpenTransaction(iStorage);
    		iCS_EditorObject pasted= null;
            try {
                iStorage.AnimateGraph(null,
                    _=> {
                        if(parent.IsFoldedInLayout || parent.IsIconizedInLayout) {
                            parent.Unfold();
                        }
                        iCS_IStorage srcIStorage= new iCS_IStorage(sourceMonoBehaviour);
                        iCS_EditorObject srcRoot= srcIStorage.EditorObjects[sourceRoot.InstanceId];
                        pasted= iStorage.Copy(srcRoot, srcIStorage, parent, globalPos, iStorage);
                        if(pasted != null) {
                            if(pasted.IsUnfoldedInLayout) {
                                pasted.Fold();            
                            }
                            iStorage.SelectedObject= pasted;
                            iStorage.ForcedRelayoutOfTree();
                        }
                    }
                );            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
    		if(pasted != null) {
    			SystemEvents.AnnounceVisualScriptElementAdded(pasted);			
    		}
            CloseTransaction(iStorage, "Add Prefab "+sourceRoot.RawName);
        }
    	// ----------------------------------------------------------------------
        public static void DragAndDropSetPortValue(iCS_EditorObject port, UnityEngine.Object value) {
            var iStorage= port.IStorage;
            OpenTransaction(iStorage);
            try {
                port.Value= value;            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Set port "+port.DisplayName);
        }

    }

}

