using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// --------------------------------------------------------------------------
// An instance of this class is dynamically created from the EditorObjects.
public class iCS_IStorageCache {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public List<iCS_EditorObjectCache>   StorageCache= new List<iCS_EditorObjectCache>();

    // ======================================================================
    // Tree Cach Containement Functionality
    // ----------------------------------------------------------------------
    public iCS_EditorObjectCache this[int i] {
        get { return StorageCache[i]; }
    }
    // ----------------------------------------------------------------------
    public bool IsValid(int id)      { return id >= 0 && id < StorageCache.Count && StorageCache[id].IsValid; }
    public bool IsInvalid(int id)    { return !IsValid(id); }
    // ----------------------------------------------------------------------
    public void CreateInstance(iCS_EditorObject obj) {
        if(obj.InstanceId < StorageCache.Count && StorageCache[obj.InstanceId].IsValid) {
            Debug.LogWarning("Trying to create a TreeNode with the same id has an existing TreeNode. (id)=>"+obj.InstanceId);
        }
        // Create slots in the tree cache to hold the new instance.
        while(StorageCache.Count <= obj.InstanceId) StorageCache.Add(new iCS_EditorObjectCache());
        UpdateInstance(obj);
    }
    public void UpdateInstance(iCS_EditorObject obj) {
        int id      = obj.InstanceId;
        int parentId= obj.ParentId;

        // Protect against misuse.
        if(id < 0 || id >= StorageCache.Count) {
            Debug.LogError("Trying to update an invalid TreeNode with id:"+id);
        }
        
        // Remove link to parent if parent has changed.
        var node= StorageCache[id];
        if(node.ParentId != parentId && IsValid(node.ParentId)) {
            StorageCache[node.ParentId].RemoveChild(id, node);
        }
        node.ParentId= parentId;

        // Update the parent if it is present.
        if(IsValid(parentId)) {
            StorageCache[parentId].AddChild(id, node);
        }
        // Scan for already configured children.
        for(int i= 0; i < StorageCache.Count; ++i) {
            if(StorageCache[i].ParentId == id) {
                node.AddChild(i, StorageCache[i]);
            }
        }
        node.IsValid= true;
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
        if(IsInvalid(id)) return;
        
        // Remove from parent.
        var nd= StorageCache[id];
        if(IsValid(nd.ParentId)) {
            StorageCache[nd.ParentId].RemoveChild(id, nd);
        }
        StorageCache[id].Reset();
    }

}
