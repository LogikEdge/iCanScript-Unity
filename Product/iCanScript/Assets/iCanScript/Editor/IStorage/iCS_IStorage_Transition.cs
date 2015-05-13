using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {    
        // ======================================================================
        // Creation methods
    	// ----------------------------------------------------------------------
        public iCS_EditorObject CreateTransition(iCS_EditorObject fromStatePort, iCS_EditorObject toStatePort) {
            // Determine transition parent
            iCS_EditorObject transitionParent= GetTransitionParent(toStatePort.Parent, fromStatePort.Parent);

            // Create transition module
            iCS_EditorObject transitionPackage= CreatePackage(transitionParent.InstanceId, "false", VSObjectType.TransitionPackage);
            var transitionIconGUID= TextureCache.IconPathToGUID(iCS_EditorStrings.TransitionPackageIcon);
            if(transitionIconGUID != null) {
                transitionPackage.IconGUID= transitionIconGUID;            
            } else {
                Debug.LogWarning("Missing transition module icon: "+iCS_EditorStrings.TransitionPackageIcon);
            }
            transitionPackage.Description= "The transition package must evaluate to 'true' for the transition to fire.";
            iCS_EditorObject inModulePort=  CreatePort("", transitionPackage.InstanceId, typeof(void), VSObjectType.InTransitionPort);
            iCS_EditorObject outModulePort= CreatePort("", transitionPackage.InstanceId, typeof(void), VSObjectType.OutTransitionPort);        
            SetSource(inModulePort, fromStatePort);
            SetSource(toStatePort, outModulePort);
            CreatePort("trigger", transitionPackage.InstanceId, typeof(bool), VSObjectType.OutFixDataPort);

            // Update port names
            UpdatePortNames(fromStatePort, toStatePort);
            return transitionPackage;
        }
        // ----------------------------------------------------------------------
        // Updates the port names of a transition.
        public void UpdatePortNames(iCS_EditorObject fromStatePort, iCS_EditorObject toStatePort) {
            // State ports
            var fromParent= fromStatePort.Parent;
            var toParent  = toStatePort.Parent;
            string statePortName= fromParent.DisplayName+"->"+toParent.DisplayName;
            fromStatePort.DisplayName= statePortName;
            toStatePort.DisplayName  = statePortName;
            // Transition module ports.
            var transitionPackage = GetTransitionPackage(toStatePort);
            var inTransitionPort = GetInTransitionPort(transitionPackage);
            var outTransitionPort= GetOutTransitionPort(transitionPackage);
            inTransitionPort.DisplayName= fromParent.DisplayName+"->"+transitionPackage.DisplayName;
            outTransitionPort.DisplayName= transitionPackage.DisplayName+"->"+toParent.DisplayName;
        }
        // ----------------------------------------------------------------------
        // Returns the common parent of given states.
        public iCS_EditorObject GetTransitionParent(iCS_EditorObject toState, iCS_EditorObject fromState) {
            bool parentFound= false;
            iCS_EditorObject fromParent= null;
            for(fromParent= fromState; fromParent != null; fromParent= fromParent.Parent) {
                iCS_EditorObject toParent= null;
                for(toParent= toState; toParent != null; toParent= toParent.Parent) {
                    if(fromParent == toParent) {
                        parentFound= true;
                        break;
                    }
                }
                if(parentFound) break;
            }
            return fromParent;        
        }
    
        // ======================================================================
        // Transition helpers.
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetFromStatePort(iCS_EditorObject transitionObject) {
    		if(transitionObject == null) {
    			return null;
    		}
            if(transitionObject.IsInStatePort) {
            	iCS_EditorObject source= transitionObject.ProducerPort;
    			if(source == null) {
    				return null;
    			}
                if(source.IsOutStatePort) return source;
                transitionObject= source.Parent;
            }
            if(transitionObject.IsTransitionPackage) {
                if(!UntilMatchingChildPort(transitionObject,
                    child=> {
                        if(child.IsInTransitionPort) {
                            transitionObject= child;
                            return true;
                        }
                        return false;
                    }
                )) {
    				return null;            	
                }
            }
            if(transitionObject.IsInTransitionPort) {
            	iCS_EditorObject source= transitionObject.ProducerPort;
    			if(source == null) {
    				return null;
    			}
    			transitionObject= source;
            }
            if(transitionObject.IsOutStatePort) return transitionObject;
            return null;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetToStatePort(iCS_EditorObject transitionObject) {
    		if(transitionObject.IsOutStatePort) {
    			transitionObject= FindAConnectedPort(transitionObject);
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsInTransitionPort) {
    			transitionObject= transitionObject.ParentNode;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsTransitionPackage) {
    	        UntilMatchingChildPort(transitionObject,
    	            p=> {
    	                if(p.IsOutTransitionPort) {
    	                    transitionObject= p;
    	                    return true;
    	                }
    	                return false;
    	            }
    	        );
    			if(!transitionObject.IsOutTransitionPort) return null;
    		}
    		if(transitionObject.IsOutTransitionPort) {
    			return FindAConnectedPort(transitionObject);
    		}
    		return null;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetInTransitionPort(iCS_EditorObject transitionObject) {
    		if(transitionObject.IsOutStatePort) {
    			transitionObject= FindAConnectedPort(transitionObject);
    			if(transitionObject == null) return null;
    		}
            if(transitionObject.IsInTransitionPort) return transitionObject;
    		if(transitionObject.IsInStatePort) {
    			transitionObject= transitionObject.ProducerPort;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsOutTransitionPort) {
    			transitionObject= transitionObject.ParentNode;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsTransitionPackage) {
    	        UntilMatchingChildPort(transitionObject,
    	            p=> {
    	                if(p.IsInTransitionPort) {
    	                    transitionObject= p;
    	                    return true;
    	                }
    	                return false;
    	            }
    	        );
    			if(transitionObject.IsInTransitionPort) return transitionObject;
    		}
    		return null;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetOutTransitionPort(iCS_EditorObject transitionObject) {
    		if(transitionObject.IsInStatePort) {
    			transitionObject= transitionObject.ProducerPort;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsOutTransitionPort) return transitionObject;
    		if(transitionObject.IsOutStatePort) {
    			transitionObject= FindAConnectedPort(transitionObject);
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsInTransitionPort) {
    			transitionObject= transitionObject.ParentNode;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsTransitionPackage) {
    	        UntilMatchingChildPort(transitionObject,
    	            p=> {
    	                if(p.IsOutTransitionPort) {
    	                    transitionObject= p;
    	                    return true;
    	                }
    	                return false;
    	            }
    	        );
    			if(transitionObject.IsOutTransitionPort) return transitionObject;
    		}
    		return null;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetTransitionPackage(iCS_EditorObject transitionObject) {
    		if(transitionObject == null) return null;
    		if(transitionObject.IsTransitionPackage) return transitionObject;
    		if(transitionObject.IsInStatePort) {
    			transitionObject= transitionObject.ProducerPort;
    			if(transitionObject == null) return null;
    		}
    		if(transitionObject.IsOutStatePort) {
    			transitionObject= FindAConnectedPort(transitionObject);
    			if(transitionObject == null) return null;
    		}
    		transitionObject= transitionObject.ParentNode;
    		if(transitionObject.IsTransitionPackage) return transitionObject;
    		return null;
        }
        // ----------------------------------------------------------------------
        public Vector2 ProposeTransitionPackagePosition(iCS_EditorObject module) {
            iCS_EditorObject fromStatePort= GetFromStatePort(module);
            iCS_EditorObject toStatePort= GetToStatePort(module);
    		if(toStatePort == null || fromStatePort ==null) return module.GlobalPosition;
            iCS_EditorObject parent= module.Parent;
    		var startPos= fromStatePort.GlobalPosition;
    		var endPos= toStatePort.GlobalPosition;
    		var delta= endPos-startPos;
    		var marginSize= iCS_EditorConfig.MarginSize; if(marginSize < 5) marginSize=5;
    		Vector2 step= 0.5f*marginSize*(delta).normalized;
    		var minPos= startPos;
    		var maxPos= endPos;
    		for(; Vector2.Distance(minPos, endPos) >= marginSize; minPos+= step) {
    			if(GetNodeAt(minPos) == parent) {
    				break;
    			}
    		}
    		for(maxPos= minPos+step; Vector2.Distance(maxPos, endPos) >= marginSize; maxPos+= step) {
    			if(GetNodeAt(maxPos) != parent) {
    				break;
    			}
    		}
    		return 0.5f*(minPos+maxPos);
        }
        // ----------------------------------------------------------------------
        public void LayoutTransitionPackage(iCS_EditorObject package) {
            package.LocalAnchorFromGlobalPosition= ProposeTransitionPackagePosition(package);
        }
        // ----------------------------------------------------------------------
        public Vector2 GetTransitionPackageVector(iCS_EditorObject package) {
    		// Preconditions.
    		if(package == null) {
    			Debug.LogWarning("iCanScript: Attempting to get Transition Package Vector with a NULL package");
    			return Vector2.zero;
    		}
            iCS_EditorObject inStatePort      = GetToStatePort(package);
            iCS_EditorObject outStatePort     = GetFromStatePort(package);
            iCS_EditorObject inTransitionPort = GetInTransitionPort(package);
            iCS_EditorObject outTransitionPort= GetOutTransitionPort(package);
            var inStatePos= inStatePort.GlobalPosition;
            var outStatePos= outStatePort.GlobalPosition;
            var inTransitionPos= inTransitionPort.GlobalPosition;
            var outTransitionPos= outTransitionPort.GlobalPosition;
            Vector2 dir= ((inStatePos-outTransitionPos).normalized+(inTransitionPos-outStatePos).normalized).normalized;
            return dir;
        }
    }

}

