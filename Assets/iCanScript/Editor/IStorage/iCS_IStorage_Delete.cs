using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	List<iCS_EditorObject>	myDestroyQueue= new List<iCS_EditorObject>();
	
    // ======================================================================
    // ----------------------------------------------------------------------
    public void DestroyInstance(int id) {
		if(IsInvalid(id)) return;
        DestroyInstance(EditorObjects[id]);
    }
    // ----------------------------------------------------------------------
    public void DestroyInstance(iCS_EditorObject eObj) {
        ProcessUndoRedo();
        DestroyInstanceInternal(eObj);
    }
    // ----------------------------------------------------------------------
    void DestroyInstanceInternal(iCS_EditorObject toDestroy) {
        if(toDestroy == null || IsInvalid(toDestroy.InstanceId)) return;
		ScheduleDestroyInstance(toDestroy);
		foreach(var obj in myDestroyQueue) {
			DestroySingleObject(obj);
		}
		myDestroyQueue.Clear();
    }
    // ----------------------------------------------------------------------
	void ScheduleDestroyInstance(iCS_EditorObject toDestroy) {
		if(toDestroy == null || toDestroy.InstanceId == -1) return;
		// Don't process if the object has already been processed.
		if(myDestroyQueue.Contains(toDestroy)) return;
		// Schedule all children to be destroyed first.
		int id= toDestroy.InstanceId;
		TreeCache.ForEachChild(id, child=> ScheduleDestroyInstance(EditorObjects[child]));
		// Add the object to the destroy queue.
		myDestroyQueue.Add(toDestroy);
		// Detroy the transition as a single block.
		if(toDestroy.IsStatePort || toDestroy.IsTransitionModule) {
	        iCS_EditorObject outStatePort= GetOutStatePort(toDestroy);
	        iCS_EditorObject inStatePort= GetInTransitionPort(outStatePort);
	        iCS_EditorObject transitionModule= GetTransitionModule(outStatePort);
			ScheduleDestroyInstance(inStatePort);
			ScheduleDestroyInstance(transitionModule);
			ScheduleDestroyInstance(outStatePort);
		}
	}
    // ----------------------------------------------------------------------
	void DestroySingleObject(iCS_EditorObject toDestroy) {
		if(toDestroy == null || toDestroy.InstanceId == -1) return;
        // Disconnect ports linking to this port.
        ExecuteIf(toDestroy, WD.IsPort, _=> DisconnectPort(toDestroy));
        // Update modules runtime data when removing a module port.
        iCS_EditorObject parent= GetParent(toDestroy);
        if(toDestroy.IsModulePort && parent.IsModule) RemovePortFromModule(toDestroy);
        // Remember entry state.
        bool isEntryState= toDestroy.IsEntryState;
        // Set the parent dirty to force a relayout.
        if(IsValid(toDestroy.ParentId)) SetDirty(parent);
		// Destroy instance.
		TreeCache.DestroyInstance(toDestroy.InstanceId);
		toDestroy.Reset();
        // Reconfigure parent state if the object removed is an entry state.
        if(isEntryState) {
            SelectEntryState(parent);
        }
        myIsDirty= true;
	}
    // ----------------------------------------------------------------------
    void SelectEntryState(iCS_EditorObject parent) {
        bool entryFound= ForEachChild(parent,
            child=> {
                if(child.IsEntryState) {
                    return true;
                }
                return false;
            }
        );        
        if(entryFound) return;
        ForEachChild(parent,
            child=> {
                if(child.IsState) {
                    child.IsRawEntryState= true;
                    return true;
                }
                return false;
            }
        );
    }
}
