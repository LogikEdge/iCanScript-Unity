using UnityEngine;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
    	// -------------------------------------------------------------------------
    	// Wraps the given object in a package
    	public iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
    		if(obj == null || !obj.CanHavePackageAsParent()) return null;
    		var parent= obj.ParentNode;
            var package= CreatePackage(parent.InstanceId, obj.DisplayName);
    		ChangeParent(obj, package);
    		// Attempt to reposition the package ports to match the object ports.		
    		obj.ForEachChildPort(
    			p => {
    				var sourcePort= p.ProducerPort;
    				if(sourcePort != null && sourcePort.ParentNode == package) {
    					sourcePort.Edge= p.Edge;
    					sourcePort.PortPositionRatio= p.PortPositionRatio;
    				}
    				else {
    					package.UntilMatchingChild(
    						pp => {
    							if(pp.ProducerPort == p) {
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
            ForcedRelayoutOfTree();
    		return package;
    	}
    	// -------------------------------------------------------------------------
    	public iCS_EditorObject WrapInPackage(iCS_EditorObject[] objects) {
            if(objects == null || objects.Length == 0) {
                Debug.LogWarning("iCanScript: No objects selected!!!");
                return null;
            }
            if(objects.Length == 1) {
                if(objects[0] == null) {
                    Debug.LogWarning("iCanScript: ony one NULL object is selected");
                }
                return WrapInPackage(objects[0]);
            }
            var parent= objects[0].ParentNode;
            var package= CreatePackage(parent.InstanceId, "");
            foreach(var obj in objects) {
        		ChangeParent(obj, package);
        		// Attempt to reposition the package ports to match the object ports.		
        		obj.ForEachChildPort(
        			p => {
        				var sourcePort= p.ProducerPort;
        				if(sourcePort != null && sourcePort.ParentNode == package) {
        					sourcePort.Edge= p.Edge;
        					sourcePort.PortPositionRatio= p.PortPositionRatio;
        				}
        				else {
        					package.UntilMatchingChild(
        						pp => {
        							if(pp.ProducerPort == p) {
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

}
