using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_IStorage {
	// -------------------------------------------------------------------------
	// Wraps the given object in a package
	public iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
		if(obj == null || !obj.CanHavePackageAsParent()) return null;
		var r= obj.LayoutRect;
		var parent= obj.ParentNode;
        var package= CreatePackage(parent.InstanceId, Math3D.Middle(r), obj.Name);
		ChangeParent(obj, package);
		// Attempt to reposition the package ports to match the object ports.		
		obj.ForEachChildPort(
			p => {
				var sourcePort= p.Source;
				if(sourcePort != null && sourcePort.ParentNode == package) {
					sourcePort.Edge= p.Edge;
					sourcePort.PortPositionRatio= p.PortPositionRatio;
				}
				else {
					package.UntilMatchingChild(
						pp => {
							if(pp.Source == p) {
								pp.Edge= p.Edge;
								pp.PortPositionRatio= p.PortPositionRatio;
								return true;
							}
							return false;
						}
					);
				}
			}
		);
        package.WrapAroundChildRect(r);
        obj.SetAnchorAndLayoutRect(r);
        obj.LayoutParentNodesUntilTop();
		return package;
	}
	// -------------------------------------------------------------------------
	public iCS_EditorObject WrapInPackage(iCS_EditorObject[] objects) {
        if(objects == null || objects.Length == 0) return null;
        if(objects.Length == 1) {
            return WrapInPackage(objects[0]);
        }
        var rs= P.map(n => n.LayoutRect, objects);
        var parent= objects[0].ParentNode;
        var package= CreatePackage(parent.InstanceId, Math3D.Middle(rs[0]), "");
        foreach(var obj in objects) {
    		ChangeParent(obj, package);
    		// Attempt to reposition the package ports to match the object ports.		
    		obj.ForEachChildPort(
    			p => {
    				var sourcePort= p.Source;
    				if(sourcePort != null && sourcePort.ParentNode == package) {
    					sourcePort.Edge= p.Edge;
    					sourcePort.PortPositionRatio= p.PortPositionRatio;
    				}
    				else {
    					package.UntilMatchingChild(
    						pp => {
    							if(pp.Source == p) {
    								pp.Edge= p.Edge;
    								pp.PortPositionRatio= p.PortPositionRatio;
    								return true;
    							}
    							return false;
    						}
    					);
    				}
    			}
    		);            
        }
        package.WrapAroundChildRects(rs);
        for(int i= 0; i < objects.Length; ++i) {
            objects[i].SetAnchorAndLayoutRect(rs[i]);
        }
        package.LayoutParentNodesUntilTop();
        return package;
    }
}
