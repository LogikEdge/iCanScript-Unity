using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_Behaviour : UK_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    UK_IDispatcher  myUpdateAction      = null;
    UK_IDispatcher  myLateUpdateAction  = null;
    UK_IDispatcher  myFixedUpdateAction = null;
    int             myUpdateFrameId     = 0;
    int             myFixedUpdateFrameId= 0;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int UpdateFrameId        { get { return myUpdateFrameId; }}
    public int FixedUpdateFrameId   { get { return myFixedUpdateFrameId; }}
    
    // ----------------------------------------------------------------------
    void Reset() {
        myUpdateAction      = null;
        myLateUpdateAction  = null;
        myFixedUpdateAction = null;
        myUpdateFrameId     = 0;
        myFixedUpdateFrameId= 0;        
    }
    
    // ----------------------------------------------------------------------
    // This function should be used to find references to other objects.
    // Awake is invoked after all the objects are initialized.  Awake replaces
    // the constructor.
    void Awake() {}

    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {}
    
    // ----------------------------------------------------------------------
    void OnEnable() {}
    // ----------------------------------------------------------------------
    void OnDisable() {}
    // ----------------------------------------------------------------------
    void OnDestroy() {
    }
    
    
    // ======================================================================
    // Graph Updates
    // ----------------------------------------------------------------------
    // Called on every frame.
    void Update() {
        ++myUpdateFrameId;
        if(myUpdateAction != null) {
            do {
                myUpdateAction.Execute(myUpdateFrameId);
            } while(!myUpdateAction.IsCurrent(myUpdateFrameId) && !myUpdateAction.IsStalled);
            if(myUpdateAction.IsStalled) {
                Debug.LogWarning("Upadte is STALLED.");
            }
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            do {
                myLateUpdateAction.Execute(myUpdateFrameId);                                            
            } while(!myLateUpdateAction.IsCurrent(myUpdateFrameId) && !myLateUpdateAction.IsStalled);
            if(myLateUpdateAction.IsStalled) {
                Debug.LogWarning("LateUpadte is STALLED.");
            }
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            do {
                myFixedUpdateAction.Execute(myFixedUpdateFrameId);                                
            } while(!myFixedUpdateAction.IsCurrent(myFixedUpdateFrameId) && !myFixedUpdateAction.IsStalled);
            if(myFixedUpdateAction.IsStalled) {
                Debug.LogWarning("FixedUpadte is STALLED.");
            }
        }
    }

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        UK_Action action= obj as UK_Action;
        if(action == null) return;
        switch(action.Name) {
            case UK_EngineStrings.UpdateAction: {
                myUpdateAction= action as UK_IDispatcher;
                break;
            }
            case UK_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= action as UK_IDispatcher;
                break;
            }
            case UK_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= action as UK_IDispatcher;
                break;
            }
            default: {
                break;
            }
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(object obj) {
        UK_Action action= obj as UK_Action;
        if(action == null) return;
        switch(action.Name) {
            case UK_EngineStrings.UpdateAction: {
                myUpdateAction= null;
                break;
            }
            case UK_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= null;
                break;
            }
            case UK_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= null;
                break;
            }
            default: {
                break;
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClearGeneratedCode() {
        Reset();
    }
}
