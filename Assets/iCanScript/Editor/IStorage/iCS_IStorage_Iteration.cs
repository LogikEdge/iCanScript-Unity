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
    public void ForEachChildPort(iCS_EditorObject node, Action<iCS_EditorObject> action) {
        ForEachChild(node, child=> ExecuteIf(child, port=> port.IsPort, action));
    }
    // ----------------------------------------------------------------------
    public bool ForEachChildPort(iCS_EditorObject node, Func<iCS_EditorObject,bool> fnc) {
        return ForEachChild(node, child=> child.IsPort ? fnc(child) : false);
    }
}
