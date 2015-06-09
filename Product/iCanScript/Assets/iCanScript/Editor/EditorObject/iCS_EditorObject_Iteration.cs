using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //  ITERATION
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public partial class iCS_EditorObject {
    	// Hierarchy Queries ====================================================
    	public bool IsParentOf(iCS_EditorObject obj) {
    		if(obj == null) return false;
    		var parent= obj.Parent;
    		if(parent == this) return true;
    		return IsParentOf(parent);
    	}
        public bool IsAboveOrEqualInHierarchyTo(iCS_EditorObject node) {
            if(node == this) return true;
            return IsAboveInHierarchyTo(node);
        }
        public bool IsAboveInHierarchyTo(iCS_EditorObject node) {
            return !IsBelowOrEqualInHierarchyTo(node);
        }
        public bool IsBelowOrEqualInHierarchyTo(iCS_EditorObject node) {
            if(node == this) return true;
            return IsBelowInHierarchyTo(node);
        }
        public bool IsBelowInHierarchyTo(iCS_EditorObject node) {
            bool result= false;
            node.ForEachChildRecursiveDepthLast(
                c=> {
                    if(c == this) {
                        result= true;
                    }
                }
            );
            return result;
        }
    	// Queries ==============================================================
    	public bool HasChildNode() {
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsNode) return true;
            }
    		return false;
    	}
        // ----------------------------------------------------------------------
        public int NumberOfChildNodes() {
            int nbOfChildNodes= 0;
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsNode) {
                    ++nbOfChildNodes;
                }
            }
            return nbOfChildNodes;
        }
        // ----------------------------------------------------------------------
    	public bool HasChildPort() {
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsPort) return true;
            }
    		return false;
    	}
        // ---------------------------------------------------------------------------------
        public int NumberOfChildPorts() {
            int count= 0;
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsPort) ++count;
            }
    		return count;        
        }
        // ---------------------------------------------------------------------------------
        public int NumberOfChildInputPorts() {
            int count= 0;
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsInputPort) ++count;
            }
    		return count;        
        }
        // ---------------------------------------------------------------------------------
        public int NumberOfChildOutputPorts() {
            int count= 0;
            foreach(var childId in Children) {
                if(IsIdValid(childId) && EditorObjects[childId].IsOutputPort) ++count;
            }
    		return count;                
        }
        // ---------------------------------------------------------------------------------
        public int NumberOfFunctionOutputPorts() {
            int count= 0;
            foreach(var childId in Children) {
                if(IsIdValid(childId)) {
                    var vsObj= EditorObjects[childId];
                    if(vsObj.IsOutputPort && !vsObj.IsSelfPort && !vsObj.IsTriggerPort) {
                        ++count;                    
                    }
                }
            }
    		return count;                
        }
        // ---------------------------------------------------------------------------------
        public iCS_EditorObject TopObjectInstanceNode() {
            iCS_EditorObject objInstance= null;
            ForEachParentNode(p=> { if(p.IsInstanceNode) objInstance= p; });
            return objInstance;
        }

        // Children Iterations =================================================
        public void ForEachParentNode(Action<iCS_EditorObject> fnc) {
    		var parent= ParentNode;
    		if(parent == null) return;
    		fnc(parent);
    		parent.ForEachParentNode(fnc);
        }
        // ----------------------------------------------------------------------
        public void ForEachChild(Action<iCS_EditorObject> fnc) {
            foreach(var childId in Children) {
                if(IsIdValid(childId)) {
                    fnc(EditorObjects[childId]);
                }
            }
        }
        // ----------------------------------------------------------------------
        public bool UntilMatchingChild(Func<iCS_EditorObject,bool> fnc) {
            foreach(var childId in Children) {
                if(IsIdValid(childId) && fnc(EditorObjects[childId])) return true;
            }
            return false;
        }

        // Node Iterations ======================================================
        public void ForEachChildNode(Action<iCS_EditorObject> action) {
            ForEachChild(o=> { if(o.IsNode) action(o); });
        }
    	// ----------------------------------------------------------------------
        public int NbOfChildNodes {
    		get {
    			int cnt= 0;
    			ForEachChildNode(_=> ++cnt);
    			return cnt;
    		}
    	}

        // Port Iterations ======================================================
        public void ForEachChildPort(Action<iCS_EditorObject> action) {
            ForEachChild(o=> { if(o.IsPort) action(o); });
        }
        // ----------------------------------------------------------------------
        public void ForEachLeftChildPort(Action<iCS_EditorObject> action) {
            ForEachChildPort(o=> { if(o.IsOnLeftEdge && !o.IsFloating) action(o); });
        }
        // ----------------------------------------------------------------------
        public void ForEachRightChildPort(Action<iCS_EditorObject> action) {
            ForEachChildPort(o=> { if(o.IsOnRightEdge && !o.IsFloating) action(o); });
        }
        // ----------------------------------------------------------------------
        public void ForEachTopChildPort(Action<iCS_EditorObject> action) {
            ForEachChildPort(o=> { if(o.IsOnTopEdge && !o.IsFloating) action(o); });
        }
        // ----------------------------------------------------------------------
        public void ForEachBottomChildPort(Action<iCS_EditorObject> action) {
            ForEachChildPort(o=> { if(o.IsOnBottomEdge && !o.IsFloating) action(o); });
        }
    	// ----------------------------------------------------------------------
        public int NbOfTopPorts {
    		get {
    			int cnt= 0;
    			ForEachTopChildPort(_=> ++cnt);
    			return cnt;
    		}
    	}
    	// ----------------------------------------------------------------------
        public int NbOfBottomPorts {
    		get {
    			int cnt= 0;
    			ForEachBottomChildPort(_=> ++cnt);
    			return cnt;
    		}
    	}
    	// ----------------------------------------------------------------------
        public int NbOfLeftPorts {
    		get {
    			int cnt= 0;
    			ForEachLeftChildPort(_=> ++cnt);
    			return cnt;
    		}
    	}
    	// ----------------------------------------------------------------------
        public int NbOfRightPorts {
    		get {
    			int cnt= 0;
    			ForEachRightChildPort(_=> ++cnt);
    			return cnt;
    		}
    	}
    	// ----------------------------------------------------------------------
        public iCS_EditorObject[] TopPorts {
    		get {
    			return BuildListOfChildPorts(c=> c.IsOnTopEdge && !c.IsFloating);
    		}
    	}
    	// ----------------------------------------------------------------------
        public iCS_EditorObject[] BottomPorts {
    		get {
    			return BuildListOfChildPorts(c=> c.IsOnBottomEdge && !c.IsFloating);
    		}
    	}
    	// ----------------------------------------------------------------------
        public iCS_EditorObject[] LeftPorts {
    		get {
    			return BuildListOfChildPorts(c=> c.IsOnLeftEdge && !c.IsFloating);
    		}
    	}
    	// ----------------------------------------------------------------------
        public iCS_EditorObject[] RightPorts {
    		get {
    			return BuildListOfChildPorts(c=> c.IsOnRightEdge && !c.IsFloating);
    		}
    	}
	    
        // Global Iterations =================================================
    	public void ForEach(Action<iCS_EditorObject> fnc) {
    		FilterWith((_,__)=> true, fnc);
    	}
    	// ----------------------------------------------------------------------
    	public void FilterWith(Func<iCS_EditorObject,
    						        Action<iCS_EditorObject>,
    							    bool> cond,
    						   Action<iCS_EditorObject> fnc) {
    		P.filterWith(
    			c=> c != null && c.IsValid && cond(c,fnc),
    			fnc,
    			EditorObjects
    		);
    	}
    	// ----------------------------------------------------------------------
    	public List<iCS_EditorObject> Filter(Func<iCS_EditorObject,bool> cond) {
    		return P.filter(
    			c=> c != null && c.IsValid && cond(c),
    			EditorObjects
    		);
    	}
    	// ----------------------------------------------------------------------
    	public List<iCS_EditorObject> FilterChildRecursive(Func<iCS_EditorObject,bool> cond) {
    		var filtered= new List<iCS_EditorObject>();
    		ForEachChildRecursiveDepthFirst(
    			c=> {
    				if(cond(c)) {
    					filtered.Add(c);
    				}
    			}
    		);
    		return filtered;
    	}
    	// ----------------------------------------------------------------------
        public void ForEachRecursiveDepthFirst(Action<iCS_EditorObject> fnc) {
            ForEachRecursiveDepthFirst((_,__)=> true, fnc);
        }
    	// ----------------------------------------------------------------------
        public void ForEachRecursiveDepthFirst(Func<iCS_EditorObject,
    												Action<iCS_EditorObject>,
    												bool> cond,
                                               Action<iCS_EditorObject> fnc) {
            // Does this node pass the given condition?
            if(!cond(this, fnc)) return;
            // First iterate through all children ...
            foreach(var childId in Children) {
    			if(IsIdValid(childId)) {
    				var child= EditorObjects[childId];
    				if(child != null) {
    		            child.ForEachRecursiveDepthFirst(cond, fnc);									
    				} else {
    					Debug.LogWarning("iCanScript: Mismatch between children list and EditorObject container !!!");
    				}
    			} else {
    				Debug.LogWarning("iCanScript: Children list includes an invalid id");
    			}
            }
            // ... then this node.
            fnc(this);
        }
    	// ----------------------------------------------------------------------
        public void ForEachRecursiveDepthLast(Action<iCS_EditorObject> fnc) {
            ForEachRecursiveDepthLast((_,__)=> true, fnc);
        }
    	// ----------------------------------------------------------------------
        public void ForEachRecursiveDepthLast(Func<iCS_EditorObject,
    										  Action<iCS_EditorObject>,
    										  bool> cond,
                                              Action<iCS_EditorObject> fnc) {
            // Does this node pass the given condition?
            if(!cond(this, fnc)) return;
            // First this node ...
            fnc(this);
            // ... then iterate through all children.
            foreach(var childId in Children) {
    			if(IsIdValid(childId)) {
    				var child= EditorObjects[childId];
    				if(child != null) {
    		            child.ForEachRecursiveDepthLast(cond, fnc);									
    				} else {
    					Debug.LogWarning("iCanScript: Mismatch between children list and EditorObject container !!!");
    				}
    			} else {
    				Debug.LogWarning("iCanScript: Children list includes an invalid id");
    			}
            }
        }
        // ----------------------------------------------------------------------
        // Recursively iterating through all child nodes invoking the given
        // function on the leaf node first then the branch nodes.
        public void ForEachChildRecursiveDepthFirst(Action<iCS_EditorObject> fnc) {
            ForEachChildRecursiveDepthFirst((_,__)=> true, fnc);
        }
        // ----------------------------------------------------------------------
        // Recursively iterating through all child nodes invoking the given
        // function on the leaf node first then the branch nodes.
        public void ForEachChildRecursiveDepthFirst(Func<iCS_EditorObject,
    													 Action<iCS_EditorObject>,
    													 bool> cond,
                                                    Action<iCS_EditorObject> fnc) {
            // Iterate through all children ...
            foreach(var childId in Children) {
    			if(IsIdValid(childId)) {
    				var child= myIStorage[childId];
    				if(child != null) {
    		            child.ForEachRecursiveDepthFirst(cond, fnc);
    				} else {
    					Debug.LogWarning("iCanScript: Mismatch between children list and EditorObject container !!!");
    				}
    			} else {
    				Debug.LogWarning("iCanScript: Children list includes an invalid id");
    			}
            }
        }
        // ----------------------------------------------------------------------
        // Recursively iterating through all child nodes invoking the given
        // function on the branch node first then the leaf nodes.
        public void ForEachChildRecursiveDepthLast(Action<iCS_EditorObject> fnc) {
            ForEachChildRecursiveDepthLast((_,__)=> true, fnc);
        }
        // ----------------------------------------------------------------------
        // Recursively iterating through all child nodes invoking the given
        // function on the branch node first then the leaf nodes.
        public void ForEachChildRecursiveDepthLast(Func<iCS_EditorObject,
    													Action<iCS_EditorObject>,
    													bool> cond,
                                                   Action<iCS_EditorObject> fnc) {
            // Iterate through all children.
            foreach(var childId in Children) {
    			if(IsIdValid(childId)) {
    				var child= myIStorage[childId];
    				if(child != null) {
                		child.ForEachRecursiveDepthLast(cond, fnc);
    				} else {
    					Debug.LogWarning("iCanScript: Mismatch between children list and EditorObject container !!!");
    				}
    			} else {
    				Debug.LogWarning("iCanScript: Children list includes an invalid id");
    			}
            }
        }

        // Speciality Iterations ================================================
        /// Exceutes the given action for each connected producer TypeCast node.
        public void ForEachConnectedProducerTypeCast(Action<iCS_EditorObject> action) {
            ForEachChildPort(
                p=> {
                    if(p.IsInDataOrControlPort) {
                        var producer= p.ProducerPort;
                        if(producer != null) {
                            var parent= producer.Parent;
                            if(parent.IsTypeCast) {
                                action(parent);
                            }                        
                        }
                    }
                }
            );
        }

        // ======================================================================
    	// List builders.
        // ----------------------------------------------------------------------
    	// Build list of children that satisfies the given criteria.
        public iCS_EditorObject[] BuildListOfChildren(Func<iCS_EditorObject, bool> cond) {
            var result= new List<iCS_EditorObject>();
            ForEachChild(c=> { if(cond(c)) result.Add(c); });
            return result.ToArray();
        }
        // ----------------------------------------------------------------------
        // Build a list of child nodes that satisfies the given criteria.
    	public iCS_EditorObject[] BuildListOfChildNodes(Func<iCS_EditorObject, bool> cond) {
    		return BuildListOfChildren(c=> c.IsNode && cond(c));
    	}
        // ----------------------------------------------------------------------
        // Build a list of child nodes that satisfies the given criteria.
    	public iCS_EditorObject[] BuildListOfVisibleChildNodes(Func<iCS_EditorObject, bool> cond) {
    		return BuildListOfChildren(c=> c.IsNode && !c.IsHidden && cond(c));
    	}
        // ----------------------------------------------------------------------
        // Build a list of ports that satisfies the given criteria.
    	public iCS_EditorObject[] BuildListOfChildPorts(Func<iCS_EditorObject, bool> cond) {
    		return BuildListOfChildren(c=> c.IsPort && cond(c));
    	}
        // ----------------------------------------------------------------------
        // Build a list of ports on the same edge.
        public iCS_EditorObject[] BuildListOfPortsOnSameEdge() {
            Func<iCS_EditorObject,bool> cond= null;
            switch(Edge) {
                case iCS_EdgeEnum.Left:     cond= p=> p.IsOnLeftEdge;   break;
                case iCS_EdgeEnum.Right:    cond= p=> p.IsOnRightEdge;  break;
                case iCS_EdgeEnum.Top:      cond= p=> p.IsOnTopEdge;    break;
                case iCS_EdgeEnum.Bottom:   cond= p=> p.IsOnBottomEdge; break;
                default: break;
            }
            if(cond == null) return new iCS_EditorObject[0];
            return ParentNode.BuildListOfChildPorts(cond);
        }
        // ----------------------------------------------------------------------
        // Builds a list of parent nodes.  The list is sorted from the top most
        // parent to the bottom most leaf.
        public iCS_EditorObject[] BuildListOfParentNodes() {
            return GraphInfo.BuildListOfParentNodes(this);
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject GetParentStateChart() {
            var parent= ParentNode;
            while(parent != null && !parent.IsStateChart) {
                parent= parent.ParentNode;
            }
            return parent;
        }
        // ----------------------------------------------------------------------
        // Return the first visible node.
        public iCS_EditorObject GetLeafVisibleNode() {
            var firstVisible= this;
            while(firstVisible.IsVisibleInLayout == false) {
                firstVisible= firstVisible.ParentNode;
            }
            return firstVisible;
        }
        // ----------------------------------------------------------------------
        // Return the first visible node.
        public iCS_EditorObject GetRootInvisibleNode() {
            var rootInvisible= this;
            if(rootInvisible.IsVisibleInLayout) {
                Debug.LogWarning("iCanScript: Unable to find root invisible node");
                return rootInvisible;
            }
            for(var tmp= rootInvisible.ParentNode; tmp != null && !tmp.IsVisibleInLayout; tmp= tmp.ParentNode) {
                rootInvisible= tmp;
            }
            return rootInvisible;
        }

        // ----------------------------------------------------------------------
		/// Returns the ordered list of parameter ports.
		///
		/// @return The array of parameter ports ordered as per the port index.
		///
		public iCS_EditorObject[] GetParameterPorts() {
			var paramPorts= BuildListOfChildPorts(
				p=> {
					var portIndex= p.PortIndex;
					return portIndex >= (int)iCS_PortIndex.ParametersStart && portIndex < (int)iCS_PortIndex.ParametersEnd;
				}
			);
			Array.Sort(paramPorts, (x,y)=> y.PortIndex-x.PortIndex);
			return paramPorts;
		}
        // ----------------------------------------------------------------------
        /// Returns the return port (if it exists).
        ///
        /// @return The return port if found. _null_ otherwise.
        ///
        public iCS_EditorObject GetReturnPort() {
            return GetPortWithIndex((int)iCS_PortIndex.Return);
        }
        // ----------------------------------------------------------------------
        /// Returns the port with the given port index.
        ///
        /// @return The found port or _null_ otherwise.
        ///
        public iCS_EditorObject GetPortWithIndex(int portIndex) {
            iCS_EditorObject result= null;
            ForEachChildPort(
                p=> {
                    if(p.PortIndex == portIndex) {
                        result= p;
                    }
                }
            );
            return result;
        }
    }
}
