using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AP_Graph : MonoBehaviour {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [HideInInspector]   public AP_RootNode  RootNode= null;
    
    [System.Serializable]
    public class UserPreferences {
        [System.Serializable]
        public class UserNodeColors {
            public Color    StateColor= Color.cyan;
            public Color    ModuleColor= Color.yellow;
            public Color    FunctionColor= Color.green;
            public Color    SelectedColor= Color.white;            
        }
        public UserNodeColors   NodeColors= new UserNodeColors();
    }
    public UserPreferences Preferences= new UserPreferences();

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public AP_Graph Init() {
        if(RootNode == null) RootNode= AP_RootNode.CreateInstance("RootNode", this);
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
