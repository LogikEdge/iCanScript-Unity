using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class UK_IStorage {
    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    public void FilterWith(Func<UK_EditorObject,bool> cond, Action<UK_EditorObject> action) {
        Prelude.filterWith(cond, action, EditorObjects);
    }
    public List<UK_EditorObject> Filter(Func<UK_EditorObject,bool> cond) {
        return Prelude.filter(cond, EditorObjects);
    }
    public void ForEachChild(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public bool ForEachChild(UK_EditorObject parent, Func<UK_EditorObject,bool> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            return TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            return TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public void ForEach(Action<UK_EditorObject> fnc) {
        Prelude.filterWith(IsValid, fnc, EditorObjects);
    }
    public void ForEachRecursive(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        ForEachRecursiveDepthLast(parent, fnc);
    }
    public void ForEachRecursiveDepthLast(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));                                
        } else {
            TreeCache.ForEachRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachRecursiveDepthFirst(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id => fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursive(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ForEachChildRecursiveDepthLast(parent, fnc);
    }
    public void ForEachChildRecursiveDepthLast(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachChildRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursiveDepthFirst(UK_EditorObject parent, Action<UK_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id=> fnc(EditorObjects[id]));                    
        } else {
            TreeCache.ForEachChildRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));        
        }
    }
    // ----------------------------------------------------------------------
    public bool IsChildOf(UK_EditorObject child, UK_EditorObject parent) {
        if(IsInvalid(child.ParentId)) return false;
        if(child.ParentId == parent.InstanceId) return true;
        return IsChildOf(GetParent(child), parent);
    }
}
