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
        if(obj == null) return;
        if(!IsDeletionAllowed()) return;
        var name= obj.Name;
        if(!obj.CanBeDeleted()) {
            ShowNotification("Fix port=> \""+name+"\" from node=> \""+obj.ParentNode.FullName+"\" cannot be deleted.");
            return;
        }
        var iStorage= obj.IStorage;
        if(obj.IsInstanceNodePort) {
    		iStorage.AnimateGraph(null,
    			_=> iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj)                        
    		);
            return;
        }
        // TODO: Should animate parent node on node delete.
		iStorage.AnimateGraph(null,
            _=> {
                var parent= obj.ParentNode;
                iStorage.DestroyInstance(obj.InstanceId);
                parent.LayoutNodeAndParents();
            }
		);
        iStorage.SaveStorage("Delete "+name);
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
        iStorage.AnimateGraph(null,
            _=> {
                foreach(var obj in selectedObjects) {
                    if(!obj.CanBeDeleted()) {
                        ShowNotification("Fix port=> \""+obj.Name+"\" from node=> \""+obj.ParentNode.FullName+"\" cannot be deleted.");
                        continue;
                    }
                    var parent= obj.ParentNode;
                    if(obj.IsInstanceNodePort) {
                		iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj);                        
                    }
                    else {
                		iStorage.DestroyInstance(obj.InstanceId);                        
                    }
                    parent.LayoutNodeAndParents();
                }                
            }
        );
        iStorage.SaveStorage("Delete Selection");
        return true;
    }
	// ----------------------------------------------------------------------
    public static void DeleteKeepChildren(iCS_EditorObject obj) {
        if(!IsDeletionAllowed()) return;
        var iStorage= obj.IStorage;
        var newParent= obj.ParentNode;
        var childNodes= obj.BuildListOfChildNodes(_ => true);
        var childPos= P.map(n => n.GlobalPosition, childNodes);
        iStorage.AnimateGraph(obj,
            _=> {
                P.forEach(n => { iStorage.ChangeParent(n, newParent);}, childNodes);
                iStorage.DestroyInstance(obj.InstanceId);
                P.zipWith((n,p) => { n.LocalAnchorFromGlobalPosition= p; }, childNodes, childPos);
                newParent.LayoutNodeAndParents();
            }
        );
        iStorage.SaveStorage("Delete "+obj.Name);
    }

}
