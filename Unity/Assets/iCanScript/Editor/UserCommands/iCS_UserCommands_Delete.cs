//
// File: iCS_UserCommands_Delete
//
using UnityEngine;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Object destruction.
	// ----------------------------------------------------------------------
    public static void DeleteObject(iCS_EditorObject obj) {
        if(obj == null) return;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Delete "+obj.Name);
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
	}
	// ----------------------------------------------------------------------
    public static bool DeleteMultiSelectedObjects(iCS_IStorage iStorage) {
        if(iStorage == null) return false;
        var selectedObjects= iStorage.GetMultiSelectedObjects();
        if(selectedObjects == null || selectedObjects.Length == 0) return false;
        // User has confirm the deletion.
        iStorage.RegisterUndo("Delete Selection");
        foreach(var obj in selectedObjects) {
            if(!obj.CanBeDeleted()) continue;
            var parent= obj.ParentNode;
            if(obj.IsInstanceNodePort) {
        		parent.AnimateGraph(
        			_=> iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj)                        
        		);
            }
            else {
                // TODO: Should animate parent node on node delete.
        		parent.AnimateGraph(
        			_=> iStorage.DestroyInstance(obj.InstanceId)                        
        		);                            
            }
        }
        return true;
    }
	// ----------------------------------------------------------------------
    public static void DeleteKeepChildren(iCS_EditorObject obj) {
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Delete "+obj.Name);
        var newParent= obj.ParentNode;
        var childNodes= obj.BuildListOfChildNodes(_ => true);
        var childPos= P.map(n => n.LayoutPosition, childNodes);
        iStorage.AnimateGraph(obj,
            _=> {
                P.forEach(n => { iStorage.ChangeParent(n, newParent);}, childNodes);
                iStorage.DestroyInstance(obj.InstanceId);
                P.zipWith((n,p) => { n.SetAnchorAndLayoutPosition(p);}, childNodes, childPos);
                newParent.LayoutNodeAndParents();
            }
        );
    }

}
