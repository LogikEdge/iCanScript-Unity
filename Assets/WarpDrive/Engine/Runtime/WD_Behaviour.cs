using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_Behaviour : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public WD_UserPreferences           Preferences  = new WD_UserPreferences();
    public List<WD_EditorObject>        EditorObjects= new List<WD_EditorObject>();
    public WD_RootNode                  RootNode     = null;
    
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_Behaviour Init() {
//        if(RootNode == null) {
//            WD_EditorObject rootNode= EditorObjects.CreateInstance<WD_RootNode>("RootNode", -1, Vector2.zero);
//            RootNode= EditorObjects.GetRuntimeObject(rootNode) as WD_RootNode;
//        }
        return this;       
    }
    
    // ----------------------------------------------------------------------
    // This function should be used to find references to other objects.
    // Awake is invoked after all the objects are initialized.  Awake replaces
    // the constructor.
    void Awake() { Init(); }

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
        RootNode.Dealloc();
    }
    
    
    // ======================================================================
    // GRAPH UPDATES
    // ----------------------------------------------------------------------
    // Called on every frame.
    void Update() {
        RootNode.Update();
    }
    // Called on evry frame after all Update have been called.
    void LateUpdate() {
        RootNode.LateUpdate();
    }
    // Fix-time update to be used instead of Update
    void FixedUpdate() {
        RootNode.FixedUpdate();
    }

}
