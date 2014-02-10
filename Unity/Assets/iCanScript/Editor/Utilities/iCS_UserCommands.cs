using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;

public class iCS_UserCommands {
    // ======================================================================
    // Object destruction editor utilities.
	// ----------------------------------------------------------------------
    public static void DeleteObject(iCS_EditorObject obj) {
        if(obj == null) return;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Deleting: "+obj.Name);
        var parent= obj.ParentNode;
        if(obj.IsInstanceNodePort) {
    		parent.AnimateGraph(
    			_=> iStorage.InstanceWizardDestroyAllObjectsAssociatedWithPort(obj)                        
    		);
            return;
        }
        // TODO: Should animate parent node on node delete.
		parent.AnimateGraph(
			_=> iStorage.DestroyInstance(obj.InstanceId)                        
		);
	}
	// ----------------------------------------------------------------------
    public static bool DeleteMultiSelectedObjects(iCS_IStorage iStorage) {
        if(iStorage == null) return false;
        var selectedObjects= iStorage.GetMultiSelectedObjects();
        if(selectedObjects == null || selectedObjects.Length == 0) return false;
        // User has confirm the deletion.
        iStorage.RegisterUndo("Multi-Selection Deletion");
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
        iStorage.RegisterUndo("Deleting Package: "+obj.Name);
        var newParent= obj.ParentNode;
        var childNodes= obj.BuildListOfChildNodes(_ => true);
        var childPos= P.map(n => n.LayoutPosition, childNodes);
        P.forEach(n => { iStorage.ChangeParent(n, newParent);}, childNodes);
        iStorage.DestroyInstance(obj.InstanceId);
        P.zipWith((n,p) => { n.SetAnchorAndLayoutPosition(p);}, childNodes, childPos);
    }
    
    // ======================================================================
    // Node Wrapping in package.
	// ----------------------------------------------------------------------
    public static void WrapInPackage(iCS_EditorObject obj) {
        if(obj == null || !obj.CanHavePackageAsParent()) return;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Wrap node in Package: "+obj.Name);
        iStorage.WrapInPackage(obj);
    }
	// ----------------------------------------------------------------------
    public static void WrapMultiSelectionInPackage(iCS_IStorage iStorage) {
        if(iStorage == null) return;
        var selectedObjects= iStorage.FilterMultiSelectionForWrapInPackage();
        if(selectedObjects == null || selectedObjects.Length == 0) return;
        iStorage.RegisterUndo("Wrap Multi-Selection in Package");
        iStorage.WrapInPackage(selectedObjects);
    }
}
