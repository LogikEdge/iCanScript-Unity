using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AP_StateChart : AP_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public  AP_State        myEntryState;
    private List<AP_State>  myActiveStack= new List<AP_State>();
    
    // ======================================================================
    // ACCESSORS
    // ----------------------------------------------------------------------
    public AP_State EntryState {
        get { return myEntryState; }
        set { myEntryState= value; }
    }
    public AP_State ActiveState {
        get {
            int end= myActiveStack.Count;
            return end == 0 ? null : myActiveStack[end-1];
        }
    }
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    protected override void Init(string _name, AP_Aggregate _parent) {
        base.Init(_name, _parent);
        myEntryState= null;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        // Process any active transition.
        ProcessTransition();
        // Make certain that at least one actibe state exists.
        if(myActiveStack.Count == 0 && myEntryState != null) MoveToState(myEntryState);
        // Execute state update functions.
        foreach(var state in myActiveStack) {
            state.OnUpdate();
        }
    }

    // ----------------------------------------------------------------------
    void ProcessTransition() {
        // Determine if a transition exists for one of the active states.
        AP_State newState= null;
        int end= myActiveStack.Count;
        for(int idx= 0; idx < end; ++idx) {
            newState= myActiveStack[idx].VerifyTransitions();
            if(newState) break;
        }
        if(newState != null && newState != ActiveState) {
            MoveToState(newState);            
        }
    }
    // ----------------------------------------------------------------------
    void MoveToState(AP_State newState) {
        int stackSize= myActiveStack.Count;
        // Determine transition parent node
        AP_State transitionParent= null;
        AP_State toTest= newState;
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
            AP_State state= myActiveStack[idx];
            if(state == transitionParent) break;
            state.OnExit();
        }
        int stableSize= idx+1;
        // Update active stack state.
        int newSize= stableSize+entrySize;
        if(newSize < stackSize) myActiveStack.RemoveRange(newSize, stackSize-newSize);
        if(newSize > myActiveStack.Capacity) myActiveStack.Capacity= newSize;
        while(myActiveStack.Count < newSize) myActiveStack.Add(null);
        AP_State toAdd= newState;
        for(int offset= entrySize-1; offset >= 0; --offset) {
            myActiveStack[stableSize+offset]= toAdd;
            toAdd= toAdd.ParentState;            
        }
        // Execute entry functions.
        for(idx= stableSize; idx < newSize; ++idx) {
            myActiveStack[idx].OnEntry();
        }
    }
    
    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(AP_Object _object) {
        AP_State state= _object as AP_State;
        if(!state) {
            Debug.LogError("Trying to add an object that is not a state into FSM!");
            return;
        }
        base.AddChild(_object);
    }
    public override void RemoveChild(AP_Object _object) {
        AP_State state= _object as AP_State;
        if(!state) {
            Debug.LogError("Trying to remove an object that is not a state into FSM!");
            return;
        }
        if(state == myEntryState) myEntryState= null;
        base.RemoveChild(_object);        
    }
}
