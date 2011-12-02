using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_StateChart : UK_Action {
    // ======================================================================
    // Internal types
    // ----------------------------------------------------------------------
    enum ExecutionState { VerifyingTransition, RunningEntry, RunningExit, RunningUpdate };
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    UK_State        myEntryState    = null;
    List<UK_State>  myActiveStack   = new List<UK_State>();
    List<UK_State>  myChildren      = new List<UK_State>();
    int             myQueueIdx      = 0;
    ExecutionState  myExecutionState= ExecutionState.VerifyingTransition;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public UK_State EntryState {
        get { return myEntryState; }
        set { myEntryState= value; }
    }
    public UK_State ActiveState {
        get {
            int end= myActiveStack.Count;
            return end == 0 ? null : myActiveStack[end-1];
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_StateChart(string name) : base(name) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Make certain that at least one active state exists.
        if(myActiveStack.Count == 0 && myEntryState != null) MoveToState(myEntryState, frameId);
        // Process any active transition.
        if(myExecutionState == ExecutionState.VerifyingTransition) {
            ExecuteTransitions(frameId);            
        }
        // Execute state update functions.
        if(myExecutionState == ExecutionState.RunningUpdate) {
            ExecuteUpdates(frameId);
        }
        MarkAsCurrent(frameId);
    }
    public override void ForceExecute(int frameId) {
        Execute(frameId);
    }
    
    // ----------------------------------------------------------------------
    void ExecuteTransitions(int frameId) {
        // Determine if a transition exists for one of the active states.
        UK_State state= null;
        int end= myActiveStack.Count;
        while(myQueueIdx < end) {
            state= myActiveStack[myQueueIdx];
            UK_VerifyTransitions transitions= state.Transitions;
            transitions.Execute(frameId);
            if(transitions.IsCurrent(frameId)) {
                IsStalled= false;
                UK_Transition firedTransition= transitions.TriggeredTransition;
                if(firedTransition != null && state != ActiveState) {
                    MoveToState(state, frameId);
                    return;
                }
                ++myQueueIdx;
                continue;
            }
            if(!transitions.IsStalled) {
                IsStalled= false;
            }
            return;
        }
        IsStalled= false;
        myQueueIdx= 0;
        myExecutionState= ExecutionState.RunningUpdate;
    }
    // ----------------------------------------------------------------------
    void ExecuteUpdates(int frameId) {
        bool stalled= true;
        while(myQueueIdx < myActiveStack.Count) {
            UK_State state= myActiveStack[myQueueIdx];
            UK_Action action= state.OnUpdateAction;
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
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        myQueueIdx= 0;
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
    void MoveToState(UK_State newState, int frameId) {
        int stackSize= myActiveStack.Count;
        // Determine transition parent node
        UK_State transitionParent= null;
        UK_State toTest= newState;
        int entrySize= -1;
        int idx;
        do {
            ++entrySize;
            for(idx= stackSize-1; idx >= 0; --idx) {
                if(myActiveStack[idx] == toTest) {
                    transitionParent= toTest;
                    break;
                }
            }
            toTest= toTest.ParentState;
            if(toTest == null) {
                ++entrySize;
                break;
            }
        } while(transitionParent == null);
        // Execute exit functions.
        for(idx= stackSize-1; idx >= 0; --idx) {
            UK_State state= myActiveStack[idx];
            if(state == transitionParent) break;
            state.OnExit(frameId);
        }
        int stableSize= idx+1;
        // Update active stack state.
        int newSize= stableSize+entrySize;
        if(newSize < stackSize) myActiveStack.RemoveRange(newSize, stackSize-newSize);
        if(newSize > myActiveStack.Capacity) myActiveStack.Capacity= newSize;
        while(myActiveStack.Count < newSize) myActiveStack.Add(null);
        UK_State toAdd= newState;
        for(int offset= entrySize-1; offset >= 0; --offset) {
            myActiveStack[stableSize+offset]= toAdd;
            toAdd= toAdd.ParentState;            
        }
        // Execute entry functions.
        for(idx= stableSize; idx < newSize; ++idx) {
            myActiveStack[idx].OnEntry(frameId);
        }
    }
    
    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_Object _object) {
        UK_State state= _object as UK_State;
        if(state != null) {
            if(myChildren.Count == 0) myEntryState= state;
            myChildren.Add(state);
        }
    }
    public void RemoveChild(UK_Object _object) {
        UK_State state= _object as UK_State;
        if(state != null) {
            if(state == myEntryState) myEntryState= null;
            myChildren.Remove(state);
        }
    }
}
