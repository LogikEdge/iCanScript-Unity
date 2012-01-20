using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public sealed partial class iCS_Behaviour : iCS_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_Action   myStartAction       = null;
    iCS_Action   myUpdateAction      = null;
    iCS_Action   myLateUpdateAction  = null;
    iCS_Action   myFixedUpdateAction = null;
    int          myUpdateFrameId     = 0;
    int          myFixedUpdateFrameId= 0;
    object[]     myRuntimeNodes      = new object[0];
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int UpdateFrameId        { get { return myUpdateFrameId; }}
    public int FixedUpdateFrameId   { get { return myFixedUpdateFrameId; }}
    
    // ----------------------------------------------------------------------
    void Reset() {
        myStartAction       = null;
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
    void Awake() {
    }

    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        GenerateCode();
        if(myStartAction != null) {
            do {
                myStartAction.Execute(-2);
                if(myStartAction.IsStalled) {
                    Debug.LogError("The Start() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!myStartAction.IsCurrent(-2));
        }
    }
    
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
                if(myUpdateAction.IsStalled) {
//                    Debug.LogWarning("Upadte is STALLED. Attempting to unblock.");
                    myUpdateAction.ForceExecute(myUpdateFrameId);
                }                
            } while(!myUpdateAction.IsCurrent(myUpdateFrameId));
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            do {
                myLateUpdateAction.Execute(myUpdateFrameId);                                            
                if(myLateUpdateAction.IsStalled) {
//                    Debug.LogWarning("LateUpadte is STALLED. Attempting to unblock.");
                    myLateUpdateAction.ForceExecute(myUpdateFrameId);
                }
            } while(!myLateUpdateAction.IsCurrent(myUpdateFrameId));
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            do {
                myFixedUpdateAction.Execute(myFixedUpdateFrameId);                                
                if(myFixedUpdateAction.IsStalled) {
//                    Debug.LogWarning("FixedUpadte is STALLED. Attempting to unblock.");
                    myFixedUpdateAction.ForceExecute(myFixedUpdateFrameId);
                }
            } while(!myFixedUpdateAction.IsCurrent(myFixedUpdateFrameId));
        }
    }

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        iCS_Action action= obj as iCS_Action;
        if(action == null) return;
        switch(action.Name) {
            case iCS_EngineStrings.BehaviourChildStart: {
                myStartAction= action;
                break;
            }
            case iCS_EngineStrings.BehaviourChildUpdate: {
                myUpdateAction= action;
                break;
            }
            case iCS_EngineStrings.BehaviourChildLateUpdate: {
                myLateUpdateAction= action;
                break;
            }
            case iCS_EngineStrings.BehaviourChildFixedUpdate: {
                myFixedUpdateAction= action;
                break;
            }
            default: {
                break;
            }
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(object obj) {
        iCS_Action action= obj as iCS_Action;
        if(action == null) return;
        switch(action.Name) {
            case iCS_EngineStrings.BehaviourChildStart: {
                myStartAction= null;
                break;
            }
            case iCS_EngineStrings.BehaviourChildUpdate: {
                myUpdateAction= null;
                break;
            }
            case iCS_EngineStrings.BehaviourChildLateUpdate: {
                myLateUpdateAction= null;
                break;
            }
            case iCS_EngineStrings.BehaviourChildFixedUpdate: {
                myFixedUpdateAction= null;
                break;
            }
            default: {
                break;
            }
        }        
    }

}
