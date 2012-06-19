using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class iCS_StateChart : iCS_Action {
    // ======================================================================
    // Internal types
    // ----------------------------------------------------------------------
    enum ExecutionState { VerifyingTransition, RunningEntry, RunningExit, RunningUpdate, RunningTransition };
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_State        myEntryState      = null;
    List<iCS_State>  myActiveStack     = new List<iCS_State>();
    List<iCS_State>  myChildren        = new List<iCS_State>();
    int              myQueueIdx        = 0;
    iCS_Transition   myFiredTransition = null;
    iCS_State        myNextState       = null;
    iCS_State        myTransitionParent= null;
    int              myEntrySize       = -1;
    ExecutionState   myExecutionState  = ExecutionState.VerifyingTransition;
    
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
    public iCS_StateChart(string name, Vector2 layout) : base(name, layout) {
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
        // Execute state transition action.
        if(myExecutionState == ExecutionState.RunningTransition) {
            ExecuteTransition(frameId);
        }
        // Execute state entry functions.
        if(myExecutionState == ExecutionState.RunningEntry) {
            ExecuteEntries(frameId);
        }
        // Execute state update functions.
        if(myExecutionState == ExecutionState.RunningUpdate) {
            ExecuteUpdates(frameId);
        }
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Process any active transition.
        if(myExecutionState == ExecutionState.VerifyingTransition) {
            ForceExecuteVerifyTransitions(frameId);            
        }
        // Execute state exit functions.
        if(myExecutionState == ExecutionState.RunningExit) {
            ForceExecuteExits(frameId);
        }
        // Execute state transition action.
        if(myExecutionState == ExecutionState.RunningTransition) {
            ForceExecuteTransition(frameId);
        }
        // Execute state entry functions.
        if(myExecutionState == ExecutionState.RunningEntry) {
            ForceExecuteEntries(frameId);
        }
        // Execute state update functions.
        if(myExecutionState == ExecutionState.RunningUpdate) {
            ForceExecuteUpdates(frameId);
        }
    }
    
    // ----------------------------------------------------------------------
    void ExecuteVerifyTransitions(int frameId) {
        // Determine if a transition exists for one of the active states.
        iCS_State state= null;
        int end= myActiveStack.Count;
        while(myQueueIdx < end) {
            state= myActiveStack[myQueueIdx];
            iCS_VerifyTransitions transitions= state.Transitions;
            transitions.Execute(frameId);
            if(!transitions.IsCurrent(frameId)) {
                if(!transitions.IsStalled) {
                    IsStalled= false;
                }
                return;
            }
            IsStalled= false;
            myFiredTransition= transitions.TriggeredTransition;
            if(myFiredTransition != null && myFiredTransition.EndState != ActiveState) {
                MoveToState(myFiredTransition.EndState, frameId);
                return;
            }
            ++myQueueIdx;
        }
        IsStalled= false;
        myQueueIdx= 0;
        myExecutionState= ExecutionState.RunningUpdate;
    }
    // ----------------------------------------------------------------------
    void ForceExecuteVerifyTransitions(int frameId) {
        // Determine if a transition exists for one of the active states.
        iCS_State state= null;
        int end= myActiveStack.Count;
        if(myQueueIdx < end) {
            state= myActiveStack[myQueueIdx];
            iCS_VerifyTransitions transitions= state.Transitions;
            transitions.ForceExecute(frameId);
            if(!transitions.IsCurrent(frameId)) {
                if(!transitions.IsStalled) {
                    IsStalled= false;
                }
                return;
            }
            IsStalled= false;
            myFiredTransition= transitions.TriggeredTransition;
            if(myFiredTransition != null && myFiredTransition.EndState != ActiveState) {
                MoveToState(myFiredTransition.EndState, frameId);
                return;
            }
        }
        if(++myQueueIdx >= end) {
            myQueueIdx= 0;
            myExecutionState= ExecutionState.RunningUpdate;            
        }
    }
    // ----------------------------------------------------------------------
    void ExecuteTransition(int frameId) {
        if(myFiredTransition != null) {
            iCS_Action action= myFiredTransition.Action;
            if(action != null) {
                action.Execute(frameId);
                if(!action.IsCurrent(frameId)) {
                    IsStalled= action.IsStalled;
                    return;
                }
            }            
        }
        // Prepare for running entry functions.
        // (myQueueIdx already setup in ExecuteExit).
        myExecutionState= ExecutionState.RunningEntry;
    }
    // ----------------------------------------------------------------------
    void ForceExecuteTransition(int frameId) {
        if(myFiredTransition != null) {
            iCS_Action action= myFiredTransition.Action;
            if(action != null) {
                action.ForceExecute(frameId);
                if(!action.IsCurrent(frameId)) {
                    IsStalled= action.IsStalled;
                    return;
                }
            }                    
        }
        // Prepare for running entry functions.
        // (myQueueIdx already setup in ExecuteExit).
        myExecutionState= ExecutionState.RunningEntry;
    }
    // ----------------------------------------------------------------------
    void ExecuteUpdates(int frameId) {
        bool stalled= true;
        while(myQueueIdx < myActiveStack.Count) {
            iCS_State state= myActiveStack[myQueueIdx];
            iCS_Action action= state.OnUpdateAction;
            if(action != null) {
                action.Execute(frameId);            
                if(!action.IsCurrent(frameId)) {
                    // Verify if the child is a staled dispatcher.
                    if(!action.IsStalled) {
                        stalled= false;
                    }                    
                    IsStalled= stalled;
                    return;
                }
                stalled= false;                
            }
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myExecutionState= ExecutionState.VerifyingTransition;
        myFiredTransition= null;            
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
    void ForceExecuteUpdates(int frameId) {
        int stackSize= myActiveStack.Count;
        if(myQueueIdx < stackSize) {
            iCS_State state= myActiveStack[myQueueIdx];
            iCS_Action action= state.OnUpdateAction;
            if(action != null) {
                action.ForceExecute(frameId);            
                if(!action.IsCurrent(frameId)) {
                    // Verify if the child is a staled dispatcher.
                    IsStalled= action.IsStalled;
                    return;
                }
                IsStalled= false;                
            }
        }
        // Reset iterators for next frame.
        if(++myQueueIdx >= stackSize) {
            myQueueIdx= 0;
            myExecutionState= ExecutionState.VerifyingTransition;            
            myFiredTransition= null;            
            MarkAsCurrent(frameId);            
        }
    }
    // ----------------------------------------------------------------------
    void ExecuteExits(int frameId) {
        bool stalled= true;
        while(myQueueIdx >= 0) {
            iCS_State state= myActiveStack[myQueueIdx];
            if(state == myTransitionParent) break;
            iCS_Action action= state.OnExitAction;
            if(action != null) {
                action.Execute(frameId);            
                if(!action.IsCurrent(frameId)) {
                    // Verify if the child is a staled dispatcher.
                    if(!action.IsStalled) {
                        stalled= false;
                    }                    
                    IsStalled= stalled;
                    return;
                }
                stalled= false;                
            }
            --myQueueIdx;
        }
        // Update active stack state.
        UpdateActiveStack();
    }
    // ----------------------------------------------------------------------
    void ForceExecuteExits(int frameId) {
        iCS_State state= null;
        if(myQueueIdx >= 0) {
            state= myActiveStack[myQueueIdx];
            if(state != myTransitionParent) {
                iCS_Action action= state.OnExitAction;
                if(action != null) {
                    action.ForceExecute(frameId);            
                    if(!action.IsCurrent(frameId)) {
                        IsStalled= action.IsStalled;
                        return;
                    }
                    IsStalled= false;                                    
                }
            }
        }
        // Update active stack state.
        if(--myQueueIdx < 0 || state == myTransitionParent) {
            UpdateActiveStack();            
        }
    }
    // ----------------------------------------------------------------------
    void ExecuteEntries(int frameId) {
        bool stalled= true;
        int stackSize= myActiveStack.Count;
        while(myQueueIdx < stackSize) {
            iCS_State state= myActiveStack[myQueueIdx];
            iCS_Action action= state.OnEntryAction;
            if(action != null) {
                action.Execute(frameId);            
                if(!action.IsCurrent(frameId)) {
                    // Verify if the child is a staled dispatcher.
                    if(!action.IsStalled) {
                        stalled= false;
                    }                    
                    IsStalled= stalled;
                    return;
                }
                stalled= false;                
            }
            ++myQueueIdx;
        }
        // Prepare to execute update functions
        myExecutionState= ExecutionState.RunningUpdate;
        myQueueIdx= 0;        
    }
    // ----------------------------------------------------------------------
    void ForceExecuteEntries(int frameId) {
        int stackSize= myActiveStack.Count;
        if(myQueueIdx < stackSize) {
            iCS_State state= myActiveStack[myQueueIdx];
            iCS_Action action= state.OnEntryAction;
            if(action != null) {
                action.ForceExecute(frameId);            
                if(!action.IsCurrent(frameId)) {
                    IsStalled= action.IsStalled;
                    return;
                }
                IsStalled= false;                
            }
        }
        // Prepare to execute update functions
        if(++myQueueIdx >= stackSize) {
            myExecutionState= ExecutionState.RunningUpdate;
            myQueueIdx= 0;                    
        }
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
        myExecutionState= ExecutionState.RunningTransition;
        myQueueIdx= stableSize;        
    }
    
    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(iCS_Object _object) {
        iCS_State state= _object as iCS_State;
        if(state != null) {
            if(myChildren.Count == 0) myEntryState= state;
            myChildren.Add(state);
        }
    }
    public void RemoveChild(iCS_Object _object) {
        iCS_State state= _object as iCS_State;
        if(state != null) {
            if(state == myEntryState) myEntryState= null;
            myChildren.Remove(state);
        }
    }
}
