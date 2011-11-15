using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_StateChart : WD_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public  WD_State        myEntryState = null;
    private List<WD_State>  myActiveStack= new List<WD_State>();
    public  List<WD_State>  myChildren   = new List<WD_State>();
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public WD_State EntryState {
        get { return myEntryState; }
        set { myEntryState= value; }
    }
    public WD_State ActiveState {
        get {
            int end= myActiveStack.Count;
            return end == 0 ? null : myActiveStack[end-1];
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_StateChart(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Process any active transition.
        ProcessTransition(frameId);
        // Make certain that at least one actibe state exists.
        if(myActiveStack.Count == 0 && myEntryState != null) MoveToState(myEntryState, frameId);
        // Execute state update functions.
        foreach(var state in myActiveStack) {
            state.OnUpdate(frameId);
        }
        MarkAsCurrent(frameId);
    }

    // ----------------------------------------------------------------------
    void ProcessTransition(int frameId) {
        // Determine if a transition exists for one of the active states.
        WD_State newState= null;
        int end= myActiveStack.Count;
        for(int idx= 0; idx < end; ++idx) {
            newState= myActiveStack[idx].VerifyTransitions(frameId);
            if(newState != null) break;
        }
        if(newState != null && newState != ActiveState) {
            MoveToState(newState, frameId);            
        }
    }
    // ----------------------------------------------------------------------
    void MoveToState(WD_State newState, int frameId) {
        int stackSize= myActiveStack.Count;
        // Determine transition parent node
        WD_State transitionParent= null;
        WD_State toTest= newState;
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
            WD_State state= myActiveStack[idx];
            if(state == transitionParent) break;
            state.OnExit(frameId);
        }
        int stableSize= idx+1;
        // Update active stack state.
        int newSize= stableSize+entrySize;
        if(newSize < stackSize) myActiveStack.RemoveRange(newSize, stackSize-newSize);
        if(newSize > myActiveStack.Capacity) myActiveStack.Capacity= newSize;
        while(myActiveStack.Count < newSize) myActiveStack.Add(null);
        WD_State toAdd= newState;
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
    public void AddChild(WD_Object _object) {
        WD_State state= _object as WD_State;
        if(state == null) {
            Debug.LogError("Trying to add an object that is not a state into State Chart!");
            return;
        }
        if(myChildren.Count == 0) myEntryState= state;
        myChildren.Add(state);
    }
    public void RemoveChild(WD_Object _object) {
        WD_State state= _object as WD_State;
        if(state == null) {
            Debug.LogError("Trying to remove an object that is not a state from a State Chart!");
            return;
        }
        if(state == myEntryState) myEntryState= null;
        myChildren.Remove(state);
    }
}
