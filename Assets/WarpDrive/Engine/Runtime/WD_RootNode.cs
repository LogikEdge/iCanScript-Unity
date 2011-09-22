using UnityEngine;
using System.Collections;

public class WD_RootNode : WD_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_Behaviour Graph;
    public WD_Top   UpdateTop= null;
    public WD_Top   LateUpdateTop= null;
    public WD_Top   FixedUpdateTop= null;
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_RootNode CreateInstance(string _name, WD_Behaviour graph) {
        return CreateInstance<WD_RootNode>().Init(_name, graph);
    }
    // ----------------------------------------------------------------------
    WD_RootNode Init(string _name, WD_Behaviour graph) {
        // This is a special case.  We need to initialize our attributes
        // since the base class expects Top to always be configured.
        Top= null;
        Graph= graph;
        // Initialize the base classes.
        base.Init(_name, null);
        // Override the base class initialization since we don't want the RootNode
        // to show in the graph.
        IsVisible= false;
        return this;
    }

    // ======================================================================
    // GRAPH UPDATES
    // ----------------------------------------------------------------------
    // Called on every frame.
    public void Update() {
        if(UpdateTop) {
            UpdateTop.Execute();                        
        }
    }
    // Called on evry frame after all Update have been called.
    public void LateUpdate() {
        if(LateUpdateTop) {
            LateUpdateTop.Execute();
        }
    }
    // Fix-time update to be used instead of Update
    public void FixedUpdate() {
        if(FixedUpdateTop) {
            FixedUpdateTop.Execute();
        }
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(WD_Object _object) {
        // Child type validation.
        WD_Top top= _object as WD_Top;
        if(!top) {
            Debug.LogError("Only Top can be added on the RootNode.");
            return;
        }
        if(top.Name == "Update") {
            UpdateTop= top;
            base.AddChild(_object); 
        }
        else if(_object.Name == "LateUpdate") {
            LateUpdateTop= top;
            base.AddChild(_object);
        }
        else if(_object.Name == "FixedUpdate") {
            FixedUpdateTop= top;
            base.AddChild(_object);
        }
        else {
            Debug.Log("Only objects with names Update, LateUpdate, and FixedUpdate can be added to a RootNode.");
        }
    }
    public override void RemoveChild(WD_Object _object) {
        // Child type validation.
        WD_Top top= _object as WD_Top;
        if(!top) {
            Debug.LogError("Only Top can be removed from the RootNode.");
            return;
        }
        if(top == UpdateTop) {
            UpdateTop= null;
            base.RemoveChild(_object); 
        }
        else if(top == LateUpdateTop) {
            LateUpdateTop= null;
            base.RemoveChild(_object);
        }
        else if(top == FixedUpdateTop) {
            FixedUpdateTop= null;
            base.RemoveChild(_object);
        }
        else {
            Debug.Log("Only objects with names Update, LateUpdate, and FixedUpdate can be added to a RootNode.");
        }
    }

}