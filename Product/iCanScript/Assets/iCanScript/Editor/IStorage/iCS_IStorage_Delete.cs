using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	bool                    myDestroyQueueInUse = false;
    	List<iCS_EditorObject>	myDestroyQueue      = new List<iCS_EditorObject>();
    	List<iCS_EditorObject>	myCachedDestroyQueue= new List<iCS_EditorObject>();
	
        // ======================================================================
        // ----------------------------------------------------------------------
        public void DestroyInstance(int id) {
    		if(!IsValid(id)) return;
            DestroyInstance(EditorObjects[id]);
        }
        // ----------------------------------------------------------------------
        public void DestroyInstance(iCS_EditorObject eObj) {
            DetectUndoRedo();
            DestroyInstanceInternal(eObj);
        }
        // ----------------------------------------------------------------------
        void DestroyInstanceInternal(iCS_EditorObject toDestroy) {
            if(toDestroy == null) return;
    		ScheduleDestroyInstance(toDestroy);
    		// Avoid reentrancy.
    		if(myDestroyQueueInUse) {
    			return;
    		}
    		// Go through schedule queue to destroy all related objects. 
    		myDestroyQueueInUse= true;
    		while(myDestroyQueue.Count != 0) {
    			foreach(var obj in myDestroyQueue) {
    				DestroySingleObject(obj);
    			}
    			myDestroyQueue.Clear();
    			if(myCachedDestroyQueue.Count != 0) {
    				var tmp= myDestroyQueue;
    				myDestroyQueue= myCachedDestroyQueue;
    				myCachedDestroyQueue= tmp;
    			}			
    		}
    		myDestroyQueueInUse= false;
        }
        // ----------------------------------------------------------------------
    	// This function deletes the given object and any associated object
    	// to maintain a clean graph without dangling connections, ports, or
    	// nodes.
    	void ScheduleDestroyInstance(iCS_EditorObject toDestroy) {
    		if(toDestroy == null || toDestroy.InstanceId == -1) return;
    		// Don't process if the object has already been processed.
    		if(myDestroyQueue.Contains(toDestroy)) return;
    		if(myCachedDestroyQueue.Contains(toDestroy)) return;			
    		// Schedule all children to be destroyed first.
    		toDestroy.ForEachChild(child=> ScheduleDestroyInstance(child));
    		// Add the object to the destroy queue.
    		if(!myDestroyQueueInUse) {
    			myDestroyQueue.Add(toDestroy);			
    		} else {
    			myCachedDestroyQueue.Add(toDestroy);
    		}
    		// Detroy the transition as a single block.
    		if(toDestroy.IsStatePort || toDestroy.IsTransitionPackage || toDestroy.IsTransitionPort) {
    	        iCS_EditorObject outStatePort= GetFromStatePort(toDestroy);
    	        iCS_EditorObject inStatePort= GetInTransitionPort(toDestroy);
    	        iCS_EditorObject transitionPackage= GetTransitionPackage(toDestroy);
    			if(inStatePort != null)       ScheduleDestroyInstance(inStatePort);
    			if(transitionPackage != null) ScheduleDestroyInstance(transitionPackage);
    			if(outStatePort != null)      ScheduleDestroyInstance(outStatePort);
    		}
    	}
        // ----------------------------------------------------------------------
    	void DestroySingleObject(iCS_EditorObject toDestroy) {
    		if(toDestroy == null || toDestroy.InstanceId == -1) return;
            // Disconnect ports linking to this port.
            ExecuteIf(toDestroy, obj=> obj.IsPort, _=> DisconnectPort(toDestroy));
            // Update modules runtime data when removing a module port.
            iCS_EditorObject parent= toDestroy.Parent;
    		if(toDestroy.IsKindOfPackagePort) {
    			MoveDynamicPortToLastIndex(toDestroy);
    		}
            // Remember entry state.
            bool isEntryState= toDestroy.IsEntryState;
    		// Destroy instance.
    		toDestroy.DestroyInstance();
            // Reconfigure parent state if the object removed is an entry state.
            if(isEntryState) {
                SelectEntryState(parent);
            }
    		if(parent != null && parent.IsParentMuxPort) {
    			CleanupMuxPort(parent);
    		}
    		if(parent != null && parent.IsNode) {
    			CleanupEnablePorts(parent);
    		}
    	}
        // ----------------------------------------------------------------------
        void SelectEntryState(iCS_EditorObject parent) {
            bool entryFound= UntilMatchingChild(parent,
                child=> {
                    if(child.IsEntryState) {
                        return true;
                    }
                    return false;
                }
            );        
            if(entryFound) return;
            UntilMatchingChild(parent,
                child=> {
                    if(child.IsState) {
                        child.IsEntryState= true;
                        return true;
                    }
                    return false;
                }
            );
        }
    }

}
