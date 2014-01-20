using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class iCS_StateChart : iCS_Action {
    // ======================================================================
    // Internal types
    // ----------------------------------------------------------------------
    enum ExecutionState { VerifyingTransition, RunningEntry, RunningExit, RunningUpdate };
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_State        		myEntryState      = null;
    List<iCS_State>  		myActiveStack     = new List<iCS_State>();
    List<iCS_State>  		myChildren        = new List<iCS_State>();
	iCS_ParallelDispatcher	myDispatcher      = null;
    int              		myQueueIdx        = 0;
    iCS_Transition   		myFiredTransition = null;
    iCS_State        		myNextState       = null;
    iCS_State        		myTransitionParent= null;
    int              		myEntrySize       = -1;
    ExecutionState   		myExecutionState  = ExecutionState.VerifyingTransition;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_State EntryState {
        get { return myEntryState; }
        set { myEntryState= value; }
    }
    public iCS_State ActiveState {
        get {
            int end= myActiveStack.Count;
            return end == 0 ? null : myActiveStack[end-1];
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_StateChart(iCS_VisualScriptImp visualScript, int priority)
    : base(visualScript, priority) {
    	myDispatcher= new iCS_ParallelDispatcher(visualScript, priority, 0, 0);
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Make certain that at least one active state exists.
        if(myActiveStack.Count == 0 && myEntryState != null) MoveToState(myEntryState, frameId);
        // Process any active transition.
        if(myExecutionState == ExecutionState.VerifyingTransition) {
            ExecuteVerifyTransitions(frameId);            
        }
        // Execute state exit functions.
        if(myExecutionState == ExecutionState.RunningExit) {
            ExecuteExits(frameId);
        }
        // Execute state entry functions.
        if(myExecutionState == ExecutionState.RunningEntry) {
            ExecuteEntries(frameId);
        }
        // Execute state update functions.
        if(myExecutionState == ExecutionState.RunningUpdate) {
            ExecuteUpdates(frameId);
        }
		// Attempt to execute all other functions (packge like)
		if(!myDispatcher.IsCurrent(frameId)) {
			myDispatcher.Execute(frameId);			
		}
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Process any active transition.
        if(myExecutionState == ExecutionState.VerifyingTransition) {
            ExecuteVerifyTransitions(frameId, /*forced=*/true);            
        }
        // Execute state exit functions.
        if(myExecutionState == ExecutionState.RunningExit) {
            ExecuteExits(frameId, /*forced=*/true);
        }
        // Execute state entry functions.
        if(myExecutionState == ExecutionState.RunningEntry) {
            ExecuteEntries(frameId, /*forced=*/true);
        }
        // Execute state update functions.
        if(myExecutionState == ExecutionState.RunningUpdate) {
            ExecuteUpdates(frameId, /*forced=*/true);
        }
		// Execute all other functions (packge like)
		if(!myDispatcher.IsCurrent(frameId)) {
			myDispatcher.ForceExecute(frameId);			
		}
    }
    
    // ----------------------------------------------------------------------
    void ExecuteVerifyTransitions(int frameId, bool forced= false) {
        // Determine if a transition exists for one of the active states.
		bool atLeastOneIsStalled = false;
		bool atLeastOneDidExecute= false;
        int end= myActiveStack.Count;
		for(int idx= myQueueIdx; idx < end; ++idx) {
            iCS_State state= myActiveStack[idx];
            iCS_VerifyTransitions transitions= state.Transitions;
			if(!transitions.IsCurrent(frameId)) {
				if(forced) {
		            transitions.ForceExecute(frameId);					
				} else {
		            transitions.Execute(frameId);						
				}
	            if(transitions.IsCurrent(frameId)) {
		            myFiredTransition= transitions.TriggeredTransition;
		            if(myFiredTransition != null && myFiredTransition.EndState != ActiveState) {
		                MoveToState(myFiredTransition.EndState, frameId);
						IsStalled= false;
		                return;
		            }
					if(idx == myQueueIdx) {
						++myQueueIdx;
					}
					atLeastOneDidExecute= true;
				} else {
					if(transitions.IsStalled) {
						atLeastOneIsStalled= true;
					}
				}
			}
		}
		if(atLeastOneIsStalled && (forced || !atLeastOneDidExecute)) {
			IsStalled= true;
			return;
		}
        IsStalled= false;
        myQueueIdx= 0;
        myExecutionState= ExecutionState.RunningUpdate;
    }
    // ----------------------------------------------------------------------
    void ExecuteUpdates(int frameId, bool forced= false) {
		// Run the update of each active state.
		bool atLeastOneIsStalled = false;
		bool atLeastOneDidExecute= false;
        int end= myActiveStack.Count;
		for(int idx= myQueueIdx; idx < end; ++idx) {
            iCS_State state= myActiveStack[idx];
            iCS_Action action= state.OnUpdateAction;
			if(action != null && !action.IsCurrent(frameId)) {
				if(forced) {
					action.ForceExecute(frameId);
				} else {
	                action.Execute(frameId);            						
				}
                if(action.IsCurrent(frameId)) {
					if(idx == myQueueIdx) {
						++myQueueIdx;
					}
					atLeastOneDidExecute= true;
				} else {
					atLeastOneIsStalled= true;
				}
				
			}
		}
		if(atLeastOneIsStalled) {
			IsStalled= !atLeastOneDidExecute;
			return;
		}
        // Reset iterators for next frame.
        IsStalled= false;
        myQueueIdx= 0;
        myExecutionState= ExecutionState.VerifyingTransition;
        myFiredTransition= null;            
        MarkAsExecuted(frameId);
    }
    // ----------------------------------------------------------------------
    void ExecuteExits(int frameId, bool forced= false) {
		// Run the OnExist functions until the common state of the transition.
		bool atLeastOneIsStalled = false;
		bool atLeastOneDidExecute= false;
		for(int idx= myQueueIdx; idx >= 0; --idx) {
            iCS_State state= myActiveStack[idx];
            if(state == myTransitionParent) break;
            iCS_Action action= state.OnExitAction;
			if(action != null && !action.IsCurrent(frameId)) {
				if(forced) {
	                action.ForceExecute(frameId);            
				} else {
	                action.Execute(frameId);		
				}
                if(action.IsCurrent(frameId)) {
					atLeastOneDidExecute= true;
					if(idx == myQueueIdx) {
						--myQueueIdx;
					}
				} else {
					atLeastOneIsStalled= true;
				}
			}
		}
		if(atLeastOneIsStalled) {
			IsStalled= !atLeastOneDidExecute;
			return;
		}
        // Update active stack state.
        UpdateActiveStack();
    }
    // ----------------------------------------------------------------------
    void ExecuteEntries(int frameId, bool forced= false) {
		bool atLeastOneIsStalled = false;
		bool atLeastOneDidExecute= false;
        int end= myActiveStack.Count;
		for(int idx= myQueueIdx; idx < end; ++idx) {
            iCS_State state= myActiveStack[idx];
            iCS_Action action= state.OnEntryAction;
			if(action != null && !action.IsCurrent(frameId)) {
				if(forced) {
	                action.ForceExecute(frameId);            
				} else {
	                action.Execute(frameId);            						
				}
                if(action.IsCurrent(frameId)) {
					atLeastOneDidExecute= true;
					if(idx == myQueueIdx) {
						++myQueueIdx;
					}
				} else {
					atLeastOneIsStalled= true;
				}
			}
		}
		if(atLeastOneIsStalled) {
			IsStalled= !atLeastOneDidExecute;
			return;
		}
        // Prepare to execute update functions
        myExecutionState= ExecutionState.RunningUpdate;
        myQueueIdx= 0;        
    }
    // ----------------------------------------------------------------------
    void MoveToState(iCS_State newState, int frameId) {
        myNextState= newState;
        int stackSize= myActiveStack.Count;
        // Determine transition parent node
        myTransitionParent= null;
        iCS_State toTest= newState;
        myEntrySize= -1;
        int idx;
        do {
            ++myEntrySize;
            for(idx= stackSize-1; idx >= 0; --idx) {
                if(myActiveStack[idx] == toTest) {
                    myTransitionParent= toTest;
                    break;
                }
            }
            toTest= toTest.ParentState;
            if(toTest == null) {
                ++myEntrySize;
                break;
            }
        } while(myTransitionParent == null);
        // Prepare to execute exit functions.
        myExecutionState= ExecutionState.RunningExit;
        myQueueIdx= stackSize-1;
    }
    // ----------------------------------------------------------------------
    void UpdateActiveStack() {
        // Update active stack state.
        int stackSize= myActiveStack.Count;
        int stableSize= myQueueIdx+1;
        int newSize= stableSize+myEntrySize;
        if(newSize < stackSize) myActiveStack.RemoveRange(newSize, stackSize-newSize);
        if(newSize > myActiveStack.Capacity) myActiveStack.Capacity= newSize;
        while(myActiveStack.Count < newSize) myActiveStack.Add(null);
        iCS_State toAdd= myNextState;
        for(int offset= myEntrySize-1; offset >= 0; --offset) {
            myActiveStack[stableSize+offset]= toAdd;
            toAdd= toAdd.ParentState;            
        }
        // Prepare to execute entry
        myExecutionState= ExecutionState.RunningEntry;
        myQueueIdx= stableSize;        
    }
    
    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(iCS_Object _object) {
        Prelude.choice<iCS_State, iCS_Mux>(_object,
        	(state)=> {
	            if(myChildren.Count == 0) myEntryState= state;
	            myChildren.Add(state);
			},
			(mux)=> {
				myDispatcher.AddChild(mux);
			},
			(otherwise)=> {
				Debug.LogWarning("iCanScript: Code Generation: Code from "+_object.Name+" added to State Chart "+this.Name+" is ignored.");			
			}
		);
    }
    public void RemoveChild(iCS_Object _object) {
        Prelude.choice<iCS_State, iCS_Mux>(_object,
        	(state)=> {
	            if(state == myEntryState) myEntryState= null;
	            myChildren.Remove(state);
			},
			(mux)=> {
				myDispatcher.RemoveChild(mux);
			},
			(otherwise)=> {
				Debug.LogWarning("iCanScript: Code Generation: Code from "+_object.Name+" added to State Chart "+this.Name+" is ignored.");			
			}
		);
    }
}
