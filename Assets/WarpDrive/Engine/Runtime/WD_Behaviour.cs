using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_Behaviour : MonoBehaviour {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_UserPreferences           Preferences= new WD_UserPreferences();
    public WD_RootNode                  RootNode   = null;
    public bool                         IsDirty= true;
    public List<WD_EditorObject>        EditorObjects= new List<WD_EditorObject>();
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public WD_Behaviour Init() {
        if(RootNode == null) RootNode= WD_RootNode.CreateInstance("RootNode", this);
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
    void Start() {
//        Debug.Log("Start");
    }
    
    // ----------------------------------------------------------------------
    void OnEnable() {
//        Debug.Log("OnEnable");
    }
    // ----------------------------------------------------------------------
    void OnDisable() {
//        Debug.Log("OnDisable");
    }
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

    // ======================================================================
    // COMMAND BUFFER
    // ----------------------------------------------------------------------
    public void AddObject(WD_Object obj) {
        IsDirty= true;
        WD_EditorObject so= new WD_EditorObject();
        so.Serialize(obj, EditorObjects.Count);
        EditorObjects.Add(so);
    }
    public void RemoveObject(WD_Object obj) {
        IsDirty= true;
        EditorObjects[obj.InstanceId].InstanceId= -1;
    }
    public void ReplaceObject(WD_Object obj) {
        IsDirty= true;
        EditorObjects[obj.InstanceId].Serialize(obj, obj.InstanceId);
    }
}
