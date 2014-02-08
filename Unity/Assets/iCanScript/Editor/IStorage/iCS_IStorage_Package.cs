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
}
