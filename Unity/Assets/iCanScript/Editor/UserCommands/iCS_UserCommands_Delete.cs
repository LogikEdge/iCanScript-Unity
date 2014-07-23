//
// File: iCS_UserCommands_Delete
//
//#define DEBUG
using UnityEngine;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Object destruction.
	// ----------------------------------------------------------------------
    public static void DeleteObject(iCS_EditorObject obj) {
#if DEBUG
		Debug.Log("iCanScript: Deleting => "+obj.Name);
#endif
        if(obj == null || obj == obj.IStorage.DisplayRoot) return;
        if(!IsDeletionAllowed()) return;
        var name= obj.Name;
        if(!obj.CanBeDeleted()) {
            ShowNotification("Fix port=> \""+name+"\" from node=> \""+obj.ParentNode.FullName+"\" cannot be deleted.");
            return;
        }
        var iStorage= obj.IStorage;
        OpenTransaction(iStorage);
        if(obj.IsInstanceNodePort) {
    		iStorage.AnimateGraph(null,
                _=> {
                    iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj);
                    iStorage.ForcedRelayoutOfTree();
                }
    		);
            CloseTransaction(iStorage, "Delete "+name);
            return;
        }
        // TODO: Should animate parent node on node delete.
		iStorage.AnimateGraph(null,
            _=> {
                // Change selected object without changing the persistent image.
                var currentSelectedId= iStorage.SelectedObject.InstanceId;
                var parent= obj.ParentNode;
                iStorage.SelectedObject= parent;
                iStorage.PersistentStorage.SelectedObject= currentSelectedId;
                iStorage.DestroyInstance(obj.InstanceId);
                iStorage.ForcedRelayoutOfTree();
            }
		);
        CloseTransaction(iStorage, "Delete "+name);
	}
	// ----------------------------------------------------------------------
    public static bool DeleteMultiSelectedObjects(iCS_IStorage iStorage) {
#if DEBUG
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
        iStorage.AnimateGraph(null,
            _=> {
                foreach(var obj in selectedObjects) {
                    if(!obj.CanBeDeleted()) {
                        ShowNotification("Fix port=> \""+obj.Name+"\" from node=> \""+obj.ParentNode.FullName+"\" cannot be deleted.");
                        continue;
                    }
                    // Change selected object without changing the persistent image.
                    var currentSelectedId= iStorage.SelectedObject.InstanceId;
                    var parent= obj.ParentNode;
                    iStorage.SelectedObject= parent;
                    iStorage.PersistentStorage.SelectedObject= currentSelectedId;
                    if(obj.IsInstanceNodePort) {
                		iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj);                        
                    }
                    else {
                		iStorage.DestroyInstance(obj.InstanceId);                        
                    }
                    iStorage.ForcedRelayoutOfTree();
                }                
            }
        );
        CloseTransaction(iStorage, "Delete Selection");
        return true;
    }
	// ----------------------------------------------------------------------
    public static void DeleteKeepChildren(iCS_EditorObject obj) {
        if(!IsDeletionAllowed()) return;
        var iStorage= obj.IStorage;
        OpenTransaction(iStorage);
        var newParent= obj.ParentNode;
        var childNodes= obj.BuildListOfChildNodes(_ => true);
        var childPos= P.map(n => n.GlobalPosition, childNodes);
        iStorage.AnimateGraph(obj,
            _=> {
                // Change selected object without changing the persistent image.
                var currentSelectedId= iStorage.SelectedObject.InstanceId;
                var parent= obj.ParentNode;
                iStorage.SelectedObject= parent;
                iStorage.PersistentStorage.SelectedObject= currentSelectedId;
                
                P.forEach(n => { iStorage.ChangeParent(n, newParent);}, childNodes);
                iStorage.DestroyInstance(obj.InstanceId);
                P.zipWith((n,p) => { n.LocalAnchorFromGlobalPosition= p; }, childNodes, childPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Delete "+obj.Name);
    }

}
