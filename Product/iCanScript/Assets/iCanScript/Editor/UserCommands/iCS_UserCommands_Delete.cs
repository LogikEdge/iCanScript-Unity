//
// File: iCS_UserCommands_Delete
//
//#define SHOW_SHOW_DEBUG
using UnityEngine;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ======================================================================
        // Object destruction.
    	// ----------------------------------------------------------------------
        public static bool DeleteObject(iCS_EditorObject obj) {
#if SHOW_DEBUG
    		Debug.Log("iCanScript: Deleting => "+obj.DisplayName);
#endif
            if(obj == null) return false;
            if(!IsDeletionAllowed()) return false;
            if(!obj.CanBeDeleted()) {
                ShowNotification("Port cannot be deleted=> "+obj.FullName);
                return false;
            }
            var name= obj.DisplayName;
            var iStorage= obj.IStorage;
            OpenTransaction(iStorage);
            if(obj.IsInstanceNodePort) {
                try {
            		iStorage.AnimateGraph(null,
                        _=> {
                            iStorage.SelectedObject= obj.ParentNode;
    						SystemEvents.AnnounceVisualScriptElementWillBeRemoved(obj);
                            iStorage.PropertiesWizardDestroyAllObjectsAssociatedWithPort(obj);
                            iStorage.ForcedRelayoutOfTree();
                        }
            		);                
                }
                catch(System.Exception) {
                    CancelTransaction(iStorage);
                    return false;
                }
                CloseTransaction(iStorage, "Delete "+name);
                return true;
            }
            // TODO: Should animate parent node on node delete.
            try {
        		iStorage.AnimateGraph(null,
                    _=> {
                        // Move the selection to the parent node
                        var parent= obj.ParentNode;
                        iStorage.SelectedObject= parent;
    					SystemEvents.AnnounceVisualScriptElementWillBeRemoved(obj);
                        iStorage.DestroyInstance(obj.InstanceId);
                        iStorage.ForcedRelayoutOfTree();
                    }
        		);            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return false;
            }
            CloseTransaction(iStorage, "Delete "+name);
            return true;
    	}
    	// ----------------------------------------------------------------------
        public static bool DeleteMultiSelectedObjects(iCS_IStorage iStorage) {
#if SHOW_DEBUG
    		Debug.Log("iCanScript: Multi-Select Delete");
#endif
            if(iStorage == null) return false;
            if(!IsDeletionAllowed()) return false;
            var selectedObjects= iStorage.GetMultiSelectedObjects();
            if(selectedObjects == null || selectedObjects.Length == 0) return false;
            if(selectedObjects.Length == 1) {
                DeleteObject(selectedObjects[0]);
                return true;
            }
            OpenTransaction(iStorage);
            try {
                iStorage.AnimateGraph(null,
                    _=> {
                        foreach(var obj in selectedObjects) {
                            if(!obj.CanBeDeleted()) {
                                ShowNotification("Fix port=> \""+obj.DisplayName+"\" from node=> \""+obj.ParentNode.FullName+"\" cannot be deleted.");
                                continue;
                            }
                            // Move the selection to the parent node
                            var parent= obj.ParentNode;
                            iStorage.SelectedObject= parent;

    						SystemEvents.AnnounceVisualScriptElementWillBeRemoved(obj);

                            if(obj.IsInstanceNodePort) {
                        		iStorage.PropertiesWizardDestroyAllObjectsAssociatedWithPort(obj);                        
                            }
                            else {
                        		iStorage.DestroyInstance(obj.InstanceId);                        
                            }
                            iStorage.ForcedRelayoutOfTree();
                        }                
                    }
                );            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return false;
            }
            CloseTransaction(iStorage, "Delete Selection");
            return true;
        }
    	// ----------------------------------------------------------------------
        public static void DeleteKeepChildren(iCS_EditorObject obj) {
            if(!IsDeletionAllowed()) return;
            var iStorage= obj.IStorage;
            OpenTransaction(iStorage);
            try {
                var newParent= obj.ParentNode;
                var childNodes= obj.BuildListOfChildNodes(_ => true);
                var childPos= P.map(n => n.GlobalPosition, childNodes);
                iStorage.AnimateGraph(obj,
                    _=> {
                        // Move the selection to the parent node
                        var parent= obj.ParentNode;
                        iStorage.SelectedObject= parent;
                
                        P.forEach(n => { iStorage.ChangeParent(n, newParent);}, childNodes);
    					SystemEvents.AnnounceVisualScriptElementWillBeRemoved(obj);
                        iStorage.DestroyInstance(obj.InstanceId);
                        P.zipWith((n,p) => { n.LocalAnchorFromGlobalPosition= p; }, childNodes, childPos);
                        iStorage.ForcedRelayoutOfTree();
                    }
                );            
            }
            catch(System.Exception) {
                CancelTransaction(iStorage);
                return;
            }
            CloseTransaction(iStorage, "Delete "+obj.DisplayName);
        }
    }

}

