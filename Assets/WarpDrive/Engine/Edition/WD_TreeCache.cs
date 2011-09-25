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
        public TreeNode(int parentId, WD_Object rtObj) {
            RuntimeObject= rtObj;
            ParentId= parentId;
        }
        public void Init() {
            ParentId= -1;
            RuntimeObject= null;
            Children.Clear();
        }
        public void Init(int parentId, WD_Object rtObj) {
            ParentId= parentId;
            RuntimeObject= rtObj;
            Children.Clear();
        }
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
    bool IsIdValid(int id)      { return id >= 0 && id < TreeCache.Count; }
    bool IsIdInvalid(int id)    { return !IsIdValid(id); }
    // ----------------------------------------------------------------------
    public void CreateInstance(int id, int parentId, WD_Object rtObj) {
        UpdateInstance(id, parentId, rtObj);
    }
    public void UpdateInstance(int id, int parentId, WD_Object rtObj) {
        // Protect against misuse.
        if(id < 0) return;
        
        // This is an update.
        TreeNode tNd= null;
        if(id < TreeCache.Count && TreeCache[id] != null) {
            tNd= TreeCache[id];
            tNd.RuntimeObject= rtObj;
            if(tNd.ParentId == parentId) return;
            if(IsIdValid(tNd.ParentId)) {
                TreeCache[tNd.ParentId].RemoveChild(id, tNd);
            }
        }
        // This is addition to the tree.
        else {
            while(TreeCache.Count <= id) TreeCache.Add(null);
            tNd= new TreeNode(parentId, rtObj);
            TreeCache[id]= tNd;            
        }

        // Update the parent if it is present.
        if(IsIdValid(parentId) && TreeCache[parentId] != null) {
            TreeCache[parentId].AddChild(id, tNd);
        }
        // Scan for already configured children.
        for(int i= 0; i < TreeCache.Count; ++i) {
            if(TreeCache[i].ParentId == id) {
                tNd.AddChild(i, TreeCache[i]);
            }
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveInstance(int id) {
        if(IsIdInvalid(id)) return;
        
        // Remove from parent.
        TreeNode nd= TreeCache[id];
        if(IsIdValid(nd.ParentId) && TreeCache[nd.ParentId] != null) {
            TreeCache[nd.ParentId].RemoveChild(id, nd);
        }
        RuntimeObject.Dealloc();
        nd.Init();
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
