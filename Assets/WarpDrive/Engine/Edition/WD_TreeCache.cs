using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WD_TreeCache {
    // ======================================================================
    // Child Classes
    // ----------------------------------------------------------------------
    [System.Serializable]
    public class TreeNode {
        public WD_Object   RuntimeObject= null;
        public int         ParentId= -1;
        public List<int>   Children= new List<int>();

        public TreeNode()  {}
        public void AddChild(int id, TreeNode toAdd) {
            foreach(var child in Children) {
                if(child == id) return;
            }
            Children.Add(id);
            RuntimeObject.AddChild(toAdd.RuntimeObject);
        }
        public void RemoveChild(int id, TreeNode toDelete) {
            for(int i= 0; i < Children.Count; ++i) {
                if(Children[i] == id) {
                    RuntimeObject.RemoveChild(toDelete.RuntimeObject);
                    Children.RemoveAt(i);
                    return;
                }
            }
        }
    }
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public List<TreeNode>   TreeCache= new List<TreeNode>();

    // ======================================================================
    // Tree Cach Containement Functionality
    // ----------------------------------------------------------------------
    public TreeNode this[int i] {
        get { return TreeCache[i]; }
    }
    // ----------------------------------------------------------------------
    bool IsIdValid(int id)      { return id >= 0 && id < TreeCache.Count && TreeCache[id] != null; }
    bool IsIdInvalid(int id)    { return !IsIdValid(id); }
    // ----------------------------------------------------------------------
    public void CreateInstance(int id, int parentId, WD_Object rtObj) {
        // Validate given inputs.
        if(id < 0) {
            Debug.LogError("Connot create a treeNode with id: "+id);            
        }
        if(id < TreeCache.Count && TreeCache[id] != null) {
            Debug.LogError("Trying to create a TreeNode with the same id has an existing TreeNode. (id)=>"+id);
        }
        // Create slots in the tree cache to hold the new instance.
        while(TreeCache.Count <= id) TreeCache.Add(null);
        TreeCache[id]= new TreeNode();
        UpdateInstance(id, parentId, rtObj);
    }
    public void UpdateInstance(int id, int parentId, WD_Object rtObj) {
        // Protect against misuse.
        if(IsIdInvalid(id)) {
            Debug.LogError("Trying to update an invalid TreeNode with id:"+id);
        }
        
        // Remove link to parent if parent has changed.
        TreeNode node= TreeCache[id];
        node.RuntimeObject= rtObj;
        if(node.ParentId != parentId && IsIdValid(node.ParentId)) {
            TreeCache[node.ParentId].RemoveChild(id, node);
        }
        node.ParentId= parentId;

        // Update the parent if it is present.
        if(IsIdValid(parentId)) {
            TreeCache[parentId].AddChild(id, node);
        }
        // Scan for already configured children.
        for(int i= 0; i < TreeCache.Count; ++i) {
            if(TreeCache[i].ParentId == id) {
                node.AddChild(i, TreeCache[i]);
            }
        }
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        if(IsIdInvalid(id)) return;
        
        // Remove from parent.
        TreeNode nd= TreeCache[id];
        if(IsIdValid(nd.ParentId)) {
            TreeCache[nd.ParentId].RemoveChild(id, nd);
        }
        nd.RuntimeObject.Dealloc();
        TreeCache[id]= null;
    }

    // ======================================================================
    // Tree Iterations
    // ----------------------------------------------------------------------
    public void ForEachChild(int id, Action<int> fnc) {
        if(IsIdInvalid(id)) return;
        TreeNode nd= TreeCache[id];
        foreach(var child in nd.Children) {
            fnc(child);
        }
    }
    public void ForEachRecursiveDepthFirst(int id, Action<int> fnc) {
        // Nothing to do if the id is invalid.
        if(IsIdInvalid(id)) return;

        // Don't use the id it is has been removed.
        TreeNode nd= TreeCache[id];
        if(id != 0 && nd.ParentId == -1) return;
        
        // First iterate through all children ...
        foreach(var child in nd.Children) {
            ForEachRecursiveDepthFirst(child, fnc);
        }
        
        // ... then this node.
        fnc(id);
    }
    public void ForEachRecursiveDepthLast(int id, Action<int> fnc) {
        // Nothing to do if the id is invalid
        if(IsIdInvalid(id)) return;

        // Don't use the id it is has been removed.
        TreeNode nd= TreeCache[id];
        if(id != 0 && nd.ParentId == -1) return;

        // First this node ...
        fnc(id);
        
        // ... then iterate through all children.
        foreach(var child in nd.Children) {
            ForEachRecursiveDepthLast(child, fnc);
        }
    }
    public void ForEachChildRecursiveDepthFirst(int id, Action<int> fnc) {
        // Nothing to do if the id is invalid.
        if(IsIdInvalid(id)) return;

        // Don't use the id it is has been removed.
        TreeNode nd= TreeCache[id];
        if(id != 0 && nd.ParentId == -1) return;
        
        // Iterate through all children ...
        foreach(var child in nd.Children) {
            ForEachRecursiveDepthFirst(child, fnc);
        }
    }
    public void ForEachChildRecursiveDepthLast(int id, Action<int> fnc) {
        // Nothing to do if the id is invalid
        if(IsIdInvalid(id)) return;

        // Don't use the id it is has been removed.
        TreeNode nd= TreeCache[id];
        if(id != 0 && nd.ParentId == -1) return;
        
        // Iterate through all children.
        foreach(var child in nd.Children) {
            ForEachRecursiveDepthLast(child, fnc);
        }
    }
}
