using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class WD_IStorage {
    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    public void FilterWith(Func<WD_EditorObject,bool> cond, Action<WD_EditorObject> action) {
        Prelude.filterWith(cond, action, EditorObjects);
    }
    public List<WD_EditorObject> Filter(Func<WD_EditorObject,bool> cond) {
        return Prelude.filter(cond, EditorObjects);
    }
    public void ForEachChild(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public bool ForEachChild(WD_EditorObject parent, Func<WD_EditorObject,bool> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            return TreeCache.ForEachChild(id=> fnc(EditorObjects[id]));            
        }
        else {
            return TreeCache.ForEachChild(parent.InstanceId, id=> fnc(EditorObjects[id]));            
        }
    }
    public void ForEach(Action<WD_EditorObject> fnc) {
        Prelude.filterWith(IsValid, fnc, EditorObjects);
    }
    public void ForEachRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        ForEachRecursiveDepthLast(parent, fnc);
    }
    public void ForEachRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));                                
        } else {
            TreeCache.ForEachRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id => fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursive(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ForEachChildRecursiveDepthLast(parent, fnc);
    }
    public void ForEachChildRecursiveDepthLast(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthLast(id=> fnc(EditorObjects[id]));        
        } else {
            TreeCache.ForEachChildRecursiveDepthLast(parent.InstanceId, id=> fnc(EditorObjects[id]));                    
        }
    }
    public void ForEachChildRecursiveDepthFirst(WD_EditorObject parent, Action<WD_EditorObject> fnc) {
        ProcessUndoRedo();
        if(parent == null) {
            TreeCache.ForEachRecursiveDepthFirst(id=> fnc(EditorObjects[id]));                    
        } else {
            TreeCache.ForEachChildRecursiveDepthFirst(parent.InstanceId, id=> fnc(EditorObjects[id]));        
        }
    }
    // ----------------------------------------------------------------------
    public bool IsChildOf(WD_EditorObject child, WD_EditorObject parent) {
        if(IsInvalid(child.ParentId)) return false;
        if(child.ParentId == parent.InstanceId) return true;
        return IsChildOf(GetParent(child), parent);
    }
}
