using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ----------------------------------------------------------------------
    public void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= node.Parent;
        if(newParent == null || newParent == oldParent) return;
		// Change parent and relayout.
		var nodePos= node.LayoutPosition;
		node.Parent= newParent;
		node.SetAnchorAndLayoutPosition(nodePos);
		node.LayoutNode();
		node.LayoutParentNodesUntilTop();
		if(node.IsState) CleanupEntryState(node, oldParent);
        RebuildConnectionsFor(node);
    }
	// ----------------------------------------------------------------------
    public void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_TypeCastInfo conversion= null) {
		iCS_EditorObject inParentNode  = inPort.ParentNode;
        iCS_EditorObject outParentNode = outPort.ParentNode;
        iCS_EditorObject inGrandParent = inParentNode.ParentNode;        
        iCS_EditorObject outGrandParent= outParentNode.ParentNode;

        // No need to create module ports if both connected nodes are under the same parent.
        if(inGrandParent == outGrandParent || inGrandParent == outParentNode || inParentNode == outGrandParent) {
            SetSource(inPort, outPort, conversion);
            OptimizeDataConnection(inPort, outPort);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= outGrandParent.ParentNode; op != null; op= op.ParentNode) {
            if(inGrandParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inGrandParent != null) {
			var sourcePort= inPort.Source;
			iCS_EditorObject newPort= null;
			// Attempt to reuse existing ports.
			if(sourcePort != null && sourcePort.ParentNode == inGrandParent && conversion == null) {
				newPort= sourcePort;
			}
			else {
				newPort= CreatePort(inPort.Name, inGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
	            SetSource(inPort, newPort, conversion);
				SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
			}
            SetNewDataConnection(newPort, outPort);
            OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= inGrandParent.ParentNode; ip != null; ip= ip.ParentNode) {
            if(outGrandParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outGrandParent != null) {
			// Attempt to reuse existing port.
			iCS_EditorObject newPort= null;
			if(conversion == null &&
			   outGrandParent.UntilMatchingChild(
				   p=> {
					   if(p.IsPort && p.Source == outPort) {
						   newPort= p;
						   return true;
					   }
					   return false;
				   }
			   )
			) {}
			else {
				newPort= CreatePort(outPort.Name, outGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
	            SetSource(newPort, outPort, conversion);
				SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
			}
            SetNewDataConnection(inPort, newPort);
            OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        SetSource(inPort, outPort, conversion);
        OptimizeDataConnection(inPort, outPort);
    }
	// ----------------------------------------------------------------------
	public void RebuildDataConnection(iCS_EditorObject outputPort, iCS_EditorObject inputPort) {
#if DEBUG
		Debug.Log("iCanScript: RebuildDataConnection: output= "+outputPort.Name+" input= "+inputPort.Name);
#endif
		// Have we completed rebuilding ... if so return.
		if(inputPort == outputPort) return;
		var inputNode= inputPort.ParentNode;
		var outputNode= outputPort.ParentNode;
		if(inputNode == outputNode) return;
		// outputPort is inside the node with the inputPort.
		var commonParentNode= outputPort.GetCommonParent(inputPort);
		if(inputNode == commonParentNode) {
			// Rebuild moving down from the common parent towards the output port.
			var newInputNode= outputPort.ParentNode;
			while(newInputNode != inputNode && newInputNode.ParentNode != inputNode) {
				newInputNode= newInputNode.ParentNode;
			}
			var existingPort= FindPortWithSourceEndPoint(newInputNode, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.Source;
				if(prevSource != existingPort) {
					inputPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= CreatePort(inputPort.Name, newInputNode.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.Source= inputPort.Source;
				inputPort.Source= newPort;
				RebuildDataConnection(outputPort, newPort);				
			}			
			return;
		}
		var inputNodeParent= inputNode.ParentNode;
		if(inputNodeParent == commonParentNode) {
			// Rebuild traversing from moving upwards to downwords.
			var newDstNode= outputPort.ParentNode;
			while(newDstNode != commonParentNode && newDstNode.ParentNode != commonParentNode) {
				newDstNode= newDstNode.ParentNode;
			}
			var existingPort= FindPortWithSourceEndPoint(newDstNode, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.Source;
				if(prevSource != existingPort) {
					inputPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= CreatePort(inputPort.Name, newDstNode.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.Source= inputPort.Source;
				inputPort.Source= newPort;
				RebuildDataConnection(outputPort, newPort);				
			}
			return;
		} else {
			// Rebuilding moving up from the destination port towards the common parent.
			var existingPort= FindPortWithSourceEndPoint(inputNodeParent, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.Source;
				if(prevSource != existingPort) {
					inputPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= CreatePort(inputPort.Name, inputNodeParent.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.Source= inputPort.Source;
				inputPort.Source= newPort;
				RebuildDataConnection(outputPort, newPort);
			}			
		}
	}
	// ----------------------------------------------------------------------
	public void RebuildConnectionsFor(iCS_EditorObject node) {
		// Rebuild connection from end-to-end.
		node.ForEachChildPort(
			p=> {
			    if(p.IsDataOrControlPort) {
    				var outputPort= p.SourceEndPort;
    				foreach(var inputPort in p.DestinationEndPoints) {
    					RebuildDataConnection(outputPort, inputPort);
    				}			        
			    }
			    if(p.IsStatePort) {
			        var fromState= GetFromStatePort(p);
			        var toState  = GetToStatePort(p);
			        RebuildStateConnection(fromState, toState);
			    }
			}
		);
	}
	
	// ----------------------------------------------------------------------
	public void RebuildStateConnection(iCS_EditorObject fromStatePort, iCS_EditorObject toStatePort) {
		var commonParent= fromStatePort.GetCommonParent(toStatePort);
		if(commonParent == null) {
			Debug.LogWarning("iCanScript: Unable to find common parent after relocating state !!!");
			return;
		}
		var transitionPackage= GetTransitionPackage(toStatePort);
		if(transitionPackage == null) return;
		if(transitionPackage.ParentNode == commonParent) return;
		ChangeParent(transitionPackage, commonParent);
		LayoutTransitionPackage(transitionPackage);
	}
	
	// ----------------------------------------------------------------------
	// This attempt to properly locate an autocreated data port.
	public void SetBestPositionForAutocreatedPort(iCS_EditorObject port, Vector2 inPortPosition, Vector2 outPortPosition) {
		// Determine the parent edge position to use.
		var parent= port.Parent;
		var parentGlobalRect= parent.LayoutRect;
		float x= port.IsInputPort ? parentGlobalRect.xMin : parentGlobalRect.xMax;
		// Assure that the in position X value is smaller then the out position.
		if(inPortPosition.x > outPortPosition.x) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Manage situation where new port is between the in & out ports.
		var parentGlobalPosition= Math3D.Middle(parentGlobalRect);
		var top   = parentGlobalPosition.y+parent.VerticalPortsTop;
		var bottom= parentGlobalPosition.y+parent.VerticalPortsBottom;
		float y;
		if(Math3D.IsSmaller(inPortPosition.x, x) && Math3D.IsGreater(outPortPosition.x, x)) {
			float ratio= (x-inPortPosition.x)/(outPortPosition.x-inPortPosition.x);
			y= Math3D.Lerp(inPortPosition.y, outPortPosition.y, ratio);
			if(y < top) { 
				y= top;
			}
			if(y > bottom) {
				y= bottom;
			}
			port.SetAnchorAndLayoutPosition(new Vector2(x,y));
			return;
		}
		if(Math3D.IsEqual(inPortPosition.y, outPortPosition.y)) {
			port.SetAnchorAndLayoutPosition(new Vector2(x, 0.5f*(top+bottom)));
			return;
		}
		// Assure that the in position Y value is smaller then the out position.
		if(inPortPosition.y > outPortPosition.y) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Compute some type of ratio if Y position traverse the top port position
		if(Math3D.IsSmaller(inPortPosition.y, top) && Math3D.IsGreater(outPortPosition.y, top)) {
			float y1= outPortPosition.y-top;
			float y2= top-inPortPosition.y;
			y= top+(y1*y1/(y1+y2));
		} else {
			float y2= outPortPosition.y-bottom;
			float y1= bottom-inPortPosition.y;
			y= bottom-(y1*y1/(y1+y2));			
		}
		port.SetAnchorAndLayoutPosition(new Vector2(x,y));
		return;			
	}
    // ----------------------------------------------------------------------
	public iCS_EditorObject FindPortWithSourceEndPoint(iCS_EditorObject node, iCS_EditorObject srcEP) {
		// Get all ports that match request (supports connection loops).
		var matchingPorts= node.BuildListOfChildPorts(p=> p.SourceEndPort == srcEP);
		if(matchingPorts.Length == 0) return null;
		if(matchingPorts.Length == 1) return matchingPorts[0];
		foreach(var p in matchingPorts) {
			if(p.IsOutputPort) return p;
		}
		Debug.LogWarning("iCanScript: Invalid circular connection of input ports on: "+node.Name);
		return matchingPorts[0];
	}
    // ----------------------------------------------------------------------
	public void CleanupHangingConnection(iCS_EditorObject port) {
		if(port.Destinations.Length == 0) {
			var src= port.Source;
			DestroyInstance(port);
			if(src != null) {
				CleanupHangingConnection(src);
			}
		}
	}
	
	// ----------------------------------------------------------------------
	public void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= state.Parent;
		bool anEntryExists= false;
		ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			UntilMatchingChild(prevParent,
				child=> {
					if(child.IsState) {
						child.IsEntryState= true;
						return true;
					}
					return false;
				}
			);
		}
	}

	
}
