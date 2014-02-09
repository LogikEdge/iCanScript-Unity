using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// -------------------------------------------------------------------------
	// Wraps the given object in a package
	public iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
		if(obj == null || !obj.CanHavePackageAsParent()) return null;
		var pos= obj.LayoutPosition;
		var parent= obj.ParentNode;
        var package= CreatePackage(parent.InstanceId, pos, obj.Name);
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
		return package;
	}
	// -------------------------------------------------------------------------
	public iCS_EditorObject WrapInPackage(iCS_EditorObject[] objects) {
        if(objects == null || objects.Length == 0) return null;
        if(objects.Length == 1) {
            return WrapInPackage(objects[0]);
        }
        // Compute new package rectangle.
        Rect rect= objects[0].LayoutRect;
        for(int i= 1; i < objects.Length; ++i) {
            var oRect= objects[i].LayoutRect;
            if(oRect.x < rect.x) rect.x= oRect.x;
            if(oRect.y < rect.y) rect.y= oRect.y;
            if(oRect.xMax > rect.xMax) rect.xMax= oRect.xMax;
            if(oRect.yMax > rect.yMax) rect.yMax= oRect.yMax;
        }
        // Add margins
        float margin= iCS_EditorConfig.MarginSize;
        rect.x-= margin; rect.y-= margin; rect.xMax+= margin; rect.yMax+= margin;
        var parent= objects[0].ParentNode;
        var package= CreatePackage(parent.InstanceId, Math3D.Middle(rect), "");
        package.SetAnchorAndLayoutRect(rect);
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
        return package;
    }
}
