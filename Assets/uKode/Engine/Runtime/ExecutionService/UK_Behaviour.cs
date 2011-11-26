using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_Behaviour : UK_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    UK_Action   myUpdateAction      = null;
    UK_Action   myLateUpdateAction  = null;
    UK_Action   myFixedUpdateAction = null;
    int         myUpdateFrameId     = 0;
    int         myFixedUpdateFrameId= 0;
    
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
            int retry= 0;
            do {
                ++retry;
                myUpdateAction.Execute(myUpdateFrameId);                
            } while(!myUpdateAction.IsCurrent(myUpdateFrameId) && retry < 10000);
            if(retry >= 10000) {
                Debug.LogWarning("Upadte graph seems to be looping.");
            }
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            int retry= 0;
            do {
                ++retry;
                myLateUpdateAction.Execute(myUpdateFrameId);                                            
            } while(!myLateUpdateAction.IsCurrent(myUpdateFrameId) && retry < 10000);
            if(retry >= 10000) {
                Debug.LogWarning("Late Upadte graph seems to be looping.");
            }
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            int retry= 0;
            do {
                ++retry;
                myFixedUpdateAction.Execute(myFixedUpdateFrameId);                                
            } while(!myFixedUpdateAction.IsCurrent(myFixedUpdateFrameId) && retry < 10000);
            if(retry >= 10000) {
                Debug.LogWarning("Fixed Upadte graph seems to be looping.");
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
                myUpdateAction= action;
                break;
            }
            case UK_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= action;
                break;
            }
            case UK_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= action;
                break;
            }
            default: {
                Debug.LogWarning("Ignoring unknown action being added to Behaviour: "+action.Name);
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
                Debug.LogWarning("Ignoring unknown action being removed to Behaviour: "+action.Name);
                break;
            }
        }        
    }
    // ----------------------------------------------------------------------
    public void ClearGeneratedCode() {
        Reset();
    }
}
