using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Port Connectivity
        // ----------------------------------------------------------------------
        public void SetSource(iCS_EditorObject obj, iCS_EditorObject producerPort) {
            int id= producerPort == null ? -1 : producerPort.InstanceId;
            if(id != obj.ProducerPortId) {
                obj.ProducerPortId= id; 
                if(id != -1) {
                    GraphEditor.RefreshPortSpec(producerPort);
                }
                else {
                    GraphEditor.SetDefaultPortSpec(obj);
                }
            }
        }
        // ----------------------------------------------------------------------
        public void SetSource(iCS_EditorObject inPort, iCS_EditorObject outPort, LibraryFunction convDesc) {
            if(convDesc == null) {
                SetSource(inPort, outPort);
                return;
            }
            var inPos= inPort.GlobalPosition;
            var outPos= outPort.GlobalPosition;
            Vector2 convPos= new Vector2(0.5f*(inPos.x+outPos.x), 0.5f*(inPos.y+outPos.y));
            int grandParentId= inPort.ParentNode.ParentId;
            iCS_EditorObject conv= CreateNode(grandParentId, convDesc);
            conv.SetInitialPosition(convPos);
            ForEachChild(conv,
                (child) => {
                    if(child.IsInputPort) {
                        SetSource(child, outPort);
                    } else if(child.IsOutputPort) {
                        SetSource(inPort, child);
                    }
                }
            );
        }
        // ----------------------------------------------------------------------
        public void DisconnectPort(iCS_EditorObject port) {
            SetSource(port, null);
            Prelude.forEach(p=> SetSource(p, null), port.ConsumerPorts);        
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject FindAConnectedPort(iCS_EditorObject port) {
            iCS_EditorObject[] connectedPorts= port.ConsumerPorts;
            return connectedPorts.Length != 0 ? connectedPorts[0] : null;
        }
        // ----------------------------------------------------------------------
        public bool IsPortSourced(iCS_EditorObject port) {
            return port.ConsumerPorts.Length != 0;
        }
        // ----------------------------------------------------------------------
        bool IsPortConnected(iCS_EditorObject port) {
            if(port.IsSourceValid) return true;
            if(FindFirst(o=> o.IsPort && o.ProducerPortId == port.InstanceId) != null) return true;
            return false;
        }
        bool IsPortDisconnected(iCS_EditorObject port) { return !IsPortConnected(port); }
        // ----------------------------------------------------------------------
        // Returns the last data port in the connection or NULL if none exist.
        public iCS_EditorObject GetSegmentProducerPort(iCS_EditorObject port) {
            iCS_EngineObject engineObject= Storage.GetSegmentProducerPort(port.EngineObject);
            return engineObject != null ? EditorObjects[engineObject.InstanceId] : null;
        }
    
    
        // ======================================================================
        // Binding Queries
    	// ----------------------------------------------------------------------
        public iCS_EditorObject GetPointToPointProducerPortForConsumerPort(iCS_EditorObject consumerPort) {
            if(consumerPort == null) return null;
            var providerPort= consumerPort.ProducerPort;
            while( providerPort != null ) {
                if(providerPort.ConsumerPorts.Length > 1) break;
                consumerPort= providerPort;
                providerPort= providerPort.ProducerPort;
            }
            return consumerPort; 
        }
    	// ----------------------------------------------------------------------
        public iCS_EditorObject GetPointToPointConsumerPortForProducerPort(iCS_EditorObject providerPort) {
            if(providerPort == null) return null;
            while(providerPort != null) {
                var consumerPorts= providerPort.ConsumerPorts;
                if(consumerPorts == null || consumerPorts.Length == 0) return providerPort;
                if(consumerPorts.Length > 1) return providerPort;
                providerPort= consumerPorts[0];            
            }
            return providerPort;
        }
    	// ----------------------------------------------------------------------
        public iCS_EditorObject[] GetPointToPointConsumerPortsForProducerPort(iCS_EditorObject providerPort) {
            if(providerPort == null) return null;
            var consumerPorts= providerPort.ConsumerPorts;
            if(consumerPorts == null || consumerPorts.Length == 0) return new iCS_EditorObject[0];
            var len= consumerPorts.Length;
            for(int i= 0; i < len; ++i) {
                consumerPorts[i]= GetPointToPointConsumerPortForProducerPort(consumerPorts[i]);
            }
            return consumerPorts;
        }
    
        // ======================================================================
        // Binding Automatic Layout
    	// ----------------------------------------------------------------------
        public void AutoLayoutOfPointToPointBinding(iCS_EditorObject providerPort, iCS_EditorObject consumerPort) {
            if(providerPort == null || consumerPort == null || providerPort == consumerPort) return;
            AutoLayoutOfProducerPort(providerPort, consumerPort);
            AutoLayoutOfConsumerPort(providerPort, consumerPort);
            AutoLayoutOfPointToPointBindingExclusive(providerPort, consumerPort);
        }
    	// ----------------------------------------------------------------------
        public Vector2 GetProducerLineSegmentPosition(iCS_EditorObject port) {
            if(port == null) {
                Debug.LogWarning("iCanScript: GetProducerPosition(...) should not be called with null!");
                return Vector2.zero;
            }
            var providerPort= port.ProducerPort;
            return providerPort == null ? port.ParentNode.GlobalPosition : providerPort.GlobalPosition;
        }
    	// ----------------------------------------------------------------------
        public Vector2 GetConsumerLineSegmentPosition(iCS_EditorObject port) {
            if(port == null) {
                Debug.LogWarning("iCanScript: GetConsumerPosition(...) should not be called with null!");
                return Vector2.zero;
            }
            var consumerPorts= port.ConsumerPorts;
            if(consumerPorts == null || consumerPorts.Length == 0) {
                return port.ParentNode.GlobalPosition;
            }
            Vector2 averagePos= Vector2.zero;
            foreach(var p in consumerPorts) {
                averagePos+= p.GlobalPosition;
            }
            return averagePos / consumerPorts.Length;
        }
    	// ----------------------------------------------------------------------
    	public void AutoLayoutPortOnNode(iCS_EditorObject node) {
    		node.ForEachChildPort(p=> AutoLayoutPort(p));
    	}
    	// ----------------------------------------------------------------------
    	public void AutoLayoutPort(iCS_EditorObject port) {
            var portPos= port.GlobalPosition;		
            // First layout from port to provider
            var providerPort= GetPointToPointProducerPortForConsumerPort(port);
            if(providerPort != null && providerPort != port) {
                var providerLayoutEndPoint= GetProducerLineSegmentPosition(providerPort);
                AutoLayoutPort(providerPort, portPos, providerLayoutEndPoint);
                AutoLayoutOfPointToPointBindingExclusive(providerPort, port);
            }
            // Secondly, layout from port to consumer.
            var consumerPorts= GetPointToPointConsumerPortsForProducerPort(port);
            if(consumerPorts != null) {
                foreach(var consumerPort in consumerPorts) {
                    var consumerLayoutEndPoint= GetConsumerLineSegmentPosition(consumerPort);
                    AutoLayoutPort(consumerPort, portPos, consumerLayoutEndPoint);
                    AutoLayoutOfPointToPointBindingExclusive(port, consumerPort);
                }
            }		
    	}
    	// ----------------------------------------------------------------------
        public bool AutoLayoutPort(iCS_EditorObject port, Vector2 p1, Vector2 p2) {
            if(port == null) return false;
            var parentNode= port.ParentNode;
            // Project port on line segment.
            Vector2 intersection;
            if(Math3D.LineSegmentAndRectEdgeIntersection(p1, p2, parentNode.GlobalRect, out intersection)) {
                if(Math3D.IsNotEqual(port.GlobalPosition, intersection)) {
                    port.LocalAnchorFromGlobalPosition= intersection;
                    return true;
                }
            }
            return false;
        }
    	// ----------------------------------------------------------------------
        public bool AutoLayoutOfProducerPort(iCS_EditorObject providerPort, iCS_EditorObject consumerPort) {
            if(providerPort == null || consumerPort == null || providerPort == consumerPort) return false;
            var providerPos= GetProducerLineSegmentPosition(providerPort);
            var consumerPos= GetConsumerLineSegmentPosition(consumerPort);
            return AutoLayoutPort(providerPort, providerPos, consumerPos);
        }
    	// ----------------------------------------------------------------------
        public bool AutoLayoutOfConsumerPort(iCS_EditorObject providerPort, iCS_EditorObject consumerPort) {
            if(providerPort == null || consumerPort == null || providerPort == consumerPort) return false;
            var providerPos= GetProducerLineSegmentPosition(providerPort);
            var consumerPos= GetConsumerLineSegmentPosition(consumerPort);
            return AutoLayoutPort(consumerPort, providerPos, consumerPos);
        }
    	// ----------------------------------------------------------------------
        public bool AutoLayoutOfPointToPointBindingExclusive(iCS_EditorObject providerPort, iCS_EditorObject consumerPort) {
            if(providerPort == null || consumerPort == null || providerPort == consumerPort) return false;
            bool hasChanged= false;
            var providerPos= providerPort.GlobalPosition;
            var consumerPos= consumerPort.GlobalPosition;
            for(consumerPort= consumerPort.ProducerPort; consumerPort != null && consumerPort != providerPort; consumerPort= consumerPort.ProducerPort) {
                hasChanged |= AutoLayoutPort(consumerPort, providerPos, consumerPos);
            }
            return hasChanged;
        }

        // ======================================================================
        // Binding Utilities
    	// ----------------------------------------------------------------------
        public void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
            iCS_EditorObject oldParent= node.Parent;
            if(newParent == null || newParent == oldParent) return;
    		// Change parent and relayout.
    		var nodePos= node.GlobalPosition;
    		node.Parent= newParent;
    		node.LocalAnchorPosition= nodePos-newParent.GlobalPosition;
    		if(node.IsState) CleanupEntryState(node, oldParent);
            RebuildConnectionsFor(node);
        }
    	// ----------------------------------------------------------------------
        public void SetAndAutoLayoutNewDataConnection(iCS_EditorObject consumerPort, iCS_EditorObject providerPort, LibraryFunction conversion= null) {
            SetNewDataConnection(consumerPort, providerPort, conversion);
            AutoLayoutOfPointToPointBindingExclusive(providerPort, consumerPort);
        }
    	// ----------------------------------------------------------------------
        public void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, LibraryFunction conversion= null) {
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
    			var sourcePort= inPort.ProducerPort;
    			iCS_EditorObject newPort= null;
    			// Attempt to reuse existing ports.
    			if(sourcePort != null && sourcePort.ParentNode == inGrandParent && conversion == null) {
    				newPort= sourcePort;
    			}
    			else {
    				newPort= CreatePort(inPort.DisplayName, inGrandParent.InstanceId, outPort.RuntimeType, VSObjectType.InDynamicDataPort);
    	            SetSource(inPort, newPort, conversion);
    				SetBestPositionForAutocreatedPort(newPort, inPort.GlobalPosition, outPort.GlobalPosition);
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
    					   if(p.IsPort && p.ProducerPort == outPort) {
    						   newPort= p;
    						   return true;
    					   }
    					   return false;
    				   }
    			   )
    			) {}
    			else {
    				newPort= CreatePort(outPort.DisplayName, outGrandParent.InstanceId, outPort.RuntimeType, VSObjectType.OutDynamicDataPort);
    	            SetSource(newPort, outPort, conversion);
    				SetBestPositionForAutocreatedPort(newPort, inPort.GlobalPosition, outPort.GlobalPosition);
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
    		Debug.Log("iCanScript: RebuildDataConnection: output= "+outputPort.DisplayName+" input= "+inputPort.DisplayName);
    #endif
    		// Have we completed rebuilding ... if so return.
    		if(inputPort == outputPort) return;
    		var inputNode= inputPort.ParentNode;
    		var outputNode= outputPort.ParentNode;
    		if(inputNode == outputNode) return;
    		// outputPort is inside the node with the inputPort.
    		var commonParentNode= GraphInfo.GetCommonParent(outputPort, inputPort);
    		if(inputNode == commonParentNode) {
    			// Rebuild moving down from the common parent towards the output port.
    			var newInputNode= outputPort.ParentNode;
    			while(newInputNode != inputNode && newInputNode.ParentNode != inputNode) {
    				newInputNode= newInputNode.ParentNode;
    			}
    			var existingPort= FindPortWithSourceEndPoint(newInputNode, outputPort);
    			if(existingPort != null) {
    				var prevSource= inputPort.ProducerPort;
    				if(prevSource != existingPort) {
    					inputPort.ProducerPort= existingPort;
    					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
    						CleanupHangingConnection(prevSource);
    					}					
    				}
    				RebuildDataConnection(outputPort, existingPort);
    			} else {
    	            iCS_EditorObject newPort= CreatePort(inputPort.DisplayName, newInputNode.InstanceId, inputPort.RuntimeType, VSObjectType.OutDynamicDataPort);
    				SetBestPositionForAutocreatedPort(newPort, outputPort.GlobalPosition, inputPort.GlobalPosition);
    				newPort.ProducerPort= inputPort.ProducerPort;
    				inputPort.ProducerPort= newPort;
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
    				var prevSource= inputPort.ProducerPort;
    				if(prevSource != existingPort) {
    					inputPort.ProducerPort= existingPort;
    					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
    						CleanupHangingConnection(prevSource);
    					}					
    				}
    				RebuildDataConnection(outputPort, existingPort);
    			} else {
    	            iCS_EditorObject newPort= CreatePort(inputPort.DisplayName, newDstNode.InstanceId, inputPort.RuntimeType, VSObjectType.OutDynamicDataPort);
    				SetBestPositionForAutocreatedPort(newPort, outputPort.GlobalPosition, inputPort.GlobalPosition);
    				newPort.ProducerPort= inputPort.ProducerPort;
    				inputPort.ProducerPort= newPort;
    				RebuildDataConnection(outputPort, newPort);				
    			}
    			return;
    		} else {
    			// Rebuilding moving up from the consumer port towards the common parent.
    			var existingPort= FindPortWithSourceEndPoint(inputNodeParent, outputPort);
    			if(existingPort != null) {
    				var prevSource= inputPort.ProducerPort;
    				if(prevSource != existingPort) {
    					inputPort.ProducerPort= existingPort;
    					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
    						CleanupHangingConnection(prevSource);
    					}					
    				}
    				RebuildDataConnection(outputPort, existingPort);
    			} else {
    	            iCS_EditorObject newPort= CreatePort(inputPort.DisplayName, inputNodeParent.InstanceId, inputPort.RuntimeType, VSObjectType.InDynamicDataPort);
    				SetBestPositionForAutocreatedPort(newPort, outputPort.GlobalPosition, inputPort.GlobalPosition);
    				newPort.ProducerPort= inputPort.ProducerPort;
    				inputPort.ProducerPort= newPort;
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
        				var outputPort= p.SegmentProducerPort;
        				foreach(var inputPort in p.SegmentEndConsumerPorts) {
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
            if(fromStatePort == null || toStatePort == null) return;
    		var commonParent= GraphInfo.GetCommonParent(fromStatePort, toStatePort);
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
    		var parentGlobalRect= parent.GlobalRect;
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
    			port.LocalAnchorFromGlobalPosition= new Vector2(x,y);
    			return;
    		}
    		if(Math3D.IsEqual(inPortPosition.y, outPortPosition.y)) {
    			port.LocalAnchorFromGlobalPosition= new Vector2(x, 0.5f*(top+bottom));
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
    		port.LocalAnchorFromGlobalPosition= new Vector2(x,y);
    		return;			
    	}
        // ----------------------------------------------------------------------
    	public iCS_EditorObject FindPortWithSourceEndPoint(iCS_EditorObject node, iCS_EditorObject srcEP) {
    		// Get all ports that match request (supports connection loops).
    		var matchingPorts= node.BuildListOfChildPorts(p=> p.SegmentProducerPort == srcEP);
    		if(matchingPorts.Length == 0) return null;
    		if(matchingPorts.Length == 1) return matchingPorts[0];
    		foreach(var p in matchingPorts) {
    			if(p.IsOutputPort) return p;
    		}
    		Debug.LogWarning("iCanScript: Invalid circular connection of input ports on: "+node.DisplayName);
    		return matchingPorts[0];
    	}
        // ----------------------------------------------------------------------
    	public void CleanupHangingConnection(iCS_EditorObject port) {
    		if(port.ConsumerPorts.Length == 0) {
    			var src= port.ProducerPort;
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

}

