using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_StateChart : UK_Action, UK_IDispatcher {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public  UK_State        myEntryState = null;
    private List<UK_State>  myActiveStack= new List<UK_State>();
    public  List<UK_State>  myChildren   = new List<UK_State>();
    private UK_ParallelDispatcher  myDispatcher;
    
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
        myDispatcher= new UK_ParallelDispatcher(name);
    }

    // ======================================================================
    // IDispatcher implementation
    // ----------------------------------------------------------------------
    public bool IsStalled { get { return myDispatcher.IsStalled; }}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Process any active transition.
        ProcessTransition(frameId);
        // Make certain that at least one active state exists.
        if(myActiveStack.Count == 0 && myEntryState != null) MoveToState(myEntryState, frameId);
        // Execute state update functions.
        foreach(var state in myActiveStack) {
            state.OnUpdate(frameId);
        }
        MarkAsCurrent(frameId);
    }
    public override void ForceExecute(int frameId) {
        Execute(frameId);
    }
    
    // ----------------------------------------------------------------------
    void ProcessTransition(int frameId) {
        // Determine if a transition exists for one of the active states.
        UK_State newState= null;
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
