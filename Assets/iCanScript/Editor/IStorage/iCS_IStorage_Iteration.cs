using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    public void FilterWith(Func<iCS_EditorObject,bool> cond, Action<iCS_EditorObject> action) {
        Prelude.filterWith(cond, action, EditorObjects);
    }
    public List<iCS_EditorObject> Filter(Func<iCS_EditorObject,bool> cond) {
        return Prelude.filter(cond, EditorObjects);
    }
	public int NbOfChildren(iCS_EditorObject parent) {
		if(!IsValid(parent)) return 0;
		return TreeCache[parent.InstanceId].Children.Count;
	}
	public int NbOfChildren(iCS_EditorObject parent, Func<iCS_EditorObject, bool> filter) {
		if(!IsValid(parent)) return 0;
		int cnt= 0;
		ForEachChild(parent, c=> { if(filter(c)) ++cnt; });
		return cnt;
	}
    public void ForEachChild(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public bool ForEachChild(iCS_EditorObject parent, Func<iCS_EditorObject,bool> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            return TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            return TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public void ForEach(Action<iCS_EditorObject> fnc) {
        Prelude.filterWith(IsValid, fnc, EditorObjects);
    }
    public void ForEachRecursive(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        ForEachRecursiveDepthLast(parent, fnc);
    }
    public void ForEachRecursiveDepthLast(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));                                
        } else {
            TreeCache.ForEachRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachRecursiveDepthFirst(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id => fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursive(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ForEachChildRecursiveDepthLast(parent, fnc);
    }
    public void ForEachChildRecursiveDepthLast(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachChildRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursiveDepthFirst(iCS_EditorObject parent, Action<iCS_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id=> fnc(EditorObjects[id]));                    
        } else {
            TreeCache.ForEachChildRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));        
        }
    }
    // ----------------------------------------------------------------------
    public bool IsChildOf(iCS_EditorObject child, iCS_EditorObject parent) {
        if(IsInvalid(child.ParentId)) return false;
        if(child.ParentId == parent.InstanceId) return true;
        return IsChildOf(GetParent(child), parent);
    }
    // ----------------------------------------------------------------------
    public void ForEachChildNode(iCS_EditorObject node, Action<iCS_EditorObject> action) {
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsNode, action));        
    }
    // ----------------------------------------------------------------------
    public bool ForEachChildNode(iCS_EditorObject node, Func<iCS_EditorObject,bool> fnc) {
        return ForEachChild(node, child=> child.IsNode ? fnc(child) : false);
    }
    // ----------------------------------------------------------------------
    public void ForEachChildPort(iCS_EditorObject node, Action<iCS_EditorObject> action) {
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, action));
    }
    // ----------------------------------------------------------------------
    public bool ForEachChildPort(iCS_EditorObject node, Func<iCS_EditorObject,bool> fnc) {
        return ForEachChild(node, child=> child.IsPort ? fnc(child) : false);
    }
    // ----------------------------------------------------------------------
	public void ForEachChildDataPort(iCS_EditorObject node, Action<iCS_EditorObject> action) {
		ForEachChildPort(node, child=> ExecuteIf(child, port=> port.IsDataPort, action));
	}
    // ----------------------------------------------------------------------
    public iCS_EditorObject FindInChildren(iCS_EditorObject parent, Func<iCS_EditorObject, bool> cond) {
        iCS_EditorObject foundChild= null;
        ForEachChild(parent,
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
    public iCS_EditorObject[] BuildListOfChildren(Func<iCS_EditorObject,bool> filter, iCS_EditorObject parent) {
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

	// ======================================================================
	// High-order functions
    // ----------------------------------------------------------------------
	public iCS_EditorObject[] GetSortedChildDataPorts(iCS_EditorObject node) {
		List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
		// Get all child data ports.
		ForEachChildDataPort(node, child=> ports.Add(child));
		// Sort child ports according to index.
		iCS_EditorObject[] result= ports.ToArray();
		Array.Sort(result, (x,y)=> x.PortIndex - y.PortIndex);
        for(int i= 0; i < result.Length; ++i) result[i].PortIndex= i;
		return result;
	}
    public iCS_EditorObject FindThisInputPort(iCS_EditorObject node) {
        return FindInChildren(node, c=> c.IsInDataPort && c.Name == "this");
    }
}
