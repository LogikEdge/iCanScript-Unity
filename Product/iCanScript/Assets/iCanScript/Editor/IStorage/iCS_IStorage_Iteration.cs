using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Editor Object Iteration Utilities
        // ----------------------------------------------------------------------
        public void FilterWith(Func<iCS_EditorObject,bool> cond, Action<iCS_EditorObject> action) {
            Prelude.filterWith(obj=> IsValid(obj) && cond(obj), action, EditorObjects);
        }
        // ----------------------------------------------------------------------
        public List<iCS_EditorObject> Filter(Func<iCS_EditorObject,bool> cond) {
            return Prelude.filter(obj=> IsValid(obj) && cond(obj), EditorObjects);
        }
        // ----------------------------------------------------------------------
    	public int NbOfChildren(iCS_EditorObject parent) {
    		if(!IsValid(parent)) return 0;
    		return parent.Children.Count;
    	}
        // ----------------------------------------------------------------------
    	public int NbOfChildren(iCS_EditorObject parent, Func<iCS_EditorObject, bool> filter) {
    		if(!IsValid(parent)) return 0;
    		int cnt= 0;
    		ForEachChild(parent, c=> { if(filter(c)) ++cnt; });
    		return cnt;
    	}
        // ----------------------------------------------------------------------
        public void ForEachChild(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            if(parent == null) {
                EditorObjects[0].ForEachChild(child=> fnc(child));            
            }
            else {
                parent.ForEachChild(child=> fnc(child));            
            }
        }
        // ----------------------------------------------------------------------
        public bool UntilMatchingChild(iCS_EditorObject parent, Func<iCS_EditorObject,bool> cond) {
            DetectUndoRedo();
            if(parent == null) {
                return EditorObjects[0].UntilMatchingChild(child=> cond(child));            
            }
            else {
                return parent.UntilMatchingChild(child=> cond(child));            
            }
        }
        // ----------------------------------------------------------------------
        public void ForEach(Action<iCS_EditorObject> fnc) {
            Prelude.filterWith(IsValid, fnc, EditorObjects);
        }
        // ----------------------------------------------------------------------
        public void ForEachRecursive(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            ForEachRecursiveDepthLast(parent, fnc);
        }
        // ----------------------------------------------------------------------
        public void ForEachRecursiveDepthLast(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            if(parent == null) {
                EditorObjects[0].ForEachRecursiveDepthLast(child=> fnc(child));                                
            } else {
                parent.ForEachRecursiveDepthLast(child=> fnc(child));                    
            }
        }
        // ----------------------------------------------------------------------
        public void ForEachRecursiveDepthFirst(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            if(parent == null) {
                EditorObjects[0].ForEachRecursiveDepthFirst(child => fnc(child));        
            } else {
                parent.ForEachRecursiveDepthFirst(child=> fnc(child));                    
            }
        }
        // ----------------------------------------------------------------------
        public void ForEachChildRecursive(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            ForEachChildRecursiveDepthLast(parent, fnc);
        }
        // ----------------------------------------------------------------------
        public void ForEachChildRecursiveDepthLast(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            if(parent == null) {
                EditorObjects[0].ForEachRecursiveDepthLast(child=> fnc(child));        
            } else {
                parent.ForEachChildRecursiveDepthLast(child=> fnc(child));                    
            }
        }
        // ----------------------------------------------------------------------
        public void ForEachChildRecursiveDepthFirst(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
            DetectUndoRedo();
            if(parent == null) {
                EditorObjects[0].ForEachRecursiveDepthFirst(child=> fnc(child));                    
            } else {
                parent.ForEachChildRecursiveDepthFirst(child=> fnc(child));        
            }
        }
        // ----------------------------------------------------------------------
        public bool IsChildOf(iCS_EditorObject child, iCS_EditorObject parent) {
            if(!IsValid(child.ParentId)) return false;
            if(child.ParentId == parent.InstanceId) return true;
            return IsChildOf(child.Parent, parent);
        }
        // ----------------------------------------------------------------------
    	public void ForEachNode(Action<iCS_EditorObject> fnc) {
    		ForEach(obj=> { if(obj.IsNode) fnc(obj); } );
    	}
        // ----------------------------------------------------------------------
        public void ForEachChildNode(iCS_EditorObject node, Action<iCS_EditorObject> action) {
            ForEachChild(node, child=> ExecuteIf(child, port=> port.IsNode, action));        
        }
        // ----------------------------------------------------------------------
        public bool UntilMatchingChildNode(iCS_EditorObject node, Func<iCS_EditorObject,bool> fnc) {
            return UntilMatchingChild(node, child=> child.IsNode ? fnc(child) : false);
        }
        // ----------------------------------------------------------------------
        public void ForEachChildPort(iCS_EditorObject node, Action<iCS_EditorObject> action) {
            ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, action));
        }
        // ----------------------------------------------------------------------
        public bool UntilMatchingChildPort(iCS_EditorObject node, Func<iCS_EditorObject,bool> fnc) {
            return UntilMatchingChild(node, child=> child.IsPort ? fnc(child) : false);
        }
        // ----------------------------------------------------------------------
    	public void ForEachChildDataPort(iCS_EditorObject node, Action<iCS_EditorObject> action) {
    		ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsDataOrControlPort, action));
    	}
        // ----------------------------------------------------------------------
        // Returns the first object that matches the given condition.
        public iCS_EditorObject FindFirst(Func<iCS_EditorObject, bool> cond) {
            foreach(var child in EditorObjects) {
                if(IsValid(child)) {
                    if(cond(child)) return child;
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public iCS_EditorObject FindInChildren(iCS_EditorObject parent, Func<iCS_EditorObject, bool> cond) {
            iCS_EditorObject foundChild= null;
            UntilMatchingChild(parent,
                child=> {
                    if(cond(child)) {
                        foundChild= child;
                        return true;
                    }
                    return false;
                }
            );
            return foundChild;
        }
    	// ======================================================================
        // List builders
        // ----------------------------------------------------------------------
        public iCS_EditorObject[] BuildFilteredListOfChildren(Func<iCS_EditorObject,bool> filter, iCS_EditorObject parent) {
            List<iCS_EditorObject> result= new List<iCS_EditorObject>();
            ForEachChild(parent, child=> { if(filter(child)) result.Add(child); });
            return result.ToArray();
        }
        // ----------------------------------------------------------------------
    	public iCS_EditorObject[] GetChildOutputDataPorts(iCS_EditorObject node) {
    		List<iCS_EditorObject> result= new List<iCS_EditorObject>();
    		ForEachChildDataPort(node, child=> ExecuteIf(child, port=> port.IsOutputPort, result.Add));
    		return result.ToArray();
    	}
        // ----------------------------------------------------------------------
    	public iCS_EditorObject[] GetChildInputDataPorts(iCS_EditorObject node) {
    		List<iCS_EditorObject> result= new List<iCS_EditorObject>();
    		ForEachChildDataPort(node, child=> ExecuteIf(child, port=> port.IsInputPort, result.Add));
    		return result.ToArray();
    	}
    }

}
