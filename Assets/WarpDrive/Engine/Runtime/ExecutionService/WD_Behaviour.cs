using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_Behaviour : WD_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    WD_Action   myUpdateAction      = null;
    WD_Action   myLateUpdateAction  = null;
    WD_Action   myFixedUpdateAction = null;
    int         myUpdateFrameId     = 0;
    int         myFixedUpdateFrameId= 0;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    
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
            myUpdateAction.Execute(myUpdateFrameId);
        }
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        if(myLateUpdateAction != null) {
            myLateUpdateAction.Execute(myUpdateFrameId);            
        }
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        ++myFixedUpdateFrameId;
        if(myFixedUpdateAction != null) {
            myFixedUpdateAction.Execute(myFixedUpdateFrameId);            
        }
    }

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        WD_Action action= obj as WD_Action;
        if(action == null) return;
        switch(action.Name) {
            case WD_EngineStrings.UpdateAction: {
                myUpdateAction= action;
                break;
            }
            case WD_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= action;
                break;
            }
            case WD_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= action;
                break;
            }
            default: {
                Debug.LogWarning("Ignoring unknown action being added to Behaviour: "+action.Name);
                break;
            }
        }
    }
    public void RemoveChild(object obj) {
        WD_Action action= obj as WD_Action;
        if(action == null) return;
        switch(action.Name) {
            case WD_EngineStrings.UpdateAction: {
                myUpdateAction= null;
                break;
            }
            case WD_EngineStrings.LateUpdateAction: {
                myLateUpdateAction= null;
                break;
            }
            case WD_EngineStrings.FixedUpdateAction: {
                myFixedUpdateAction= null;
                break;
            }
            default: {
                Debug.LogWarning("Ignoring unknown action being removed to Behaviour: "+action.Name);
                break;
            }
        }        
    }
}
